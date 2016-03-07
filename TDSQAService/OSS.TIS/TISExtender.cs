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
using System.Data;
using System.Xml;
using TDSQASystemAPI;
using TDSQASystemAPI.Config;
using TDSQASystemAPI.Data;
using TDSQASystemAPI.TestResults;
using ScoringEngine.ConfiguredTests;
using AIR.Configuration;
using AIR.Common;
using OSS.TIS.DAL;
using OSS.TIS.ART;
using TDSQASystemAPI.Routing.Authorization;

namespace OSS.TIS
{
    internal class TISExtender : ITISExtender
    {
        #region ITISExtender Members

        public ITISExtenderState CreateStateContainer()
        {
            return new NoTISExtenderState();
        }

        public void PreScore(QASystem tis, TDSQASystemAPI.TestResults.TestResult tr, TDSQASystemAPI.Data.XmlRepositoryItem xmlRepoItem, TDSQASystemAPI.Config.ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            //add accommodations if we're supposed to
            ConfigurationHolder configHolder = ServiceLocator.Resolve<ConfigurationHolder>();
            if (tr.Testee != null && configHolder.GetFromMetaData(tr.ProjectID, "Accommodations").Exists(x => x.IntVal.Equals(1)))
            {
                bool useAlternateStudentId = false;
                TesteeAttribute ssid = tr.Testee.GetAttribute("StudentIdentifier", TesteeProperty.PropertyContext.INITIAL);
                if (ssid == null)
                {
                    ssid = tr.Testee.GetAttribute("AlternateSSID", TesteeProperty.PropertyContext.INITIAL);
                    useAlternateStudentId = true;
                }

                if (ssid == null)
                    throw new NullReferenceException("Neither StudentIdentifier nor AlternateSSID were found as ExamineeAttributes in the XML file with context = INITIAL.  At least one is required to get accommodations from ART. OppID = " + tr.Opportunity.OpportunityID);

                List<TesteeRelationship> stateAbbrevsList = tr.Testee.GetRelationships("StateAbbreviation", TesteeProperty.PropertyContext.INITIAL);
                if (stateAbbrevsList.Count != 1)
                    throw new MissingMemberException(string.Format("StateAbbreviation ExamineeRelationship with context = INITIAL appeared {0} times, but it was exepected to appear only 1 time. OppID = {1}", stateAbbrevsList.Count, tr.Opportunity.OpportunityID));

                WebService webservice = Settings.WebService["ART"];
                if (webservice == null)
                    throw new NullReferenceException("ART web service is not defined in config file. This is required for getting accommodations from ART");

                XmlDocument doc = new ARTDAL(webservice).GetStudentPackageXML(ssid.Value, stateAbbrevsList[0].Value, useAlternateStudentId);
                if (doc == null)
                    throw new NullReferenceException("ART Student package could not be retrieved for OppID " + tr.Opportunity.OpportunityID);

                ARTStudentPackage package = new ARTStudentPackage(doc);
                Dictionary<string, ARTAccommodation> artAccs = package.GetAccommodations(tr.Subject);
                //TODO: should we throw an exception if no accommodations are found?
                if (artAccs == null)
                    return;

                //now grab the values from TDS configs DB. Key = type, value = list of accoms.
                Dictionary<string, List<TestAccomodation>> accomsDict = configHolder.GetTestAccommodations(tis.dbHandleConfig, tr.test.TestName);
                //now convert this to be key = code, value = list of accoms with distinct type / code
                Dictionary<string, List<TestAccomodation>> myAccomsDict = new Dictionary<string, List<TestAccomodation>>(StringComparer.InvariantCultureIgnoreCase);
                foreach (List<TestAccomodation> acclist in accomsDict.Values)
                    foreach (TestAccomodation acc in acclist)
                    {
                        if (!myAccomsDict.ContainsKey(acc.Code))
                            myAccomsDict.Add(acc.Code, new List<TestAccomodation>() { acc });
                        //get only the list of distinct accom code / types (this is all that is needed for scoring)
                        if (!myAccomsDict[acc.Code].Any(x => x.Code.Equals(acc.Code) && x.Type.Equals(acc.Type)))
                            myAccomsDict[acc.Code].Add(acc);
                    }

                //now add new accommodations to the Opportunity combining values from each
                foreach (ARTAccommodation accom in artAccs.Values)
                {
                    if (!myAccomsDict.ContainsKey(accom.AccomCode))
                        continue; // ART accoms are by subject; it seems conceivable that not every test that the student is eligible for with the same subject would have the same accom configuration
                    //throw new NullReferenceException(string.Format("ART Accommodation code {0} was not found in TDS Configs DB.", accom.AccomCode));

                    //add all distinct type/code accommodations. Note that we hardcode segment to 0
                    foreach (TestAccomodation tdsAccom in myAccomsDict[accom.AccomCode])
                        tr.Opportunity.AddRTSAccomodation(tdsAccom.Type, tdsAccom.Description, tdsAccom.Code, 0/*tdsAccom.Segment*/, "");
                }
            }
        }

        public bool ShouldScore(QASystem tis, TDSQASystemAPI.TestResults.XMLAdapter adapter, TDSQASystemAPI.TestResults.TestResult tr, TDSQASystemAPI.Config.ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            // score if it's not a reset or an opp with items still requiring scores (operational, selected, not dropped, and not marked as notForScoring)
            return !tr.Opportunity.Status.Equals("reset", StringComparison.InvariantCultureIgnoreCase)
                    && !tr.HasItemsRequiringHandscores(TestResult.ItemOperationalStatus.Operational, true, true, true);
        }

        public void PostScore(QASystem tis, TDSQASystemAPI.TestResults.TestResult tr, TDSQASystemAPI.Data.XmlRepositoryItem xmlRepoItem, TDSQASystemAPI.Config.ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            // nothing
        }

        public List<TDSQASystemAPI.TestResults.ValidationRecord> Validate(QASystem tis, TDSQASystemAPI.TestResults.TestResult tr, XmlRepositoryItem xmlRepoItem, ProjectMetaData projectMetaData, ITISExtenderState state, out bool isFatal, out TDSQASystemAPI.Routing.SendToModifiers sendToModifiers)
        {
            isFatal = false;
            sendToModifiers = new TDSQASystemAPI.Routing.SendToModifiers();
            return new List<TDSQASystemAPI.TestResults.ValidationRecord>();
        }

        public void PreRoute(QASystem tis, TDSQASystemAPI.TestResults.XMLAdapter adapter, TDSQASystemAPI.TestResults.TestResult tr, TDSQASystemAPI.Data.XmlRepositoryItem xmlRepoItem, TDSQASystemAPI.Config.ProjectMetaData projectMetaData, TDSQASystemAPI.Routing.SendToModifiers sendToModifiers, ITISExtenderState state)
        {
            // nothing
        }

        public void PostRoute(QASystem tis, TDSQASystemAPI.TestResults.TestResult tr, TDSQASystemAPI.Data.XmlRepositoryItem xmlRepoItem, TDSQASystemAPI.Config.ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            // nothing
        }

        public void PostSave(QASystem tis, TDSQASystemAPI.TestResults.TestResult tr, TDSQASystemAPI.Data.XmlRepositoryItem xmlRepoItem, TDSQASystemAPI.Config.ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            // nothing
        }

        #endregion
    }
}
