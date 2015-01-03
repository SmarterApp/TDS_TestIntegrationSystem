/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
namespace TDS.ScoringDaemon.Abstractions
{
    public class ItemScoringRule
    {
        public string Context { get; set; }
        public string ItemType { get; set; }
        public bool Enabled { get; set; } // Item_in
        public int Priority { get; set; }
        public string ServerUrl { get; set; }
        public string StudentAppUrl { get; set; }
    }
}
