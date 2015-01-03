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

namespace TDSQASystemAPI.Routing.ItemScoring
{
    public class ItemScoringDaemonTarget : Target
    {
        public ItemScoringDaemonTarget(string targetName)
            : base(targetName, TargetClass.Handscoring, TargetType.Custom)
        {
        }

        protected List<ItemResponse> GetItemsToScore(TestResult testResult)
        {
            List<ItemResponse> itemsToScore = new List<ItemResponse>();

            // get all selected items on the test that are configured for item scoring
            //  and are not already scored
            foreach (ItemResponse ir in testResult.ItemResponses)
                if (ItemScoringConfig.Instance.ScoreItem(ir) && ir.IsSelected && !ir.ScoreStatus.Equals("Scored", StringComparison.InvariantCultureIgnoreCase))
                    itemsToScore.Add(ir);

            return itemsToScore;
        }

        public override ITargetResult Send(TestResult testResult, Action<object> outputProcessor, params object[] inputArgs)
        {
            // get the items that need to be scored
            List<ItemResponse> itemsToScore = GetItemsToScore(testResult);

            // if nothing to score and it's not a reset or invalidation, don't send the file to HS.
            //  This assumes that TSS wants to know about invalidations in the same way they want to know
            //  about resets.
            if (itemsToScore.Count == 0
                && !testResult.Opportunity.Status.Equals("reset", StringComparison.InvariantCultureIgnoreCase)
                && !testResult.Opportunity.Status.Equals("invalidated", StringComparison.InvariantCultureIgnoreCase))
                return new TargetResult() { Sent = false };

            //write the items that will be scored to TestOpportunityItemScore table
            bool batchScoring = false;
            if (inputArgs != null && inputArgs.Length == 1)
                batchScoring = Convert.ToBoolean(inputArgs[0]);
            int updatedCount = DAL.TISScoreMergerDAL.UpdateItemsToScore(testResult, itemsToScore,
                ItemScoringConfig.Instance.ScoreInvalidations, batchScoring, ItemScoringConfig.Instance.UpdateSameReportingVersion);

            if (outputProcessor != null)
                outputProcessor(updatedCount);

            return new TargetResult() { Sent = true };
        }
    }
}
