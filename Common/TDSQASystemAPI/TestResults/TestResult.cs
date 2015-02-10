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
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using TDSQASystemAPI.Config;
using ScoringEngine.ConfiguredTests;
using ScoringEngine.Scoring;
using ScoringEngine;
using TDSQASystemAPI.ExceptionHandling;
using AIR.Common;

namespace TDSQASystemAPI.TestResults
{
    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "TDSReport", Namespace = "", IsNullable = false)]
    public class TestResult
    {
        public enum ItemOperationalStatus
        {
            Operational,
            FieldTest,
            Any
        }

        #region Properties

        [XmlElement("Test", Order = 1)]
        public Test test { get; set; }

        [XmlIgnore]
        private Testee testee = null;
        [XmlElement("Examinee", Order = 2)]
        public Testee Testee
        {
            get
            {
                return testee;
            }
            set
            {
                testee = value;
            }
        }

        [XmlIgnore]
        public TestMode TestMode
        {
            get
            {
                if (Mode == "online")
                    return TestMode.Online;
                if (Mode == "paper")
                    return TestMode.Paper;
                if (Mode == "scanned")
                    return TestMode.ScannedPaper;
                throw new QAException(String.Format("Unknown test mode: {0}", Mode), QAException.ExceptionType.InvalidXml);
            }
        }

        [XmlIgnore]
        public TestBlueprint Blueprint { get; set; }

        [XmlIgnore]
        private List<ValidationRecord> validationRecords = null;
        [XmlIgnore]
        public List<ValidationRecord> ValidationRecords
        {
            get
            {
                return validationRecords;
            }
            set
            {
                validationRecords = value;
            }
        }

        [XmlIgnore]
        private bool acknowledged = false;
        [XmlIgnore]
        public bool Acknowledged
        {
            get
            {
                return acknowledged;
            }
            set
            {
                acknowledged = value;
            }
        }

        [XmlIgnore]
        private Opportunity opportunity = null;
        [XmlElement("Opportunity", Order = 3)]
        public Opportunity Opportunity
        {
            get
            {
                return opportunity;
            }
            set
            {
                opportunity = value;
            }
        }

        [XmlIgnore]
        public Scores Scores
        {
            get
            {
                if (Opportunity != null)
                    return Opportunity.Scores;
                return null; // todo: this was previously not possible. What are the impacts of this?
            }
        }

        [XmlIgnore]
        public List<ItemResponse> ItemResponses
        {
            get
            {
                if (Opportunity != null)
                    return Opportunity.ItemResponses;
                return null;
            }
        }

        //AM: added for 2011-2012; moved from item node
        [XmlIgnore]
        private List<Comment> comments = new List<Comment>();
        [XmlElement("Comment", Order = 4)]
        public List<Comment> Comments
        {
            get
            {
                return comments;
            }
            set
            {
                comments = value;
            }
        }

        // ARP: Added 04/21/2014
        [XmlIgnore]
        private List<ToolUsage> toolUsages = new List<ToolUsage>();
        [XmlElement("ToolUsage", Order = 5)]
        public List<ToolUsage> ToolUsages
        {
            get
            {
                return toolUsages;
            }
            set
            {
                toolUsages = value;
            }
        }

        /// <summary>
        /// The test name. Ex. OAKS-Math-3
        /// </summary>
        [XmlIgnore]
        public string Name
        {
            get
            {
                return test.TestName;
            }
            set
            {
                test.TestName = value;
            }
        }

        [XmlIgnore]
        public string Subject
        {
            get
            {
                return test.Subject;
            }
            private set
            {
                test.Subject = value;
            }
        }

        [XmlIgnore]
        public string GradeBand
        {
            get
            {
                return test.Grade;
            }
            set
            {
                test.Grade = value;
            }
        }

        [XmlIgnore]
        public int ItemBank
        {
            get
            {
                return test.AirBank;
            }
            private set
            {
                test.AirBank = value;
            }
        }


        [XmlIgnore]
        public int HandscoringProjectID
        {
            get
            {
                return test.HandscoringProjectID;
            }
        }

        // AM: New for 2011-2012... The ITS ID of the test, independent of client, season and year
        [XmlIgnore]
        public string TestID
        {
            get
            {
                return test.TestID;
            }
            private set
            {
                test.TestID = value;
            }
        }

        // AM: New for 2011-2012... e.g. 'OAKS', 'HSA'
        [XmlIgnore]
        public string Contract
        {
            get
            {
                return test.Contract;
            }
            private set
            {
                test.Contract = value;
            }
        }

        [XmlIgnore]
        private long tisRequestID;
        [Obsolete("I don't think we still need this.", false)]
        [XmlIgnore]
        public long TisRequestID
        {
            get
            {
                return tisRequestID;
            }
            set
            {
                tisRequestID = value;
            }
        }

        //AM: new for 2012-2013
        [XmlIgnore]
        public string Mode
        {
            get
            {
                return test.Mode;
            }
            private set
            {
                test.Mode = value;
            }
        }

        [XmlIgnore]
        private int projectID;
        [XmlIgnore]
        public int ProjectID
        {
            get
            {
                return projectID;
            }
        }

        [XmlIgnore]
        private IProjectMetaDataLoader projectMetaDataLoader;
        [XmlIgnore]
        public IProjectMetaDataLoader ProjectMetaDataLoader 
        {
            get
            {
                return projectMetaDataLoader;
            }
        }

        public void SetProject(IProjectMetaDataLoader projectMetaDataLoader)
        {
            this.projectMetaDataLoader = projectMetaDataLoader;
            if (this.Blueprint != null && ConfigurationHolder.IsLoaded && this.ProjectMetaDataLoader != null)
            {
                this.projectID = ServiceLocator.Resolve<ConfigurationHolder>().GetProjectIDFromMetaData(this.ProjectMetaDataLoader);
            }
            else
            {
                this.projectID = -1;
            }
        }

        #endregion Properties

        public TestResult()
        {
            test = new Test();
        }

        public TestResult(TestBlueprint blueprint, string name, int itemBank, string contract, string mode)
        {
            this.Blueprint = blueprint;
            this.test = new Test(null, mode, contract, itemBank, null, null, name, 0); //Zach 10/31/2014: hardcoding  HS ProjectID to 0 when not specified
            this.projectID = -1;
            if (blueprint != null)
            {
                this.Subject = blueprint.Subject;
                this.GradeBand = blueprint.GradeCode;
                this.TestID = blueprint.TestID;
            }
        }

        public TestResult(TestBlueprint blueprint, string name, string subject, string gradeBand, int itemBank,
            int handscoringProjectID, string testId, string contract, string mode, long tisRequestID, IProjectMetaDataLoader projectMetaDataLoader)
        {
            this.Blueprint = blueprint;
            this.test = new Test(gradeBand, mode, contract, itemBank, testId, subject, name, handscoringProjectID);
            this.tisRequestID = tisRequestID;
            this.SetProject(projectMetaDataLoader);
        }

        public bool RefreshQAProject()
        {
            int oldProject = this.ProjectID;

            if (this.Blueprint != null && ConfigurationHolder.IsLoaded && this.ProjectMetaDataLoader != null)
            {
                this.ProjectMetaDataLoader.Refresh(this);
                this.SetProject(ProjectMetaDataLoader);
            }
            else
            {
                this.projectID = -1;
            }

            return oldProject != this.ProjectID;
        }

        /// <summary>
        /// Moved this here from QASystem.AddScores so that it could be reused by TIS.PaperTestStatus
        /// </summary>
        /// <returns></returns>
        public List<ScoringEngine.ConfiguredTests.ItemScore> GetItemScoresForScoringEngine()
        {
            List<ScoringEngine.ConfiguredTests.ItemScore> itemScores = new List<ScoringEngine.ConfiguredTests.ItemScore>();

            // if the bp wasn't loaded, we won't be able to perform this routine.
            //  just return an empty list.
            if (this.Blueprint == null)
                return itemScores;

            bool excludeDropped = false;

            // TestScoringEngine can now deal correctly with dropped responses, so always send them in.
            foreach (ItemResponse ir in this.GetItemResponses(excludeDropped))
            {
                // ir.SegmentID is really segmentName (or Key) not ID
                string segmentID = Blueprint.GetSegment(ir.SegmentID).SegmentID;

                // Attemptedness (this will probably get more complicated at some point...)
                bool attempted = ir.Response.Length > 0;

                foreach (TestItemScoreInfo si in ir.TestItem.ScoreInfo)
                {
                    if (ir.ItemHandscoreSet)
                    {
                        bool findIt = false;
                        foreach (HandScore hs in ir.ItemHandScore.HSScores)
                            if (hs.Type == HandScore.ReadType.Final && hs.Dimension.Equals(si.Dimension, StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (findIt)
                                    throw new Exception("Two final scores for dimension'" + si.Dimension + "' for item " + ir.TestItem.ItemName);
                                itemScores.Add(new ScoringEngine.ConfiguredTests.ItemScore(ir.TestItem, si, ir.Position, Convert.ToDouble(hs.Value), hs.ConditionCode, segmentID, ir.Operational == 0, ir.IsSelected, attempted, ir.Dropped));
                                findIt = true;
                            }
                        if (!findIt)
                            itemScores.Add(new ScoringEngine.ConfiguredTests.ItemScore(ir.TestItem, si, ir.Position, -1.0, "", segmentID, ir.Operational == 0, ir.IsSelected, attempted, ir.Dropped));
                    }
                    else
                    {
                        if (ir.TestItem.ScoreInfo.Count == 1)
                            itemScores.Add(new ScoringEngine.ConfiguredTests.ItemScore(ir.TestItem, si, ir.Position, ir.PresentedScore, "", segmentID, ir.Operational == 0, ir.IsSelected, attempted, ir.Dropped));
                        else if (ir.ScoreInfo != null && ir.ScoreInfo.SubScores != null && ir.ScoreInfo.SubScores.Count > 0) // if there exist dimension scores, use them
                        {
                            bool findIt = false;

                            foreach (ItemScoreInfo dimScoreInfo in ir.ScoreInfo.SubScores)
                            {
                                if (si.Dimension.Equals(dimScoreInfo.Dimension ?? "", StringComparison.InvariantCultureIgnoreCase)
                                    && (dimScoreInfo.Status ?? ScoringStatus.NotScored) == ScoringStatus.Scored)
                                {
                                    if (findIt)
                                        throw new Exception("Two scores for dimension'" + si.Dimension + "' for item " + ir.TestItem.ItemName);
                                    // TODO: we may eventually need to start handling scorepoints as a double.  For now, it's the same as machine-scored == int
                                    double scorePoint = dimScoreInfo.Points == null ? -1 : dimScoreInfo.Points.Value;
                                    string cc = dimScoreInfo.Rationale == null || dimScoreInfo.Rationale.HandScoreDetails == null ? null : dimScoreInfo.Rationale.HandScoreDetails.conditionCode;
                                    itemScores.Add(new ScoringEngine.ConfiguredTests.ItemScore(ir.TestItem, si, ir.Position, scorePoint, cc, segmentID, ir.Operational == 0, ir.IsSelected, attempted, ir.Dropped));
                                    findIt = true;
                                }
                            }

                            if (!findIt)
                                itemScores.Add(new ScoringEngine.ConfiguredTests.ItemScore(ir.TestItem, si, ir.Position, -1.0, "", segmentID, ir.Operational == 0, ir.IsSelected, attempted, ir.Dropped));
                        }
                        else
                            // ir.TestItem.ScoreInfo.Count > 1 implies dimension scores, but there are no handscores or dim scores in the ScoreInfo
                            itemScores.Add(new ScoringEngine.ConfiguredTests.ItemScore(ir.TestItem, si, ir.Position, -1.0, "", segmentID, ir.Operational == 0, ir.IsSelected, attempted, ir.Dropped));
                    }
                }

                //            if (finalCC != null)
                //                throw new Exception("There can't be two final condition codes here! Item " + ir.ItemName);
                //            else
                //                finalCC = hs;
                //    if (finalCC != null)
                //        cc = finalCC.Value;
                //}
                //if (cc.Length > 0)
                //    itemScores.Add(new ScoringEngine.ConfiguredTests.ItemScore(ir.TestItem, si, 0.0, cc, ir.IsSelected, attempted));
                //else if (ir.IRTScores.Count > 0)
                //    itemScores.Add(new ScoringEngine.ConfiguredTests.ItemScore(ir.TestItem, si, ir.IRTScores[si.Dimension], cc, ir.IsSelected, attempted));
                //else
                //    itemScores.Add(new ScoringEngine.ConfiguredTests.ItemScore(ir.TestItem, si, -1.0, cc, ir.IsSelected, attempted));
                //}
            }

            return itemScores;
        }

        /// <summary>
        /// Returns ItemResponses if excludeDropped == false; otherwise, it returns only 
        /// the ItemResponses where Dropped==false.
        /// </summary>
        /// <param name="excludeDropped"></param>
        /// <returns></returns>
        public List<ItemResponse> GetItemResponses(bool excludeDropped)
        {
            // this is needed for MN's first not-responded-to-item rule.
            List<ItemResponse> sortedItemResponses = ItemResponses;
            sortedItemResponses.Sort(delegate(ItemResponse ir1, ItemResponse ir2) { return ir1.Position.CompareTo(ir2.Position); });

            if (!excludeDropped)
                return sortedItemResponses;

            List<ItemResponse> itemsNotDropped = new List<ItemResponse>();
            foreach (ItemResponse ir in sortedItemResponses)
            {
                if (!ir.Dropped)
                    itemsNotDropped.Add(ir);
            }
            return itemsNotDropped;
        }

        private static readonly List<string> HandscoredTypes = new List<string>() { "SER", "SSR", "WER", "WSR", "EI", "ER", "SA", "CR" };
        /// <summary>
        /// Returns whether or not the test opp has items requiring hand-scoring.
        /// </summary>
        /// <param name="excludeDropped"></param>
        /// <param name="selectedOnly"></param>
        /// <param name="opStatus"></param>
        /// <returns></returns>
        public bool HasItemsRequiringHandscores(ItemOperationalStatus opStatus, bool excludeDropped, bool selectedOnly)
        {
            return HasItemsRequiringHandscores(opStatus, excludeDropped, selectedOnly, null);
        }

        /// <summary>
        /// Overload that takes a machine scorer confidence level threshold under which the item is to be hand-scored.
        /// No score info or confidence level will result in an item being counted as requiring hand-scores if machineScoreConfLevelThreshold is supplied
        /// </summary>
        /// <param name="opStatus"></param>
        /// <param name="excludeDropped"></param>
        /// <param name="selectedOnly"></param>
        /// <param name="machineScoreConfLevelThreshold"></param>
        /// <returns></returns>
        public bool HasItemsRequiringHandscores(ItemOperationalStatus opStatus, bool excludeDropped, bool selectedOnly, double? machineScoreConfLevelThreshold)
        {
            return ItemResponses.Exists(ir =>
                ((!Routing.ItemScoring.ItemScoringConfig.Instance.ItemIsConfigured(ir.Format, ir.ItemName)
                        && HandscoredTypes.Contains(ir.Format) && !ir.ItemHandscoreSet)
                    || Routing.ItemScoring.ItemScoringConfig.Instance.ScoreItem(ir))
                && (!excludeDropped || !ir.Dropped)
                && (!selectedOnly || ir.IsSelected)
                && (opStatus == ItemOperationalStatus.Any
                        || (opStatus == ItemOperationalStatus.FieldTest && ir.Operational == 0)
                        || (opStatus == ItemOperationalStatus.Operational && ir.Operational == 1))
                && (machineScoreConfLevelThreshold == null || ir.ScoreInfo == null || ir.ScoreInfo.ConfLevel == null || ir.ScoreInfo.ConfLevel.Value.CompareTo(machineScoreConfLevelThreshold) < 0));
        }

        public void ClearScores()
        {
            if (this.Opportunity != null)
                this.Opportunity.ClearScores();
        }

        public bool PassedAllValidations()
        {
            if ((validationRecords != null) && (validationRecords.Count == 0)) return true;
            return false;
        }

        public bool AddScores(TestCollection tc)
        {
            Dictionary<string, Dictionary<string, MeasureValue>> measureValues = new Dictionary<string, Dictionary<string, MeasureValue>>();

            Dictionary<string, Dictionary<string, MeasureValue>> measureValuesIn = new Dictionary<string, Dictionary<string, MeasureValue>>();
            foreach (Dictionary<string, Score> scores in this.Scores.TestScores.Values)
            {
                foreach (Score score in scores.Values)
                {
                    string measureOf = score.MeasureOf;
                    string measureLabel = score.MeasureLabel;
                    string measureValue = score.MeasureValue;
                    if (!measureValuesIn.ContainsKey(measureLabel)) measureValuesIn[measureLabel] = new Dictionary<string, MeasureValue>();
                    double score_d, se_d;
                    if (Double.TryParse(measureValue, out score_d) && Double.TryParse(score.MeasureSE, out se_d))
                        measureValuesIn[measureLabel][measureOf] = new MeasureValue(measureLabel, measureOf, score_d, se_d);
                    else
                        measureValuesIn[measureLabel][measureOf] = new MeasureValue(measureLabel, measureOf, measureValue);
                }
            }

            List<ScoringEngine.ConfiguredTests.ItemScore> itemScores = this.GetItemScoresForScoringEngine();

            measureValues = new ScoringEngine.Scorer(tc).ApplyComputationRules(this.Name, this.Testee.EnrolledGrade, measureValuesIn, itemScores, this.Opportunity.StartDate, this.Opportunity.Status, this.Opportunity.Forms, this.Opportunity.DateForceCompleted, this.TestMode, this.Opportunity.Accomodations, this.Opportunity.RTSAccommodations);

            if (measureValues.Count == 0)
                return false;

            bool xmlChanged = false;
            foreach (Dictionary<string, MeasureValue> measures in measureValues.Values)
            {
                foreach (MeasureValue measureValue in measures.Values)
                {
                    Score score = new Score(measureValue.MeasureOf, measureValue.MeasureLabel, measureValue.ScoreString, measureValue.StandardErrorString);
                    bool changed = this.Opportunity.AddScoreIfNew(score);
                    xmlChanged = xmlChanged ? xmlChanged : changed;
                }
            }
            return xmlChanged;
        }

        /// <summary>
        /// default groupname = DoR
        /// </summary>
        /// <returns></returns>
        internal XmlDocument ToXml(ITestResultSerializerFactory serializerFactory)
        {
            return ToXml(MetaDataEntry.GroupName.DoR, serializerFactory);
        }

        internal XmlDocument ToXml(MetaDataEntry.GroupName group, ITestResultSerializerFactory serializerFactory)
        {
            return ToXml(group.ToString(), serializerFactory);
        }

        internal XmlDocument ToXml(string group, ITestResultSerializerFactory serializerFactory)
        {
            XmlDocument doc = new XmlDocument();
            //Check Metadata for xml serializer type, TDS is default value
            MetaDataEntry entry = ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(this.ProjectID, group, "XMLVersion");

            XMLAdapter.AdapterType xmlType = XMLAdapter.AdapterType.TDS;
            if (entry != null)
                xmlType = Utilities.Utility.Value(entry.TextVal, XMLAdapter.AdapterType.TDS);

            // if set to 1/true, any demographics in the file will be preserved
            //  Used only in the OSS environment to minimize configuration
            entry = ServiceLocator.Resolve<ConfigurationHolder>().GetFromMetaData(this.ProjectID, group, "IncludeAllDemographics");

            SerializationConfig config = null;
            if (entry != null && Convert.ToBoolean(entry.IntVal))
            {
                config = new SerializationConfigIncludeAllDemographics();
            }
            else
            {
                //get the serialization config from the project metadata
                config = new SerializationConfig(/*ConfigurationHolder.IncludeAccommodations(ProjectID, group),*/
                    ServiceLocator.Resolve<ConfigurationHolder>().GetRTSAttributes(this.projectID, group));
            }

            doc.LoadXml(serializerFactory.CreateSerializer(xmlType.ToString(), this).Serialize(config));
            return doc;
        }
    }
}
