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

namespace ScoringEngine.ConfiguredTests
{
    /// <summary>
    /// Computations to be done for a particular test
    /// </summary>
    public class ComputationSpec
    {
        string measureLabel = "";
        string measureOf = "";
        string computationRule = "";
        List<string> computationLocations = new List<string>();
        Dictionary<int, List<ParameterValue>> parameterValues = new Dictionary<int, List<ParameterValue>>();

        public string MeasureLabel
        {
            get
            {
                return measureLabel;
            }
        }

        public string MeasureOf
        {
            get
            {
                return measureOf;
            }
        }

        public string ComputationRule
        {
            get
            {
                return computationRule;
            }
        }

        public Dictionary<int, List<ParameterValue>> ParameterValues
        {
            get
            {
                return parameterValues;
            }
        }

        public ComputationSpec(string measureLabel, string measureOf, string computationRule)
        {
            this.measureLabel = measureLabel;
            this.measureOf = measureOf;
            this.computationRule = computationRule;
        }
        
        public void AddParameter(int parameterPosition, string indexType, string index, string type, string value)
        {
            if (parameterPosition != -1)
            {
                if (!parameterValues.ContainsKey(parameterPosition))
                    parameterValues[parameterPosition] = new List<ParameterValue>();
                parameterValues[parameterPosition].Add(new ParameterValue(parameterPosition, indexType, index, type, value));
            }
        }

        public void AddComputationLocation(string location)
        {
            computationLocations.Add(location);
        }

        public bool HasComputationLocation(string location)
        {
            foreach (String loc in computationLocations)
                if (String.Compare(loc, location, true) == 0)
                    return true;
            return false;
        }
    }
}
