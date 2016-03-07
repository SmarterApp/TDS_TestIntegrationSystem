/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;

namespace AIR.Common.ApplicationMetrics
{
    /// <summary>
    /// Used for tracking how many times an event has occured.
    /// </summary>
    public class CounterData
    {
        public string Name { get; private set; }
        public long Value { get; set; }
        public DateTime First { get; set; }
        public DateTime Last { get; set; }

        public CounterData(string name)
        {
            Name = name;
        }
    }
}