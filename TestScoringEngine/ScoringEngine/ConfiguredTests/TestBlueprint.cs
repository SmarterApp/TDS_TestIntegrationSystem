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

namespace ScoringEngine.ConfiguredTests
{
    public class TestBlueprint
    {
        private Dictionary<string, TestItem> _itemPool = new Dictionary<string, TestItem>();
        private Dictionary<string, TestForm> _forms = new Dictionary<string, TestForm>();
        private Dictionary<string, List<FeatureSpec>> _featureSpecifications = new Dictionary<string, List<FeatureSpec>>();
        private List<CutScoreStrand> _subjectPL = new List<CutScoreStrand>();
        private Dictionary<string, SegmentBlueprint> _segmentBlueprints = new Dictionary<string, SegmentBlueprint>();
		
		public enum SelectionAlgorithmType { FixedForm = 0, Adaptive = 1, Virtual = 2 };

		#region Properties

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
		public string TestName
		{
			get
			{
				return testName;
			}
		}

        private string testID = string.Empty;
        public string TestID
        {
            get
            {
                return testID;
            }
        }

        private string subject = string.Empty;
        public string Subject
        {
            get
            {
                return subject;
            }
        }

        private string gradeCode = string.Empty;
        public string GradeCode
        {
            get
            {
                return gradeCode;
            }
        }

        private string gradeSpan = string.Empty;
        public string GradeSpan
        {
            get
            {
                return gradeSpan;
            }
        }

		private int maxItems = 0;
		public int MaxItems
		{
			get
			{
				return maxItems;
			}
		}

		private int minItems = 0;
		public int MinItems
		{
			get
			{
				return minItems;
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
		public int MinNumFieldTest
		{
			get
			{
				return minNumFieldTest;
			}
		}

		private int maxNumFieldTest = 0;
		public int MaxNumFieldTest
		{
			get
			{
				return maxNumFieldTest;
			}
		}

		private int fieldTestStartPosition = 0;
		public int FieldTestStartPosition
		{
			get
			{
				return fieldTestStartPosition;
			}
		}

		private int fieldTestEndPosition = 0;
		public int FieldTestEndPosition
		{
			get
			{
				return fieldTestEndPosition;
			}
		}

		private DateTime fieldTestStartDate;
		public DateTime FieldTestStartDate
		{
			get
			{
				return fieldTestStartDate;
			}
		}

		private DateTime fieldTestEndDate;
		public DateTime FieldTestEndDate
		{
			get
			{
				return fieldTestEndDate;
			}
		}

		private int opportunityLimit = 0;
		public int OpportunityLimit
		{
			get
			{
				return opportunityLimit;
			}
		}

		private SelectionAlgorithmType selectionAlgorithm = SelectionAlgorithmType.Adaptive;
		public SelectionAlgorithmType SelectionAlgorithm
		{
			get
			{
				return selectionAlgorithm;
			}
		}

		#endregion

        internal TestBlueprint(string testName, string testID, string subject, string gradeCode, string gradeSpan, int maxItems, int minItems, double startDifMax, double startDifMin, 
			int minNumFieldTest, int maxNumFieldTest, int FieldTestStartPosition, int FieldTestEndPosition, DateTime FieldTestStartDate, 
			DateTime FieldTestEndDate, double slope, double intercept, int opportunity, 
			SelectionAlgorithmType selectionAlgorithm)
        {
            this.testName = testName;
            this.testID = testID;
            this.subject = subject;
            this.gradeCode = gradeCode;
            this.gradeSpan = gradeSpan;
            this.maxItems = maxItems;
			this.minItems = minItems;
			this.startDifficultyMax = startDifMax;
			this.startDifficultyMin = startDifMin;
			this.minNumFieldTest = minNumFieldTest;
			this.maxNumFieldTest = maxNumFieldTest;
			this.fieldTestStartPosition = FieldTestStartPosition;
			this.fieldTestEndPosition = FieldTestEndPosition;
			this.fieldTestStartDate = FieldTestStartDate;
			this.fieldTestEndDate = FieldTestEndDate;
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
        internal void AddFeatureSpecification(string featureName, string featureValue, int minNum, int maxNum, double startAbility, double startInfo, double cut, double lambda, double scaleCutScore, string testSegment)
        {
            FeatureSpec spec = new FeatureSpec(featureName, featureValue, minNum, maxNum, startAbility, startInfo, cut, lambda, scaleCutScore, testSegment);

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
        /// <param name="itemBank"></param>
        /// <param name="formName"></param>
        /// <param name="formKey"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="position"></param>
        /// <param name="ti"></param>
        internal void AddFormItem(long itemBank, string formName, long formKey, DateTime startDate, DateTime endDate, int position, TestItem ti)
        {
            if (!HasForm(formName))
            {
                _forms[formName] = new TestForm(itemBank, formName, formKey, startDate, endDate);
            }
            else
            {
                if (_forms[formName].StartDate != startDate)
                    throw new ScoringEngineException("Non-unique start date for form '" + formName + "', got both " + _forms[formName].StartDate.ToString() + " and " + startDate.ToString());
                if (_forms[formName].EndDate != endDate)
                    throw new ScoringEngineException("Non-unique end date for form '" + formName + "', got both " + _forms[formName].EndDate.ToString() + " and " + endDate.ToString());
            }
            _forms[formName].AddItem(formKey, position, ti);
        }

        private bool HasForm(string formName)
        {
            if (_forms.ContainsKey(formName))
                return true;
            // caseinsensitively?
            foreach (string knownForm in _forms.Keys)
                if (formName.Equals(knownForm, StringComparison.InvariantCultureIgnoreCase))
                    throw new ScoringEngineException("Can't look for forms case insensitively! Have form " + knownForm + ", but you are looking for " + formName + "?");
            return false;
        }

        public TestForm GetForm(string formName)
        {
            if (_forms.ContainsKey(formName))
                return _forms[formName];
            else
                return null;
        }

        public void AddSegment(SegmentBlueprint segmentBlueprint)
        {
            if (_segmentBlueprints.ContainsKey(segmentBlueprint.SegmentName))
                _segmentBlueprints[segmentBlueprint.SegmentName] = segmentBlueprint;
            else
                _segmentBlueprints.Add(segmentBlueprint.SegmentName, segmentBlueprint);
        }

        public SegmentBlueprint GetSegment(string segmentName) 
        {
            return _segmentBlueprints.ContainsKey(segmentName) ? _segmentBlueprints[segmentName] : null;
        }

        public TestSegment GetTestSegment(string segmentName, string sFormID)
        {
            SegmentBlueprint segmentBlueprint = GetSegment(segmentName);
            return (segmentBlueprint != null) ? segmentBlueprint.GetTestSegment(sFormID) : null;
        }

        public List<string> SegmentNames()
        {
            return new List<String>(_segmentBlueprints.Keys);
        }

        /// <summary>
        /// In case of a test with multiple segments and formNames being a dash separated list of segment form names, 
        /// return the test form (as a concatenated list of all segment items).
        /// </summary>
        /// <param name="formName"></param>
        /// <returns></returns>
        public TestForm GetMultiSegmentForm(string formNames)
        {
            List<TestForm> forms = new List<TestForm>();
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            while (formNames.Length > 0)
            {
                TestForm foundForm = null;
                foreach (string formName in _forms.Keys)
                {
                    if (formNames == formName)
                    {
                        formNames = "";
                        foundForm = _forms[formName];
                        break;
                    }
                    else if (formNames.StartsWith(formName + "-"))
                    {
                        formNames = formNames.Substring(formName.Length+1);
                        foundForm = _forms[formName];
                        break;
                    }
                }
                if (foundForm == null)
                    return null;
                else
                {
                    forms.Add(foundForm);
                    if (foundForm.StartDate > startDate) startDate = foundForm.StartDate;
                    if (foundForm.EndDate < endDate) endDate = foundForm.EndDate;
                }
            }
            if (forms.Count == 0)
                return null;
            if (forms.Count == 1)
                return forms[0];
            TestForm catForm = new TestForm(-1, formNames, -1, startDate, endDate);
            int position = 0;
            foreach (TestForm form in forms)
            {
                for (int i = 1; i <= form.Items.Count; i++)
                {
                    position += 1;
                    catForm.AddItem(-1, position, form.Items[i]);
                }
            }
            return catForm;
        }

        public List<string> FormNames()
        {
            return new List<string>(_forms.Keys);
        }

        internal void AddTestPerformanceLevels(string domain, double min, double max, string testName, string performanceLevel)
        {
            CutScoreStrand cs = new CutScoreStrand(domain, min, max, testName, performanceLevel);
            _subjectPL.Add(cs);
        }

        #endregion loading



        #region Access

        public List<FeatureSpec> GetFeatureSpecifications(string strandId)
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
                count += ti.CountItemsWithFeature(featureName, featureValue);
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

        public TestItem GetItem(string itemName)
        {
            if (!_itemPool.ContainsKey(itemName))
                return null;
            return _itemPool[itemName];
        }

        public FeatureSpec GetFeatureSpec(string featureName, string featureValue)
        {
            if (!_featureSpecifications.ContainsKey(featureName))
                return null;
            List<FeatureSpec> featureVals = _featureSpecifications[featureName];
            FeatureSpec spec = featureVals.Find(delegate(FeatureSpec fs) { return (fs.FeatureValue == featureValue); });
            return spec;
        }

        public bool HasFeature(string featureName, string featureValue)
        {
            if (_featureSpecifications.ContainsKey(featureName))
            {
                List<FeatureSpec> featureVals = _featureSpecifications[featureName];
                FeatureSpec spec = featureVals.Find(delegate(FeatureSpec fs) { return (fs.FeatureValue == featureValue); });
                return (spec != null);
            }
            return false;
        }

        public List<CutScoreStrand> SubjectPerformanceLevel
        {
            get { return _subjectPL; }
        }

        #endregion Access



    }//end class
}//end namespace

