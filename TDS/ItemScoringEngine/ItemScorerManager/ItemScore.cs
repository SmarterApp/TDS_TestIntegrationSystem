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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using AIR.Common.Xml;

namespace TDS.ItemScoringEngine
{
    /// <summary>
    /// Class that represents the output (final or intermim) of a scoring engine    
    /// </summary>
    [Serializable]
    [XmlRoot("Score")]
    public class ItemScore
    {
        [XmlElement("ScoreInfo")]
        public ItemScoreInfo ScoreInfo { get; set; }

        /// <summary>
        /// Placeholder to associate add'l info related to this student response (such as testeeid, position etc) 
        /// </summary>
        [XmlElement("ContextToken")]
        public XmlCDataText ContextToken { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public long ScoreLatency { get; set; }

        public ItemScore()
        {
        }

        public ItemScore(ItemScoreInfo scoreInfo, string contextToken) : this()
        {
            ScoreInfo = scoreInfo;
            ContextToken = contextToken;
        }

        public ItemScore(int scorePoint, int maxScore, ScoringStatus status, string dimension, ScoreRationale rationale, string contextToken)
            : this()
        {
            ScoreInfo = new ItemScoreInfo(scorePoint, maxScore, status, dimension, rationale);
            ContextToken = contextToken;
        }

        public ItemScore(int scorePoint, int maxScore, ScoringStatus status, string dimension, ScoreRationale rationale, List<ItemScoreInfo> childScores, string contextToken)
            : this()
        {
            ScoreInfo = new ItemScoreInfo(scorePoint, maxScore, status, dimension, rationale);
            ScoreInfo.SubScores = childScores;
            ContextToken = contextToken;
        }        
    }
}