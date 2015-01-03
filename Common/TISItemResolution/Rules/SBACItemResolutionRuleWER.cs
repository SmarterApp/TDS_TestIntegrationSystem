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

using TDSQASystemAPI.TestResults;

namespace TISItemResolution.Rules
{
    public class SBACItemResolutionRuleWER : TISItemResolutionRule
    {
        public SBACItemResolutionRuleWER() : base() { }

        /// <summary>
        /// Evidence, purpose, and conventions are the scoring dimensions for the writing essays. Scores for evidence and purpose dimensions will be averaged, and the average will be rounded up.
        /// The conventions dimension gets renamed "C", and the new merged dimension is named "D"
        /// </summary>
        /// <param name="scoreInfo"></param>
        /// <returns></returns>
        public override ItemScoreInfo ResolveItemScore(ItemScoreInfo scoreInfo)
        {
            if (scoreInfo == null) return null;

            //only do the merging when given an overall dimension
            if (!scoreInfo.Dimension.Equals("Overall", StringComparison.InvariantCultureIgnoreCase))
                throw new NullReferenceException("ScoreInfo does not contain the Overall dimension. This is required for WER items in SBACItemResolutionRuleWER");

            //check we have subscores
            if (scoreInfo.SubScores == null)
                throw new NullReferenceException("ScoreInfo does not contain any subscores. Conventions, Evidence/Elaboration, and Organization/Purpose are required subscore dimensions for WER items in SBACItemResolutionRuleWER");

            //grab conventions, evidence, and purpose dimensions for processing
            ItemScoreInfo conventions = scoreInfo.SubScores.FirstOrDefault(x => x.Dimension.Equals("Conventions"));
            if (conventions == null)
                throw new NullReferenceException("Conventions dimension was not present. This is required for WER items in SBACItemResolutionRuleWER");

            ItemScoreInfo evidence = scoreInfo.SubScores.FirstOrDefault(x => x.Dimension.Equals("Evidence/Elaboration"));
            if (evidence == null)
                throw new NullReferenceException("Evidence/Elaboration dimension was not present. This is required for WER items in SBACItemResolutionRuleWER");

            ItemScoreInfo purpose = scoreInfo.SubScores.FirstOrDefault(x => x.Dimension.Equals("Organization/Purpose"));
            if (purpose == null)
                throw new NullReferenceException("Organization/Purpose dimension was not present. This is required for WER items in SBACItemResolutionRuleWER");

            //averaging of purpose/evidence. We round up and convert to int.
            double avg = (double)(Convert.ToInt16(evidence.pointsAsText) + Convert.ToInt16(purpose.pointsAsText)) / 2.0;
            int avgInt = (int)Math.Round(avg, MidpointRounding.AwayFromZero);
            int maxScore = (int)Math.Round(((Convert.ToInt16(evidence.maxScoreAsText) + Convert.ToInt16(purpose.maxScoreAsText)) / 2.0), MidpointRounding.AwayFromZero);

            //add new ScoreInfo node to hold the averaged dimension with dimension name "D".
            //We add a ScoreInfo node to the SubScoreList of this new "D" dimension node with dimension = "Final"
            //TODO: what if one or both dimensions collapsed have condition codes? For now just hardcoded to empty string
            ItemScoreInfo avgDim = new ItemScoreInfo(avgInt, maxScore, ScoringStatus.Scored, "D", GetRationale(null));
            ItemScoreInfo avgDimSubScore = new ItemScoreInfo(avgInt, maxScore, ScoringStatus.Scored, "Final", null/*new ScoreRationale() { Msg = "{\"scorerID\":\"SBACItemResolutionRuleWER\"}" }*/);
            avgDim.SubScores = new List<ItemScoreInfo>();
            avgDim.SubScores.Add(avgDimSubScore);
            scoreInfo.SubScores.Add(avgDim);

            // add new ScoreInfo node which copies the conventions dimension but changes name to "C"
            ItemScoreInfo newConventionDim = new ItemScoreInfo(conventions);
            newConventionDim.Dimension = "C";
            newConventionDim.Rationale = GetRationale(newConventionDim.Rationale);
            if (newConventionDim.SubScores == null)
                newConventionDim.SubScores = new List<ItemScoreInfo>();

            ItemScoreInfo newConvDimInitialSubScore = newConventionDim.SubScores.FirstOrDefault(x => x.Dimension.Equals("Initial"));

            if (newConventionDim.SubScores.Count != 1 || newConvDimInitialSubScore == null)
                throw new NullReferenceException(string.Format("The Conventions dimension must contain only the Initial read. This dimension contains {0} reads, and no Initial read. These requirements are for WER items in resolution rule SBACItemResolutionRuleWER", newConventionDim.SubScores.Count));
            
            newConvDimInitialSubScore.Dimension = "Final";
            newConvDimInitialSubScore.Rationale = GetRationale(newConvDimInitialSubScore.Rationale);

            scoreInfo.SubScores.Add(newConventionDim);

            return scoreInfo;
        }

        /// <summary>
        /// Replace scorerID with SBACItemResolutionRuleWER but keep any other properties in the JSON string. If no rationale is passed then
        /// return null
        /// </summary>
        /// <param name="rationale"></param>
        /// <returns></returns>
        private ScoreRationale GetRationale(ScoreRationale rationale)
        {
            ScoreRationale ret = rationale == null ? null : new ScoreRationale(rationale);
            if (ret != null && ret.Msg != null)
            {
                int idIdx = ret.Msg.IndexOf("scorerID\":\"");
                //replace scorerID value with SBACItemResolution
                if (idIdx >= 0)
                {
                    idIdx += "scorerID\":\"".Length;
                    int idEndIdx = ret.Msg.IndexOf("\"", idIdx);
                    ret.Msg = ret.Msg.Substring(0, idIdx) + "SBACItemResolutionRuleWER" + ret.Msg.Substring(idEndIdx);
                }
            }
            return ret;
        }
    }
}
