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
using System.Xml.Serialization;
using System.Linq;

using TDSQASystemAPI.Config;
using ScoringEngine.ConfiguredTests;

namespace TDSQASystemAPI.TestResults
{
	/// <summary>
	/// MC Item element format...
	/// <item position="1" airkey="1264" operational="1" scorepoints="1" score="0" response="D" answerkey="B" admindate="6/3/2008 11:17:28 AM"
	/// responsedate="6/3/2008 11:18:21 AM" responsetime="53" comment="" clientID="M0551290" strand="Measurement" mimetype="" >
	/// 
	/// Handscored item element format...
	/// <item position="20" airkey="14026" operational="1" IsSelected="1" Format="CR" scorepoints="1" score="0" response="B" admindate="10/6/2009 5:37:53 PM" responsedate="10/6/2009 5:41:14 PM" answerkey="C" numberVisits="1" clientID="R0721080" strand="Read-6-DU" contentLevel="Read-6-DU|CCG3|CS1" mimetype="">
	///		<comment><![CDATA[]]></comment>
	///		<HSscores airkey="14026" hsSequence="1">
	///			<Score Name="UCMX_OE_FIRSTCC_4" Value="D" CompName="CONDITION CODES" Dimension="" Sequence="1" Type="INITIAL" UserID="744" UserLastName="SCORER" UserFirstName="SIVA" />
	///			<Score Name="UCMX_OE_SECONDCC_4" Value="C" CompName="CONDITION CODES" Dimension="" Sequence="1" Type="RELIABILITY" UserID="745" UserLastName="SCORER2" UserFirstName="SIVA" /> 
	///			<Score Name="UCMX_OE_THIRDCC_4" Value="D" CompName="DIMENSIONS" Dimension="GRAMMAR" Sequence="1" Type="RESOLUTION" UserID="746" UserLastName="SUPERVISOR 2" UserFirstName="SIVA" /> 
	///		</HSscores>
	/// </item>
    /// </summary>
    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class ItemResponse
    {
        [XmlIgnore]
		private Dictionary<string, string> _reportedFeatures = new Dictionary<string, string>();

        public enum ItemScoreStatus
        {
            NOTSCORED,
            SCORED,
            WAITINGFORMACHINESCORE,
            NOSCORINGENGINE,
            SCORINGERROR,
            APPEALED
        }

        #region Properties

        //AM: comment moved to tdsreport node for 2011-2012 season
        //private string comment;
        //public string Comment
        //{
        //    get
        //    {
        //        return comment;
        //    }
        //}
        [XmlIgnore]
        private string scoreRationale;
        [XmlIgnore]
        public string ScoreRationale
        {
            get
            {
                return scoreRationale;
            }
            set
            {
                scoreRationale = value;
            }
        }

        [XmlElement("Response", Order = 1)]
        public Response ResponseObject;

        [XmlIgnore]
        public string Response
        {
            get
            {
                return ResponseObject.ResponseString;
            }
            set
            {
                ResponseObject.ResponseString = value;
            }
        }

        /// <summary>
        /// Whether or not the item has a ScoreInfo node as a child of scorerationale.
        /// This is currently used for ER items that are machine-scored on the first read
        /// across multiple dimensions.  Ex:
        /// <scorerationale>
        ///  <ScoreInfo scorePoint="-1" scoreDimension="overall" scoreStatus="Scored">
        ///    <SubScoreList>
        ///      <ScoreInfo scorePoint="0" scoreDimension="evid" scoreStatus="Scored">
        ///        <SubScoreList />
        ///      </ScoreInfo>
        ///      <ScoreInfo scorePoint="0" scoreDimension="purp" scoreStatus="Scored">
        ///        <SubScoreList />
        ///      </ScoreInfo>
        ///      <ScoreInfo scorePoint="0" scoreDimension="conv" scoreStatus="Scored">
        ///        <SubScoreList />
        ///      </ScoreInfo>
        ///    </SubScoreList>
        ///  </ScoreInfo>
        ///</scorerationale>
        ///We're not currently parsing this, so we need to know how to handle it.  Any other
        ///information contained within the scorrationale will continue to be treated as CDATA.
        ///This will be handled as an XML Fragment.  Note that it will be stored in the DoR as a string, same as CDATA.
        /// </summary>
        [XmlIgnore]
        public bool HasScoreInfo
        {
            get
            {
                if (!hasScoreInfo.HasValue)
                {
                    hasScoreInfo = ScoreInfo != null;
                }
                return hasScoreInfo.Value;
            }
            private set
            {
                hasScoreInfo = value;
            }
        }
        [XmlIgnore]
        private bool? hasScoreInfo;

        //Zach 10/31/2014: the ScoreInfo property is for the OSS XML only. 
        //We will serialize/deserialize them, but they are ignored in the code
        [XmlElement(Order=2)]
        public ItemScoreInfo ScoreInfo;

        [XmlIgnore]
		private int operational;
        [XmlAttribute("operational")]
		public int Operational
		{
			get
			{
				return operational;
            }
            set { operational = value; }
		}

        [XmlAttribute("score")]
        public double presentedScore;
        [XmlIgnore]
		public double PresentedScore
		{
			get
			{
                //if ((presentedScore >= 0.0) && (presentedScore <= ScorePoints))
                //    return presentedScore;
                //else return -1.0;
                return presentedScore;
			}

            set
            {
                presentedScore = value;
            }
		}

        //AM: added access to original score w/o checks above in PresentedScore
        [XmlIgnore]
        public double Score
        {
            get
            {
                return presentedScore;
            }
        }

        [XmlIgnore]
        private double? scorePoints;
        [XmlIgnore]
		public double ScorePoints
		{
			get
			{
                // if scorePoints is set, return this value
                //TODO: even want to do this, or just ignore it?  OSS format doesn't support it.
                if (scorePoints != null)
                    return scorePoints.Value;

                // otherwise, get the score points from the item bank config
                TestItemScoreInfo overall = null;
                if (testItem != null && testItem.ScoreInfo != null && testItem.ScoreInfo.Count > 0
                    && (overall = testItem.ScoreInfo.FirstOrDefault(si => String.IsNullOrEmpty(si.Dimension) || si.Dimension.Equals("overall", StringComparison.InvariantCultureIgnoreCase))) != null)
                    return overall.ScorePoints;

                // no value and no BP, just return 0.
                return default(double);
			}
		}

        //AM 9/30/2010: new attribute
        public bool ShouldSerializeScoreStatus() { return !String.IsNullOrEmpty(ScoreStatus); }
        [XmlIgnore]
        private string scoreStatus;
        [XmlAttribute("scoreStatus")]
        public string ScoreStatus
        {
            get
            {
                return scoreStatus;
            }
            set
            {
                scoreStatus = value;
            }
        }

        //todo: look at this further
        [XmlIgnore]
        private TestItem testItem;
        [XmlIgnore]
		public TestItem TestItem
		{
			get
			{
				return testItem;
			}
            set
            {
                testItem = value;
            }
		}

        //AML: added this for paper tests with CR items
        //  Will be "reference" for paper response and Empty
        //  for online.  DTD also supports "value", but TDS will
        //  not be providing this.
        [XmlIgnore]
        public string ResponseType
        {
            get
            {
                return ResponseObject.type;
            }
        }

        [XmlIgnore]
        private string answerKey;
        [XmlIgnore]
		public string AnswerKey
		{
			get
			{
				return answerKey;
			}
		}

        [XmlIgnore]
        private string strandKey;
        [XmlAttribute("strand")]
		public string StrandKey
		{
			get
			{
				return strandKey;
			}
            set { strandKey = value; }
		}

        [XmlIgnore]
        private string contentLevel;
        [XmlAttribute("contentLevel")]
		public string ContentLevel
		{
			get
			{
				return contentLevel;
            }
            set { contentLevel = value; }
		}

        [XmlIgnore]
        private int position;
        [XmlAttribute("position")]
		public int Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

        [XmlIgnore]
        private long bankKey;
        [XmlAttribute("bankKey")]
        public long BankKey
        {
            get
            {
                return bankKey;
            }
            set
            {
                bankKey = value;
            }
        }

        [XmlIgnore]
		private long itemKey;
		/// <summary>
		/// The name of this item, which is its ITS key, and in XML - the airkey attribute.
        /// </summary>
        [XmlAttribute("key")]
		public long ItemKey
		{
			get
			{
				return itemKey;
            }
            set { itemKey = value; }
		}

        [XmlIgnore]
        public string ItemName
        {
            get
            {
                return string.Format("{0}-{1}", bankKey, itemKey);
            }
        }

        [XmlIgnore]
        private string clientKey;
        [XmlAttribute("clientId")]
		public string ClientKey
		{
			get
			{
				return clientKey;
            }
            set { clientKey = value; }
		}

        [XmlIgnore]
        private DateTime adminDate;
        [XmlAttribute("adminDate")]
		public DateTime AdminDate
		{
			get
			{
				return adminDate;
            }
            set { adminDate = value; }
		}

        [XmlIgnore]
		public DateTime ResponseDate
		{
			get
			{
				return ResponseObject.date;
			}
            set
            {
                //AM 7/12/2010: added setter for paper test MC items
                ResponseObject.date = value;
            }
		}

        [XmlIgnore]
        private bool isSelected
        {
            get
            {
                return Convert.ToBoolean(IsSelectedInt);
            }
            set
            {
                IsSelectedInt = Convert.ToInt32(value);
            }
        }
        [XmlIgnore]
		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
            set
            {
                //AM 7/12/2010: added setter for paper test MC items
                isSelected = value;
            }
		}

        [XmlAttribute("isSelected")]
        public int IsSelectedInt = 0;

        [XmlIgnore]
        private string format = string.Empty;
        [XmlAttribute("format")]
		public string Format
		{
			get
			{
				return format;
            }
            set { format = value; }
		}

        //AM 8-30-2010 - added back
        [XmlIgnore]
        private int numberVisits = 0;
        [XmlAttribute("numberVisits")]
        public int NumberVisits
        {
            get
            {
                return numberVisits;
            }
            set { numberVisits = value; }
        }

        //AM 7/28/2010: added as required attr to TDS file
        [XmlIgnore]
        private int pageVisits = 0;
        [XmlAttribute("pageVisits")]
		public int PageVisits
		{
			get
			{
				return pageVisits;
            }
            set { pageVisits = value; }
		}

        //AM 7/28/2010: added as required attr to TDS file
        [XmlIgnore]
        private int pageNumber = 0;
        [XmlAttribute("pageNumber")]
        public int PageNumber
        {
            get
            {
                return pageNumber;
            }
            set
            {
                pageNumber = value;
            }
        }

        //AM 7/28/2010: added as required attr to TDS file
        [XmlIgnore]
        private int pageTime = 0;
        [XmlAttribute("pageTime")]
        public int PageTime
        {
            get
            {
                return pageTime;
            }
            set { pageTime = value; }
        }

        // JT 5/11/2016: added for 2015-16
        [XmlIgnore]
        private double responseDuration = 0.0;
        [XmlAttribute("responseDuration")]
        public double ResponseDuration
        {
            get { return responseDuration; }
            set { responseDuration = value; }
        }

        //AM: new for 2011
        [XmlIgnore]
        private bool dropped
        {
            get
            {
                return Convert.ToBoolean(droppedInt);
            }
            set
            {
                droppedInt = Convert.ToInt32(value);
            }
        }
        [XmlIgnore]
        public bool Dropped
        {
            get
            {
                return dropped;
            }
            set { dropped = value; }
        }

        [XmlAttribute("dropped")]
        public int droppedInt = 0;

        [XmlIgnore]
        private string mimeType = string.Empty;
        [XmlAttribute("mimeType")]
		public string MimeType
		{
			get
			{
				return mimeType;
            }
            set { mimeType = value; }
		}

        [XmlIgnore]
        private bool itemHandscoreSet = false;
        [XmlIgnore]
		public bool ItemHandscoreSet
		{
			get
			{
				return itemHandscoreSet;
			}
		}

        [XmlIgnore]
		private ItemScore itemHandScore;
		/// <summary>
		/// This contains all the score information from the Handscoring extract.
        /// </summary>
        [XmlIgnore]
		public ItemScore ItemHandScore
		{
			get
			{
				return itemHandScore;
			}
			set
			{
				if (!itemHandscoreSet)
				{
					itemHandScore = value;
					itemHandscoreSet = true;
				}
			}
		}

        [XmlIgnore]
        public List<ErasureResponse> ErasureResponses { get; private set; }

        [XmlAttribute("segmentId")]
        public string SegmentID { get; set; }

        //private Dictionary<string, double> irtScores = new Dictionary<string,double>(); // key is Dimension
        //public Dictionary<string, double> IRTScores
        //{
        //    get
        //    {
        //        return irtScores;
        //    }
        //}

        #endregion

        public ItemResponse() { }
        public ItemResponse(TestItem ti, int position, long bankKey, long itemname, string clientKey, int operational, double scorePoints, double score, string scoreStatus,
			string response, string responseType, string answerKey, DateTime adminDate, DateTime responseDate, string scoreRationale, string strandKey, string contentLevel,
            bool isSelected, string format, int numberVisits, int pagenumber, int pagetime, int pagevisits, string mimeType, bool dropped, ItemScore itemHandScore, 
            List<ErasureResponse> erasureResponses, string segmentId, bool hasScoreInfo, ItemScoreInfo scoreInfo)
        {
            this.testItem = ti;
            this.position = position;
            this.bankKey = bankKey;
            this.itemKey = itemname;
            this.clientKey = clientKey;
            this.operational = operational;
            this.scorePoints = scorePoints;
            this.presentedScore = score;
            this.scoreStatus = scoreStatus;
            this.ResponseObject = new Response(response, responseType, responseDate);
            this.answerKey = answerKey;
            this.adminDate = adminDate;
            this.scoreRationale = scoreRationale;
            this.strandKey = strandKey;
			this.contentLevel = contentLevel;
			this.isSelected = isSelected;
			this.format = format;
            this.numberVisits = numberVisits;
            this.pageNumber = pagenumber;
            this.pageTime = pagetime;
			this.pageVisits = pagevisits;
			this.mimeType = mimeType;
            this.dropped = dropped;
			this.itemHandScore = itemHandScore;
            this.ErasureResponses = erasureResponses;
            this.SegmentID = segmentId;
            this.HasScoreInfo = hasScoreInfo;
            this.ScoreInfo = scoreInfo;

            if (itemHandScore != null)
                this.itemHandscoreSet = true;
            //if (this.format == "MC" || this.ItemHandScore == null || (this.itemHandScore != null && this.itemHandScore.HSScores.Count == 0))
            //{
            //    if (this.PresentedScore != -1)
            //        this.irtScores[""] = this.PresentedScore;
            //}
            //else
            //{
            //    foreach (HandScore handScore in this.itemHandScore.HSScores)
            //    {
            //        if (handScore.Type == HandScore.ReadType.Final)
            //        {
            //            if (handScore.CompName == HandScore.CompNameAttribute.ConditionCodes)
            //                if (ti == null)
            //                    // When used from the TIS no blueprint available, so we can't distribute the CC
            //                    // accross dimensions. See HandleCC.
            //                    this.irtScores[handScore.Dimension.ToLower()] = 0.0;
            //                else
            //                    foreach(TestItemScoreInfo si in ti.ScoreInfo)
            //                        this.irtScores[si.Dimension.ToLower()] = 0.0;
            //            else
            //                this.irtScores[handScore.Dimension.ToLower()] = Convert.ToDouble(handScore.Value);
            //        }
            //    }
            //}
        }

        public ItemResponse DeepCopy()
        {
            ItemResponse copy = (ItemResponse)this.MemberwiseClone();
            copy._reportedFeatures = new Dictionary<string,string>(_reportedFeatures);
            if (testItem != null)
                copy.testItem = testItem.DeepCopy();
            if (itemHandScore != null)
                copy.itemHandScore = itemHandScore.DeepCopy();
            if (ErasureResponses != null)
                copy.ErasureResponses = ErasureResponses.ConvertAll(x => x.DeepCopy());
            copy.ResponseObject = this.ResponseObject.DeepCopy();
            return copy;
        }

        public void AddFeature(string featureName, string featureValue)
        {
            if (_reportedFeatures.ContainsKey(featureName))
                _reportedFeatures.Remove(featureName);
            _reportedFeatures[featureName] = featureValue;
        }

        //public double Score(string dimension)
        //{
        //    if (!irtScores.ContainsKey(dimension.ToLower()))
        //        throw new Exception("No irtScore for dimension " + dimension);
        //    return irtScores[dimension.ToLower()];
        //}

        //public void SetScore(string dimension, double value)
        //{
        //    irtScores[dimension.ToLower()] = value;
        //}

        //public void ClearScores()
        //{
        //    irtScores.Clear();
        //}

        // Check if there is some recode rule that causes this score to be treated as not presented
        // e.g. ELPA condition code A
        public bool TreatAsNotPresented(TestItemScoreInfo sc)
        {
            if (sc.RecodeRuleName.StartsWith("GRR("))
                return false;
            switch (sc.RecodeRuleName)
            {
                case "":
                case "Utah_Writing_ER_Recode":
                    return false;
                case "ELPA_CC_Rule":
                case "ELPA_Illocution_Recode":
                case "ELPA_Grammar_Recode":
                    if (this.ItemHandScore == null || this.itemHandScore.HSScores.Count == 0)
                        return false;
                    else
                    {
                        foreach (HandScore handScore in this.itemHandScore.HSScores)
                            if (handScore.Type == HandScore.ReadType.Final
                                && handScore.CompName == HandScore.CompNameAttribute.ConditionCodes
                                && handScore.Value == "A")
                                return true;
                    }
                    return false;
                case "DCAS_Alt_EI_Recode":
                case "DCAS_Alt_PresentedRule":
                    if (this.Response == "AL") return true;
                    return false;
                default:
                    throw new Exception("Item " + itemKey.ToString() + " has an unknown recode rule: " + sc.RecodeRuleName);
            }
        }

        /// <summary>
        /// Clears hand scores for the item
        /// </summary>
        public void ClearHandScore()
        {
            itemHandscoreSet = false;
            itemHandScore = null;
        }
    }

    /// <summary>
    /// Wrapper to allow a List<ItemResponses> to be serialized as a document with a root node of ItemResponses
    /// </summary>
    [Serializable]
    [XmlRoot("ItemResponses")]
    public class ItemResponseListAdapter : List<ItemResponse>
    {
        public ItemResponseListAdapter() { }
        public ItemResponseListAdapter(List<ItemResponse> itemResponses)
            : base(itemResponses)
        { }
    }
}
