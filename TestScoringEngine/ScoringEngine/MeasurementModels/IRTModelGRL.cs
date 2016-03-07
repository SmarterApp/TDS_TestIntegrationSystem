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
    public class IRTModelGRL : IRTModel
    {
        public IRTModelGRL()
            : base(IRTModelFactory.Model.IRTGRL)
        {
        }

        public override IRTModel DeepCopy()
        {
            IRTModelGRL copy = (IRTModelGRL)(this.MemberwiseClone());
            return copy;
        }

        public override void SetParameterCount(int parameterCount, int scorePoints)
        {
            if (parameterCount != scorePoints + 1)
                throw new Exception("ScoringEngine.MeasurementModels.IRTGRL.SetParameterCount. The GRL model must have one more parameter than score points but this item has scorePoints = " + scorePoints.ToString() + " and " + parameterCount.ToString() + " parameters");
            _parameterCount = parameterCount;
            _parameters = new double[_parameterCount];
        }

        public override double ComputeProbability(double score, double theta)
        {
            double Da = 1.7 * _parameters[0];
            int z = Convert.ToInt32(Math.Round(score));
            if (Math.Abs(z - score) > 0.00001)
                throw new ScoringEngineException("The IRTModelGRL code was not checked for use with non integral scores. Does it work?");
            if (z == 0)
                return 1.0 / (1.0 + Math.Exp(Da * (theta - _parameters[1])));
            if (z == _parameterCount - 1)
                return 1.0 / (1.0 + Math.Exp(-Da * (theta - _parameters[_parameterCount-1])));
            return 1.0 / (1.0 + Math.Exp(Da * (theta - _parameters[z+1]))) 
                - 1.0 / (1.0 + Math.Exp(Da * (theta - _parameters[z])));
        }

        public override double D1LnlWrtTheta(double score, double theta)
        {
            double Da = 1.7 * _parameters[0];
            int z = Convert.ToInt32(Math.Round(score));
            if (z == 0)
                return -Da / (1.0 + Math.Exp(-Da * (theta - _parameters[1])));
            if (z == _parameterCount - 1)
                return Da / (1.0 + Math.Exp(Da * (theta - _parameters[_parameterCount - 1])));
            double E = Math.Exp(Da * (theta - _parameters[z]));
            double E1 = Math.Exp(Da * (theta - _parameters[z + 1]));
            return Da*(1-E*E1)/((1+E)*(1+E1));
        }
        
        public override double D2LnlWrtTheta(double score, double theta)
        {
            double Da = 1.7 * _parameters[0];
            int z = Convert.ToInt32(Math.Round(score));
            if (z == 0)
            {
                double E = Math.Exp(Da * (theta - _parameters[1]));
                return -Da*Da*E/((1+E)*(1+E));
            }
            if (z == _parameterCount - 1)
            {
                double E = Math.Exp(Da * (theta - _parameters[_parameterCount-1]));
                return -Da*Da*E/((1+E)*(1+E));
            }
            double E0 = Math.Exp(Da * (theta - _parameters[z]));
            double E1 = Math.Exp(Da * (theta - _parameters[z + 1]));
            return -Da * Da * (E0 + E1 + E0 * E1 * (4 + E0 + E1)) / ((1 + E0) * (1 + E0) * (1 + E1) * (1 + E1));
        }

        public override double ExpectedScore(double theta)
        {
            double expectedScore = 0.0;
            for (int score = 1; score < _parameterCount; score++)
                expectedScore += score * ComputeProbability(score, theta);
            return expectedScore;
        }

        public override void RescaleParameters(double slope, double intercept)
        {
        }

        public override double GetDifficulty()
        {
            double sum = 0.0;
            for (int i = 1; i < _parameterCount; i++)
                sum += _parameters[i];
            return sum / (_parameterCount - 1);
        }

        public override double GetSlope()
        {
            return _parameters[0];
        }

        public override double Information(double theta)
        {
            double Da = 1.7 * _parameters[0];
            double[] Es = new double[_parameterCount];
            for (int i = 1; i < _parameterCount; i++)
            {
                Es[i] = Math.Exp(Da * (theta - _parameters[i]));
            }
            double info = Es[1] * Es[1] / Math.Pow(1 + Es[1], 3.0) + Es[_parameterCount - 1] / Math.Pow(1 + Es[_parameterCount - 1], 3.0);
            for (int i = 1; i < _parameterCount - 1; i++)
            {
                info += (Es[i] - Es[i + 1]) * Math.Pow(Es[i] * Es[i + 1] - 1, 2.0) / Math.Pow((1 + Es[i]) * (1 + Es[i + 1]), 3.0);
            }
            return Da * Da * info;
        }

        public override double GuessProbability()
        {
            return 0.0;
        }

        public override void PrintDebugInfo(string itemName, double score, double theta, string filename)
        {
            TextWriter tw = new StreamWriter(filename, true);
            tw.Write(itemName + ",IRTGRL," + score.ToString() + "," + theta.ToString() + "," + ComputeProbability(score, theta) + "," + D1LnlWrtTheta(score, theta) + "," + D2LnlWrtTheta(score, theta));
            for (int i = 0; i < _parameterCount; i++)
                tw.Write(",p" + i.ToString() + "=" + _parameters[i].ToString());
            tw.WriteLine();
            tw.Close();
        }

        public override string ToString()
        {
            return "IRTGRL";
        }

    }
}
