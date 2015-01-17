/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using AIR.Common.Web;

namespace TDS.Shared.Browser
{
    public class BrowserInfo
    {
        public BrowserOS OSName { get; set; }
        public double OSVersion { get; set; }
        public string Architecture { get; set; }
        public string Name { get; set; }
        public double Version { get; set; }

        public BrowserInfo()
        {
        }

        public BrowserInfo(BrowserOS osName, double osVersion, string architecture, string name, double version)
        {
            OSName = osName;
            OSVersion = osVersion;
            Architecture = architecture;
            Name = name;
            Version = version;
        }

        /// <summary>
        /// Get the current http browser info
        /// </summary>
        public static BrowserInfo GetHttpCurrent()
        {
            BrowserParser browser = new BrowserParser();
            return new BrowserInfo(browser.OSName, browser.OSVersion, browser.HardwareArchitecture, browser.Name, browser.Version);
        }
    }
}