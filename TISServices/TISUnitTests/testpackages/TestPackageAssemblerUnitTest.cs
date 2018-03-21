using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TDSQASystemAPI.TestPackage;
using TDSQASystemAPI.TestPackage.utils;

namespace TISUnitTests.testpackages
{
    [TestClass]
    public class TestPackageAssemblerUnitTest
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\test-specification-sample-1.xml";
        private const string SCORING_RULES_XML_FILE = @"..\..\resources\test-packages\new-xsd\scoring-rules.xml";
        private const string SEGMENT_BLUEPRINT_XML_FILE = @"..\..\resources\test-packages\new-xsd\segment-blueprint-element.xml";


        TestPackage testPackage = null;

        [TestInitialize]
        public void Setup()
        {
            testPackage = TestPackageAssembler.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
        }

        [TestMethod]
        public void ShouldBuildATestPackageFromXml()
        {
            Assert.AreEqual("SBAC_PT", testPackage.publisher);
            Assert.AreEqual("MATH", testPackage.subject);

            // The Segment's Item can be either a AssessmentSegmentPool or AssessmentSegmentSegmentForms 
            // In this case, the Item is an AssessmentSegmentPool.
            var pool = testPackage.Test[0].Segments[0].Item as TestSegmentPool;
            Assert.AreEqual(3, pool.ItemGroup[0].Item[0].BlueprintReferences.Length);
            Assert.IsNotNull(pool.ItemGroup[0].Item[0].ItemGroup);
            Assert.IsNotNull(pool.ItemGroup[0].Item[0].TestPackage);

            // Verify the Assessments are configured properly:
            // 1.  Make sure they've been assigned to the correct TestPackage parent
            // 2.  Make sure each segment has been assigned the correct Assessment parent
            foreach (var assessment in testPackage.Test)
            {
                Assert.IsNotNull(assessment.TestPackage);

                foreach (var segment in assessment.Segments)
                {
                    Assert.IsNotNull(segment.Assessment);
                }
            }

            // Verify forms and/or pools are configured properly
            var allSegments = from a in testPackage.Test
                              from s in a.Segments
                              select s;

            foreach(var segment in allSegments)
            {
                // Verify the forms are built properly for fixed-form assessments.  The pool (for adaptive assessments) 
                // has already been verified above
                if (segment.Item is TestSegmentSegmentForms)
                {
                    foreach (var form in (segment.Item as TestSegmentSegmentForms).SegmentForm)
                    {
                        Assert.IsNotNull(form.AssessmentSegment);
                        foreach (var itemGroup in form.ItemGroup)
                        {
                            Assert.IsNotNull(itemGroup.AssessmentSegment);
                            foreach (var item in itemGroup.Item)
                            {
                                Assert.IsNotNull(item.ItemGroup);
                                Assert.IsNotNull(item.SegmentForm);
                                Assert.IsNotNull(item.TestPackage);
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ShouldBuildATestPackageFromXmlWithAssessmentKeys()
        {
            var catAssessment = testPackage.Test[0];
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", catAssessment.Key);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", catAssessment.Segments[0].Key);

            var perfAssessment = testPackage.Test[1];
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018", perfAssessment.Key);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018", perfAssessment.Segments[0].Key);
        }

        [TestMethod]
        public void ShouldDeserializeAnItemScoreParameterWithAnItemScoreDimension()
        {
            testPackage = TestPackageAssembler.FromXml(new XmlTextReader(SCORING_RULES_XML_FILE));

            var assessmentSegmentForm = testPackage.Test[0].Segments[0].Item as TestSegmentSegmentForms;
            var dimension = assessmentSegmentForm.SegmentForm[0]
                .ItemGroup[0]
                .Item[0]
                .ItemScoreDimension.dimension;

            Assert.AreEqual("test-dimension", dimension);
        }

        [TestMethod]
        public void ShouldGetTheCorrectPresentationLabelWhenTheLabelIsNull()
        {
            var assessmentSegmentPool = testPackage.Test[0].Segments[0].Item as TestSegmentPool;
            var presentation = assessmentSegmentPool.ItemGroup[0]
                .Item[0]
                .Presentations[0];
            Assert.AreEqual("English", presentation.GetLabel());
        }

        [TestMethod]
        public void ShouldBuildCorrectItemGroupKeys()
        {
            var firstAssessmentPool = testPackage.Test[0].Segments[0].Item as TestSegmentPool;
            var itemGroup = firstAssessmentPool.ItemGroup[0];

            Assert.AreEqual("I-187-1899", itemGroup.Key);

            var secondAssessmentForm = testPackage.Test[1].Segments[0].Item as TestSegmentSegmentForms;
            var formItemGroup = secondAssessmentForm.SegmentForm[0].ItemGroup[0];

            Assert.AreEqual("G-187-3688-0", formItemGroup.Key);
        }

        [TestMethod]
        public void ShouldDeserializeSegmentBlueprintElementFromXmlWithEmptyListDefaults()
        {
            testPackage = TestPackageAssembler.FromXml(new XmlTextReader(SEGMENT_BLUEPRINT_XML_FILE));

            var segmentBlueprints = testPackage.Test[0].Segments[0].SegmentBlueprint;
            Assert.AreEqual(1, segmentBlueprints[0].ItemSelection.Length);
            Assert.AreEqual(1, segmentBlueprints[1].ItemSelection.Length);
            Assert.IsNull(segmentBlueprints[2].ItemSelection);
        }
    }
}
