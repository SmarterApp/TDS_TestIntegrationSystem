/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System.Linq;
using System.Collections.Generic;
using AIR.Common.Web;

namespace TDS.Shared.Browser
{
    public class BrowserValidation
    {
        private const string WILDCARD = "*";
        private readonly List<BrowserRule> _rules = new List<BrowserRule>(); 
        
        public List<BrowserRule> GetRules()
        {
            return _rules;
        }

        public void AddRule(BrowserRule browserRule)
        {
            _rules.Add(browserRule);
        }

        /// <summary>
        /// Check if the browser user agent string matches this rule.
        /// </summary>
        private bool MatchRule(BrowserInfo browserInfo, BrowserRule browserRule)
        {
            // check if OS name matches ('*' will be Unknown, which means skip this check)
            if (browserRule.OSName != BrowserOS.Unknown &&
                browserInfo.OSName != browserRule.OSName) return false;

            // check if OS meets minimum version (0 means skip this check)
            if (browserRule.OSMinVersion > 0 &&
                browserInfo.OSVersion < browserRule.OSMinVersion) return false;

            // check if OS meets maximum version (0 means skip this check)
            if (browserRule.OSMaxVersion > 0 &&
                browserInfo.OSVersion > browserRule.OSMaxVersion) return false;

            // check if the hardware architecture matches ('*' means skip this check)
            if (browserRule.Architecture != WILDCARD &&
                browserInfo.Architecture != browserRule.Architecture) return false;

            // check if the browser name matches ('*' means skip this check)
            if (browserRule.Name != WILDCARD &&
                browserInfo.Name != browserRule.Name) return false;

            // check if browser meets minimum version (0 means skip this check)
            if (browserRule.MinVersion > 0 &&
                browserInfo.Version < browserRule.MinVersion) return false;

            // check if browser meets maximum version (0 means skip this check)
            if (browserRule.MaxVersion > 0 &&
                browserInfo.Version > browserRule.MaxVersion) return false;

            // everything matched
            return true;
        }

        /// <summary>
        /// Find all the matching browser rules for this user agent.
        /// </summary>
        public IOrderedEnumerable<BrowserRule> FindRules(BrowserInfo browserInfo)
        {
            return _rules.Where(r => MatchRule(browserInfo, r)).OrderByDescending(r => r.Priority);
        }

        /// <summary>
        /// Find the highest priority browser rule for this user agent.
        /// </summary>
        /// <returns>Either the rule or NULL if none found.</returns>
        public BrowserRule FindRule(BrowserInfo browserInfo)
        {
            return FindRules(browserInfo).FirstOrDefault();
        }

        /// <summary>
        /// Check the action for this user agent string.
        /// </summary>
        public BrowserAction Check(BrowserInfo browserInfo)
        {
            BrowserRule browserRule = FindRule(browserInfo);
            return (browserRule != null) ? browserRule.Action : BrowserAction.Deny;
        }
    }
}