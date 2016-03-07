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
    public class IRTModel3pl : IRTModel
    {
        public IRTModel3pl()
            : base(IRTModelFactory.Model.IRT3PL)
        {
        }

        public override IRTModel DeepCopy()
        {
            IRTModel3pl copy = (IRTModel3pl)(this.MemberwiseClone());
            return copy;
        }

        public override void  SetParameterCount(int parameterCount, int scorePoints)
        {
            if (scorePoints != 1)
                throw new ScoringEngineException("An IRT3pl item must have a scorePoint of 1");
            if (parameterCount != 3)
                throw new ScoringEngineException("An IRT3pl item must have 3 parameters");
            _parameterCount = parameterCount;
            _parameters = new double[_parameterCount];
        }

        public override void RescaleParameters(double slope, double intercept)
        {
            _parameters[0] = (_parameters[0] - intercept) / slope;
            _parameters[1] = _parameters[1] / slope;
        }

        public override double ComputeProbability(double score, double theta)
        {
            double prob;
            double a = _parameters[0];
            double b = _parameters[1];
            double c = _parameters[2];

            prob = c + (1.0 - c) / (1 + Math.Exp(-a * (theta - b)));

            /*
            if (score == 1.0) return prob;
            else if (score == 0.0) return 1.0 - prob;
            else return 1.0;
            */
            return Math.Pow(prob,score)*Math.Pow(1.0 - prob,1.0-score);
        }

        public override double ExpectedScore(double theta)
        {
            return ComputeProbability(1.0, theta);
        }

        public override double D1LnlWrtTheta(double score, double theta)
        {
            double a = _parameters[0];
            double b = _parameters[1];
            double c = _parameters[2];

            // if ((score == 0.0) || (score  == 1.0))
            //{
                double kern = Math.Exp(a * (b - theta));
                // return a * (score / (1.0 + c * kern) - 1.0 / (1.0 + kern));
                return -((a * (1 + c * kern - score - kern * score)) / ((1 + kern) * (1 + c * kern)));
            // }
            // else return 0.0;
        }

        public override double D2LnlWrtTheta(double score, double theta)
        {
            double a = _parameters[0];
            double b = _parameters[1];
            double c = _parameters[2];

            double kern = Math.Exp(a* (b - theta));
            // return a * a* kern * (c * score / Math.Pow(1.0 + c * kern, 2.0)
            //                      - 1.0 / Math.Pow(1.0 + kern, 2.0));
            return -((a*a*kern*(1 + c*c*kern*kern - c*(2*kern*(-1 + score) + score + kern*kern*score)))/(Math.Pow(1 + kern,2.0)*Math.Pow(1 + c*kern,2.0)));
        }

        public override double GetDifficulty()
        {
            return _parameters[1];
        }

        public override double GetSlope()
        {
            return _parameters[0];
        }

        public override double Information(double theta)
        {
            double p, q, t;
            double a = _parameters[0];
            double b = _parameters[1];
            double c = _parameters[2];

            p = c + (1.0 - c) / (1 + Math.Exp(-a * (theta - b)));
            q = 1.0 - p;
            t = (p - c) / (1 - c);
            return a * a * q * t * t / p;
        }

        public override double GuessProbability()
        {
            return _parameters[2];
        }

        public override void PrintDebugInfo(string itemName, double score, double theta, string filename)
        {
            TextWriter tw = new StreamWriter(filename, true);
            tw.WriteLine(itemName + ",IRT3pl," + score.ToString() + "," + theta.ToString() + "," + ComputeProbability(score, theta) + "," + D1LnlWrtTheta(score, theta) + "," + D2LnlWrtTheta(score, theta) + ",a=" + _parameters[0].ToString() + ",b=" + _parameters[1].ToString() + ",c=" + _parameters[2].ToString());
            tw.Close();
        }

        public override string ToString()
        {
            return "IRT3pl";
        }
    }//end class
}//end namespace
