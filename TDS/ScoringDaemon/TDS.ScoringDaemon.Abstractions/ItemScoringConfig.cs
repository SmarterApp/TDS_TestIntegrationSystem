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

namespace TDS.ScoringDaemon.Abstractions
{
    public class ItemScoringConfig
    {
        public DateTime LoadTime { get; set; }
        public List<ItemScoringRule> ItemScoringRules { get; set; }
        public List<ScoreHostInfo> Satellites { get; set; }
        public long AccessCount { get; set; }

        /// <summary>
        /// Get the item scoring config.
        /// </summary>
        public ItemScoringRule GetItemScoringRule(string format, string testID)
        {
            // Find the best matching rule
            ItemScoringRule bestMatch =  (from rule in ItemScoringRules
                    where (rule.ItemType == format || rule.ItemType == "*") &&
                          (rule.Context == testID || rule.Context == "*")
                    orderby rule.Priority descending
                    select rule).FirstOrDefault();

            if (bestMatch == null) return null;

            //Now lets figure out which satellite to use
            string serverUrl = bestMatch.ServerUrl;
            string studentAppUrl = null;
            if(Satellites.Count > 0)
            {
                ScoreHostInfo chosenScoreHost = Satellites[(int) (AccessCount++ % Satellites.Count)];
                serverUrl = serverUrl != null ? serverUrl.Replace("{host}", chosenScoreHost.ScoreHost) : null;
                studentAppUrl = chosenScoreHost.StudentApp;
            }

            return new ItemScoringRule
                    {
                        Context = bestMatch.Context,
                        Enabled = bestMatch.Enabled,
                        ItemType = bestMatch.ItemType,
                        Priority = bestMatch.Priority,
                        ServerUrl = serverUrl,
                        StudentAppUrl = studentAppUrl
                    };
        }
    }
}
