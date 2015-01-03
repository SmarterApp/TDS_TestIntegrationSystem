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
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Configuration;

using TDSQASystemAPI.Config;
using System.Globalization;
using ScoringEngine;
using ScoringEngine.ConfiguredTests;
using System.IO;
using TDSQASystemAPI.ExceptionHandling;

namespace TDSQASystemAPI.TestResults
{
    public abstract class XMLAdapter
    {
        public enum ValidateResult
        {
            Unknown = 0, Valid = 1, NotFound = 2, DisallowedMultiple = 3, TypeConversionFail = 4, UnknownTest = 5, UnknownItem = 6,
            ScoreForUnknownFeature = 7, SemanticRuleFailed = 8
        };
        public enum AdapterType { TDS = 0, OSS };
        protected AdapterType _adapterType = AdapterType.TDS;
        protected XmlDocument _xmlDoc = null;

        protected static string environment = ConfigurationManager.AppSettings["Environment"];

        /// <summary>
        /// The original unedited XML from the XmlRepository
        /// </summary>
        public XmlDocument XMLDoc
        {
            get
            {
                return _xmlDoc;
            }
        }

        protected XMLAdapter(string xml)
        {
            _xmlDoc = new XmlDocument();
            _xmlDoc.LoadXml(xml);
            //todo: put in a try/catch block around load
        }

        protected XMLAdapter(XmlDocument xml)
        {
            _xmlDoc = xml;
        }

        abstract public TestResult CreateTestResult(TestCollection tc, out bool isValid, bool loadBlueprint);

        abstract public void GetKeyValues(out string testName, out long oppId, out long testeeKey, out DateTime statusDate, out bool isDemo);

        public TestResult CreateTestResult(TestCollection testCollection, out bool isValid)
        {
            return CreateTestResult(testCollection, out isValid, testCollection != null);
        }

        internal bool MergeScores(TestResult tr, out bool qaProjectChanged)
        {
            bool scoresMerged = qaProjectChanged = false;
            TIS.TISScoreMerger merger = new TIS.TISScoreMerger();
            try
            {
                scoresMerged = merger.MergeScores(tr, out qaProjectChanged);
            }
            catch (Exception e)
            {
                AddValidationRecord(ValidationRecord.ValidationType.QASYSTEM, ValidateResult.TypeConversionFail, "Merge Scores", e.Message);
                scoresMerged = false;
            }
            return scoresMerged;
        }
    
        #region validation records

		protected List<ValidationRecord> _validationRecords = new List<ValidationRecord>();
        public List<ValidationRecord> ValidationRecords
        {
            get
            {
                return _validationRecords;
            }
        }

        public void AddValidationRecord(ValidationRecord.ValidationType vType, ValidateResult result, string xpath, string message)
        {
			ValidationRecord record = _validationRecords.Find(delegate(ValidationRecord vr) { return (vr.Message.Equals(message, StringComparison.CurrentCultureIgnoreCase) &&
				vr.Type.ToString().Equals(vType.ToString(), StringComparison.CurrentCultureIgnoreCase) && vr.ValidateResult.ToString().Equals(result.ToString(), StringComparison.CurrentCultureIgnoreCase)
				&& vr.XPath.Equals(xpath, StringComparison.CurrentCultureIgnoreCase));
		});

			if (record == null)
			{
				ValidationRecord vRec = new ValidationRecord(vType, result, xpath, message);
				_validationRecords.Add(vRec);
			}
        }

        public void AddValidationRecord(ValidationRecord.ValidationType vType, ValidateResult result, string xpath)
        {
			ValidationRecord record = _validationRecords.Find(delegate(ValidationRecord vr) { return (vr.Type.ToString().Equals(vType.ToString(), StringComparison.CurrentCultureIgnoreCase) 
				&& vr.ValidateResult.ToString().Equals(result.ToString(), StringComparison.CurrentCultureIgnoreCase)
				&& vr.XPath.Equals(xpath, StringComparison.CurrentCultureIgnoreCase));
			});
			
			if (record == null)
			{
				ValidationRecord vRec = new ValidationRecord(vType, result, xpath, "Missing value");
				_validationRecords.Add(vRec);
			}
        }
        #endregion validation records
    }
}
