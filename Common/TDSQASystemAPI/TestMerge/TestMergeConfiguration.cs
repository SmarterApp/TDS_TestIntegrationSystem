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
using TDSQASystemAPI.DAL;

namespace TDSQASystemAPI.TestMerge
{
    /// <summary>
    /// A singleton class/object which holds the current list of configured merge settings
    /// </summary>
    public sealed class TestMergeConfiguration
    {
        /// <summary>
        /// Access the test merge configuration using singleton accessor
        /// </summary>
        public static TestMergeConfiguration Instance
        {
            get 
            {
                if (_instance == null)
                {
                    lock (syncObject)
                    {
                        if (_instance == null)
                            _instance = new TestMergeConfiguration();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Returns a new test merge object with applicable config for the passed test. 
        /// Returns null if the passed test is not a merge participant 
        /// </summary>
        /// <param name="sTestName"></param>
        /// <returns></returns>
        public TestMerge GetTestMerge(string sTestName)
        {
            return _activeTestMergeConfigs.ContainsKey(sTestName) ? new TestMerge(_activeTestMergeConfigs[sTestName]) : null;
        }


        /// <summary>
        /// Configure test merging for possible tests
        /// </summary>
        private TestMergeConfiguration()
        {
            List<MergeConfig> mergeConfigs = TestMergeDataAccess.GetConfiguredMergeConfigs();

            foreach (MergeConfig mergeConfig in mergeConfigs)
            {
                foreach (string sSourceTestName in mergeConfig.SourceTestNamesForMerge)
                    _activeTestMergeConfigs.Add(sSourceTestName, mergeConfig);
            }
        }
        
        /// <summary>
        /// Dictionary mapping tests to its test merge config object
        /// </summary>
        private Dictionary<string, MergeConfig> _activeTestMergeConfigs = new Dictionary<string, MergeConfig>();

        /// <summary>
        /// Single instance of test merge configurations 
        /// </summary>
        private static volatile TestMergeConfiguration _instance;

        /// <summary>
        /// Synchronization object 
        /// </summary>
        private static object syncObject = new Object();    
    }
}
