/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using AIR.Common.Web;

namespace TDS.Shared.Browser
{
    public class BrowserRule
    {
        /// <summary>
        /// The priority in which this rule takes precedence. 
        /// The higher the number the more it overrides a lower number.
        /// </summary>
        public int Priority { get; set; }
        
        public BrowserOS OSName { get; set; }
        public double OSMinVersion { get; set; }
        public double OSMaxVersion { get; set; }
        public string Architecture { get; set; }
        public string Name { get; set; }
        public double MinVersion { get; set; }
        public double MaxVersion { get; set; }
        public BrowserAction Action { get; set; }
        public string MessageKey { get; set; }

        public BrowserRule()
        {
            Priority = 0;
            OSName = BrowserOS.Unknown;
            OSMinVersion = 0;
            OSMaxVersion = 0;
            Architecture = "*";
            Name = "*";
            MinVersion = 0;
            MaxVersion = 0;
            Action = BrowserAction.Allow;
        }

        public BrowserRule(int priority, BrowserOS osName, double osMinVersion, double osMaxVersion, string architecture, string name, double minVersion, double maxVersion, BrowserAction action)
        {
            Priority = priority;
            OSName = osName;
            OSMinVersion = osMinVersion;
            OSMaxVersion = osMaxVersion;
            Architecture = architecture;
            Name = name;
            MinVersion = minVersion;
            MaxVersion = maxVersion;
            Action = action;
        }
    }
}