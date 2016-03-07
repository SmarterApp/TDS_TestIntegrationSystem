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
using System.Configuration;
using TDSQASystemAPI.TestResults;

namespace TDSQASystemAPI.Routing.ItemScoring
{
    public class ItemScoringConfigManager
    {
        private static ItemScoringConfigManager itemScoringConfigManager;
        private static object lockme = new object();

        // config cache
        private Dictionary<string, AIR.Configuration.ItemScoring> targetConfigs;
        
        private ItemScoringConfigManager()
        {
            targetConfigs = new Dictionary<string, AIR.Configuration.ItemScoring>(StringComparer.InvariantCultureIgnoreCase);

            // cache all targets; I'm not sure we can rely on ConfigurationElement instances sticking around, 
            //  and we don't want to have to rebuild the collections each time it's accessed.
            //TODO: test this and remove cache if not needed.
            foreach (AIR.Configuration.ItemScoring itemScoringConfig in AIR.Configuration.Settings.ItemScoringSettings)
                targetConfigs[itemScoringConfig.Target] = itemScoringConfig;
        }

        /// <summary>
        /// Returns the singleton instance using lazy instantiation
        /// </summary>
        public static ItemScoringConfigManager Instance
        {
            get
            {
                if (itemScoringConfigManager == null)
                {
                    lock(lockme)
                    {
                         if (itemScoringConfigManager == null)
                         {
                             itemScoringConfigManager = new ItemScoringConfigManager();
                         }
                    }
                }
                return itemScoringConfigManager;
            }
        }

        /// <summary>
        /// Returns the ItemScoring config settings for this target
        /// </summary>
        /// <param name="itemScoringTargetName"></param>
        /// <returns></returns>
        public AIR.Configuration.ItemScoring GetItemScoringConfig(string itemScoringTargetName)
        {
            if (!targetConfigs.ContainsKey(itemScoringTargetName))
                throw new ApplicationException(String.Format("There are no ItemScoring settings for target: {0}", itemScoringTargetName));
            
            return targetConfigs[itemScoringTargetName];
        }

        /// <summary>
        /// Does this item need to be scored by this target
        /// </summary>
        /// <param name="itemScoringTargetName"></param>
        /// <param name="ir"></param>
        /// <returns></returns>
        public bool ScoreItem(string itemScoringTargetName, ItemResponse ir)
        {
            return ScoreItem(GetItemScoringConfig(itemScoringTargetName), ir);
        }

        /// <summary>
        /// Does this item need to be scored by any target
        /// </summary>
        /// <param name="ir"></param>
        /// <returns></returns>
        public bool ScoreItem(ItemResponse ir)
        {
            bool scoreItem = false;

            foreach (AIR.Configuration.ItemScoring itemScoringConfig in targetConfigs.Values)
            {
                if (ScoreItem(itemScoringConfig, ir))
                {
                    scoreItem = true;
                    break;
                }
            }

            return scoreItem;
        }

        /// <summary>
        /// Does this item need to be scored by this target
        /// </summary>
        /// <param name="itemScoringConfig"></param>
        /// <param name="ir"></param>
        /// <returns></returns>
        private bool ScoreItem(AIR.Configuration.ItemScoring itemScoringConfig, ItemResponse ir)
        {
            return itemScoringConfig.ScoreStatusIsConfigured(ir.ScoreStatus)
               && itemScoringConfig.ItemIsConfigured(ir.Format, ir.ItemName);
        }
    }
}
