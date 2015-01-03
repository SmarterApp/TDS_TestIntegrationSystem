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
using System.Linq;
using System.Text;

namespace ScoringEngine.ConfiguredTests
{
    public class SegmentBlueprint
    {
        private Dictionary<string, TestForm> _testForms = new Dictionary<string, TestForm>();

        public string SegmentName { private set; get; }
        public string SegmentID { private set; get; }
        public int MaxItems { private set; get; }
        public int MinItems { private set; get; }
        public int MinNumFieldTest { private set; get; }
        public int MaxNumFieldTest { private set; get; }
        public int FieldTestStartPosition { private set; get; }
        public int FieldTestEndPosition { private set; get; }
        public int TestPosition { private set; get; }
        public TestBlueprint.SelectionAlgorithmType SelectionAlgorithm { private set; get; }
        public string SelectionAlgorithmString { private set; get;}

        internal SegmentBlueprint(string segmentName, string segmentID, int maxItems, int minItems, int maxNumFieldTest , int minNumFieldTest, 
            int fieldTestStartPosition, int fieldTestEndPosition, int testPosition, string selectionAlgorithm)
        {
            this.SegmentName = segmentName;
            this.SegmentID = segmentID;
            this.MaxItems = maxItems;
            this.MinItems = minItems;
            this.MaxNumFieldTest = maxNumFieldTest;
            this.MinNumFieldTest = minNumFieldTest;
            this.FieldTestStartPosition = fieldTestStartPosition;
            this.FieldTestEndPosition = fieldTestEndPosition;
            this.TestPosition = testPosition;
            this.SelectionAlgorithmString = selectionAlgorithm;
            this.SelectionAlgorithm = Util.Value((object)selectionAlgorithm, TestBlueprint.SelectionAlgorithmType.Adaptive);
        }

        public void AddTestForm(TestForm form)
        {
            if (_testForms.ContainsKey(form.ITSFormKey))
                _testForms[form.ITSFormKey] = form;
            else
                _testForms.Add(form.ITSFormKey, form);
        }

        public TestForm GetTestForm(string itsFormKey)
        {
            return _testForms.ContainsKey(itsFormKey) ? _testForms[itsFormKey] : null;
        }

        public TestSegment GetTestSegment(string itsFormKey)
        {
            TestForm testForm = string.IsNullOrEmpty(itsFormKey) ? null: GetTestForm(itsFormKey);
            return testForm == null ?
                new TestSegment(SegmentName, TestPosition, null, null, SelectionAlgorithmString, SelectionAlgorithm) :
                new TestSegment(SegmentName, TestPosition, testForm.FormID, testForm.ITSFormKey, SelectionAlgorithmString, SelectionAlgorithm);
        }
    }
}
