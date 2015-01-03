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
    /// Used for tracking how many times an event occurs.
    /// </summary>
    public class CounterCollector
    {
        private static readonly ReductionVariable<Dictionary<string, CounterData>> _counterLookupInstance;

        static CounterCollector()
        {
            _counterLookupInstance = new ReductionVariable<Dictionary<string, CounterData>>(() => new Dictionary<string, CounterData>());
        }

        /// <summary>
        /// Increment the counter for the given name. 
        /// </summary>
        public static void IncrementBy(string name, long value)
        {
            Dictionary<string, CounterData> lookup = _counterLookupInstance.Value;

            CounterData counter;

            if (!lookup.TryGetValue(name, out counter))
            {
                counter = new CounterData(name);
                lookup.Add(name, counter);
            }

            DateTime now = DateTime.Now;

            if (counter.First == DateTime.MinValue)
            {
                counter.First = now;
            }

            counter.Last = now;
            counter.Value = counter.Value + value;
        }

        public static void Increment(string name)
        {
            IncrementBy(name, 1);
        }

        public static void Decrement(string name)
        {
            IncrementBy(name, -1);
        }

        public static IEnumerable<CounterData> GetResults()
        {
            var counterGroups = _counterLookupInstance.Values.SelectMany(lookup => lookup.Values).GroupBy(meas => meas.Name);
            foreach (var counterGroup in counterGroups)
            {
                CounterData counter = new CounterData(counterGroup.Key);
                counter.Value = counterGroup.Sum(meas => meas.Value);
                counter.First = counterGroup.Min(meas => meas.First);
                counter.Last = counterGroup.Max(meas => meas.Last);
                yield return counter;
            }
        }

    }
}
