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
using System.Data;
using ScoringEngine.MeasurementModels;

namespace ScoringEngine.ConfiguredTests
{
    public class TestItem
    {
        private bool parametersOnTestScale = true;
		#region Properties

		private int itemBank;
		public int ItemBank
		{
			get
			{
				return itemBank;
			}
		}

		private long itemID;
		public long ItemID
		{
			get
			{
				return itemID;
			}
		}

        public string ItemName
        {
            get
            {
                return itemBank.ToString() + '-' + itemID.ToString();
            }
        }

		private Stimulus stimulusInfo = null;
		public Stimulus StimulusInfo
		{
			get
			{
				return stimulusInfo;
			}
			set
			{
				stimulusInfo = value;
			}
		}

		private bool isActive;
		public bool IsActive
		{
			get
			{
				return isActive;
			}
		}

        private bool isRequired;
        public bool IsRequired
        {
            get
            {
                return isRequired;
            }
        }

		private bool isScored;
		public bool IsScored
		{
			get
			{
				return isScored;
			}
		}

		private bool isFieldTest;
		public bool IsFieldTest
		{
			get
			{
				return isFieldTest;
			}
		}

		private string answer;
		public string Answer
		{
			get
			{
				return answer;
			}
		}

        public bool IsMC
        {
            get
            {
                // this is a bit dodgy...
                return (answer == "A" || answer == "B" || answer == "C" || answer == "D" || answer == "E");
            }
        }

		private int position;
		public int Position
		{
			get
			{
				return position;
			}
		}

        private int scorePoint;
        public int ScorePoint
        {
            get
            {
                return scorePoint;
            }
        }

        private List<TestItemScoreInfo> scoreInfo;
        public List<TestItemScoreInfo> ScoreInfo
        {
            get
            {
                return scoreInfo;
            }
        }

        private string strand;
        /// <summary>
        /// Strand used by the adaprive algorithm
        /// </summary>
        public string Strand
        {
            get
            {
                return strand;
            }
        }

        private string testSegment;
        /// <summary>
        /// TestSegment that the item is associated with
        /// </summary>
        public string TestSegment
        {
            get
            {
                return testSegment;
            }
        }

        #endregion

        public TestItem(int itemBank, long itemID, string strand,
            bool isActive, bool isRequired, bool isScored, bool notForScoring, bool isFieldtest, bool parametersOnTestScale, string answer, int scorePoint, string testSegment)
        {
            this.itemBank = itemBank;
			this.itemID = itemID;
            this.strand = strand;
			this.isActive = isActive;
            this.isRequired = isRequired;
			this.isScored = (isScored && !notForScoring);  // the input isScored is true if scorePoints > 0.
			this.isFieldTest = isFieldtest;
			this.parametersOnTestScale = parametersOnTestScale;

			this.answer = answer;
			this.scorePoint = scorePoint;

            this.testSegment = testSegment;

            scoreInfo = new List<TestItemScoreInfo>();
        }

        public TestItem DeepCopy()
        {
            TestItem copy = (TestItem) (this.MemberwiseClone());
            if (scoreInfo != null)
                copy.scoreInfo = scoreInfo.ConvertAll(x => x.DeepCopy());
            return copy;
        }

        private TestItemScoreInfo FindScoreInfo(string dimension, IRTModelFactory.Model irtType)
        {
            foreach (TestItemScoreInfo score in this.scoreInfo)
                if (score.Dimension == dimension && score.MeasurementModel == irtType)
                    return score;
            return null;
        }

        public bool HasDimension(string dimension)
        {
            foreach (TestItemScoreInfo score in this.scoreInfo)
                if (score.Dimension == dimension)
                    return true;
            return false;
        }

        public TestItemScoreInfo GetScoreInfo(string dimension)
        {
            foreach (TestItemScoreInfo score in this.scoreInfo)
                if (score.Dimension.Equals(dimension, StringComparison.InvariantCultureIgnoreCase))
                    return score;
            return null;
        }

        public void SetDimension(string dimension, IRTModelFactory.Model model, string recodeRule, double weight, int points, int parameteCount)
        {
            if (recodeRule.StartsWith("GRR(") && recodeRule.Length != 2 * ScorePoint + 6)
                throw new ScoringEngineException("recodeRule " + recodeRule + " for item " + ItemName + " does not seem to provide a recode for each of its possible scores (scorePoint = " + ScorePoint + ")");
            scoreInfo.Add(new TestItemScoreInfo(this.itemBank, this.itemID, dimension, model, recodeRule, parameteCount, weight, points));
        }

        public int CountItemsWithFeature(string featureName, string featureValue)
        {
            int count = 0;
            foreach (TestItemScoreInfo si in this.scoreInfo)
            {
                if (si.HasFeature(featureName, featureValue))
                    count += 1;
            }
            return count;
        }

        public void RescaleParameters(double slope, double intercept)
        {
            if (this.parametersOnTestScale)
                foreach (TestItemScoreInfo score in this.scoreInfo)
                    score.RescaleParameters(slope, intercept);
        }

        public void SetParameters(string testName, DataTable dtItemParameters)
        {
            foreach (TestItemScoreInfo score in this.scoreInfo)
            {
                DataRow[] drItemParameters = dtItemParameters.Select(
                    string.Format("_fk_testName = '{0}' AND _fk_AIRBank = {1} AND _fk_ItemName = '{2}' AND Dimension = '{3}' AND _fk_MeasurementModel = {4}",
                        testName, this.itemBank, this.itemID, score.Dimension, (int)score.MeasurementModel),
                    "_fk_MeasurementParameter");

                if (drItemParameters.Length != score.ParameterCount)
                    throw new Exception("Bad Configuration: item " + ItemName + " does not have the correct number of parameters");

                for (int i = 0; i < score.ParameterCount; i++)
                {
                    if (Util.Value(drItemParameters[i]["_fk_MeasurementParameter"], 0) != i)
                        throw new Exception("Bad Configuration: item " + this.ItemName + " does not have sequential parameters starting at 0");
                    score.SetParameter(i, Util.Value(drItemParameters[i]["ParameterValue"], 0.0));
                }
            }
        }

        public void SetFeature(string dimension, IRTModelFactory.Model model, string featureName, string featureValue)
        {
            TestItemScoreInfo score = FindScoreInfo(dimension, model);
            if (score == null)
                throw new Exception("Item " + this.ItemName + " does not have dimension " + dimension + " and IRT model " + model.ToString());
            score.SetFeature(featureName, featureValue);
        }

        public int SimulatedScore(Random rand, double theta)
        {
            int score = 0;
            foreach (TestItemScoreInfo dimension in this.scoreInfo)
                score += dimension.SimulatedScore(rand, theta);
            return score;
        }
    }//end class
}//end namespace
