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
using System.Configuration;
using System.Collections;

namespace AIR.Configuration
{
    public class ItemScoring : ConfigurationElement
    {
        private object lockme = new object();

        public ItemScoring()
        {
        }

        /// <summary>
        /// itemType, items
        /// </summary>
        private Dictionary<string, HashSet<string>> itemConfig;
        private Dictionary<string, HashSet<string>> ItemConfig
        {
            get
            {
                if (itemConfig == null)
                {
                    lock (lockme)
                    {
                        if (itemConfig == null)
                        {
                            BuildItemConfig();
                        }
                    }
                }
                return itemConfig;
            }
        }

        /// <summary>
        /// key = itemType
        /// if value == true, then any items in ItemConfig for the itemType are considered not configured for scoring, whereas
        ///     all items of that type not listed are configured for scoring.  
        /// Ex: SA:157-6574,157-9851:true means that all SA items except the 2 listed are scored.
        /// Ex: SA:157-6574,157-9851 or SA:157-6574,157-9851:false means that those 2 items are configured for scoring; all other ER items are not scored
        /// </summary>
        private Dictionary<string, bool> ItemsAreExclusions { get; set; }

        /// <summary>
        /// NotScored, WaitingForMachineScore etc; default = NotScored
        /// </summary>
        private HashSet<string> scoreStatuses;
        private HashSet<string> ScoreStatuses
        {
            get
            {
                if (scoreStatuses == null)
                {
                    lock (lockme)
                    {
                        if (scoreStatuses == null)
                        {
                            scoreStatuses = new HashSet<string>(ScoreStatusesString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()), StringComparer.InvariantCultureIgnoreCase);
                        }
                    }
                }
                return scoreStatuses;
            }
        }

        #region settings

        [ConfigurationProperty("target",
            IsRequired = true,
            IsKey = true)]
        public string Target
        {
            get
            {
                return (string)this["target"];
            }
            set
            {
                this["target"] = value;
            }
        }

        [ConfigurationProperty("callbackUrl",
            IsRequired = false,
            IsKey = false)]
        public string CallbackURL
        {
            get
            {
                return (string)this["callbackUrl"];
            }
            set
            {
                this["callbackUrl"] = value;
            }
        }

        [ConfigurationProperty("scoreStatuses",
            IsRequired = false,
            IsKey = false,
            DefaultValue="NotScored")]
        private string ScoreStatusesString
        {
            get
            {
                return (string)this["scoreStatuses"];
            }
            set
            {
                this["scoreStatuses"] = value;
            }
        }

        [ConfigurationProperty("itemTypes",
            IsRequired = false,
            IsKey = false)]
        private string ItemTypesString
        {
            get
            {
                return (string)this["itemTypes"];
            }
            set
            {
                this["itemTypes"] = value;
            }
        }

        [ConfigurationProperty("scoreInvalidations",
            IsRequired = false,
            IsKey = false,
            DefaultValue = true)]
        public bool ScoreInvalidations
        {
            get
            {
                if (this["scoreInvalidations"] == null)
                    return true;
                return (bool)this["scoreInvalidations"];
            }
            set
            {
                this["scoreInvalidations"] = value;
            }
        }

        [ConfigurationProperty("updateSameReportingVersion",
            IsRequired = false,
            IsKey = false,
            DefaultValue = true)]
        public bool UpdateSameReportingVersion
        {
            get
            {
                if (this["updateSameReportingVersion"] == null)
                    return true;
                return (bool)this["updateSameReportingVersion"];
            }
            set
            {
                this["updateSameReportingVersion"] = value;
            }
        }

        [ConfigurationProperty("isHandscoringTarget",
            IsRequired = false,
            IsKey = false,
            DefaultValue = false)]
        public bool IsHandscoringTarget
        {
            get
            {
                if (this["isHandscoringTarget"] == null)
                    return true;
                return (bool)this["isHandscoringTarget"];
            }
            set
            {
                this["isHandscoringTarget"] = value;
            }
        }

        [ConfigurationProperty("batchRequest",
            IsRequired = false,
            IsKey = false,
            DefaultValue = false)]
        public bool BatchRequest
        {
            get
            {
                if (this["batchRequest"] == null)
                    return true;
                return (bool)this["batchRequest"];
            }
            set
            {
                this["batchRequest"] = value;
            }
        }

        #endregion

        #region methods

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

        /// <summary>
        /// Config string format = <itemType>:<item>,<item>:<itemsAreExclusions>;<itemType>:<item>,<item>;<itemType>
        ///  If no items specified for a type, all items of that type are scored
        ///  if no item types specified, no scoring
        ///  If itemsAreExclusions = true, then all items of that itemType which are not listed will be scored; all items listed are not scored
        ///  default for itemsAreExclusions = false
        /// </summary>
        /// <param name="itemConfig"></param>
        private void BuildItemConfig()
        {
            itemConfig = new Dictionary<string, HashSet<string>>();
            ItemsAreExclusions = new Dictionary<string, bool>();

            if (String.IsNullOrEmpty(ItemTypesString))
                return;
            List<string> itemTypeConfigs = ItemTypesString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string itemTypeConfig in itemTypeConfigs)
            {
                List<string> itcToks = itemTypeConfig.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                HashSet<string> items = (itcToks.Count > 1) ? new HashSet<string>(itcToks[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) : new HashSet<string>();
                ItemConfig.Add(itcToks[0], items);
                ItemsAreExclusions[itcToks[0]] = (itcToks.Count > 2) ? Convert.ToBoolean(itcToks[2]) : false;
            }
        }

        #endregion
    }
}
