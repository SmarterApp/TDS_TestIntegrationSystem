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
using System.IO;

namespace ScoringEngine.MeasurementModels
{
    class RawModel : IRTModel
    {
        // this is just a placeholder for the case where we need an item that does not have an IRT model associated with it. (only the raw score will be used).
        public RawModel()
            : base(IRTModelFactory.Model.raw)
        {
        }

        public override IRTModel DeepCopy()
        {
            RawModel copy = (RawModel)(this.MemberwiseClone());
            return copy;
        }

        public override void SetParameterCount(int parameterCount, int scorePoints)
        {
            _parameterCount = parameterCount;
            _parameters = new double[_parameterCount];
        }

        public override double D1LnlWrtTheta(double score, double theta)
        {
            return 0.0;
        }

        public override double ComputeProbability(double score, double theta)
        {
            return 0.0;
        }

        public override double ExpectedScore(double theta)
        {
            return 0.0;
        }

        public override double D2LnlWrtTheta(double score, double theta)
        {
            return 0.0;
        }
        public override void RescaleParameters(double slope, double intercept)
        {
        }

        public override double GetDifficulty()
        {
            return 0.0;
        }

        public override double GetSlope()
        {
            return 0.0;
        }

        public override double Information(double theta)
        {
            return 0.0;
        }

        public override double GuessProbability()
        {
            return 0.0;
        }

        public override void PrintDebugInfo(string itemName, double score, double theta, string filename)
        {
            TextWriter tw = new StreamWriter(filename, true);
            tw.Write(itemName + ",IRTraw");
            tw.Close();
        }

        public override string ToString()
        {
            return "IRTRaw";
        }
    }
}
