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
using TDSQASystemAPI;
using TDSQASystemAPI.Config;
using TDSQASystemAPI.Data;
using ScoringEngine.ConfiguredTests;

namespace OSS.TIS
{
    internal class TISExtender : ITISExtender
    {
        #region ITISExtender Members

        public ITISExtenderState CreateStateContainer()
        {
            return new NoTISExtenderState();
        }

        //TODO: refactor
        //TODO: ART
        public void PreScore(QASystem tis, TDSQASystemAPI.TestResults.TestResult tr, TDSQASystemAPI.Data.XmlRepositoryItem xmlRepoItem, TDSQASystemAPI.Config.ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            //add accommodations if we're supposed to
            if (tr.Testee != null && ConfigurationHolder.GetFromMetaData(tr.ProjectID, "Accommodations").Exists(x => x.IntVal.Equals(1)))
            {
                //the date to use to grab accommodations from RTS is configurable based on project, so check if it was
                //configured. If not then use opp startDate
                List<MetaDataEntry> accomDateEntry = ConfigurationHolder.GetFromMetaData(tr.ProjectID, "AccommodationDate");
                DateTime accomDate = tr.Opportunity.StartDate; // startDate is default value
                if (accomDateEntry != null && accomDateEntry.Count > 0)
                {
                    string date = accomDateEntry[0].TextVal ?? "null";
                    if (date.Equals("OpportunityStartDate", StringComparison.InvariantCultureIgnoreCase))
                        accomDate = tr.Opportunity.StartDate;
                    else if (date.Equals("OpportunityEndDate", StringComparison.InvariantCultureIgnoreCase))
                        accomDate = tr.Opportunity.StartDate;
                    else
                    {
                        if (!DateTime.TryParse(date, out accomDate))
                            throw new FormatException(string.Format("Could not parse configured AccommodationDate from project metadata for project ID {0}, value '{1}'", tr.ProjectID, date));
                    }
                }
                //TODO: fetch accoms from ART
                DataTable accomsDT = new DataTable();// = new TDSQASystemAPI.DAL.RtsDB().GetAccommodations(tr.Testee.EntityKey, QASystemConfigSettings.Instance.Client, accomDate);
                Dictionary<string, List<TestAccomodation>> accomsDict = ConfigurationHolder.GetTestAccommodations(tis.dbHandleConfig, tr.TestID);
                //bool addedAccommodation = false; // Zach 11/20/2014: We are only adding RTS accommodations, which don't get output in the XML. No need to archive.

                foreach (DataRow row in accomsDT.Rows)
                {
                    //todo: how should we handle null values in the datatable from RTS?
                    string type = row["Type"].ToString();
                    string code = row["Code"].ToString();
                    string description = "";
                    int segment = 0;
                    string source = "";
                    if (accomsDict != null && accomsDict.ContainsKey(type))
                    {
                        TestAccomodation acc = accomsDict[type].FirstOrDefault(x => x.Code.Equals(code));
                        if (acc != null)
                        {
                            description = acc.Description;
                            segment = acc.Segment;
                            source = acc.Source;
                        }
                    }
                    tr.Opportunity.AddRTSAccomodation(type, description, code, segment, source);
                    //    addedAccommodation = true;
                }
                //archive if we added any accommodations
                //if (addedAccommodation)
                //    SetArchiveStrategy(ArchiveStrategy.ArchiveAndInsert);
            }
        }

        public bool ShouldScore(QASystem tis, TDSQASystemAPI.TestResults.XMLAdapter adapter, TDSQASystemAPI.TestResults.TestResult tr, TDSQASystemAPI.Config.ProjectMetaData projectMetaData, ITISExtenderState state)
        {
            return true;
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
