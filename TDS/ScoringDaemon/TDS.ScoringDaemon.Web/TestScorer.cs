/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoringEngine;
using TDS.ScoringDaemon.Abstractions;
using TDS.Shared.Logging;

namespace TDS.ScoringDeamon.Web
{
    public class TestScorer
    {
        private readonly string _clientName;
        private readonly Scorer _scorer;
        private readonly TestCollection _testScorerCollection; // A collection of tests used for scoring.
        private readonly object _testScorerLock = new Object(); // for locking test scorer when loading
        private string _testScorerLoading = null; // test that is currently loading (null means none). We do this because the scoring engine marks tests as loaded prior to them actually completely the load resulting in possible race condition in case 2 threads arrive trying to score an opp in the same test
        private bool _conversionTablesLoaded = false;

        public TestScorer(string ibCS, string clientName)
        {
            _testScorerCollection = new TestCollection(ibCS, "TDS", false);
            _clientName = clientName;
            _scorer = new Scorer(_testScorerCollection);
        }

        public ScoredTest ScoreTest(ScorableTest scorableTest)
        {
           
            if (!LoadTestForScoring(scorableTest.TestKey)) return null;
            // call Paul's test scoring engine
            string testScoreString = _scorer.TestScore(scorableTest.TestKey, scorableTest.ItemString, scorableTest.DateCompleted, scorableTest.RowDelimiter, scorableTest.ColDelimiter, "Completed");               
            
            return !String.IsNullOrEmpty(testScoreString)
                       ? new ScoredTest() { OppKey = scorableTest.OppKey, TestScoreString = testScoreString }
                       : null;
        }

        private bool LoadTestForScoring(string testKey)
        {
            if (_testScorerCollection == null)
            {
                return false;
            }

            // check if test is loading/loaded
            if (HasTestForScoring(testKey)) return true;

            // lock any other test for this client from loading
            lock (_testScorerLock)
            {
                // double check if test is loading/loaded
                if (HasTestForScoring(testKey)) return true;

                // check if conversion tables have been loaded
                if (!_conversionTablesLoaded)
                {
                    // load the conversion tables
                    _testScorerCollection.LoadConversionTables(_clientName);
                    _conversionTablesLoaded = true;
                }

                // mark this test as loading
                _testScorerLoading = testKey;

                try
                {
                    // load test scoring data for this test
                    _testScorerCollection.LoadTest(testKey);
                }
                catch (Exception ex)
                {
                    // if the test failed to load then make sure to remove it 
                    if (_testScorerCollection.HasTest(testKey))
                    {
                        _testScorerCollection.DeleteTest(testKey);
                    }

                    throw;
                }
                finally
                {
                    // unmark this test as loading
                    _testScorerLoading = null;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the test is not currently loading and exists in the scorer collection. 
        /// </summary>
        /// <param name="testKey"></param>
        /// <returns></returns>
        private bool HasTestForScoring(string testKey)
        {
            // BUG: HasTest can report true but the test can still be in process of loading
            return (_testScorerLoading != testKey) && _testScorerCollection.HasTest(testKey);
        }
    }
}
