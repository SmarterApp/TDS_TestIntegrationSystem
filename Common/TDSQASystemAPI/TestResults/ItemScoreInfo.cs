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
using System.Xml.Serialization;

namespace TDSQASystemAPI.TestResults
{
    /// <summary>
    /// Class that represents the score details.
    /// </summary>
    [Serializable]
    [XmlRoot("ScoreInfo")]
    public class ItemScoreInfo
    {
        public enum ScoreStatus
        {
            NotScored,
            Scored,
            WaitingForMachineScore,
            NoScoringEngine,
            ScoringError
        }

        [XmlIgnore]
        public int? Points { get; private set; }
        /// <summary>
        /// Score point for this dimension
        /// </summary>
        [XmlAttribute("scorePoint")]
        public string pointsAsText
        {
            get { return (Points.HasValue) ? Points.ToString() : null; }
            set { Points = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        /// <summary>
        /// Confidence level associated with this score
        /// </summary>
        [XmlAttribute("confLevel")]
        public string ConfLevelText { get; set; }

        [XmlIgnore]
        public double? ConfLevel
        {
            get
            {
                return String.IsNullOrEmpty(ConfLevelText) ? default(double?) : double.Parse(ConfLevelText);
            }
        }

        [XmlIgnore]
        public int? MaxScore { get; private set; }
        /// <summary>
        /// Max score possible for this dimension
        /// </summary>
        [XmlAttribute("maxScore")]
        public string maxScoreAsText
        {
            get { return (MaxScore.HasValue) ? MaxScore.ToString() : null; }
            set { MaxScore = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        /// <summary>
        /// Dimension that this score is for
        /// </summary>
        [XmlAttribute("scoreDimension")]
        public string Dimension { get; set; }

        [XmlIgnore]
        public ScoringStatus? Status { get; private set; }
        /// <summary>
        /// Status of this score
        /// </summary>
        [XmlAttribute("scoreStatus")]
        public string statusAsText
        {
            get { return (Status.HasValue) ? Status.ToString() : null; }
            set { Status = !string.IsNullOrEmpty(value) ? (ScoringStatus)Enum.Parse(typeof(ScoringStatus), value) : default(ScoringStatus?); }
        }

        /// <summary>
        /// ScoreRationale for this score for this dimension
        /// </summary>
        [XmlElement("ScoreRationale")]
        public ScoreRationale Rationale { get; set; }

        /// <summary>
        /// Any children (sub-dimensional) scores associated with this compound score
        /// </summary>
        [XmlArray("SubScoreList")]
        [XmlArrayItem("ScoreInfo")]
        public List<ItemScoreInfo> SubScores { get; set; }
        public bool SubScoresSpecified { get { return SubScores != null && SubScores.Count > 0; } }

        public ItemScoreInfo()
        {

        }

        public ItemScoreInfo(int points, int maxPoints, ScoringStatus status, string dimension, ScoreRationale rationale)
            : this(points, maxPoints, null, status, dimension, rationale)
        {
        }

        public ItemScoreInfo(int points, int maxPoints, string confLevel, ScoringStatus status, string dimension, ScoreRationale rationale)
        {
            this.Points = points;
            this.MaxScore = maxPoints;  // -1 indicates unknown
            this.ConfLevelText = confLevel;
            this.Status = status;
            this.Dimension = dimension ?? "overall";
            this.Rationale = rationale;
        }

        public ItemScoreInfo(ItemScoreInfo other)
        {
            this.Points = other.Points;
            this.MaxScore = other.MaxScore;
            this.ConfLevelText = other.ConfLevelText;
            this.Status = other.Status;
            this.Dimension = other.Dimension;
            this.Rationale = other.Rationale == null ? null : new ScoreRationale(other.Rationale);
            if (other.SubScores != null)
            {
                this.SubScores = new List<ItemScoreInfo>();
                foreach (ItemScoreInfo otherSubScore in other.SubScores)
                    this.SubScores.Add(new ItemScoreInfo(otherSubScore));
            }
        }
    }
}
