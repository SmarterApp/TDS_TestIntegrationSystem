using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.BL.testpackage;
using TDSQASystemAPI.TestPackage;
using TISUnitTests.utils;

namespace TISUnitTests.services
{
    [TestClass]
    public class TestPackageLoaderServiceIntegrationTest : TestPackageDaoIntegrationTestBase<TestPackage>
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml";

        private readonly ITestPackageLoaderService testPackageLoaderService = new TestPackageLoaderService();

        [TestMethod]
        public void ShouldLoadTestPackage()
        {
            using (var testPackageStream = File.OpenRead(TEST_PACKAGE_XML_FILE))
            {
                var result = testPackageLoaderService.LoadTestPackage(testPackageStream);
            }
        }
    }
}
