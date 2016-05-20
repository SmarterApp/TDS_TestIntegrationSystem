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
using System.IO;
using System.Text;
using ScoringEngine.MeasurementModels;
using ScoringEngine.ConfiguredTests;

namespace ScoringEngine.Scoring
{
    internal static class MLEScorer
    {            
        /// <summary>
        /// 
        /// </summary>
        /// <param name="testResponses"></param>
        /// <param name="startValue"></param>
        /// <param name="seType">seType of 0 asks for the inverse of the information
        //      seType of 1 asks for the sandwich estimator</param>
        /// <param name="maxIter"></param>
        /// <param name="converge"></param>
        /// <param name="endAdjust"></param>
        /// <returns></returns>
        private static IRTScore MLEScore(List<ItemScore> testScores, double startValue, int seType, int maxIter, double converge)
        {
            double lnl = -9999999999.9, lnlChng = 99999.9, lnlNext = 0.0, se;
            int iter = 0;

            double step = 0.0, negD2 = 0.0;
            TestItem ti = null;
            IRTModel irt = null;
            double theta = startValue, x, d1Squared = 0.0, deriv1, d1 = 0.0, d2 = 0.0;
            step = 0.0;

            while ((lnlChng > converge) && (iter < maxIter))
            {
                d1 = 0.0;
                d2 = 0.0;
                lnlNext = 0.0;
                d1Squared = 0.0;
                x = 0.0;

                foreach (ItemScore ir in testScores)
                {
                    TestItemScoreInfo si = ir.ScoreInfo;
                    irt = si.IRTModel;
                    double score = ir.Score;

                    x = irt.ComputeProbability(score, theta);

                    if (x <= 0.0)
                        x = -1.0E10; //double.MinValue;
                    else x = Math.Log(x);

                    lnlNext += x;
                    deriv1 = irt.D1LnlWrtTheta(score, theta);
                    d1Squared += deriv1 * deriv1;
                    d1 += deriv1;
                    d2 += irt.D2LnlWrtTheta(score, theta);
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

            if (iter == maxIter)
                return new IRTScore(double.MaxValue, double.MaxValue, IRTScoreType.FailedConvergence);

            // NOTE: this computes se not at theta, but at the previous theta!
            if (seType == 1)
            {
                double k = testScores.Count;
                d1Squared *= k / (k - 1.0);
                se = Math.Sqrt(d1Squared / (d2 * d2));
            }
            else se = Math.Sqrt(1.0 / (-d2));

            //check for bad values
            if (double.IsNaN(theta))
                return new IRTScore(theta, se, IRTScoreType.Diverged);

            return new IRTScore(theta, se, IRTScoreType.Converged);
        }

        private static double GetSE(List<ItemScore> testScores, double theta, int seType)
        {
            double d2 = 0.0;
            double d1Squared = 0.0;

            foreach (ItemScore ir in testScores)
            {
                TestItemScoreInfo si = ir.ScoreInfo;
                IRTModel irt = si.IRTModel;
                double score = ir.Score;

                double deriv1 = irt.D1LnlWrtTheta(score, theta);
                d1Squared += deriv1 * deriv1;
                d2 += irt.D2LnlWrtTheta(score, theta);
            }

            if (seType == 1)
            {
                double k = testScores.Count;
                d1Squared *= k / (k - 1.0);
                return Math.Sqrt(d1Squared / (d2 * d2));
            }
            else return Math.Sqrt(1.0 / (-d2));
        }

        private static double SteepestStep(double d1, double d2, double d1Squared, double lambda)
        {
            //return (1.0/d1 );// / (-d2)) * lambda;
            if (d2 < 0)  return d1 / (-1.0 * d2);
            if (d1 < 0)
                return -1.0 / d1Squared;
            else
                return 1.0 / d1Squared;
        }

        private static double AverageDifficulty(List<ItemScore> testResponses)
        {
            double sum = 0.0;
            double count = 0.0;
            foreach (ItemScore ir in testResponses)
            {
                TestItemScoreInfo si = ir.ScoreInfo;
                sum += si.IRTModel.GetDifficulty();
                count += 1.0;
            }
            if (count > 0.0) return sum / count;
            return 0.0;
        }

        internal static IRTScore MLEScore3PL(List<ItemScore> testScores, double gridSize, double minTheta, double maxTheta)
        {
            return MLEScore3PL(testScores, gridSize, minTheta, maxTheta, 0, 50, 0.00001);
        }

        internal static IRTScore MLEScore3PL(List<ItemScore> testScores, double gridSize, double minTheta, double maxTheta, int seType, int maxIter, double converge)
        {
            if (testScores.Count == 0)
                return new IRTScore(0.0, 0.0, IRTScoreType.NoItems);
            double maxLikelihood;

            /*
            TextWriter tw = new StreamWriter(@"C:\tmp\Likelihoods.csv", false);
            for (double theta = -8.0; theta < 8; theta = theta + 0.1)
            {
                tw.WriteLine(theta.ToString() + "," + Likelihood(testScores, theta).ToString());
            }
            tw.Close();
            */

            IRTScore bestScore = new IRTScore(minTheta, 0.0, IRTScoreType.Converged);
            maxLikelihood = Likelihood(testScores, minTheta);
            double likelihood = Likelihood(testScores, minTheta - 20 * (maxTheta - minTheta));
            if (Double.IsNaN(likelihood))
                likelihood = ExtremeLikelihood(testScores, minTheta - 20 * (maxTheta - minTheta));
            if (likelihood > maxLikelihood)
                maxLikelihood = likelihood;
            bool wasLikelihoodIncreasing = IsLikelihoodIncreasing(testScores, minTheta);
            if (!wasLikelihoodIncreasing)
            {
                // don't assume it will be decreasing all the way to -infinity:
                IRTScore min = MLEScore(testScores, minTheta, seType, maxIter, converge);
                if (min.Type == IRTScoreType.Converged && min.Score < minTheta)
                {
                    likelihood = Likelihood(testScores, min.Score);
                    if (likelihood > maxLikelihood)
                        maxLikelihood = likelihood;
                }
            }
            bool isLikelihoodIncreasing = false;
            double lastGridPosition = minTheta;
            for (double gridPosition = minTheta + gridSize; gridPosition < maxTheta + gridSize; gridPosition += gridSize)
            {
                if (gridPosition > maxTheta) gridPosition = maxTheta;
                isLikelihoodIncreasing = IsLikelihoodIncreasing(testScores, gridPosition);
                if (wasLikelihoodIncreasing && !isLikelihoodIncreasing)
                {
                    IRTScore irtScore = MLEScore(testScores, (lastGridPosition + gridPosition)/2.0, seType, maxIter, converge);
                    double localTheta = irtScore.Score;
                    if (irtScore.Type != IRTScoreType.Converged || localTheta < lastGridPosition - gridSize/5.0 || localTheta > gridPosition + gridSize/5.0) // the adjustment to lastGridPosition and gridPosition is made for the case where the likelihood is changing very slowly at one of these points, slower than the converge value. So the max found might actually be slightly outside the interval.
                        return new IRTScore(irtScore.Score, irtScore.StandardError, IRTScoreType.FailedConvergence);
                    likelihood = Likelihood(testScores, localTheta);
                    if (likelihood > maxLikelihood)
                    {
                        //if (likelihoodIncreasingAtStart == false)
                        //    return new IRTScore(minTheta, 0.0, IRTScoreType.FailedConvergence);
                        maxLikelihood = likelihood;
                        bestScore = irtScore;
                    }
                }
                wasLikelihoodIncreasing = isLikelihoodIncreasing;
                lastGridPosition = gridPosition;
            }

            likelihood = Likelihood(testScores, maxTheta + 20 * (maxTheta - minTheta));
            if (Double.IsNaN(likelihood))
                likelihood = ExtremeLikelihood(testScores, maxTheta + 20*(maxTheta - minTheta));
            if (likelihood > maxLikelihood)
            {
                maxLikelihood = likelihood;
                bestScore = new IRTScore(maxTheta, 0.0, IRTScoreType.Converged);
            }
            if (isLikelihoodIncreasing)
            {
                // don't assume it will be increasing all the way to -infinity:
                IRTScore max = MLEScore(testScores, maxTheta, seType, maxIter, converge);
                if (max.Type == IRTScoreType.Converged && max.Score > maxTheta)
                {
                    likelihood = Likelihood(testScores, max.Score);
                    if (likelihood > maxLikelihood)
                        bestScore = new IRTScore(maxTheta, 0.0, IRTScoreType.Converged);
                }
            }
            //if (bestScore.Score == minTheta || bestScore.Score == maxTheta)
            //{
            //    // don't use 0.0 SE
            //    bestScore = new IRTScore(bestScore.Score, GetSE(testScores, bestScore.Score, seType), IRTScoreType.Converged);
            //}
            return bestScore;
        }

        private static bool IsLikelihoodIncreasing(List<ItemScore> testScores, double theta)
        {
            double deriv1 = 0.0;
            foreach (ItemScore ir in testScores)
                deriv1 += ir.ScoreInfo.IRTModel.D1LnlWrtTheta(ir.Score, theta);

            return deriv1 > 0.0;
        }

        // If Likelihood returns a NaN use this to estimate a "good" value.
        // This tries to find a theta as close to the given theta as possible where the Likelihood computation still return a number.
        // It only finds the "best" theta to within 1.0 on the theta scale.
        private static double ExtremeLikelihood(List<ItemScore> testScores, double theta)
        {
            double badtheta = theta;
            double goodtheta = 0.0;
            double l = Likelihood(testScores, goodtheta);
            while (Math.Abs(badtheta - goodtheta) > 1.0)
            {
                double newtheta = (badtheta + goodtheta)/2.0;
                double newl = Likelihood(testScores, newtheta);
                if (Double.IsNaN(newl))
                {
                    badtheta = newtheta;
                }
                else
                {
                    goodtheta = newtheta;
                    l = newl;
                }
            }
            if (Double.IsNaN(l))
                throw new ScoringEngineException("No good theta in ExtremeLikelihood!?");
            return l;
        }

        private static double Likelihood(List<ItemScore> testScores, double theta)
        {
            double x;
            double lnl = 0.0;
            foreach (ItemScore ir in testScores)
            {
                x = ir.ScoreInfo.IRTModel.ComputeProbability(ir.Score, theta);

                if (x <= 0.0) 
                    x = -1.0E10; //double.MinValue;
                else x = Math.Log(x);

                lnl += x;
            }
            return lnl;
        }

        // See Tao's TccSEM_a.doc
        internal static double Information(List<ItemScore> testScores, double theta)
        {
            double info = 0.0;
            foreach (ItemScore ir in testScores)
                info += ir.ScoreInfo.IRTModel.Information(theta);
            return info;
        }
    }//end class
}//end namespace
