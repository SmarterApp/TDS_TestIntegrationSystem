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

using TDSQASystemAPI.Config;
using TDSQASystemAPI.MeasurementModels;
using ScoringEngine.ConfiguredTests;

namespace TDSQASystemAPI.TestResults
{
    internal class TestScorer
    {
        internal enum PerfectScoreType { None = 0, Min = 1, Max = 2, NonePresented = 3 };

        internal double MLEScore(TestResult tr, string featureName, string featureValue, int seType, int maxIter, double converge, out double se)
        {
            double startValue = AverageDifficulty(tr.ItemResponses);
            return MLEScore(tr, featureName, featureValue, startValue, seType, maxIter, converge, 0.5, out se);
        }

        /* This one applies the 0.5 rule at the extreme scores */
        /* TODO: test that this is only used for the Rasch model */
        internal double MLEScore(TestResult tr, string featureName, string featureValue, double startValue, int seType, int maxIter, double converge, double EndAdjust, out double se)
        {

            // seType of 0 asks for the inverse of the information
            // seType of 1 asks for the sandwich estimator
            // this sets the standard message to missing if you hit the maximum iterations.
            double lnl = -9999999999.9, lnlChng = 99999.9, lnlNext = 0.0;
            int iter = 0;

            double step = 0.0, negD2 = 0.0;
            TestItem ti = null;
            IRTModel irt = null;
            double theta = startValue, x, d1Squared = 0.0, deriv1, d1 = 0.0, d2 = 0.0;
            step = 0.0;
            int good = 0;

            PerfectScoreType perfect = PerfectScore(tr, featureName, featureValue);

            if (perfect != PerfectScoreType.None)
            {
                if (perfect == PerfectScoreType.NonePresented)
                {
                    se = 0.0;
                    // this is evil: a theta = 0 could be a good score! (but it is what TDS does...)
                    return 0.0;
                }

                // adjust the first item score by EndAdjust
                foreach (ItemResponse ir in tr.ItemResponses)
                {
                    ti = ir.TestItem;
                    foreach (TestItemScoreInfo ts in ti.ScoreInfo)
                    {
                        if (((featureName != null) && (!ts.HasStrand(featureValue)))
                            || !ti.IsScored || ti.IsFieldTest || ir.TreatAsNotPresented(ts))
                            continue;

                        if (ts.RecodeScore(ir.Score(ts.Dimension)) >= 0) //if it is a good score
                        {
                            if (perfect == PerfectScoreType.Min)
                            {
                                ir.SetScore(ts.Dimension, EndAdjust);
                                goto Done;
                            }
                            else
                            {
                                ir.SetScore(ts.Dimension, ts.MaxUnrecodedScoreAdjust(EndAdjust));
                                goto Done;
                            }
                        }
                    }
                }
            Done: 
                ;
            }

            while ((lnlChng > converge) && (iter < maxIter))
            {
                d1 = 0.0;
                d2 = 0.0;
                lnlNext = 0.0;
                d1Squared = 0.0;
                x = 0.0;

                foreach (ItemResponse ir in tr.ItemResponses)
                {
                    ti = ir.TestItem;
                    foreach (TestItemScoreInfo sc in ti.ScoreInfo)
                    {
                        irt = sc.IRTModel;

                        //skip items not in the requested scale
                        if (((featureName != null) && (!sc.HasStrand(featureValue)))
                            || !ti.IsScored || ti.IsFieldTest || ir.TreatAsNotPresented(sc))
                            continue;

                        double score = sc.RecodeScore(ir.Score(sc.Dimension));
                        if (score >= 0) //if it is a good score
                        {
                            x = irt.ComputeProbability(score, theta);

                            if (x <= 0.0) x = -10.00; //double.MinValue;
                            else x = Math.Log(x);

                            if (iter == 0)
                            {
                                if (x < 1.0) good++; //keep track of scorable responses
                            }
                        }

                        lnlNext += x;
                        deriv1 = irt.D1LnlWrtTheta(score, theta);
                        d1Squared += deriv1 * deriv1;
                        d1 += deriv1;
                        d2 += irt.D2LnlWrtTheta(score, theta);
                    }
                }

                negD2 = d2 * -1.0;

                if (lnlNext >= lnl)
                {
                    step = SteepestStep(d1, d2, d1Squared, 1.0);
                    theta += step;
                    lnlChng = lnlNext - lnl;
                    lnl = lnlNext;
                }
                else
                {
                    step = step * .5;
                    theta -= step;
					lnlChng = lnl - lnlNext;
                    if (Double.IsNaN(lnlChng)) lnlChng = 100.0;
                }
                ++iter;
            } //end while

            if (perfect != PerfectScoreType.None)
            {
                // unadjust the first item score (and rawScore) in case they are used later
                foreach (ItemResponse ir in tr.ItemResponses)
                {
                    ti = ir.TestItem;
                    foreach (TestItemScoreInfo sc in ti.ScoreInfo)
                    {
                        if (((featureName != null) && (!sc.HasStrand(featureValue)))
                            || !ti.IsScored || ti.IsFieldTest || ir.TreatAsNotPresented(sc))
                            continue;

                        if (sc.RecodeScore(ir.Score(sc.Dimension)) >= 0) //if it is a good score
                        {
                            if (perfect == PerfectScoreType.Min)
                            {
                                ir.SetScore(sc.Dimension, 0.0);
                                goto Done;
                            }
                            else
                            {
                                ir.SetScore(sc.Dimension, sc.MaxUnrecodedScore());
                                goto Done;
                            }
                        }
                    }
                }
            Done:
                ;
            }

            if (iter == maxIter)
            {
                se = double.MaxValue;
                theta = double.MaxValue;
            }

            else
            {
                if (seType == 1)
                {
                    //Jon: d1Squared should divide by k/(k-1) for k items
                    d1Squared *= (double)good / ((double)good - 1.0);
                    se = Math.Sqrt(d1Squared / (d2 * d2));
                }
                else se = Math.Sqrt(1.0 / (-d2));
            }

            //check for bad values
            if (double.IsNaN(theta)) return double.MaxValue;
            return theta;
        }

        private PerfectScoreType PerfectScore(TestResult tr, string featureName, string featureValue)
        {
            PerfectScoreType perfect = PerfectScoreType.None;

            foreach (ItemResponse ir in tr.ItemResponses)
            {
                TestItem ti = ir.TestItem;

                foreach (TestItemScoreInfo isi in ti.ScoreInfo)
                {
                    //skip items not in the requested scale
                    if (((featureName != null) && (!isi.HasStrand(featureValue)))
                        || !ti.IsScored || ti.IsFieldTest || ir.TreatAsNotPresented(isi))
                        continue;

                    double score = isi.RecodeScore(ir.Score(isi.Dimension));
                    PerfectScoreType nextType = (score == isi.ScorePoints) ? PerfectScoreType.Max : (score == 0) ? PerfectScoreType.Min : PerfectScoreType.None;
                    if (nextType == PerfectScoreType.None) return PerfectScoreType.None;
                    else if (perfect == PerfectScoreType.None) perfect = nextType;
                    else if (perfect != nextType) return PerfectScoreType.None;
                }
            }

            if (perfect == PerfectScoreType.None)
                // no items on this strand
                perfect = PerfectScoreType.NonePresented;

            return perfect;
        }

        private double SteepestStep(double d1, double d2, double d1Squared, double lambda)
        {
            //return (1.0/d1 );// / (-d2)) * lambda;
            if (d2 < 0)  return d1 / (-1.0 * d2);
            return 1.0 / d1Squared;
        }

        internal double AverageDifficulty(List<ItemResponse> responses)
        {
            double sum = 0.0;
            double count = 0.0;
            foreach (ItemResponse ir in responses)
            {
                TestItem ti = ir.TestItem;
                foreach (TestItemScoreInfo isi in ti.ScoreInfo)
                {
                    if ((!ti.IsScored) || (ti.IsFieldTest)) continue;
                    sum += isi.IRTModel.GetDifficulty();
                    count += 1.0;
                }
            }
            if (count > 0.0) return sum / count;
            return 0.0;
        }



    }//end class
}//end namespace
