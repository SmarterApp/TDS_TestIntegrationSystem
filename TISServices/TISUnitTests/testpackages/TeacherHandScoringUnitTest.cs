﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml;
using TDSQASystemAPI.TestPackage;
using TDSQASystemAPI.TestPackage.utils;

namespace TISUnitTests.testpackages
{
    [TestClass]
    public class TeacherHandScoringUnitTest
    {
        private const string THSS_TEST_PACKAGE_XML = @"..\..\resources\test-packages\new-xsd\thss-test-specification-sample-1.xml";

        [TestMethod]
        public void ShouldDeserializeAThssPackageAndHaveParentObjectsWiredUp()
        {
            var thssTestPackage = TestPackageAssembler.FromXml(new XmlTextReader(THSS_TEST_PACKAGE_XML));

            Assert.AreEqual("SBAC_PT", thssTestPackage.publisher);
            Assert.AreEqual("ELA", thssTestPackage.subject);

            var forms = thssTestPackage.Assessment[0].Segments[0].Item as AssessmentSegmentSegmentForms;
            var handScoring = forms.SegmentForm[0]
                .ItemGroup[0]
                .Item[0]
                .TeacherHandScoring;

            Assert.IsNotNull(handScoring.TestPackage);
            Assert.IsNotNull(handScoring.ItemGroupItem);
        }

        [TestMethod]
        public void ShouldDeserializeAThssPackageAndContainRubricLists()
        {
            var thssTestPackage = TestPackageAssembler.FromXml(new XmlTextReader(THSS_TEST_PACKAGE_XML));

            var forms = thssTestPackage.Assessment[0].Segments[0].Item as AssessmentSegmentSegmentForms;
            var rubricList = forms.SegmentForm[0]
                .ItemGroup[0]
                .Item[0]
                .TeacherHandScoring
                .rubriclist.OfType<ItemGroupItemTeacherHandScoringRubriclistRubric>();
            Assert.AreEqual(5, rubricList.Count());
        }
    }
}