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
using ScoringEngine.MeasurementModels;

namespace ScoringEngine.ConfiguredTests
{
    public class TestItemScoreInfo
    {
        #region Properties

        private string dimension;
        public string Dimension
        {
            get
            {
                return dimension;
            }
        }

        public int ParameterCount
        {
            get
            {
                if (irtModel == null)
                    return 0;
                else
                    return irtModel.ParameterCount;
            }
        }

        public string Strand
        {
            get
            {
                if (!_features.ContainsKey("Strand"))
                    throw new Exception("Item " + itemName.ToString() + " in bank " + bank.ToString() + " does not have a 'Strand' feature");
                if (_features["Strand"].Count != 1)
                    throw new Exception("Item " + itemName.ToString() + " in bank " + bank.ToString() + " does not have a unique strand value");
                return _features["Strand"][0];
            }
        }

        private IRTModelFactory.Model measurementModel;
        public IRTModelFactory.Model MeasurementModel
        {
            get
            {
                return measurementModel;
            }
        }

        private string recodeRuleName;
        public string RecodeRuleName
        {
            get
            {
                return recodeRuleName;
            }
        }

        private double weight;
        public double Weight
        {
            get
            {
                return weight;
            }
        }

        private int scorePoints;
        public int ScorePoints
        {
            get
            {
                return scorePoints;
            }
        }

        private IRTModel irtModel;
        public IRTModel IRTModel
        {
            get
            {
                return irtModel;
            }
        }

        #endregion

        private Dictionary<string, List<string>> _features = new Dictionary<string, List<string>>(); //key is type of features followed by a list of values
        private long itemName;
        private int bank;

        public TestItemScoreInfo(int itemBank, long itemName, string dimension, IRTModelFactory.Model irtType, string recodeRule, int parameterCount, double weight, int scorePoint)
        {
            this.bank = itemBank;
            this.itemName = itemName;
            this.dimension = dimension;
            this.measurementModel = irtType;
            if (irtType != IRTModelFactory.Model.Unknown)
            {
    			this.irtModel = IRTModelFactory.CreateModel(irtType);
	    		this.irtModel.SetParameterCount(parameterCount, scorePoint);
            }
            this.recodeRuleName = recodeRule;
            this.weight = weight;
			this.scorePoints = scorePoint;
            if (recodeRule.StartsWith("GRR("))
            {
                string[] recodes = recodeRuleName.Substring(4, recodeRuleName.Length - 5).Split('-');
                foreach (string recode in recodes)
                {
                    if (recode.Length != 1)
                        throw new ScoringEngineException("Recodes must be one digit integers: Item " + bank + "-" + itemName + ", recodeRule " + recodeRuleName);
                    if (!Char.IsDigit(recode[0]))
                        throw new ScoringEngineException("Recodes must be integers: Item " + bank + "-" + itemName + ", recodeRule " + recodeRuleName);
                    if (Convert.ToInt32(recode) > scorePoint)
                        throw new ScoringEngineException("Recodes must be less than or equal to dimension score points: Item " + bank + "-" + itemName + ", recodeRule " + recodeRuleName);
                }
            }
        }

        public TestItemScoreInfo DeepCopy()
        {
            TestItemScoreInfo copy = (TestItemScoreInfo)(this.MemberwiseClone());
            if (_features != null)
            {
                copy._features = new Dictionary<string, List<string>>();
                foreach (KeyValuePair<string, List<string>> kvp in _features)
                    copy._features.Add(kvp.Key, new List<string>(kvp.Value));
            }
            if (irtModel != null)
                copy.irtModel = irtModel.DeepCopy();
            return copy;
        }


        public void SetParameter(int position, double value)
        {
            irtModel.SetParameter(position, value);
        }

        /// <summary>
        /// Identifies this item as having a specific feature/value pair.
        /// </summary>
        /// <param name="featureType"></param>
        /// <param name="featureValue"></param>
        public void SetFeature(string featureType, string featureValue)
        {

            if (!_features.ContainsKey(featureType))
            {
                _features.Add(featureType, new List<string>());
            }

            List<string> lstValues = _features[featureType];

            if (lstValues.Find(delegate(string exists) { return exists.Equals(featureValue, StringComparison.InvariantCultureIgnoreCase); }) != null)
                lstValues.RemoveAll(delegate(string exists) { return exists.Equals(featureValue, StringComparison.InvariantCultureIgnoreCase); });

            // Special handling of Strand feature: resolve pipe delimited hierarchy into separate strands
            if (featureType == "Strand")
            {
                string value = featureValue;
                int len = value.Length;
                do
                {
                    value = value.Substring(0, len);
                    lstValues.Add(value);
                    len = value.LastIndexOf('|');
                } while (len > 0);
            }
            else
                lstValues.Add(featureValue);
        }

        /*
        public bool HasFeature(string featureName, string featureValue)
        {
            if (_features.ContainsKey(featureName))
                if (_features[featureName].Find(delegate(string fv) { int i = fv.IndexOf('|'); if (i == -1) return fv == featureValue; else return fv.Substring(0, i) == featureValue; }) != null)
                    return true;
            return false;
        }
        */

        public List<string> FeaturesByName(string featureName)
        {
            if (_features.ContainsKey(featureName))
                return _features[featureName];
            else
                return new List<string>();
        }

        public bool HasFeature(string featureName, string featureValue)
        {
            if (_features.ContainsKey(featureName))
                if (_features[featureName].Find(delegate(string fv) { return fv.Equals(featureValue, StringComparison.InvariantCultureIgnoreCase); }) != null)
                    return true;
            return false;
        }

        internal bool FeatureHasSubstring(string featureName, string featureValueSubstring)
        {
            if (_features.ContainsKey(featureName))
                if (_features[featureName].Find(delegate(string fv) { return fv.Contains(featureValueSubstring); }) != null)
                    return true;
            return false;
        }

        public bool HasStrand(string strandkey)
        {
            if (_features.ContainsKey("Strand"))
                if (_features["Strand"].Find(delegate(string fv) { return fv.Equals(strandkey, StringComparison.InvariantCultureIgnoreCase); }) != null)
                    return true;
            return false;
        }

        /*
        public bool HasTopStrand(string strandkey)
        {
            // Note that the strand value in the item bank is the leaf in the strand hierarchy. This function just checks for the top strand,
            // the string before the first pipe symbol.
            if (_features.ContainsKey("Strand"))
                if (_features["Strand"].Find(delegate(string fv) { int i = fv.IndexOf('|'); if (i == -1) return fv == strandkey; else return fv.Substring(0, i) == strandkey; }) != null)
                    return true;
            return false;
        }
         * */

        public void RescaleParameters(double slope, double intercept)
        {
            irtModel.RescaleParameters(slope, intercept);
        }

        public double RecodeScore(double score)
        {
            if (recodeRuleName == "")
                return score;
            if (recodeRuleName.StartsWith("GRR("))
            {
                string[] recodes = recodeRuleName.Substring(4, recodeRuleName.Length - 5).Split('-');
                int sc = Convert.ToInt32(score);
                if (Math.Abs(score - sc) > 0.000001)
                    throw new ScoringEngineException("Can't recode " + score + " because it isn't an integer. Item " + bank + "-" + itemName + ", recodeRule " + recodeRuleName);
                return Convert.ToDouble(recodes[sc]);
            }
            switch (recodeRuleName)
            {
                case "DCAS_Alt_PresentedRule":
                case "ELPA_CC_Rule":
                    return score;
                case "ELPA_Grammar_Recode":
                    if (score > 1.9)
                        return score - 1.0;
                    else
                        return score;
                case "ELPA_Illocution_Recode":
                    if (score > 0.9)
                        return score - 1.0;
                    else
                        return score;
                case "DCAS_Alt_EI_Recode":
                    if (score > 0.9)
                        return score - 1.0;
                    else
                        return score;
                case "Utah_Writing_ER_Recode":
                    if (score > 0.9)
                        return score - 1.0;
                    else
                        return score;
                default:
                    throw new ScoringEngineException("Item " + itemName.ToString() + " in bank " + bank.ToString() + " has an unknown recode rule: " + recodeRuleName);
            }
        }

        //// only used when student had a perfect score
        //internal double MaxUnrecodedScore()
        //{
        //    if (recodeRuleName == "" || recodeRuleName == "ELPA_CC_Rule")
        //        return this.scorePoints;

        //    if (recodeRuleName == "ELPA_Grammar_Recode" || recodeRuleName == "ELPA_Illocution_Recode")
        //    {
        //        return this.scorePoints + 1;
        //    }

        //    throw new Exception("Item " + itemName.ToString() + " in bank " + bank.ToString() + " has an unknown recode rule: " + recodeRuleName);
        //}

        //internal double MaxUnrecodedScoreAdjust(double EndAdjust)
        //{
        //    if (recodeRuleName == "" || recodeRuleName == "ELPA_CC_Rule")
        //        return this.scorePoints - EndAdjust;
        //    if (recodeRuleName == "ELPA_Grammar_Recode" || recodeRuleName == "ELPA_Illocution_Recode")
        //    {
        //        return this.scorePoints + 1 - EndAdjust;
        //    }

        //    throw new Exception("Item " + itemName.ToString() + " in bank " + bank.ToString() + " has an unknown recode rule: " + recodeRuleName);
        //}

        internal int SimulatedScore(Random rand, double theta)
        {
            double rnd = rand.NextDouble();
            double probOfScoreOrLess = 0.0;
            for (int score = 0; score < ScorePoints; score++)
            {
                probOfScoreOrLess += IRTModel.ComputeProbability(score, theta);
                if (rnd < probOfScoreOrLess)
                    return score;
            }
            return ScorePoints;
        }
    }
}
