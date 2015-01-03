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

namespace ScoringEngine.Scoring
{
    public class MeasureValue
    {
        string measureLabel;
        string measureOf;
        double score = Double.NaN;
        string scoreString = "";
        double se = Double.NaN;
        string seString = "";

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

        public double Score
        {
            get
            {
                return score;
            }
        }

        public string ScoreString
        {
            get
            {
                return scoreString;
            }
        }

        public double StandardError
        {
            get
            {
                return se;
            }
        }

        public string StandardErrorString
        {
            get
            {
                return seString;
            }
        }
 
        public MeasureValue(string measureLabel, string measureOf, double score, double se)
        {
            this.measureLabel = measureLabel;
            this.measureOf = measureOf;
            this.score = score;
            this.scoreString = score.ToString();
            this.se = se;
            this.seString = se.ToString();
        }

        public MeasureValue(string measureLabel, string measureOf, string score)
        {
            this.measureLabel = measureLabel;
            this.measureOf = measureOf;
            this.scoreString = score;
        }

        internal string TDSValue(char separator)
        {
            return measureOf + separator + measureLabel + separator + scoreString + separator + seString;
        }
    }
}
