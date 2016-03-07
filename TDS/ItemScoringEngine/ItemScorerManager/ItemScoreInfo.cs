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
using System.Xml.Serialization;
using AIR.Common.Xml;

namespace TDS.ItemScoringEngine
{
    /// <summary>
    /// Class that represents the score details.
    /// </summary>
    [Serializable]
    [XmlRoot("ScoreInfo")]
    public class ItemScoreInfo
    {
        /// <summary>
        /// Score point for this dimension
        /// </summary>
        [XmlAttribute("scorePoint")]
        public int Points { get; set; }

        /// <summary>
        /// Confidence level associated with this score
        /// </summary>
        [XmlAttribute("confLevel")]
        public string ConfLevel { get; set; }

        /// <summary>
        /// Max score possible for this dimension
        /// </summary>
        [XmlAttribute("maxScore")]
        public int MaxScore { get; set; }

        /// <summary>
        /// Dimension that this score is for
        /// </summary>
        [XmlAttribute("scoreDimension")]
        public string Dimension { get; set; }

        /// <summary>
        /// Status of this score
        /// </summary>
        [XmlAttribute("scoreStatus")]
        public ScoringStatus Status { get; set; }
        
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
        
        public ItemScoreInfo()
        {
        }

        public ItemScoreInfo(int points, int maxPoints, ScoringStatus status, string dimension, ScoreRationale rationale)
        {
            Points = points;
            MaxScore = maxPoints;  // -1 indicates unknown
            Status = status;
            Dimension = dimension ?? "overall";
            Rationale = rationale;
        }
    }
}