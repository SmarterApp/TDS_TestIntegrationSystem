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

namespace TDSQASystemAPI.TestResults
{
    public enum Severity { Unknown = 0, Information = 10, Warning = 100, Fatal = 200 };
    public enum Outcome { Unknown = 0, Passed = 1, Failed = 2 };

    public class ValidationRecord
    {
        public enum ValidationType { Unknown = 0, Syntactic = 1, Semantic = 2 , ReportingTarget = 3, TDS = 4, QASYSTEM = 5};

		#region Properties

		private string ruleID = "";
		public string RuleID
		{
			get
			{
				return ruleID;
			}
			set
			{
				ruleID = value;
			}
		}

		private Severity resultSeverity = Severity.Unknown;
		public Severity ResultSeverity
		{
			get
			{
				return resultSeverity;
			}
		}

		private ValidationType validationType = ValidationType.Unknown;
		public ValidationType Type
		{
			get
			{
				return validationType;
			}
		}

		private XMLAdapter.ValidateResult validationResult = XMLAdapter.ValidateResult.Unknown;
		public XMLAdapter.ValidateResult ValidateResult
		{
			get
			{
				return validationResult;
			}
		}

		private string xpath = null;
		public string XPath
		{
			get
			{
				return xpath;
			}
		}

		private string message = null;
		public string Message
		{
			get
			{
				return message;
			}
		}

		#endregion

        public ValidationRecord()
        {
        }

		public ValidationRecord(ValidationType vType, XMLAdapter.ValidateResult validateResult, string xpath)
        {
            this.validationType = vType;
            this.validationResult = validateResult;
            this.xpath = xpath;
            this.message = "No additional message";
        }

        public ValidationRecord(ValidationType vType, XMLAdapter.ValidateResult validateResult, string xpath, string message)
        {
            this.validationType = vType;
            this.validationResult = validateResult;
            this.xpath = xpath;
            this.message = message;
        }

        public ValidationRecord(string ruleID, string message, Severity severity)
        {
            this.validationType = ValidationType.Semantic;
            this.validationResult = XMLAdapter.ValidateResult.SemanticRuleFailed;
            this.ruleID = ruleID;
            this.message = message;
			this.resultSeverity = severity;
        }
    }
}
