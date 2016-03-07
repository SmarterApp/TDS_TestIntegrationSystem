/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
namespace AIR.Common.ApplicationMetrics
{
    /// <summary>
    /// Used for keeping timing information on an event.
    /// </summary>
    public class PerformanceData
    {
        public string Name { get; private set; }
        public long Count { get; set; }
        public double Average { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }

        public PerformanceData(string name)
        {
            Name = name;
        }
    }
}