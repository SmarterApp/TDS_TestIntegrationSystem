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
using TDSQASystemAPI.TestResults;

namespace TDSQASystemAPI.Config
{
    public class TestBlueprint
    {
        //pool must take airbank into account as well.
        private int itemBankKey = -1;
        private Dictionary<long, TestItem> _itemPool = new Dictionary<long, TestItem>();
        private Dictionary<string, TestForm> _forms = new Dictionary<string, TestForm>();
        private Dictionary<string, List<FeatureSpec>> _featureSpecifications = new Dictionary<string, List<FeatureSpec>>();
        private List<CutScoreStrand> _subjectPL = new List<CutScoreStrand>();
		
		public enum SelectionAlgorithmType { FixedForm = 0, Adaptive = 1 };

		#region Properties

		private double minPossibleScore = 0.0;
		internal double MinScore
		{
			get
			{
				return (minPossibleScore - Intercept) / Slope;
			}
		}

		private double maxPossibleScore = 0.0;
		internal double MaxScore
		{
			get
			{
				return (maxPossibleScore - Intercept) / Slope;
			}
		}

		private double slope = 1.0;
		internal double Slope
		{
			get
			{
				return slope;
			}
		}

		private double intercept = 0.0;
		internal double Intercept
		{
			get
			{
				return intercept;
			}
		}

		private string testName = string.Empty;
		internal string TestName
		{
			get
			{
				return testName;
			}
		}

		private int maxItems = 0;
		internal int MaxItems
		{
			get
			{
				return maxItems;
			}
		}

		private int minItems = 0;
		internal int MinItems
		{
			get
			{
				return minItems;
			}
		}

		private int selectNumber = 0;
		internal int SelectNumber
		{
			get
			{
				return selectNumber;
			}
		}

		private double startDifficultyMax = 0;
		internal double StartDifficultyMax
		{
			get
			{
				return startDifficultyMax;
			}
		}

		private double startDifficultyMin = 0;
		internal double StartDifficultyMin
		{
			get
			{
				return startDifficultyMin;
			}
		}

		private double startAbility = 0;
		internal double StartAbility
		{
			get
			{
				return startAbility;
			}
		}

		private int minNumFieldTest = 0;
		internal int MinNumFieldTest
		{
			get
			{
				return minNumFieldTest;
			}
		}

		private int maxNumFieldTest = 0;
		internal int MaxNumFieldTest
		{
			get
			{
				return maxNumFieldTest;
			}
		}

		private int fieldTestStartPosition = 0;
		internal int FieldTestStartPosition
		{
			get
			{
				return fieldTestStartPosition;
			}
		}

		private int fieldTestEndPosition = 0;
		internal int FieldTestEndPosition
		{
			get
			{
				return fieldTestEndPosition;
			}
		}

		private DateTime fieldTestStartDate;
		internal DateTime FieldTestStartDate
		{
			get
			{
				return fieldTestStartDate;
			}
		}

		private DateTime fieldTestEndDate;
		internal DateTime FieldTestEndDate
		{
			get
			{
				return fieldTestEndDate;
			}
		}

		private int opportunityLimit = 0;
		internal int OpportunityLimit
		{
			get
			{
				return opportunityLimit;
			}
		}

		private SelectionAlgorithmType selectionAlgorithm = SelectionAlgorithmType.Adaptive;
		internal SelectionAlgorithmType SelectionAlgorithm
		{
			get
			{
				return selectionAlgorithm;
			}
		}

		#endregion


        internal TestBlueprint(string testName, int itemBankKey, int maxItems, int minItems, int selectNum, double startDifMax, double startDifMin, 
			int minNumFieldTest, int maxNumFieldTest, int FieldTestStartPosition, int FieldTestEndPosition, DateTime FieldTestStartDate, 
			DateTime FieldTestEndDate, double minPoss, double maxPoss, double slope, double intercept, int opportunity, 
			SelectionAlgorithmType selectionAlgorithm)
        {
            this.itemBankKey = itemBankKey;
            this.testName = testName;
            this.maxItems = maxItems;
			this.minItems = minItems;
			this.selectNumber = selectNum;
			this.startDifficultyMax = startDifMax;
			this.startDifficultyMin = startDifMin;
			this.minNumFieldTest = minNumFieldTest;
			this.maxNumFieldTest = maxNumFieldTest;
			this.fieldTestStartPosition = FieldTestStartPosition;
			this.fieldTestEndPosition = FieldTestEndPosition;
			this.fieldTestStartDate = FieldTestStartDate;
			this.fieldTestEndDate = FieldTestEndDate;
			this.minPossibleScore = minPoss;
			this.maxPossibleScore = maxPoss;
			this.intercept = intercept;
			this.slope = slope;
			this.opportunityLimit = opportunity;
			this.selectionAlgorithm = selectionAlgorithm;
        }

        #region Loading


        /// <summary>
        /// instantiates a featureSpecification object and adds it to the blueprint. If it finds a matching existing specification, 
        /// it replaces it with the new one
        /// </summary>
        /// <param name="featureName">e.g., Strand</param>
        /// <param name="featureValue">e.g., Algebra</param>
        /// <param name="minNum">minimum allowable number</param>
        /// <param name="maxNum">maximim allowable number</param>
        /// <param name="startAbility">starting ability on for this feature/value pair. May be null</param>
        /// <param name="startInfo">starting information on this feature/value pair. May be null</param>
        /// <param name="cut">cut score on this feature/value pair</param>
        /// <param name="lambda">lambda multiplier. see adaptive algorithm</param>
        internal void AddFeatureSpecification(string featureName, string featureValue, int minNum, int maxNum, double startAbility, double startInfo, double cut, double lambda, double scaleCutScore)
        {
            FeatureSpec spec = new FeatureSpec(featureName, featureValue, minNum, maxNum, startAbility, startInfo, cut, lambda, scaleCutScore);

            if (!_featureSpecifications.ContainsKey(featureName))
            {
                _featureSpecifications.Add(featureName, new List<FeatureSpec>());
            }

            List<FeatureSpec> lstSpec = _featureSpecifications[featureName];

            if (lstSpec.Find(delegate(FeatureSpec existSpec) { return (featureValue == existSpec.FeatureValue); }) != null)
                lstSpec.RemoveAll(delegate(FeatureSpec existSpec) { return (featureValue == existSpec.FeatureValue); });

            lstSpec.Add(spec);
        }

        /// <summary>
        /// Adds or replaces the item of this name
        /// </summary>
        /// <param name="ti"></param>
        internal void AddItem(TestItem ti)
        {
            if (_itemPool.ContainsKey(ti.ItemName))
                _itemPool.Remove(ti.ItemName);

            _itemPool[ti.ItemName] = ti;
        }

        /// <summary>
        /// Add an item to a form
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="ti"></param>
        internal void AddFormItem(string formName, long formKey, int position, TestItem ti)
        {
            if (!_forms.ContainsKey(formName))
                _forms[formName] = new TestForm(formName, formKey);
            _forms[formName].AddItem(formKey, position, ti);
        }

        internal TestForm GetForm(string formName)
        {
            if (_forms.ContainsKey(formName))
                return _forms[formName];
            else
                return null;
        }

        internal void AddTestPerformanceLevels(double min, double max, string testName, string performanceLevel)
        {
            CutScoreStrand cs = new CutScoreStrand(min, max, testName, performanceLevel);
            _subjectPL.Add(cs);
        }

        #endregion loading

        #region Access

        internal List<FeatureSpec> GetFeatureSpecifications(string strandId)
        {
            return _featureSpecifications[strandId];
        }

        internal void PrintBlueprintSummary(TextWriter tw)
        {
            tw.WriteLine("{0}{0}Blueprint: {1}. Feature Specifications: {2}. Items: {3}", Environment.NewLine, TestName, CountFeatureSpecifications(), _itemPool.Count.ToString());
            foreach (List<FeatureSpec> lstSpec in _featureSpecifications.Values)
            {

                foreach (FeatureSpec spec in lstSpec)
                {
                    tw.WriteLine("Specification: FeatureType: {0} FeatureValue: {1} MinCount: {2} MaxCount: {3} Pool Availability: {4}",
                                  spec.FeatureName, spec.FeatureValue, spec.MinimumNumber, spec.MaximumNumber, CountItemsWithFeature(spec.FeatureName, spec.FeatureValue));

                }
            }
        }

        private int CountItemsWithFeature(string featureName, string featureValue)
        {
            int count = 0;
            foreach (TestItem ti in _itemPool.Values)
            {
                if (ti.HasFeature(featureName, featureValue))
                    count++;
            }
            return count;
        }

        private int CountFeatureSpecifications()
        {
            int cnt = 0;
            foreach (List<FeatureSpec> spec in _featureSpecifications.Values)
            {
                cnt += spec.Count;
            }
            return cnt;
        }

        internal TestItem GetItem(long itemName)
        {
            if (!_itemPool.ContainsKey(itemName))
                return null;
            return _itemPool[itemName];
        }

        internal FeatureSpec GetFeatureSpec(string featureName, string featureValue)
        {
            if (!_featureSpecifications.ContainsKey(featureName))
                return null;
            List<FeatureSpec> featureVals = _featureSpecifications[featureName];
            FeatureSpec spec = featureVals.Find(delegate(FeatureSpec fs) { return (fs.FeatureValue == featureValue); });
            return spec;
        }

        internal bool HasFeature(string featureName, string featureValue)
        {
            if (_featureSpecifications.ContainsKey(featureName))
            {
                List<FeatureSpec> featureVals = _featureSpecifications[featureName];
                FeatureSpec spec = featureVals.Find(delegate(FeatureSpec fs) { return (fs.FeatureValue == featureValue); });
                return (spec != null);
            }
            return false;
        }

        internal List<CutScoreStrand> SubjectPerformanceLevel
        {
            get { return _subjectPL; }
        }

        #endregion Access



    }//end class
}//end namespace

