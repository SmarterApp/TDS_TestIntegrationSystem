using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TDSQASystemAPI.BL.testpackage.osstis;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.osstis.dtos;
using TDSQASystemAPI.TestPackage.utils;

namespace TISUnitTests.services
{
    [TestClass]
    public class CombinationTestMapServiceUnitTest
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml";

        private Mock<ITestPackageDao<CombinationTestMapDTO>> mockCombinationTestMapDao = new Mock<ITestPackageDao<CombinationTestMapDTO>>();

        private ICombinationTestMapService combinationTestMapService;

        [TestInitialize]
        public void Setup()
        {
            combinationTestMapService = new CombinationTestMapService(mockCombinationTestMapDao.Object);
        }

        [TestMethod]
        public void CombinationTestMap_ShouldCreateACollectionOfCombinatioNTestMapRecords()
        {
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));

            mockCombinationTestMapDao.Setup(dao => dao.Insert(It.IsAny<IList<CombinationTestMapDTO>>()))
                .Verifiable();

            combinationTestMapService.CreateCombinationTestMap(testPackage);

            mockCombinationTestMapDao.Verify(dao => dao.Insert(It.Is<IList<CombinationTestMapDTO>>(result =>
                EvaluateShouldCreateACollectionOfCombinationTestMapRecords(result))));
        }

        private bool EvaluateShouldCreateACollectionOfCombinationTestMapRecords(IList<CombinationTestMapDTO> combinationTestMaps)
        {
            Assert.AreEqual(2, combinationTestMaps.Count);

            var firstCombinationTestMap = combinationTestMaps.First();
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", firstCombinationTestMap.ComponentTestName);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", firstCombinationTestMap.ComponentSegmentName);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-COMBINED-MATH-11-2017-2018", firstCombinationTestMap.CombinationTestName);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", firstCombinationTestMap.CombinationSegmentName);

            var secondCombinationTestMap = combinationTestMaps.Last();
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018", secondCombinationTestMap.ComponentTestName);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018", secondCombinationTestMap.ComponentSegmentName);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-COMBINED-MATH-11-2017-2018", secondCombinationTestMap.CombinationTestName);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018", secondCombinationTestMap.CombinationSegmentName);

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }
    }
}
