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
using System.Text;

namespace TDSQASystemAPI.Config
{
    public class TestEnvironment
    {
        private int _oppExpire = -1;
        private int _oppTestDiff = -1;
 
        public int OppExpire
        {
            get { return _oppExpire; }
        }
        public int OppTestDiff
        {
            get { return _oppTestDiff; }
        }

        public TestEnvironment(int oppExpire, int oppTestDiff)
        {
            _oppExpire = oppExpire;
            _oppTestDiff = oppTestDiff;
        }
    }
}
