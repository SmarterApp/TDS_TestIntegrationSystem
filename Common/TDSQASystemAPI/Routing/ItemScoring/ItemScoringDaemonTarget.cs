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
    public class ItemScoringDaemonTarget : ItemScoringTarget
    {
        public ItemScoringDaemonTarget(string targetName)
            : base(targetName, TargetClass.Handscoring, TargetType.Custom)
        {
        }

        public override ITargetResult Send(TestResult testResult, Action<object> outputProcessor, params object[] inputArgs)
        {
            TargetResult result = new TargetResult() { Sent = false };

            // get the items that need to be scored
            List<ItemResponse> itemsToScore = GetItemsToScore(testResult);

            int updatedCount = 0;

            if (itemsToScore.Count > 0)
            {
                //write the items that will be scored to TestOpportunityItemScore table
                updatedCount = DAL.TISScoreMergerDAL.UpdateItemsToScore(testResult, itemsToScore,
                    ItemScoringConfigManager.Instance.GetItemScoringConfig(base.Name).ScoreInvalidations,
                    ItemScoringConfigManager.Instance.GetItemScoringConfig(base.Name).BatchRequest,
                    ItemScoringConfigManager.Instance.GetItemScoringConfig(base.Name).UpdateSameReportingVersion,
                    ItemScoringConfigManager.Instance.GetItemScoringConfig(base.Name).IsHandscoringTarget);

                result.Sent = true;
            }
        
            // allow the count of items actually updated in the db to be evaluated by the caller
            if (outputProcessor != null)
                outputProcessor(updatedCount);

            return result;
        }
    }
}
