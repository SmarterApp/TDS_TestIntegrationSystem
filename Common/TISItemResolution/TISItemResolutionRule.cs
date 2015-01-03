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

//todo: after refactor, instead include the "Common" project that will contain all common classes
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.Routing.ItemScoring;
using TISItemResolution.Rules;

namespace TISItemResolution
{
    /// <summary>
    /// An abstract class to be overridden for item resolution rules. Can be different by client, subject, and grade.
    /// </summary>
    public abstract class TISItemResolutionRule
    {
        protected TISItemResolutionRule() { }

        public abstract ItemScoreInfo ResolveItemScore(ItemScoreInfo scoreInfo);

        /// <summary>
        /// Try resolving the item score. If it successfully resolves return true, if it throws an error return false. The original object is unedited.
        /// </summary>
        /// <param name="scoreInfo"></param>
        /// <returns></returns>
        public bool CheckValidResolvedItemScore(ItemScoreInfo scoreInfo)
        {
            try
            {
                ItemScoreInfo _obj = new ItemScoreInfo(scoreInfo);
                ResolveItemScore(_obj);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static TISItemResolutionRule CreateRule(string itemType, string itemID, string clientName)
        {
            //todo: use client name
            TISItemResolutionRule rule = null;
            if (ItemScoringConfig.Instance.ItemIsConfigured(itemType, itemID))
            {
                //check specific rule by item ID first
                switch (itemID)
                {
                    default:
                        break;
                }

                //if none was found then try item type
                switch (itemType.ToUpper())
                {
                    case "WER":
                        return new SBACItemResolutionRuleWER();
                    default:
                        break;
                }
            }
            return rule;
        }
    }
}
