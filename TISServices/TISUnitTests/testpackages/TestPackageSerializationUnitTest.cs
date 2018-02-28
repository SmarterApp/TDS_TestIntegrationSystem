using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TDSQASystemAPI.TestPackage;

namespace TISUnitTests.testpackages
{
    [TestClass]
    public class TestPackageSerializationUnitTest
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\test-specification-sample-1.xml";

        [TestMethod]
        public void ShouldDeserializeXmlFileIntoTestPackage()
        {
            var testPackage = new TestPackage();
            var serializer = new XmlSerializer(typeof(TestPackage));

            using(var fileStream = new FileStream(TEST_PACKAGE_XML_FILE, FileMode.Open))
            {
                testPackage = serializer.Deserialize(fileStream) as TestPackage;
            }

            Assert.AreEqual("SBAC_PT", testPackage.publisher);
            Assert.AreEqual("MATH", testPackage.subject);

            // The Segment's Item can be either a AssessmentSegmentPool or AssessmentSegmentSegmentForms 
            // In this case, the Item is an AssessmentSegmentPool.
            var pool = testPackage.Assessment[0].Segments[0].Item as AssessmentSegmentPool;
            Assert.AreEqual(3, pool.ItemGroup[0].Item[0].BlueprintReferences.Length);
        }

        [TestMethod]
        public void ShouldDeserializeXmlFileIntoTestPackageWithAssessmentKey()
        {
            var testPackage = new TestPackage();
            var serializer = new XmlSerializer(typeof(TestPackage));

            using (var fileStream = new FileStream(TEST_PACKAGE_XML_FILE, FileMode.Open))
            {
                testPackage = serializer.Deserialize(fileStream) as TestPackage;
            }

            Assert.AreEqual("SBAC_PT", testPackage.publisher);
            Assert.AreEqual("MATH", testPackage.subject);

            // The Segment's Item can be either a AssessmentSegmentPool or AssessmentSegmentSegmentForms 
            // In this case, the Item is an AssessmentSegmentPool.
            var pool = testPackage.Assessment[0].Segments[0].Item as AssessmentSegmentPool;
            Assert.AreEqual(3, pool.ItemGroup[0].Item[0].BlueprintReferences.Length);

            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", testPackage.Assessment[0].id);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", testPackage.Assessment[0].Segments[0].id);
        }
    }
}
