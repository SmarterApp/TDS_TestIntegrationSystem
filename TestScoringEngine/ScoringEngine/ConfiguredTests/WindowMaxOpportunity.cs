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

namespace ScoringEngine.ConfiguredTests
{
    public class WindowMaxOpportunity
    {

        //public properties
        #region Public Properties

        public string testId { get; set; }

        public int MaxOpportunityNo { get; set; }

        public string WindowId { get; set; }

        #endregion

        //Consutructor of WindowMaxOpportunity
        internal WindowMaxOpportunity(string testId, int maxOpportunityNo, string windowId)
        {
            this.testId = testId;
            MaxOpportunityNo = maxOpportunityNo;
            WindowId = windowId;

        }

    }
}
