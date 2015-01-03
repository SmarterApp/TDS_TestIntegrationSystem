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
    public class ItemScoringConfig
    {
        private static object lockme = new object();
        private static ItemScoringConfig instance;

        /// <summary>
        /// itemType, items
        /// </summary>
        private Dictionary<string, HashSet<string>> ItemConfig { get; set; }
        /// <summary>
        /// NotScored, etc; default = NotScored
        /// </summary>
        private HashSet<string> ScoreStatuses { get; set; }

        /// <summary>
        /// key = itemType
        /// if value == true, then any items in ItemConfig for the itemType are considered not configured for scoring, whereas
        ///     all items of that type not listed are configured for scoring.  
        /// Ex: SA:157-6574,157-9851:true means that all SA items except the 2 listed are scored.
        /// Ex: SA:157-6574,157-9851 or SA:157-6574,157-9851:false means that those 2 items are configured for scoring; all other ER items are not scored
        /// </summary>
        private Dictionary<string, bool> ItemsAreExclusions { get; set; }

        /// <summary>
        /// Whether or not invalidations should be scored.  Default = true
        /// </summary>
        public bool ScoreInvalidations { get; private set; }

        /// <summary>
        /// If items that were already sent for handscores, determines whether or not to resend if the opp has the same reportingVersion
        /// default true.  If true, it resets the scoringDate and attempts.  Note that if the item was scored already, it would not
        /// be sent back to hand-scoring unless the response changed, in which case we'd expect a different reportingVersion.  This is 
        /// for resubmits where the items are still waiting for machine scores or have a NotScored status for some reason (was previously
        /// reset/invalidated for ex).
        /// TODO: revisit whether this needs to be configurable.  Not sure if we'd ever want this to be false.
        /// </summary>
        public bool UpdateSameReportingVersion { get; private set; }

        private ItemScoringConfig()
        {
            ItemConfig = new Dictionary<string, HashSet<string>>();
            ItemsAreExclusions = new Dictionary<string, bool>();
            
            BuildItemConfig(ConfigurationManager.AppSettings["ItemScoring:ItemTypes"]);
            
            ScoreStatuses = String.IsNullOrEmpty(ConfigurationManager.AppSettings["ItemScoring:ScoreStatus"])
                ? new HashSet<string>(new string[] { ScoringStatus.NotScored.ToString() }, StringComparer.InvariantCultureIgnoreCase)
                : new HashSet<string>(ConfigurationManager.AppSettings["ItemScoring:ScoreStatus"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), StringComparer.InvariantCultureIgnoreCase);
            
            ScoreInvalidations = String.IsNullOrEmpty(ConfigurationManager.AppSettings["ItemScoring:ScoreInvalidations"]) ? true : Convert.ToBoolean(ConfigurationManager.AppSettings["ItemScoring:ScoreInvalidations"]);
            
            UpdateSameReportingVersion = String.IsNullOrEmpty(ConfigurationManager.AppSettings["ItemScoring:UpdateSameReportingVersion"]) ? true : Convert.ToBoolean(ConfigurationManager.AppSettings["ItemScoring:UpdateSameReportingVersion"]);
        }

        public static ItemScoringConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockme)
                    {
                        if (instance == null)
                            instance = new ItemScoringConfig();
                    }
                }
                return instance;
            }
        }

        public bool ScoreStatusIsConfigured(string scoreStatus)
        {
            return ScoreStatuses.Contains(scoreStatus);
        }

        public bool ItemIsConfigured(string itemType, string itemKey)
        {
            if (!ItemConfig.ContainsKey(itemType))
                return false;
            HashSet<string> items = ItemConfig[itemType];
            return items.Count == 0
                || (items.Contains(itemKey) && !ItemsAreExclusions[itemType])
                || (!items.Contains(itemKey) && ItemsAreExclusions[itemType]);
        }

        public bool ScoreItem(ItemResponse ir)
        {
            return ScoreStatusIsConfigured(ir.ScoreStatus)
                && ItemIsConfigured(ir.Format, ir.ItemName);
        }

        /// <summary>
        /// Config string format = <itemType>:<item>,<item>:<itemsAreExclusions>;<itemType>:<item>,<item>;<itemType>
        ///  If no items specified for a type, all items of that type are scored
        ///  if no item types specified, no scoring
        ///  If itemsAreExclusions = true, then all items of that itemType which are not listed will be scored; all items listed are not scored
        ///  default for itemsAreExclusions = false
        /// </summary>
        /// <param name="itemConfig"></param>
        private void BuildItemConfig(string itemConfig)
        {
            if (String.IsNullOrEmpty(itemConfig))
                return;
            List<string> itemTypeConfigs = itemConfig.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string itemTypeConfig in itemTypeConfigs)
            {
                List<string> itcToks = itemTypeConfig.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                HashSet<string> items = (itcToks.Count > 1) ? new HashSet<string>(itcToks[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) : new HashSet<string>();
                ItemConfig.Add(itcToks[0], items);
                ItemsAreExclusions[itcToks[0]] = (itcToks.Count > 2) ? Convert.ToBoolean(itcToks[2]) : false;
            }
        }
    }
}
