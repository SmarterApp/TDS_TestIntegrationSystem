/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Text;

namespace TDS.ItemScoringEngine
{
    /// <summary>
    /// Struct that represents meta-information about the Item Scorer
    /// </summary>
    public class ScorerInfo
    {
        public string Version { get; protected set; }
        public bool SupportsAsyncMode { get; private set; }
        public bool SupportsRubricCaching { get; private set; }
        public RubricContentSource RubricContentSource { get; private set; }
        public virtual string Details
        {
            get { return "N/A"; }
        }

        public ScorerInfo(string version, bool supportsAsyncMode, bool supportsRubricCaching,
                          RubricContentSource rubricContentSource)
        {
            this.Version = version;
            this.SupportsAsyncMode = supportsAsyncMode;
            this.SupportsRubricCaching = supportsRubricCaching;
            this.RubricContentSource = rubricContentSource;
        }
    }
}