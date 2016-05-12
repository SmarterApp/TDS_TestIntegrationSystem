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
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Linq;

using TDSQASystemAPI.Config;
using ScoringEngine.ConfiguredTests;

namespace TDSQASystemAPI.TestResults
{
	//- <opportunity server="WSTESAST002" database="TDS_Session_" oppid="2001327" startDate="3/12/2009 1:11:58 PM" status="started" 
	// opportunity="1" statusDate="3/12/2009 1:11:58 PM" datecompleted="" pausecount="0" itemcount="29" ftcount="0" abnormalStarts="0" 
	// FormID="G10W::Apr142009::Form A">

    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class Opportunity
    {
		#region Properties

        //AM: added for 2011-2012
        [XmlIgnore]
        private Dictionary<string, TestSegment> segments;
        [XmlIgnore]
        public Dictionary<string, TestSegment> Segments
        {
            get
            {
                if (segments == null)
                {
                    segments = new Dictionary<string, TestSegment>();
                    foreach (TestSegment seg in segmentsList)
                        segments.Add(seg.ID, seg);
                }
                return segments;
            }
        }

        [XmlElement("Segment", Order=1)]
        public List<TestSegment> segmentsList { get; set; }

        [XmlIgnore]
        private List<TestAccomodation> accomodations;
        [XmlElement("Accommodation", Order=2)]
        public List<TestAccomodation> Accomodations
        {
            get
            {
                return accomodations;
            }
            set { accomodations = value; }
        }

        [XmlIgnore]
        private List<TestAccomodation> rtsAccommodations;
        [XmlIgnore]
        public List<TestAccomodation> RTSAccommodations
        {
            get
            {
                if (rtsAccommodations == null)
                    rtsAccommodations = new List<TestAccomodation>();
                return rtsAccommodations;
            }
        }

        [XmlElement("Score", Order = 3)]
        public List<Score> ScoresList;

        //AM: added for 2011-2012
        [XmlElement("GenericVariable", Order=4)]
        public List<GenericVariable> GenericVariables { get; set; }

        [XmlIgnore]
        private Scores scores;
        [XmlIgnore]
        public Scores Scores
        {
            get
            {
                if (scores == null)
                {
                    scores = new Scores(ScoresList);
                }
                return scores;
            }
            set
            {
                ScoresList = new List<Score>();
                foreach (Dictionary<string, Score> ds in value.TestScores.Values)
                    foreach (Score s in ds.Values)
                        ScoresList.Add(s);
                scores = value;
            }
        }

        [XmlElement("Item", Order=5)]
        public List<ItemResponse> ItemResponses;

        [XmlIgnore]
		private int opportunity;
        [XmlAttribute("opportunity")]
		public int OpportunityNumber
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
        [XmlAttribute("windowOpportunity")]
        public string windowOpportunityAsText
        {
            get { return (WindowOpportunityNumber.HasValue) ? WindowOpportunityNumber.ToString() : null; }
            set { WindowOpportunityNumber = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlIgnore]
        public int? WindowOpportunityNumber { get; set; }

        [XmlIgnore]
		private string opportunityID;
        [XmlAttribute("oppId")]
		public string OpportunityID
		{
			get
			{
				return opportunityID;
			}
            set { opportunityID = value; }
		}

        [XmlIgnore]
        private DateTime startDate;
        [XmlAttribute("startDate")]
		public DateTime StartDate
		{
			get
			{
                // will return the status date if there's no start date
                //  OriginalStartDate will always return the startDate 
                return startDate == DateTime.MinValue ? StatusDate : startDate;
			}
            set
            {
                startDate = value;
            }
		}

        // JT 5/11/2016: added for 2015-16 year
        [XmlIgnore]
        private string administrationCondition;
        [XmlAttribute("administrationCondition")]
        public string AdministrationCondition
        {
            get { return administrationCondition; }
            set { administrationCondition = value; }
        }


        /// <summary>
        /// The true startdate from the XML file.  If there is no start date in the file, this will
        /// be DateTime.MinValue and StartDate will return the StatusDate.
        /// </summary>
        [XmlIgnore]
        public DateTime OriginalStartDate
        {
            get
            {
                return startDate;
            }
        }

        [XmlIgnore]
        private DateTime qaSystemDateRecorded;
        [XmlIgnore]
        public DateTime QASystemDateRecorded
        {
            get
            {
                return qaSystemDateRecorded;
            }
			set
			{
				qaSystemDateRecorded = value;
			}
        }

        [XmlIgnore]
		private DateTime statusDate;
        [XmlAttribute("statusDate")]
		public DateTime StatusDate
		{
			get
			{
				return statusDate;
			}
            set
            {
                //AM: added setter for ppt.
                statusDate = value;
            }
		}

        [XmlIgnore]
		private string status;
        [XmlAttribute("status")]
		public string Status
		{
			get
			{
				return status;
			}
            set
            {
                status = value;
            }
		}

        [XmlIgnore]
        private string completeness;
        [XmlAttribute("completeStatus")]
        public string Completeness
        {
            get { return completeness; }
            set { completeness = value; }
        }

        [XmlIgnore]
		private int pauses;
        [XmlAttribute("pauseCount")]
		public int Pauses
		{
			get
			{
				return pauses;
			}
            set { pauses = value; }
		}

        [XmlIgnore]
		private int itemCount;
        [XmlAttribute("itemCount")]
		public int ItemCount
		{
			get
			{
				return itemCount;
			}
            set { itemCount = value; }
		}

        [XmlIgnore]
		private int fieldTestCount;
        [XmlAttribute("ftCount")]
		public int FieldTestCount
		{
			get
			{
				return fieldTestCount;
            }
            set { fieldTestCount = value; }
		}

        [XmlIgnore]
        public DateTime? CompletedDate;
        [XmlAttribute("dateCompleted")]
        public string completedDateAsText
        {
            get { return (CompletedDate.HasValue) ? System.Xml.XmlConvert.ToString(CompletedDate.Value, System.Xml.XmlDateTimeSerializationMode.Unspecified) : null; }
            set { CompletedDate = !string.IsNullOrEmpty(value) ? DateTime.Parse(value) : default(DateTime?); }
        }        

        /// <summary>
        /// Find the test "form" as a '-' seperated list of segment forms
        /// If a formID contains '-' wrap in [].
        /// Include blank formID for adaptive segments.
        /// </summary>
        [XmlIgnore]
        public string Forms
        {
            get
            {
                bool first = true;
                string formCat = "";
                foreach (TestSegment segment in GetSegmentsAsReadOnlySortedList())
                {
                    string form = segment.FormID ?? "";
                    if (form.Contains("-"))
                        form = "[" + form + "]";
                    if (first)
                    {
                        formCat = form;
                        first = false;
                    }
                    else
                    {
                        formCat = formCat + '-' + form;
                    }
                }
                return formCat;
            }
        }

        
        [XmlIgnore]
        public bool IsDiscrepancy
        {
            get
            {
                bool isDiscrep = false;
                // only a paper test can be a discrepancy
                if (TestMode == "scanned")
                {
                    GenericVariable discrepGenVar =
                        GetGenericVariableByContextName(GenericVariable.VariableContext.CALCULATED, "discrepant");
                    isDiscrep = (discrepGenVar != null && discrepGenVar.Value.Equals("Y", StringComparison.InvariantCultureIgnoreCase));
                }
                return isDiscrep;
            }
        }
        [XmlIgnore]
        public bool IsRescore
        {
            get
            {
                bool isRescore = false;

                GenericVariable rescoreIndGenVar =
                    GetGenericVariableByContextName(GenericVariable.VariableContext.DEMOGRAPHIC, GenericVariable.RESCORE_IND_VAR_NAME);
                isRescore = (rescoreIndGenVar != null && rescoreIndGenVar.Value.Equals("Y", StringComparison.InvariantCultureIgnoreCase));

                return isRescore;
            }
        }

        
        [Obsolete("Change to !IsPartial; more descriptive", false)]
        public bool IsCompletedDateValid()
        {
            return !IsPartial;
        }

        [XmlIgnore]
        public bool IsPartial
        {
            get
            {
                return (CompletedDate == null || CompletedDate.Equals(DateTime.MinValue));
            }
        }

        [XmlIgnore]
        public List<string> MergeParticipants
        {
            get
            {
                List<string> mergeParticipants = new List<string>();
                // only a paper test can currently participant in a merge
                if (TestMode == "scanned")
                {
                    GenericVariable mergeParticipantGenVar =
                        GetGenericVariableByContextName(GenericVariable.VariableContext.MERGE, "participants");
                    if (mergeParticipantGenVar != null && !String.IsNullOrEmpty(mergeParticipantGenVar.Value))
                        mergeParticipants = new List<string>(mergeParticipantGenVar.Value.Split(','));
                }
                return mergeParticipants;
            }
        }

        [XmlIgnore]
		private int gracePeriodRestarts;
        [XmlAttribute("gracePeriodRestarts")]
		public int GracePeriodRestarts
		{
			get
			{
				return gracePeriodRestarts;
			}
            set { gracePeriodRestarts = value; }
		}

        [XmlIgnore]
        private int abnormalStarts;
        [XmlAttribute("abnormalStarts")]
		public int AbnormalStarts
		{
			get
			{
				return abnormalStarts;
            }
            set { abnormalStarts = value; }
		}

        [XmlIgnore]
        private string serverName = string.Empty;
        [XmlAttribute("server")]
		public string ServerName
		{
			get
			{
				return serverName;
            }
            set { serverName = value; }
		}

        [XmlIgnore]
        private string databaseName = string.Empty;
        [XmlAttribute("database")]
		public string DatabaseName
		{
			get
			{
				return databaseName;
            }
            set { databaseName = value; }
		}

        // new for 2011-2012
        [XmlIgnore]
        private Guid key;
        [XmlAttribute("key")]
        public Guid Key
        {
            get
            {
                return key;
            }
            set { key = value; }
        }

        //AM: next 3 added for 2011 DEALT
        [XmlIgnore]
        private string taId = String.Empty;
        [XmlAttribute("taId")]
        public string TAID
        {
            get
            {
                return taId;
            }
            set { taId = value; }
        }

        [XmlIgnore]
        private string taName = String.Empty;
        [XmlAttribute("taName")]
        public string TAName
        {
            get
            {
                return taName;
            }
            set { taName = value; }
        }

        [XmlIgnore]
        private string sessionId = String.Empty;
        [XmlAttribute("sessionId")]
        public string SessionID
        {
            get
            {
                return sessionId;
            }
            set { sessionId = value; }
        }

        //new for 2011-2012
        [XmlIgnore]
        private DateTime dateForceCompleted;
        [XmlIgnore]
        public DateTime DateForceCompleted
        {
            get
            {
                return dateForceCompleted;
            }
            set { dateForceCompleted = value; }
        }
        [XmlAttribute("dateForceCompleted")]
        public string dateForceCompletedAsText
        {
            get { return (DateForceCompleted != default(DateTime)) ? System.Xml.XmlConvert.ToString(DateForceCompleted, System.Xml.XmlDateTimeSerializationMode.Unspecified) : null; }
            set { DateForceCompleted = !string.IsNullOrEmpty(value) ? DateTime.Parse(value) : default(DateTime); }
        }

        //new for 2011-2012
        [XmlIgnore]
        private string windowID;
        [XmlAttribute("windowId")]
        public string WindowID
        {
            get
            {
                return windowID;
            }
            set { windowID = value; }
        }

        [XmlIgnore]
        private string qaLevel = String.Empty;
        [XmlAttribute("qaLevel")]
        public string QALevel
        {
            get
            {
                return qaLevel;
            }
            set
            {
                qaLevel = value;
            }
        }

        [XmlIgnore]
        private string TestMode { get; set; }

        //AM: new for 2013-2014
        [XmlAttribute("clientName")]
        public string ClientName { get; set; }

        //AM: new for 2014-2015
        [XmlAttribute("reportingVersion")]
        public int ReportingVersion { get; set; }

        //Zach 10/31/2014: the below properties are for the OSS XML only. 
        //We will serialize/deserialize them, but they are ignored in the code
        /*
        <xs:attribute name="assessmentParticipantSessionPlatformUserAgent" use="required" />
            <!-- the first date of the first window for a given assessment.  Format = YYYY-MM-DD -->
            <xs:attribute name="effectiveDate" use="required" />
        */
        [XmlAttribute("assessmentParticipantSessionPlatformUserAgent")]
        public string AssessmentParticipantSessionPlatformUserAgent { get; set; }
        [XmlAttribute("effectiveDate")]
        public string EffectiveDate { get; set; }

		#endregion

        public Opportunity() { }
        public Opportunity(string oppID, DateTime startDate, int opportunity, string status, DateTime statusDate, int pauses, int itemCount, int fieldTestCount,
            DateTime? completedDate, int gracePeriodRestarts, int abnormalStarts, DateTime qaDate, string serverName, string databaseName,
            Guid key, string taId, string taName, string sessionId, DateTime dateForceCompleted, string windowID, int? windowOpportunity, string qaLevel, string mode, string clientName, int reportingVersion, string userAgent, string adminCondition)
        {
            this.opportunityID = oppID;
            this.StartDate = startDate;
            this.opportunity = opportunity;
            this.status = status;
            this.statusDate = statusDate;
            this.pauses = pauses;
            this.itemCount = itemCount;
            this.fieldTestCount = fieldTestCount;
            this.CompletedDate = completedDate;
			this.gracePeriodRestarts = gracePeriodRestarts;
			this.abnormalStarts = abnormalStarts;
            this.qaSystemDateRecorded = qaDate;
			this.serverName = serverName;
			this.databaseName = databaseName;
            this.key = key;
            this.taId = taId;
            this.taName = taName;
            this.sessionId = sessionId;
            this.dateForceCompleted = dateForceCompleted;
            this.windowID = windowID;
            this.WindowOpportunityNumber = windowOpportunity;
            this.qaLevel = qaLevel;
            this.TestMode = mode;
            this.ClientName = clientName;
            this.ReportingVersion = reportingVersion;
            this.segments = null;
            this.accomodations = new List<TestAccomodation>();
            this.segmentsList = new List<TestSegment>();
            this.GenericVariables = new List<GenericVariable>();
            this.ScoresList = new List<Score>();
            this.scores = null;
            this.ItemResponses = new List<ItemResponse>();
            this.rtsAccommodations = new List<TestAccomodation>();
            this.completeness = null;
            this.AdministrationCondition = adminCondition;
            this.AssessmentParticipantSessionPlatformUserAgent = userAgent;
        }

        #region Collections

        /// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <param name="code"></param>
		/// <param name="source"></param>
        public void AddAccomodation(string type, string value, string code, int segment, string source)
        {
			accomodations.Add(new TestAccomodation(type, value, code, segment, source, 0));
        }

        public void AddRTSAccomodation(string type, string value, string code, int segment, string source)
        {
            RTSAccommodations.Add(new TestAccomodation(type, value, code, segment, source, 0));
        }

        public void AddAccomodation(TestAccomodation testAccomodation)
        {
            accomodations.Add(testAccomodation);
        }

        public bool AddTestSegment(TestSegment segment)
        {
            bool added = false;
            if (!Segments.ContainsKey(segment.ID)
                || !segmentsList.Contains(segment))
            {
                //add the segment to both the dictionary and the list
                segments[segment.ID] = segment;
                segmentsList.Add(segment);
                added = true;
            }
            return added;
        }

        public void AddGenericVariable(string context, string name, string value)
        {
            GenericVariables.Add(new GenericVariable(context, name, value));
        }

        public void AddGenericVariable(GenericVariable.VariableContext context, string name, string value)
        {
            GenericVariables.Add(new GenericVariable(context, name, value));
        }

        public TestSegment GetSegment(string Id)
        {
            TestSegment returnVal = null;
            if (HasSegment(Id))
                returnVal = Segments[Id];
            return returnVal;
        }

        public bool HasSegment(string Id)
        {
            return Segments.ContainsKey(Id);
        }

        public bool HasSegment(TestBlueprint.SelectionAlgorithmType type)
        {
            return HasSegment(new List<TestBlueprint.SelectionAlgorithmType>() { type });
        }

        public bool HasSegment(List<TestBlueprint.SelectionAlgorithmType> types)
        {
            foreach (TestSegment segment in segmentsList)
                if (types.Contains(segment.Algorithm))
                    return true;
            return false;
        }

        public List<TestSegment> GetSegmentsAsReadOnlySortedList()
        {
            List<TestSegment> s = new List<TestSegment>(this.segmentsList);
            s.Sort(delegate(TestSegment s1, TestSegment s2) { return s1.Position.CompareTo(s2.Position); });
            return s;
        }

        public GenericVariable GetGenericVariableByContextName(GenericVariable.VariableContext context, string varName)
        {
            GenericVariable retVal = null;
            if (GenericVariables != null && GenericVariables.Count > 0)
            {
                retVal = GenericVariables.Find(delegate(GenericVariable v) { return v.Context == context && v.Name.Equals(varName, StringComparison.InvariantCultureIgnoreCase); });
            }
            return retVal;
        }

        public string GetFormKeys(string separator)
        {
            List<string> formKeys = new List<string>();
            foreach (TestSegment seg in GetSegmentsAsReadOnlySortedList())
                if (!String.IsNullOrEmpty(seg.FormKey))
                    formKeys.Add(seg.FormKey);
            return string.Join(separator, formKeys.ToArray());
        }

        //Zach 10/31/2014: this seems useless with the new implementation. This is handled in the XmlSerializer now.
/*        public void SetOpportunityNumbers(int oppNumber, int? windowOppNumber)
        {
            XmlNode opportunityNode = _xmlDoc.SelectSingleNode(_pathOpp);
            opportunityNode.Attributes["opportunity", ""].Value = oppNumber.ToString();

            // this methods is currently only used for paper tests, which will always have the windowOpportunity
            //  attribute.  Initial value will be blank.  In order to support online tests (future?), we'll create
            //  the attr if it's not there and just leave it blank if there's no value.
            XmlAttribute windowOppAttribute = opportunityNode.Attributes["windowOpportunity", ""];
            if (windowOppAttribute == null)
            {
                windowOppAttribute = _xmlDoc.CreateAttribute("windowOpportunity");
                opportunityNode.Attributes.Append(windowOppAttribute);
            }
            WindowOpportunityNumber = windowOppNumber == null ? null : windowOppNumber);
        }*/

        public bool AddScoreIfNew(Score score)
        {
            bool newScore = this.Scores.AddIfNew(score);
            if (newScore)
                this.ScoresList.Add(score);
            return newScore;
        }

        public void ClearScores()
        {
            this.scores = null;
            this.ScoresList = new List<Score>();
        }

        #endregion

    }//end class
}//end namespace
