using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            Assert.IsNotNull(testPackage);
        }
    }
}
