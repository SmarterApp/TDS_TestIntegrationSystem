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
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.ExceptionHandling;

namespace TDSQASystemAPI.BL
{
    public class TestOpportunity
    {
        protected const string XML_ADAPTER_TYPE = "TDS";

        public string OppID { get; private set; }
        protected XmlRepository XmlRepositoryBL { get; private set; }
        private DAL.TestOpportunity testOpportunityDAL;

        public TestOpportunity(string oppId)
        {
            this.OppID = oppId;
            this.XmlRepositoryBL = new XmlRepository();
            this.testOpportunityDAL = new TDSQASystemAPI.DAL.TestOpportunity();
        }

        /// <summary>
        /// Returns the most recent TestOpportunityStatusRow for the opp
        /// </summary>
        /// <returns></returns>
        public DataRow GetCurrentTestOppStatus()
        {
            return testOpportunityDAL.GetCurrentTestOpportunityStatus(OppID);
        }

        /// <summary>
        /// Returns a table containing the latest record for each opportunity for the
        /// provided testee and testID.  If mode is null/String.Empty, the results will not
        /// discriminate by mode.  Otherwise, only results of the specified mode will be returned.
        /// This is used to count a student's opportunities for a given test either across paper and online or
        /// for a single mode.
        /// </summary>
        /// <param name="testeeId">Student's internal RTS key</param>
        /// <param name="testID">The TestID (not test name/key)</param>
        /// <param name="mode">Optional: "paper" or "online"; default = all</param>
        /// <param name="ignorePaperTestStatus">Optional: Excludes/includes status of "papertest".  If true and the latest 
        /// status is "papertest", the previous status will be returned as latest (if exists).  This is used to calculate
        /// the opp number for paper tests.  Otherwise, pass false or use the overload.  Default = false.</param>
        /// <param name="excludePaperTestDiscreps">true excludes paper test discrepancies from the results</param>
        /// <returns></returns>
        public DataTable GetCurrentTestOpportunityStatus(long testeeId, string testID,
            string mode, bool excludePaperTestDiscreps)
        {
            return testOpportunityDAL.GetCurrentTestOpportunityStatus(testeeId, testID, mode, excludePaperTestDiscreps);
        }

        public DataTable GetCurrentTestOpportunityStatus(long testeeId, string testID)
        {
            return testOpportunityDAL.GetCurrentTestOpportunityStatus(testeeId, testID, null, true);
        }

        /// <summary>
        /// Current a current QASystemDataSet.TestOpportunityStatusRow, returns an inflated TestResult instance.
        /// Refactored from UpdateStatus
        /// </summary>
        /// <param name="currentStatus"></param>
        /// <returns></returns>
        public TestResult GetTestResult(DataRow currentStatus, ITestResultSerializerFactory serializerFactory)
        {
            XmlDocument testXml = XmlRepositoryBL.GetXmlContent(Convert.ToInt64(currentStatus["_fk_XMLRepository"]));

            if (testXml == null)
                throw new ApplicationException(String.Format("Could not locate XML for OppId: {0}, FileID: (1).", this.OppID, Convert.ToInt64(currentStatus["_fk_XMLRepository"])));

            // create the TestResult instance from the xml file
            TestResult testResult = InflateTestResult(testXml, serializerFactory);
            if (testResult == null)
                throw new ApplicationException(String.Format("Could not inflate TestResult for OppId: {0}, FileID: {1}.", this.OppID, Convert.ToInt64(currentStatus["_fk_XMLRepository"])));

            return testResult;
        }

        /// <summary>
        /// Updates the status of a paper test and writes it back out to the QA
        /// system for processing.
        /// </summary>
        /// <param name="newSatus"></param>
        /// <param name="destinationEnvironment"></param>
        public void UpdateStatus(TestResult testResult, string newStatus, long? appealRequestId, ITestResultSerializerFactory serializerFactory)
        {
            // if there's an appeal requestId gen var, remove it.  A fresh one will be added below if 
            //  appealRequestId is not null.  This is done to handle resets and invalidations through the DoR website.  
            //  Those are not appeals and therefore should not have a requestId.  If a rescore appeal is reset through 
            //  the DoR website, the appeal request id that will be found here is that of the rescore appeal.
            //  This will cause confusion downstream, since the status would be "reset".
            //TODO: consider making resets/invalidations through the DoR website appeals as well.
            GenericVariable appealRequestIdGenVar =
                    testResult.Opportunity.GetGenericVariableByContextName(GenericVariable.VariableContext.APPEAL, GenericVariable.APPEAL_REQUEST_ID_VAR_NAME);
            if (appealRequestIdGenVar != null)
                testResult.Opportunity.GenericVariables.Remove(appealRequestIdGenVar);

            // if an appeal requestid was provided, embed it in the file as a generic variable
            if (appealRequestId != null)
                testResult.Opportunity.AddGenericVariable(GenericVariable.VariableContext.APPEAL, GenericVariable.APPEAL_REQUEST_ID_VAR_NAME, appealRequestId.Value.ToString());

            // set the status of the TestResult to the new one
            //  also set the status date to reflect when this action was taken
            testResult.Opportunity.Status = newStatus;
            testResult.Opportunity.StatusDate = DateTime.Now;

            // also clear out any qaLevel.  Currently, the only one we're using is "BatchReport".
            //  If a test was batch-reported but is being resubmitted with a different status, the 
            //  resubmitted opp should be considered a new version which has not been reported yet.
            //TODO: as other uses of qaLevel arise, revisit this.
            testResult.Opportunity.QALevel = null;

            // now write the test to the 
            XmlRepositoryBL.InsertXml(XmlRepository.Location.source, testResult, serializerFactory);
        }

        /// <summary>
        /// Returns an inflated TestResult instance given a TDS-format XML string
        /// </summary>
        /// <param name="testXml"></param>
        /// <returns></returns>
        private TestResult InflateTestResult(XmlDocument testXml, ITestResultSerializerFactory serializerFactory)
        {
            XMLAdapter xmlAdaptor = serializerFactory.CreateDeserializer(testXml);
            bool isValid;
            TestResult tr = xmlAdaptor.CreateTestResult(null, out isValid, false);
            if (!isValid)
                tr = null;
            return tr;
        }
    }
}
