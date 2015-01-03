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
        private int? points;
        /// <summary>
        /// Score point for this dimension
        /// </summary>
        [XmlAttribute("scorePoint")]
        public string pointsAsText
        {
            get { return (points.HasValue) ? points.ToString() : null; }
            set { points = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }


        /// <summary>
        /// Confidence level associated with this score
        /// </summary>
        [XmlAttribute("confLevel")]
        public string ConfLevel { get; set; }

        [XmlIgnore]
        private int? maxScore;
        /// <summary>
        /// Max score possible for this dimension
        /// </summary>
        [XmlAttribute("maxScore")]
        public string maxScoreAsText
        {
            get { return (maxScore.HasValue) ? maxScore.ToString() : null; }
            set { maxScore = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }


        /// <summary>
        /// Dimension that this score is for
        /// </summary>
        [XmlAttribute("scoreDimension")]
        public string Dimension { get; set; }

        [XmlIgnore]
        private ScoringStatus? status;
        /// <summary>
        /// Status of this score
        /// </summary>
        [XmlAttribute("scoreStatus")]
        public string statusAsText
        {
            get { return (status.HasValue) ? status.ToString() : null; }
            set { status = !string.IsNullOrEmpty(value) ? (ScoringStatus)Enum.Parse(typeof(ScoringStatus), value) : default(ScoringStatus?); }
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
        {
            this.points = points;
            this.maxScore = maxPoints;  // -1 indicates unknown
            this.status = status;
            this.Dimension = dimension ?? "overall";
            this.Rationale = rationale;
        }

        public ItemScoreInfo(ItemScoreInfo other)
        {
            this.points = other.points;
            this.maxScore = other.maxScore;
            this.status = other.status;
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
