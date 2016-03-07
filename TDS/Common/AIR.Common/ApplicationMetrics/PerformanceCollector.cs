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
using AIR.Common.Threading;

namespace AIR.Common.ApplicationMetrics
{
    /// <summary>
    /// Used for tracking application metrics.
    /// </summary>
    public class PerformanceCollector
    {
        private static readonly ReductionVariable<Dictionary<string, PerformanceData>> _timingLookupInstance;

        static PerformanceCollector()
        {
            _timingLookupInstance = new ReductionVariable<Dictionary<string, PerformanceData>>(() => new Dictionary<string, PerformanceData>());
        }
        
        /// <summary>
        /// Records a response time in milliseconds for the given name. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="milliseconds"></param>
        public static void Add(string name, long milliseconds)
        {
            Dictionary<string, PerformanceData> lookup = _timingLookupInstance.Value;

            PerformanceData data;

            if (!lookup.TryGetValue(name, out data))
            {
                data = new PerformanceData(name);
                lookup.Add(name, data);
            }

            if (data.Count == 0)
            {
                data.Average = milliseconds;
                data.Min = milliseconds;
                data.Max = milliseconds;
                data.Count++;
            }
            else
            {
                // calculate average
                data.Average = ((data.Average * data.Count) + milliseconds) / (data.Count + 1);
                
                // check min
                if (milliseconds < data.Min)
                {
                    data.Min = milliseconds;
                }

                // check max
                if (milliseconds > data.Max)
                {
                    data.Max = milliseconds;
                }
                
                data.Count++;
            }

        }

        public static IEnumerable<PerformanceData> GetResults()
        {
            var timingGroups = _timingLookupInstance.Values.SelectMany(lookup => lookup.Values).GroupBy(meas => meas.Name);
            foreach (var timingGroup in timingGroups)
            {
                PerformanceData data = new PerformanceData(timingGroup.Key);
                data.Count = timingGroup.Sum(meas => meas.Count);
                data.Average = timingGroup.Average(meas => meas.Average);
                data.Min = timingGroup.Min(meas => meas.Average);
                data.Max = timingGroup.Max(meas => meas.Average);
                yield return data;
            }
        }
        
    }
}
