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
using ScoringEngine;

namespace TDSQASystemAPI.BL
{
    public class LockingTestCollection
    {
        private object tcLock = new object();

        public TestCollection TestCollection { get; private set; }

        public LockingTestCollection(TestCollection tc)
        {
            this.TestCollection = tc;
        }

        public bool HasTest(string testName)
        {
            return this.TestCollection != null 
                && this.TestCollection.HasTest(testName);
        }

        public void LoadTest(string testName)
        {
            if (this.TestCollection != null
                && !this.TestCollection.HasTest(testName))
            {
                lock (tcLock)
                {
                    if (!this.TestCollection.HasTest(testName))
                        this.TestCollection.LoadTest(testName);
                }
            }
        }
    }
}
