using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TDSQASystemAPI.BL.testpackage.osstis;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.DAL.osstis.dtos;
using TDSQASystemAPI.TestPackage;
using TDSQASystemAPI.TestPackage.utils;

namespace TISUnitTests.services
{
    [TestClass]
    public class CombinationTestMapServiceUnitTest
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml";

        private Mock<ITestPackageDao<CombinationTestMapDTO>> mockCombinationTestMapDao = new Mock<ITestPackageDao<CombinationTestMapDTO>>();
        private Mock<ITestPackageDao<CombinationTestFormMapDTO>> mockCombinationTestFormMapDao = new Mock<ITestPackageDao<CombinationTestFormMapDTO>>();

        private ICombinationTestMapService combinationTestMapService;

        [TestInitialize]
        public void Setup()
        {
            combinationTestMapService = new CombinationTestMapService(mockCombinationTestMapDao.Object,
                mockCombinationTestFormMapDao.Object);
        }

        [TestMethod]
        public void CombinationTestMap_ShouldCreateACollectionOfCombinationTestMapRecords()
        {
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));

            mockCombinationTestMapDao.Setup(dao => dao.Insert(It.IsAny<IList<CombinationTestMapDTO>>()))
                .Verifiable();

            combinationTestMapService.CreateCombinationTestMap(testPackage);

            mockCombinationTestMapDao.Verify(dao => dao.Insert(It.Is<IList<CombinationTestMapDTO>>(result =>
                EvaluateShouldCreateACollectionOfCombinationTestMapRecords(result))));
        }

        [TestMethod]
        public void CombinationTestMap_ShouldNotCreateCombinationTestMapRecordsForATestPackageThatIsNotCombined()
        {
            var testPackage = new TestPackage
            {
                publisher = "SBAC_PT",
                subject = "MATH",
                type = "summative",
                Blueprint = new BlueprintElement[]
                {
                    new BlueprintElement
                    {
                        id = "SBAC-IRP-COMBINED-MATH-11",
                        type = "fixedform"
                    }
                }
            };

            combinationTestMapService.CreateCombinationTestMap(testPackage);

            mockCombinationTestMapDao.Verify(dao => dao.Insert(It.IsAny<IList<CombinationTestMapDTO>>()), Times.Never);
        }

        [TestMethod]
        public void CombinationTestFormMap_ShouldCreateACollectionOfCombinationTestFormMapRecords()
        {
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
            var mockTdsTesForm = new TestFormDTO
            {
                TestFormKey = "187-2112",
                SegmentKey = "(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018",
                ITSBankKey = 2112L,
                ITSKey = 2112L,
                Language = "ENU",
                FormId = "IRP::MathG11::Perf::SP15",
                Cohort = "Default"
            };

            mockCombinationTestFormMapDao.Setup(dao => dao.Insert(It.IsAny<IList<CombinationTestFormMapDTO>>()))
                .Verifiable();

            combinationTestMapService.CreateCombinationTestFormMap(testPackage, new List<TestFormDTO> { mockTdsTesForm });

            mockCombinationTestFormMapDao.Verify(dao => dao.Insert(It.Is<IList<CombinationTestFormMapDTO>>(result => 
                EvaluateShouldCreateACollectionOfCombinationTestFormMapRecords(result))));
        }

        [TestMethod]
        public void CombinationTestFormMap_ShouldNotCreateCombinationTestFormMapRecordsForATestPackageThatIsNotCombined()
        {
            var testPackage = new TestPackage
            {
                publisher = "SBAC_PT",
                subject = "MATH",
                type = "summative",
                Blueprint = new BlueprintElement[]
                {
                    new BlueprintElement
                    {
                        id = "SBAC-IRP-COMBINED-MATH-11",
                        type = "fixedform"
                    }
                }
            };
            var mockTdsTesForm = new TestFormDTO
            {
                TestFormKey = "187-2112",
                SegmentKey = "(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018",
                ITSBankKey = 2112L,
                ITSKey = 2112L,
                Language = "ENU",
                FormId = "IRP::MathG11::Perf::SP15",
                Cohort = "Default"
            };

            combinationTestMapService.CreateCombinationTestFormMap(testPackage, new List<TestFormDTO> { mockTdsTesForm });

            mockCombinationTestFormMapDao.Verify(dao => dao.Insert(It.IsAny<IList<CombinationTestFormMapDTO>>()), Times.Never);
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

        private bool EvaluateShouldCreateACollectionOfCombinationTestFormMapRecords(IList<CombinationTestFormMapDTO> combinationTestFormMapDtos)
        {
            Assert.AreEqual(1, combinationTestFormMapDtos.Count);

            var combinationTestForm = combinationTestFormMapDtos.First();
            Assert.AreEqual("187-2112", combinationTestForm.CombinationFormKey);
            Assert.AreEqual("187-2112", combinationTestForm.ComponentFormKey);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018", combinationTestForm.ComponentSegmentName);

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }
    }
}
