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
using System.Xml;
using System.Xml.XPath;
using ScoringEngine.ConfiguredTests;
using TDSQASystemAPI.Config;
using AIR.Common;

namespace TDSQASystemAPI.TestResults
{
    internal class XMLAdapterOSS : XMLAdapter
    {
        internal XMLAdapterOSS(string xmlString)
            : base(xmlString)
        {
            _adapterType = AdapterType.OSS;
        }

        internal XMLAdapterOSS(XmlDocument xml)
            : base(xml)
        {
            _adapterType = AdapterType.OSS;
        }

        public override TestResult CreateTestResult(ScoringEngine.TestCollection tc, out bool isValid, bool loadBlueprint)
        {
            TestResult tr = null;
            isValid = true;
            try
            {
                tr = Utilities.Serialization.DeserializeXml<TestResult>(Utilities.Serialization.XmlDocumentToString(_xmlDoc));
            }
            catch (Exception ex)
            {
                isValid = false;
                AddValidationRecordRecursive(ex, "OSS TestFromXML deserialization fatal error", ValidationRecord.ValidationType.QASYSTEM, ValidateResult.Unknown);
                //AddValidationRecord(ValidationRecord.ValidationType.QASYSTEM, ValidateResult.Unknown, "OSS TestFromXML", string.Format("Fatal exception when deserealizing OSS XML into a TestResult. Message: {0}", ex.Message));
            }

            //if this is a valid TestResult then grab the blueprint and set the TestItem properties in each item response
            if (isValid && tr != null)
            {
                if (isValid && loadBlueprint)
                {
                    tr.Blueprint = tc.GetBlueprint(tr.Name);
                }

                if (isValid && tr.Blueprint == null && loadBlueprint)
                {
                    isValid = false;
                    AddValidationRecord(ValidationRecord.ValidationType.Semantic, ValidateResult.UnknownTest, "", "no blueprint available for test name " + tr.Name);
                }

                // set the configured project id for this test opp
                tr.SetProject(new ProjectMetaDataLoader(tr.HandscoringProjectID, tr.test.TestName, tr.Opportunity.Status, tr.Opportunity.QALevel));

                if (isValid && tr.ItemResponses != null && tr.Blueprint != null)
                {
                    foreach (ItemResponse ir in tr.ItemResponses)
                    {
                        TestItem item = tr.Blueprint.GetItem(ir.ItemName);
                        if (item != null)
                            ir.TestItem = item;
                        else
                        {

                            isValid = false;
                            AddValidationRecord(ValidationRecord.ValidationType.Semantic, ValidateResult.UnknownItem, "", "Item named in result xml not found: " + ir.ItemKey);
                        }
                    }
                }

                //  throw an error if there are no responses; 
                if (isValid && (tr.ItemResponses == null || tr.ItemResponses.Count == 0) && tr.Opportunity.Status != "reset" && tr.Opportunity.Status != "invalidated" && tr.Opportunity.Status != "expired" && tr.Opportunity.Status != "paused" && tr.Opportunity.Status != "partial" && tr.Opportunity.Status != "denied" && tr.Opportunity.Status != "pending")
                {
                    AddValidationRecord(ValidationRecord.ValidationType.Syntactic, ValidateResult.NotFound, @"/TDSReport/Opportunity/Item", "No items found or missing item tag in xml");
                    isValid = false;
                }

                //check the startDate based on the status
                //Zach 11/5/2014: This is not possible since StartDate is not nullable. 
                // This needs to change when we implement IXmlSerializable to check if there
                // is no startDate specified and the status is not one of those listed
                if (isValid && tr.Opportunity.OriginalStartDate == DateTime.MinValue && !((tr.Opportunity.Status == "invalidated") || (tr.Opportunity.Status == "reset") || (tr.Opportunity.Status == "expired") || (tr.Opportunity.Status == "paused") || (tr.Opportunity.Status == "partial") || (tr.Opportunity.Status == "denied")))
                    AddValidationRecord(ValidationRecord.ValidationType.Syntactic, ValidateResult.NotFound, @"/TDSReport/Opportunity/@startDate");

                if (isValid)
                {
                    //make sure there are no duplicate segment IDs
                    List<String> ids = new List<string>();
                    foreach (TestSegment segment in tr.Opportunity.segmentsList)
                    {
                        if (!ids.Contains(segment.ID, StringComparer.InvariantCultureIgnoreCase))
                            ids.Add(segment.ID);
                        else
                        {
                            isValid = false;
                            AddValidationRecord(ValidationRecord.ValidationType.Semantic, ValidateResult.SemanticRuleFailed,
                                                @"/TDSReport/Opportunity/Segment", String.Format("Duplicate test segment: {0}", segment.ID));
                        }
                    }
                }

                // Check server that sent the file.
                if (isValid)
                {
                    bool ignoreWrongServer = false;
                    MetaDataEntry metaDataIgnoreWrongServer = ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(tr.ProjectID, "QA", "IgnoreWrongServer");
                    if (metaDataIgnoreWrongServer != null && metaDataIgnoreWrongServer.IntVal == 1)
                        ignoreWrongServer = true;

                    if (tc != null &&
                        !ignoreWrongServer &&
                        !environment.StartsWith("local", StringComparison.InvariantCultureIgnoreCase) &&
                        !environment.StartsWith("dev", StringComparison.InvariantCultureIgnoreCase) &&
                        !tr.Mode.Equals("scanned", StringComparison.InvariantCultureIgnoreCase) &&
                        !ServiceLocator.Resolve<ConfigurationHolder>().IsSessionDatabaseConfigured(tr.Opportunity.ServerName, tr.Opportunity.DatabaseName))
                    {
                        isValid = false;
                        AddValidationRecord(ValidationRecord.ValidationType.Semantic, ValidateResult.Unknown, "server/database",
                            string.Format("Posted file to wrong Server. Given server {0}, Given DB:{1}, Configured Servers/databases: {2}",
                            tr.Opportunity.ServerName, tr.Opportunity.DatabaseName, ServiceLocator.Resolve<ConfigurationHolder>().TDSSessionDatabasesValue()));
                    }
                }

                //check no duplicate testee attributes
                if (isValid)
                {
                    List<TesteeAttribute> attList = new List<TesteeAttribute>();
                    foreach (TesteeAttribute ta in tr.Testee.TesteeAttributes)
                    {
                        if (!attList.Contains(ta))
                            attList.Add(ta);
                        else
                        {
                            isValid = false;
                            AddValidationRecord(ValidationRecord.ValidationType.Semantic, ValidateResult.SemanticRuleFailed,
                                @"/TDSReport/Examinee/ExamineeAttribute", String.Format("Duplicate testeeattribute... context: {0}, name: {1}", ta.Context, ta.Name));
                        }
                    }
                }

                //check no duplicate testee relationships
                if (isValid)
                {
                    List<TesteeRelationship> attList = new List<TesteeRelationship>();
                    foreach (TesteeRelationship ta in tr.Testee.TesteeRelationships)
                    {
                        if (!attList.Contains(ta))
                            attList.Add(ta);
                        else
                        {
                            isValid = false;
                            AddValidationRecord(ValidationRecord.ValidationType.Semantic, ValidateResult.SemanticRuleFailed,
                                @"/TDSReport/Examinee/ExamineeAttribute", String.Format("Duplicate testeerelationship... context: {0}, name: {1}", ta.Context, ta.Name));
                        }
                    }
                }

                //item level validation
                if (isValid)
                {
                    if ((tr.Opportunity.ItemResponses == null || tr.Opportunity.ItemResponses.Count == 0) && tr.Opportunity.Status != "reset" && tr.Opportunity.Status != "invalidated" && tr.Opportunity.Status != "expired" && tr.Opportunity.Status != "paused" && tr.Opportunity.Status != "partial" && tr.Opportunity.Status != "denied" && tr.Opportunity.Status != "pending")
                    {
                        AddValidationRecord(ValidationRecord.ValidationType.Syntactic, ValidateResult.NotFound, @"/TDSReport/Opportunity/Item", "No items found or missing item tag in xml");
                        isValid = false;
                    }

                    if (isValid)
                    {
                        foreach (ItemResponse ir in tr.Opportunity.ItemResponses)
                        {
                            //if the item is selected, there should be a response
                            if (ir.ResponseObject == null && ir.IsSelected)
                            {
                                AddValidationRecord(ValidationRecord.ValidationType.Semantic, ValidateResult.NotFound, @"/TDSReport/Opportunity/Item/Response", String.Format("No Response element found for selected item: {0}", ir.ItemKey));
                                isValid = false;
                            }
                        }

                    }
                }
            }

            return tr;
        }

        /// <summary>
        /// recursively add validation records from inner exceptions. Deserialization errors get more specific the further in you go
        /// </summary>
        /// <param name="e"></param>
        /// <param name="xpath"></param>
        /// <param name="type"></param>
        /// <param name="result"></param>
        private void AddValidationRecordRecursive(Exception e, string xpath, ValidationRecord.ValidationType type, ValidateResult result)
        {
            //if the inner exception is null (this is the inner most error) then include the stacktrace
            //if there was an error with the format of the data then the stacktrace will show us which class it is in
            AddValidationRecord(type, result, xpath, string.Format("{0}{1}",e.Message, e.InnerException != null ? "" : (", StackTrace: " + e.StackTrace)));
            if (e.InnerException != null)
                AddValidationRecordRecursive(e.InnerException, xpath, type, result);
        }

        public override void GetKeyValues(out string testName, out long oppId, out long testeeKey, out DateTime statusDate, out bool isDemo)
        {
            XPathNavigator nav = _xmlDoc.CreateNavigator();
            nav = nav.SelectSingleNode("//TDSReport/Test");
            testName = nav.GetAttribute("name", "");
            nav = nav.SelectSingleNode("//TDSReport/Opportunity");
            oppId = long.Parse(nav.GetAttribute("oppId", ""));
            statusDate = DateTime.Parse(nav.GetAttribute("statusDate", ""));
            nav = nav.SelectSingleNode("//TDSReport/Examinee");
            testeeKey = long.Parse(nav.GetAttribute("key", ""));
            try
            {
                // TODO: not sure if isDemo is going to be received from OSS TDS
                //  If not, treat all as operational
                isDemo = Convert.ToBoolean(nav.GetAttribute("isDemo", ""));
            }
            catch
            {
                isDemo = false;
            }
        }
    }
}
