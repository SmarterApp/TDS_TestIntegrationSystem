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
using ScoringEngine.ConfiguredTests;
using System.Text.RegularExpressions;
using System.Reflection;

namespace ScoringEngine.Scoring
{
    public class SpecificComputationRules
    {
        /// <summary>
        /// All scores for the current test 
        /// </summary>
        List<ItemScore> nonDroppedItemScores = null;
        List<ItemScore> allItemScores = null;
        /// <summary>
        /// First key is measure label, second key is "measure of".
        /// All public functions below are expected to each add one entry to this collection.
        /// The collection is kept here so that all computation rules have access to results of earlier computations.
        /// </summary>
        Dictionary<string, Dictionary<string, MeasureValue>> measures = new Dictionary<string,Dictionary<string,MeasureValue>>();
        TestCollection tc;
        string testName;
        DateTime testDate;
        DateTime forceCompletionDate;
        string enrolledGrade;
        string testStatus;
        string form = "";
        string[] forms;
        TestMode mode = TestMode.Online;
        List<TestAccomodation> TDSAccommodations;
        List<TestAccomodation> ARTAccommodations;

        internal Dictionary<string, Dictionary<string, MeasureValue>> MeasureValues
        {
            get
            {
                return measures;
            }
        }

        internal SpecificComputationRules(string testName, string status, string enrolledGrade, string form, Dictionary<string, Dictionary<string, MeasureValue>> measuresIn, List<ItemScore> scores, DateTime testDate, TestCollection tc, DateTime forceCompletionDate, TestMode mode, List<TestAccomodation> TDSAccomms, List<TestAccomodation> ARTAccomms)
        {
            CheckScores(scores);
            ScoreIntake(scores);
            this.tc = tc;
            this.testName = testName;
            this.testDate = testDate;
            this.measures = measuresIn;
            this.enrolledGrade = enrolledGrade;
            this.testStatus = status;
            this.form = form;
            this.mode = mode;
            this.forceCompletionDate = forceCompletionDate;
            this.TDSAccommodations = TDSAccomms;
            this.ARTAccommodations = ARTAccomms;
        }

        private void CheckScores(List<ItemScore> scores)
        {
            foreach (ItemScore score in scores)
            {
                if (score.Score < 0 && score.ConditionCode.Length == 0 && !score.IsFieldTest && score.IsSelected && score.IsAttempted && !score.TreatAsNotPresented() && score.Item.IsScored)
                    throw new ScoringEngineException("Score for item " + score.Item.ItemName + " is " + score.Score.ToString() + ", negative scores are not allowed");
                if (score.ScoreInfo.IRTModel != null && score.ScoreInfo.RecodeScore(score.Score) > score.ScoreInfo.ScorePoints && !score.IsFieldTest)
                    throw new ScoringEngineException("Item " + score.Item.ItemName + " is a " + score.ScoreInfo.ScorePoints.ToString() + " point item, and can therefore not have a score of " + score.ScoreInfo.RecodeScore(score.Score).ToString());
            }
        }

        private void ScoreIntake(List<ItemScore> scores)
        {
            allItemScores = scores;
            nonDroppedItemScores = new List<ItemScore>();
            foreach (ItemScore score in scores)
            {
                if (!score.IsDropped)
                    nonDroppedItemScores.Add(score);
            }
        }

        public void SBACAttemptedness(string measureOf, string measureLabel, Dictionary<string, int> testPart)
        {
            if (testStatus == "pending")
            {
                AddMeasureValue(measureLabel, measureOf, "N");
                return;
            }

            bool catItem = false;
            bool perfItem = false;
            foreach (ItemScore ir in allItemScores)
            {
                if (ir.IsSelected)
                {
                    if (!testPart.ContainsKey(ir.SegmentID))
                        throw new ScoringEngineException("SBACAttemptedness: Unknown segmentID '" + ir.SegmentID + " for item " + ir.Item.ItemName);

                    switch (testPart[ir.SegmentID])
                    {
                        case 1:
                            catItem = true;
                            break;
                        case 2:
                            perfItem = true;
                            break;
                        default:
                            throw new ScoringEngineException("Can't have a test part of " + testPart[ir.SegmentID]);
                    }
                }
            }
            if (catItem && perfItem)
                AddMeasureValue(measureLabel, measureOf, "Y");
            else
                AddMeasureValue(measureLabel, measureOf, "P");
        }

        /// <summary>
        /// This is the number of items presented. Field test and operational items are counted. If an item is 
        /// selectable it is counted (whether actually selected or not). If an item is scored on n dimensions it is 
        /// counted n times.
        /// </summary>
        /// <param name="measureOf"></param>
        public void ItemCount(string subscale, string measureLabel)
        {
            int cnt = 0;
            foreach (ItemScore ir in nonDroppedItemScores)
            {
                if (
                    (subscale.ToLower() == "overall" || ir.ScoreInfo.HasStrand(subscale))
                    && !ir.TreatAsNotPresented()
                    )
                {
                    cnt += 1;
                }
            }
            AddMeasureValue("ItemCount", subscale, cnt, 0.0);
        }

        /// <summary>
        /// Same as ItemCount but uses all relevant items on the specified list of strands
        /// </summary>
        /// <param name="subscale"></param>
        /// <param name="measureLabel"></param>
        /// <param name="subscales"></param>
        public void MultipleStrandItemCount(string subscale, string measureLabel, Dictionary<int, string> subscales)
        {
            int cnt = 0;
            foreach (ItemScore ir in nonDroppedItemScores)
            {
                if (
                    ItemOnSubScales(ir, subscales)
                    && !ir.TreatAsNotPresented()
                    )
                {
                    cnt += 1;
                }
            }
            AddMeasureValue("ItemCount", subscale, cnt, 0.0);
        }

        /// <summary>
        /// This is the number of items scored. Field test items are not counted. If an item is selectable it is 
        /// only counted if it was selected. If an item is scored on n dimensions (and is used in scoring) it is 
        /// counted n times. Note that inactive items are (usually) still scored and contribute to this count because 
        /// the item was not inactive when it was administered (but TreatAsNotPresented is supported)
        /// </summary>
        /// <param name="measureOf"></param>
        public void ItemCountScored(string subscale, string measureLabel)
        {
            int cnt = 0;
            foreach (ItemScore ir in nonDroppedItemScores)
            {
                if (
                    (subscale.ToLower() == "overall" || ir.ScoreInfo.HasStrand(subscale))
                    && !ir.IsFieldTest
                    && ir.Item.IsScored
                    && !ir.TreatAsNotPresented()
                    && ir.IsSelected
                    )
                {
                    cnt += 1;
                }
            }
            AddMeasureValue(measureLabel, subscale, cnt, 0.0);
        }

        /// <summary>
        /// Same as ItemCountScored but uses all relevant items on the specified list of strands
        /// </summary>
        /// <param name="subscale"></param>
        /// <param name="measureLabel"></param>
        /// <param name="subscales"></param>
        public void MultipleStrandItemCountScored(string subscale, string measureLabel, Dictionary<int, string> subscales)
        {
            int cnt = 0;
            foreach (ItemScore ir in nonDroppedItemScores)
            {
                if (
                    ItemOnSubScales(ir, subscales)
                    && !ir.IsFieldTest
                    && ir.Item.IsScored
                    && !ir.TreatAsNotPresented()
                    && ir.IsSelected
                    )
                {
                    cnt += 1;
                }
            }
            AddMeasureValue(measureLabel, subscale, cnt, 0.0);
        }

        /// <summary>
        /// Simply add up scores on all operational items on the relevant scale, applying weights as appropriate. 
        /// </summary>
        /// <param name="measureOf"></param>
        public void RawScore(string subscale, string measureLabel)
        {
            double rawScore = 0;
            foreach (ItemScore ir in nonDroppedItemScores)
            {
                if (
                    (subscale.ToLower() == "overall" || ir.ScoreInfo.HasStrand(subscale))
                    && !ir.IsFieldTest
                    && ir.Item.IsScored
                    && !ir.TreatAsNotPresented()
                    && ir.IsSelected
                    )
                {
                    double itemscore = ir.ScoreInfo.RecodeScore(ir.Score) * ir.ScoreInfo.Weight;
                    if (itemscore < 0)
                        throw new ScoringEngineException("Really!? Negative item score being added to raw score? Item " + ir.Item.ItemName + ", score = " + itemscore);
                    rawScore += itemscore;
                }
            }
            AddMeasureValue(measureLabel, subscale, rawScore, 0.0);
        }

        public void MultiStrandRawScore(string subscale, string measureLabel, Dictionary<int, string> subscales)
        {
            double rawScore = 0;
            foreach (ItemScore ir in nonDroppedItemScores)
            {
                if (
                    ItemOnSubScales(ir, subscales)
                    && !ir.IsFieldTest
                    && ir.Item.IsScored
                    && !ir.TreatAsNotPresented()
                    && ir.IsSelected
                    )
                {
                    double itemscore = ir.ScoreInfo.RecodeScore(ir.Score) * ir.ScoreInfo.Weight;
                    if (itemscore < 0)
                        throw new ScoringEngineException("Really!? Negative item score being added to raw score? Item " + ir.Item.ItemName + ", score = " + itemscore);
                    rawScore += itemscore;
                }
            }
            AddMeasureValue(measureLabel, subscale, rawScore, 0.0);
        }

        public void SBACTheta(string measureOf, string measureLabel, double LOT, double HOT, double seLimit)
        {
            SBACCATTheta(measureOf, measureLabel, LOT, HOT, seLimit, -1.0, 0.0);
        }

        public void SBACCATTheta(string measureOf, string measureLabel, double LOT, double HOT, double seLimit, double averageA, double averageB)
        {
            Dictionary<int, string> strands = new Dictionary<int, string>();
            strands[1] = measureOf;
            SBACCATMultiStrandTheta(measureOf, measureLabel, LOT, HOT, seLimit, strands, averageA, averageB);
        }

        public void SBACMultiStrandTheta(string measureOf, string measureLabel, double LOT, double HOT, double seLimit, Dictionary<int, string> strands)
        {
            SBACCATMultiStrandTheta(measureOf, measureLabel, LOT, HOT, seLimit, strands, -1.0, 0.0);
        }

        public void SBACCATMultiStrandTheta(string measureOf, string measureLabel, double LOT, double HOT, double seLimit, Dictionary<int, string> strands, double averageA, double averageB)
        {
            MeasureValue attemptedness = GetMeasureValue("Attempted", "Overall");
            if (attemptedness.ScoreString != "Y")
            {
                AddMeasureValue(measureLabel, measureOf, "");
                return;
            }

            List<ItemScore> sbacTestScores = SBACRecodeAndSubset(nonDroppedItemScores, strands, averageA, averageB);
            if (sbacTestScores == null)
            {
                AddMeasureValue(measureLabel, measureOf, "");
                return;
            }
            List<ItemScore> testScores = MultiSubScaleRecodeAndSubset(nonDroppedItemScores, strands);
            IRTScore thetaScore = MLEScorer.MLEScore3PL(sbacTestScores, 0.1, -15.0, 15.0);
            if (thetaScore.Type == IRTScoreType.Converged)
            {
                double score = thetaScore.Score;
                if (score < LOT) score = LOT;
                if (score > HOT) score = HOT;
                // SE is calculated based only on the answered item(s) for both complete and incomplete tests.
                double se = 1 / Math.Sqrt(MLEScorer.Information(testScores, score));
                if (se > seLimit) se = seLimit;
                if (se < -seLimit) se = -seLimit;
                AddMeasureValue(measureLabel, measureOf, score, se);
            }
            else if (thetaScore.Type == IRTScoreType.NoItems)
                AddMeasureValue(measureLabel, measureOf, "");
            else
                throw new ScoringEngineException("MLE scoring failed to converge");
        }

        public void SBACMultiSegmentTheta(string measureOf, string measureLabel, double LOT, double HOT, double seLimit, Dictionary<string, int> segments)
        {
            MeasureValue attemptedness = GetMeasureValue("Attempted", measureOf);
            if (attemptedness.ScoreString != "Y")
            {
                AddMeasureValue(measureLabel, measureOf, "");
                return;
            }

            List<ItemScore> sbacTestScores = SBACIABRecodeAndSubset(nonDroppedItemScores, segments);
            List<ItemScore> testScores = MultiSegmentRecodeAndSubset(nonDroppedItemScores, segments);
            IRTScore thetaScore = MLEScorer.MLEScore3PL(sbacTestScores, 0.1, -15.0, 15.0);
            if (thetaScore.Type == IRTScoreType.Converged)
            {
                double score = thetaScore.Score;
                if (score < LOT) score = LOT;
                if (score > HOT) score = HOT;
                // SE is calculated based only on the answered item(s) for both complete and incomplete tests.
                double se = 1 / Math.Sqrt(MLEScorer.Information(testScores, score));
                if (se > seLimit) se = seLimit;
                if (se < -seLimit) se = -seLimit;
                AddMeasureValue(measureLabel, measureOf, score, se);
            }
            else if (thetaScore.Type == IRTScoreType.NoItems)
                AddMeasureValue(measureLabel, measureOf, "");
            else
                throw new ScoringEngineException("SBACMultiSegmentTheta: MLE scoring failed to converge");
        }

        public void ScaleScore(string measureOf, string measureLabel)
        {
            MeasureValue thetaScoreValue = GetMeasureValue("ThetaScore", measureOf);
            if (thetaScoreValue.ScoreString.Length > 0)
            {
                double theta = thetaScoreValue.Score;
                double se = thetaScoreValue.StandardError;
                TestBlueprint tb = tc.GetBlueprint(testName);
                double scaleScore = tb.Intercept + theta * tb.Slope;
                double scaledSE = se * tb.Slope;
                AddMeasureValue(measureLabel, measureOf, scaleScore, scaledSE);
            }
            else
                AddMeasureValue(measureLabel, measureOf, "");
        }

        public void SBACNumBlocks(string measureOf, string measureLabel, Dictionary<int, string> blocks)
        {
            int numBlocksAttempted = 0;
            foreach (string block in blocks.Values)
            {
                if (GetMeasureValue("Attempted", block).ScoreString == "Y") numBlocksAttempted += 1;
            }
            AddMeasureValue(measureLabel, measureOf, numBlocksAttempted, 0.0);
        }

        public void SBACNumBlocksProficient(string measureOf, string measureLabel, Dictionary<int, string> blocks)
        {
            int numBlocksProficient = 0;
            foreach (string block in blocks.Values)
            {
                if (GetMeasureValue("PerformanceLevel", block).ScoreString == "3") numBlocksProficient += 1;
            }
            AddMeasureValue(measureLabel, measureOf, numBlocksProficient, 0.0);
        }

        /// <summary>
        ///  1 if score is   less than  or equal to proficiency cut - seMultiple * SE
        ///  3 if score is greater than or equal to proficiency cut + seMultiple * SE
        ///  2 otherwise
        /// </summary>
        /// <param name="measureOf"></param>
        /// <param name="measureLabel"></param>
        /// <param name="proficiencyPerformanceLevel">performance level corresponding to "proficient"</param>
        public void SEBasedPerformanceIndicator(string measureOf, string measureLabel, double seMultiple, int proficientPerformanceLevel)
        {
            MeasureValue scaleScoreValue = GetMeasureValue("ScaleScore", measureOf);
            if (scaleScoreValue.ScoreString.Length > 0)
            {
                double scaleScore = scaleScoreValue.Score;
                double se = scaleScoreValue.StandardError;
                TestBlueprint tb = tc.GetBlueprint(testName);
                double cutScore = Double.NaN;
                foreach (CutScoreStrand cs in tb.SubjectPerformanceLevel)
                {
                    if (cs.PerformanceLevel == proficientPerformanceLevel.ToString())
                        cutScore = cs.Min;
                }
                if (Double.IsNaN(cutScore))
                    throw new ScoringEngineException("Failed to find profiecient cut");

                string pl = "";
                if (scaleScore > cutScore + seMultiple * se)
                {
                    pl = "3";
                }
                else if (scaleScore < cutScore - seMultiple * se)
                {
                    pl = "1";
                }
                else
                {
                    pl = "2";
                }
                AddMeasureValue(measureLabel, measureOf, pl);
            }
            else
                AddMeasureValue(measureLabel, measureOf, "");
        }

        /// <summary>
        ///  1 if rounded scale score + seMultiple * SE is less than the proficiency cut
        ///  3 if rounded scale score - seMultiple * SE is greater than or equal to proficiency cut
        ///  2 otherwise
        /// </summary>
        /// <param name="measureOf"></param>
        /// <param name="measureLabel"></param>
        /// <param name="proficiencyPerformanceLevel">performance level corresponding to "proficient"</param>
        public void SEBasedPLWithRounding(string measureOf, string measureLabel, double seMultiple, int proficientPerformanceLevel, double LOT, double HOT)
        {
            MeasureValue scaleScoreValue = GetMeasureValue("ScaleScore", measureOf);
            MeasureValue thetaScoreValue = GetMeasureValue("ThetaScore", measureOf);
            if (scaleScoreValue.ScoreString.Length > 0)
            {

                string pl = "";
                double thetaScore = thetaScoreValue.Score;
                if (thetaScore < LOT + 1E-10)
                {
                    pl = "1";
                }
                else if (thetaScore > HOT - 1E-10)
                {
                    pl = "3";
                }
                else
                {
                    double scaleScore = scaleScoreValue.Score;
                    double se = scaleScoreValue.StandardError;
                    TestBlueprint tb = tc.GetBlueprint(testName);
                    double cutScore = Double.NaN;
                    foreach (CutScoreStrand cs in tb.SubjectPerformanceLevel)
                    {
                        if (cs.PerformanceLevel == proficientPerformanceLevel.ToString())
                            cutScore = cs.Min;
                    }
                    if (Double.IsNaN(cutScore))
                        throw new ScoringEngineException("Failed to find profiecient cut");

                    if (Math.Round(scaleScore - seMultiple * se, MidpointRounding.AwayFromZero) >= cutScore)
                    {
                        pl = "3";
                    }
                    else if (Math.Round(scaleScore + seMultiple * se, MidpointRounding.AwayFromZero) < cutScore)
                    {
                        pl = "1";
                    }
                    else
                    {
                        pl = "2";
                    }
                }
                AddMeasureValue(measureLabel, measureOf, pl);
            }
            else
                AddMeasureValue(measureLabel, measureOf, "");
        }

        public void TestPerformanceLevel(string measureOf, string measureLabel)
        {
            MeasureValue scaleScoreValue = GetMeasureValue("ScaleScore", measureOf);
            PLFrom(measureOf, measureLabel, scaleScoreValue);
        }

        /// <summary>
        /// Calculates performance level and adds it to the collection of measures
        /// </summary>
        /// <param name="measureOf"></param>
        /// <param name="measureLabel"></param>
        /// <param name="scaleScoreValue"></param>

        private void PLFrom(string measureOf, string measureLabel, MeasureValue scaleScoreValue)
        {
            string pl;
            PLFrom(measureOf, measureLabel, scaleScoreValue, out pl);
            AddMeasureValue(measureLabel, measureOf, pl);
        }

        /// <summary>
        /// Calculates performance level and returns as an output param.  Does not add to collection of measures
        /// </summary>
        /// <param name="measureOf"></param>
        /// <param name="measureLabel"></param>
        /// <param name="scaleScoreValue"></param>
        /// <param name="pl"></param>
        private void PLFrom(string measureOf, string measureLabel, MeasureValue scaleScoreValue, out string pl)
        {
            pl = "";
            if (scaleScoreValue.ScoreString.Length > 0)
            {
                double scaleScore = scaleScoreValue.Score;
                TestBlueprint tb = tc.GetBlueprint(testName);
                List<CutScoreStrand> cutScores = tb.SubjectPerformanceLevel;
                double roundedScore = Math.Round(scaleScore, MidpointRounding.AwayFromZero);
                CutScoreStrand min = null;
                CutScoreStrand max = null;

                if (cutScores.Count == 0)
                    throw new ScoringEngineException("No cutscores configured");
                foreach (CutScoreStrand ct in cutScores)
                {
                    if (ct.TestName.Equals(testName, StringComparison.CurrentCultureIgnoreCase) && ct.Domain == "Overall"
                        && ct.Min <= roundedScore && ct.Max > roundedScore)
                    {
                        pl = ct.PerformanceLevel;
                        break;
                    }
                    if (min == null || min.Min > ct.Min) min = ct;
                    if (max == null || max.Max < ct.Max) max = ct;
                }
                if (pl == "")
                {
                    if (roundedScore < min.Min)
                        pl = min.PerformanceLevel;
                    else if (roundedScore >= max.Max)
                        pl = max.PerformanceLevel;
                    else
                        throw new ScoringEngineException("Could not find performance level for scale: " + testName + ", Score: " + roundedScore);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measureLabel"></param>
        /// <param name="measureOf"></param>
        /// <param name="accomNoCodes"></param> Key is Accommodation Type and the value is the code that corresponds to "No". We need the Type in order to
        /// identify the corresponding "yes" codes.
        public void SBACAccommodationUseCodes(string measureLabel, string measureOf, Dictionary<string, string> accomNoCodes)
        {
            List<TestAccomodation> eligibleAccommodations = ARTAccommodations;
            List<TestAccomodation> availableAccommodations = TDSAccommodations;
            Dictionary<string, int> accommodationUseCodes = new Dictionary<string, int>();
            // find the set of known accommodations (for eligible and available)
            HashSet<string> knownEligibleAccommodations = new HashSet<string>();
            foreach (TestAccomodation eligibleAccom in eligibleAccommodations) knownEligibleAccommodations.Add(eligibleAccom.Type);
            HashSet<string> knownAvailableAccommodations = new HashSet<string>();
            foreach (TestAccomodation availableAccom in availableAccommodations) knownAvailableAccommodations.Add(availableAccom.Type);
            // start filling in accommodationUseCodes (measureOf as key, measure value as value)
            foreach (TestAccomodation eligibleAccom in eligibleAccommodations)
            {
                if (accomNoCodes.ContainsKey(eligibleAccom.Type))
                {
                    if (accomNoCodes[eligibleAccom.Type] == eligibleAccom.Code)
                    {
                        if (!knownAvailableAccommodations.Contains(eligibleAccom.Type))
                            accommodationUseCodes[eligibleAccom.ToTypeCodeString()] = 9; // means eligible = no, available = unknown
                        else
                            accommodationUseCodes[eligibleAccom.ToTypeCodeString()] = 12; // means eligible = no, available = no
                        // if there is an available accommodation of this type, we only want a no-no entry if the available accom is also no. This will
                        // be removed below.
                    }
                    else
                    {
                        if (knownAvailableAccommodations.Contains(eligibleAccom.Type))
                            accommodationUseCodes[eligibleAccom.ToTypeCodeString()] = 21; // means eligible = yes, available = no
                        // this will be changed to yes-yes below if needed.
                        else
                            accommodationUseCodes[eligibleAccom.ToTypeCodeString()] = 18; // means eligible = yes, available = unknown
                    }
                }
            }

            foreach (TestAccomodation availableAccom in availableAccommodations)
            {
                if (accomNoCodes.ContainsKey(availableAccom.Type))
                {
                    if (accomNoCodes[availableAccom.Type] == availableAccom.Code)
                    {
                        if (!knownEligibleAccommodations.Contains(availableAccom.Type))
                            accommodationUseCodes[availableAccom.ToTypeCodeString()] = 3; // means eligible = unknown, available = no
                        // nothing to do for yes-no or no-no
                    }
                    else
                    {
                        // first remove possible no-no
                        if (accommodationUseCodes.ContainsKey(availableAccom.Type + "|" + accomNoCodes[availableAccom.Type]))
                            accommodationUseCodes.Remove(availableAccom.Type + "|" + accomNoCodes[availableAccom.Type]);

                        if (accommodationUseCodes.ContainsKey(availableAccom.ToTypeCodeString()))
                        {
                            accommodationUseCodes[availableAccom.ToTypeCodeString()] = 24; // means eligible = yes, available = yes
                        }
                        else
                        {
                            if (knownEligibleAccommodations.Contains(availableAccom.ToTypeCodeString()))
                                accommodationUseCodes[availableAccom.ToTypeCodeString()] = 15; // means eligible = no, available = yes
                            else
                                accommodationUseCodes[availableAccom.ToTypeCodeString()] = 6; // means eligible = unknown, available = yes
                        }
                    }
                }
            }
            // Do unknow-unknown
            foreach (string type in accomNoCodes.Keys)
                if (!knownEligibleAccommodations.Contains(type) && !knownAvailableAccommodations.Contains(type))
                    accommodationUseCodes[type + "|" + accomNoCodes[type]] = 0;

            foreach (KeyValuePair<string, int> acc in accommodationUseCodes)
                AddMeasureValue("Accommodation", acc.Key, acc.Value, 0);
        }

        /// <summary>
        /// Replace unresponded items on incomplete tests on adaptive segments with "simulated" items. Make sure
        /// unresponded items on incomplete tests on fixed form segments get a score of 0.
        /// </summary>
        /// <param name="allTestResponses"></param>
        /// <param name="subscales"></param>
        /// <returns></returns>
        private List<ItemScore> SBACRecodeAndSubset(List<ItemScore> allTestResponses, Dictionary<int, string> subscales, double averageA, double averageB)
        {
            if (!IsPartialTest())
                return MultiSubScaleRecodeAndSubset(allTestResponses, subscales);

            TestBlueprint bp = tc.GetBlueprint(testName);
            // count responded items by segment

            Dictionary<string, int> segmentCounts = new Dictionary<string, int>();
            Dictionary<string, SegmentBlueprint> segments = new Dictionary<string, SegmentBlueprint>();
            foreach (string segmentName in bp.SegmentNames())
            {
                SegmentBlueprint sbp = bp.GetSegment(segmentName);
                segmentCounts[sbp.SegmentID] = 0;
                segments[sbp.SegmentID] = sbp;
            }

            foreach (ItemScore ir in allTestResponses)
            {
                if (ir.IsSelected && !ir.IsFieldTest)
                    segmentCounts[ir.SegmentID] += 1;
            }

            List<ItemScore> newResponses = new List<ItemScore>();

            foreach (KeyValuePair<string, int> kv in segmentCounts)
            {
                string segmentID = kv.Key;
                SegmentBlueprint sbp = segments[segmentID];
                if (kv.Value < sbp.MinItems)
                {
                    // found an incomplete segment
                    if (sbp.SelectionAlgorithm == TestBlueprint.SelectionAlgorithmType.Adaptive)
                    {
                        // Not doing this for contentlevels
                        if (subscales.Count > 1 || String.Compare(subscales[1], "overall", true) != 0)
                            return null;

                        if (averageA < 0)
                            throw new ScoringEngineException("Need to specify averageA and averageB for SBAC theta computation");

                        foreach (ItemScore ir in allTestResponses)
                        {
                            if (ir.SegmentID == segmentID
                                && !ir.IsFieldTest
                                && ir.Item.IsScored
                                && !ir.TreatAsNotPresented()
                                && !ir.IsDropped
                                )
                            {
                                if (ir.IsSelected)
                                    newResponses.Add(new ItemScore(ir.Item, ir.ScoreInfo, ir.ScoreInfo.RecodeScore(ir.Score), ir.ConditionCode, ir.IsFieldTest, ir.IsSelected, ir.IsAttempted));
                                else
                                    newResponses.Add(new ItemScore(ir.Item, ir.ScoreInfo, 0.0, ir.ConditionCode, ir.IsFieldTest, true, true));
                            }
                        }

                        int count = 0;
                        foreach (ItemScore ir in allTestResponses)
                        {
                            // include non selected items...
                            if (ir.SegmentID == segmentID && !ir.IsFieldTest)
                            {
                                count += 1;
                            }
                        }

                        TestItem ni = new TestItem(-1, -1, "", true, true, true, false, false, false, "", 1, segmentID);
                        TestItemScoreInfo nsi = new TestItemScoreInfo(-1, -1, "", ScoringEngine.MeasurementModels.IRTModelFactory.Model.IRT3PLn, "", 3, 1.0, 1);
                        nsi.SetParameter(0, averageA);
                        nsi.SetParameter(1, averageB);
                        nsi.SetParameter(2, 0.0);
                        ItemScore nis = new ItemScore(ni, nsi, 0.0, true, false);

                        for (int i = 0; i < sbp.MinItems - count; i++)
                        {
                            newResponses.Add(nis);
                        }
                    }
                    else if (sbp.SelectionAlgorithm == TestBlueprint.SelectionAlgorithmType.FixedForm)
                    {
                        Dictionary<string, ItemScore> scores = new Dictionary<string, ItemScore>();
                        foreach (ItemScore ir in allTestResponses)
                            if (ir.SegmentID == segmentID)
                                scores[ir.Item.ItemName + "-" + ir.ScoreInfo.Dimension] = ir;

                        // Assign a score of 0 to all non-selected items (even if they aren't passed to the scoring engine)
                        TestForm formObject = bp.GetForm(forms[sbp.TestPosition - 1]);
                        if (formObject == null)
                            throw new ScoringEngineException("No form '" + form + "' in the blueprint for test name '" + testName + "'");
                        for (int i = 0; i < formObject.Items.Count; i++)
                        {
                            TestItem fti = formObject.Items[i + 1];
                            foreach (TestItemScoreInfo si in fti.ScoreInfo)
                            {
                                if (scores.ContainsKey(fti.ItemName + "-" + si.Dimension))
                                {
                                    ItemScore ir = scores[fti.ItemName + "-" + si.Dimension];
                                    if (
                                        ItemOnSubScales(ir, subscales)
                                        && !ir.IsFieldTest
                                        && !ir.TreatAsNotPresented()
                                        && !ir.IsDropped
                                        && ir.Item.IsScored
                                    )
                                    {
                                        if (!ir.IsSelected)
                                            newResponses.Add(new ItemScore(ir.Item, ir.ScoreInfo, ir.ScoreInfo.RecodeScore(0.0), ir.ConditionCode, ir.IsFieldTest, true, true));
                                        else
                                            newResponses.Add(new ItemScore(ir.Item, ir.ScoreInfo, ir.ScoreInfo.RecodeScore(ir.Score), ir.ConditionCode, ir.IsFieldTest, ir.IsSelected, ir.IsAttempted));
                                    }
                                }
                                else
                                {
                                    // item dimension doesn't appear in TDS XML file (or wasn't sent to the TestScoringEngine from TDS (e.g. field test items)) but is on the form.
                                    if (
                                        ScoreInfoOnSubScales(si, subscales)
                                        && !fti.IsFieldTest
                                        && fti.IsScored
                                        )
                                    {
                                        newResponses.Add(new ItemScore(fti, si, 0, "", false, true, true));
                                    }
                                }
                            }
                        }
                    }
                    else
                        throw new ScoringEngineException("SBACRecodeAndSubset: Don't know how to deal with a segment of type " + sbp.SelectionAlgorithmString + " (segment " + kv.Key + ")");
                }
                else
                {
                    // all items responded
                    foreach (ItemScore ir in allTestResponses)
                    {
                        if (ir.SegmentID == segmentID
                            && ItemOnSubScales(ir, subscales)
                            && !ir.IsFieldTest
                            && ir.Item.IsScored
                            && !ir.TreatAsNotPresented()
                            && !ir.IsDropped
                            && ir.IsSelected
                            )
                        {
                            newResponses.Add(new ItemScore(ir.Item, ir.ScoreInfo, ir.ScoreInfo.RecodeScore(ir.Score), ir.ConditionCode, ir.IsFieldTest, ir.IsSelected, ir.IsAttempted));
                        }
                    }
                }
            }

            return newResponses;
        }

        /// <summary>
        /// Assign a score of 0 to unresponded items for items on the segments specified.
        /// </summary>
        /// <param name="allTestResponses"></param>
        /// <param name="subscales"></param>
        /// <returns></returns>
        private List<ItemScore> SBACIABRecodeAndSubset(List<ItemScore> allTestResponses, Dictionary<string, int> segments)
        {
            TestBlueprint bp = tc.GetBlueprint(testName);

            Dictionary<string, SegmentBlueprint> segmentObjects = new Dictionary<string, SegmentBlueprint>();
            foreach (string segmentName in bp.SegmentNames())
            {
                SegmentBlueprint sbp = bp.GetSegment(segmentName);
                if (segments.ContainsKey(sbp.SegmentID))
                    segmentObjects[sbp.SegmentID] = sbp;
            }

            List<ItemScore> newResponses = new List<ItemScore>();

            Dictionary<string, ItemScore> scores = new Dictionary<string, ItemScore>();
            foreach (ItemScore ir in allTestResponses)
                if (segments.ContainsKey(ir.SegmentID))
                    scores[ir.Item.ItemName + "-" + ir.ScoreInfo.Dimension] = ir;

            // Assign a score of 0 to all non-selected items (even if they aren't passed to the scoring engine)
            foreach (string segmentID in segments.Keys)
            {
                SegmentBlueprint sbp = segmentObjects[segmentID];
                TestForm formObject = bp.GetForm(forms[sbp.TestPosition - 1]);
                if (formObject == null)
                    throw new ScoringEngineException("No form '" + forms[sbp.TestPosition - 1] + "' in the blueprint for segment '" + segmentID + "'");
                for (int i = 0; i < formObject.Items.Count; i++)
                {
                    TestItem fti = formObject.Items[i + 1];
                    foreach (TestItemScoreInfo si in fti.ScoreInfo)
                    {
                        if (scores.ContainsKey(fti.ItemName + "-" + si.Dimension))
                        {
                            ItemScore ir = scores[fti.ItemName + "-" + si.Dimension];
                            if (
                                    !ir.IsFieldTest
                                    && !ir.TreatAsNotPresented()
                                    && !ir.IsDropped
                                    && ir.Item.IsScored
                               )
                            {
                                if (!ir.IsSelected)
                                    newResponses.Add(new ItemScore(ir.Item, ir.ScoreInfo, ir.ScoreInfo.RecodeScore(0.0), ir.ConditionCode, ir.IsFieldTest, true, true));
                                else
                                    newResponses.Add(new ItemScore(ir.Item, ir.ScoreInfo, ir.ScoreInfo.RecodeScore(ir.Score), ir.ConditionCode, ir.IsFieldTest, ir.IsSelected, ir.IsAttempted));
                            }
                        }
                        else
                        {
                            // item dimension doesn't appear in TDS XML file (or wasn't sent to the TestScoringEngine from TDS (e.g. field test items)) but is on the form.
                            if (
                                    !fti.IsFieldTest
                                    && fti.IsScored
                                )
                            {
                                newResponses.Add(new ItemScore(fti, si, 0, "", false, true, true));
                            }
                        }
                    }
                }
            }

            return newResponses;
        }

        /// <summary> 
        /// Drop field test items.
        /// Drop non-selected items.
        /// Drop items that should be treated as not-presented. 
        /// Keep only items on one of the given subscales.
        /// Recode item scores.
        /// </summary>
        /// <param name="allTestResponses"></param>
        /// <returns></returns>
        private static List<ItemScore> MultiSubScaleRecodeAndSubset(List<ItemScore> allTestResponses, Dictionary<int, string> subscales)
        {
            List<ItemScore> newResponses = new List<ItemScore>();
            foreach (ItemScore ir in allTestResponses)
            {
                if (
                    ItemOnSubScales(ir, subscales)
                    && !ir.IsFieldTest
                    && ir.Item.IsScored
                    && !ir.TreatAsNotPresented()
                    && !ir.IsDropped
                    && ir.IsSelected
                    )
                {
                    newResponses.Add(new ItemScore(ir.Item, ir.ScoreInfo, ir.ScoreInfo.RecodeScore(ir.Score), ir.ConditionCode, ir.IsFieldTest, ir.IsSelected, ir.IsAttempted));
                }
            }
            return newResponses;
        }

        private static List<ItemScore> MultiSegmentRecodeAndSubset(List<ItemScore> allTestResponses, Dictionary<string, int> segments)
        {
            List<ItemScore> newResponses = new List<ItemScore>();
            foreach (ItemScore ir in allTestResponses)
            {
                if (
                    segments.ContainsKey(ir.SegmentID)
                    && !ir.IsFieldTest
                    && ir.Item.IsScored
                    && !ir.TreatAsNotPresented()
                    && !ir.IsDropped
                    && ir.IsSelected
                    )
                {
                    newResponses.Add(new ItemScore(ir.Item, ir.ScoreInfo, ir.ScoreInfo.RecodeScore(ir.Score), ir.ConditionCode, ir.IsFieldTest, ir.IsSelected, ir.IsAttempted));
                }
            }
            return newResponses;
        }

        /// <summary> 
        /// Drop field test items.
        /// Drop non-selected items.
        /// Drop non-attempted items.
        /// Drop items that should be treated as not-presented. 
        /// Keep only items on the current scale.
        /// Recode item scores.
        /// </summary>
        /// <param name="allTestResponses"></param>
        /// <returns></returns>
        private List<ItemScore> RecodeAndSubsetAttempted(List<ItemScore> allTestResponses, string subscale)
        {
            List<ItemScore> newResponses = new List<ItemScore>();
            foreach (ItemScore ir in allTestResponses)
            {
                if (
                        (subscale.ToLower() == "overall" || ir.ScoreInfo.HasStrand(subscale))
                        && !ir.IsFieldTest
                        && ir.Item.IsScored
                        && !ir.TreatAsNotPresented()
                        && ir.IsSelected
                        && ir.IsAttempted
                    )
                {
                    newResponses.Add(new ItemScore(ir.Item, ir.ScoreInfo, ir.ScoreInfo.RecodeScore(ir.Score), ir.ConditionCode, ir.IsFieldTest, ir.IsSelected, ir.IsAttempted));
                    if (ir.ScoreInfo.IRTModel == null)
                        throw new ScoringEngineException("Item " + ir.Item.ItemName + " isn't associated with a valid IRT model");
                }
            }
            
            //TextWriter tw = new StreamWriter(@"C:\tmp\SubsetItemsSE.csv", false);
            //foreach (ItemScore iS in newResponses)
            //{
            //    tw.WriteLine(iS.Item.ItemName);
            //}
            //tw.Close();
     
            return newResponses;
        }

        private bool IsPartialTest()
        {
            return (testStatus == "partial" || testStatus == "paused" || testStatus == "denied" || testStatus == "expired"
                || testStatus == "started" || testStatus == "suspended" || testStatus == "review");
        }

        private static bool ItemOnSubScales(ItemScore ir, Dictionary<int, string> subscales)
        {
            return ScoreInfoOnSubScales(ir.ScoreInfo, subscales);
        }

        private static bool ScoreInfoOnSubScales(TestItemScoreInfo si, Dictionary<int, string> subscales)
        {
            foreach (string subscale in subscales.Values)
            {
                if (String.Compare("overall", subscale, true) == 0)
                    return true;
                if (si.HasStrand(subscale))
                    return true;
            }
            return false;
        }

        private MeasureValue GetMeasureValue(string measureLabel, string measureOf)
        {
            if (measures.ContainsKey(measureLabel))
                if (measures[measureLabel].ContainsKey(measureOf))
                    return measures[measureLabel][measureOf];
                else
                    throw new ScoringEngineException("No " + measureLabel + " available for measure of '" + measureOf + "', problem with computation order?");
            else
                throw new ScoringEngineException("No " + measureLabel + " available. Problem with computation order?");
        }

        private void AddMeasureValue(string measureLabel, string measureOf, string scoreString)
        {
            if (!measures.ContainsKey(measureLabel)) measures[measureLabel] = new Dictionary<string, MeasureValue>();
            if (measures[measureLabel].ContainsKey(measureOf) && !(measures[measureLabel][measureOf].ScoreString.Length == 0 && measures[measureLabel][measureOf].StandardErrorString.Length == 0))
            {
                if (measures[measureLabel][measureOf].ScoreString != scoreString)
                    throw new ScoringEngineException("Scores changed. Already have a '" + measureLabel + "' score of " + measures[measureLabel][measureOf].ScoreString + " for " + measureOf + ", but now we have a new value of " + scoreString);
                if (measures[measureLabel][measureOf].StandardErrorString.Length != 0)
                    throw new ScoringEngineException("SE changed. Already have a '" + measureLabel + "' SE of " + measures[measureLabel][measureOf].StandardErrorString + " for " + measureOf + ", but now we have a blank value");
            }
            measures[measureLabel][measureOf] = new MeasureValue(measureLabel, measureOf, scoreString);
        }

        private void AddMeasureValue(string measureLabel, string measureOf, double score, double se)
        {
            if (!measures.ContainsKey(measureLabel)) measures[measureLabel] = new Dictionary<string, MeasureValue>();
            if (measures[measureLabel].ContainsKey(measureOf) && !(measures[measureLabel][measureOf].ScoreString.Length == 0 && measures[measureLabel][measureOf].StandardErrorString.Length == 0))
            {
                if (Double.IsNaN(measures[measureLabel][measureOf].Score))
                {
                    if (measures[measureLabel][measureOf].ScoreString != score.ToString())
                        throw new ScoringEngineException("Scores changed. Already have a '" + measureLabel + "' score of " + measures[measureLabel][measureOf].ScoreString + " for " + measureOf + ", but now we have a new value of " + score.ToString());
                }
                else
                {
                    if (
                        (score < 10 && (Math.Abs(measures[measureLabel][measureOf].Score - score) > Math.Pow(10, -5)))
                        || (score >= 10 && (Math.Abs((measures[measureLabel][measureOf].Score - score) / score) > Math.Pow(10, -5))))
                        throw new ScoringEngineException("Scores changed. Already have a '" + measureLabel + "' score of " + measures[measureLabel][measureOf].ScoreString + " for " + measureOf + ", but now we have a new value of " + score.ToString());
                }

                if (Double.IsNaN(measures[measureLabel][measureOf].StandardError))
                {
                    if (measures[measureLabel][measureOf].StandardErrorString != se.ToString())
                        throw new ScoringEngineException("SE changed. Already have a '" + measureLabel + "' SE of " + measures[measureLabel][measureOf].StandardErrorString + " for " + measureOf + ", but now we have a SE value of" + se.ToString());
                }
                else
                {
                    if (
                        (se < 10 && (Math.Abs(measures[measureLabel][measureOf].StandardError - se) > Math.Pow(10, -5)))
                        || (se >= 10 && Math.Abs((measures[measureLabel][measureOf].StandardError - se) / se) > Math.Pow(10, -5)))
                        throw new ScoringEngineException("SE changed. Already have a '" + measureLabel + "' SE of " + measures[measureLabel][measureOf].StandardErrorString + " for " + measureOf + ", but now we have a SE value of" + se.ToString());
                }
            }
            measures[measureLabel][measureOf] = new MeasureValue(measureLabel, measureOf, score, se);
        }
    }
}
