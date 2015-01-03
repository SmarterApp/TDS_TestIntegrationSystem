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
using TDSQASystemAPI.DAL;
using AIR.Common;
using TDSQASystemAPI.Config;

namespace TDSQASystemAPI.TIS
{
    /// <summary>
    /// This class will merge the ScoreInfo nodes received from an outside vendor into the XML file in XmlRepository. This includes
    /// setting the item status and score, as well as the test status.
    /// 
    /// Zach 11/5/2014: Note that we also set the `scorerationale` property of the ItemResponse object to the string of the ScoreInfo XML we grab.
    /// </summary>
    internal class TISScoreMerger
    {
        private static bool KeepExpiredStatus
        {
            get
            {
                return Convert.ToBoolean(ServiceLocator.Resolve<ISystemConfigurationManager>().GetConfigSettingsValue(ConfigurationHolder.Instance.ClientName, "KeepExpiredStatus"));
            }
        }

        internal TISScoreMerger()
        {
        }

        internal List<ItemResponse> MergeScores(List<TestOpportunityItemScore> mergeFrom, List<ItemResponse> mergeTo, 
            Func<TestOpportunityItemScore, ItemResponse, bool> shouldMerge)
        {
            List<ItemResponse> merged = new List<ItemResponse>();
            foreach (TestOpportunityItemScore score in mergeFrom)
            {
                ItemResponse ir = mergeTo.FirstOrDefault(x => x.ItemKey == score.ItemKey && x.BankKey == score.BankKey);
                if (ir == null)
                    throw new NullReferenceException(string.Format("Item not found in opportunity when merging scores from outside vendor. ItemKey={0}, BankKey={1}, oppID={2}", score.ItemKey, score.BankKey, score.OppID));

                if (!shouldMerge(score, ir))
                    continue;
                
                ItemScoreInfo scoreInfo = null;
                try
                {
                    scoreInfo = score.GetItemScoreInfo();
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format("Failed serializing ItemScoreInfo. ItemKey={0}, BankKey={1}, oppID={2}, ScoreInfo text='{3}'.  Error: {4}", score.ItemKey, score.BankKey, score.OppID, score.ScoreInfo ?? "null", e.Message));
                }
                
                //finally merge the score info
                ir.ScoreInfo = scoreInfo;
                ir.ScoreRationale = score.ScoreInfo;
                ir.ScoreStatus = score.ScoreStatus.ToUpper(); // item scorestatus is all caps. // todo: validation?
                ir.presentedScore = score.Score == null ? -1 : (double)score.Score.Value;

                merged.Add(ir);
            }
            return merged;
        }

        internal List<ItemResponse> MergeScores(List<TestOpportunityItemScore> mergeFrom, List<ItemResponse> mergeTo)
        {
            return MergeScores(mergeFrom, mergeTo, delegate(TestOpportunityItemScore s, ItemResponse r) { return true; });
        }

        /// <summary>
        /// Merge scores into the testResult and return true if the testResult was changed
        /// </summary>
        /// <returns></returns>
        internal bool MergeScores(TestResult testResult, out bool qaProjectChanged)
        {
            qaProjectChanged = false;
            bool scoresMerged = false;
            bool unscoredMerged = false;
            if (testResult.Opportunity != null && testResult.Opportunity.ItemResponses != null)
            {
                // get all item scores, including responses
                //TODO: can optimize this by only pulling responses when we need to; responses may be big
                List<TestOpportunityItemScore> scores = TISScoreMergerDAL.GetItemScores(Convert.ToInt64(testResult.Opportunity.OpportunityID), true);
                
                // if we have scores, merge them in unless the response has changed
                // Note that we're also only merging if the score and scorestatus in the file are not the same as what
                //  we have in the db.  This is to handle resubmits of scored tests or resets/invalidations that
                //  were already merged, so that we don't think the file has changed when it hasn't and unnecessarily archive it.
                scoresMerged = MergeScores(scores.FindAll(s => s.ScoreStatus.Equals(TestResults.ScoringStatus.Scored.ToString(), StringComparison.InvariantCultureIgnoreCase)), 
                    testResult.Opportunity.ItemResponses, 
                    delegate(TestOpportunityItemScore s, ItemResponse r) {
                        return ((s.Response ?? "") == (r.Response ?? ""))
                            && !(s.ScoreStatus.Equals(r.ScoreStatus, StringComparison.InvariantCultureIgnoreCase) && (s.Score ?? -1).Equals(r.Score));
                    }).Count > 0;

                // we may have responses that have errored out or timed out or otherwise not been scored.
                //  merge them in too so that validation in TIS/QA will let us know about them
                unscoredMerged =
                    MergeScores(scores.FindAll(s => !s.ScoreStatus.Equals(TestResults.ScoringStatus.Scored.ToString(), StringComparison.InvariantCultureIgnoreCase) 
                        && !s.ScoreStatus.Equals(TestResults.ScoringStatus.WaitingForMachineScore.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                                    testResult.Opportunity.ItemResponses).Count > 0; 

                //if we merged all scores into a completed or expired test, mark the opp as scored
                if (scoresMerged && !unscoredMerged
                        && (testResult.Opportunity.Status == "completed"
                            || testResult.Opportunity.Status == "submitted"
                            || testResult.Opportunity.Status == "reported"
                            || (testResult.Opportunity.Status == "expired" && !KeepExpiredStatus))
                        && scores.FirstOrDefault(s => s.ScoreStatus.Equals(TestResults.ScoringStatus.WaitingForMachineScore.ToString(), StringComparison.InvariantCultureIgnoreCase)) == null)
                {
                    testResult.Opportunity.Status = "scored";
                    qaProjectChanged = testResult.RefreshQAProject();
                }
            }
            return scoresMerged || unscoredMerged;
        }
    }
}
