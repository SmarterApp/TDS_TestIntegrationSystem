using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.BL.testpackage.osstis;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.osstis.dtos;
using TDSQASystemAPI.TestPackage;

namespace TISUnitTests.services
{
    [TestClass]
    public class TestNameLookupServiceUnitTest
    {
        private readonly Mock<ITestPackageDao<TestNameLookupDTO>> mockTestNameLookupDao = new Mock<ITestPackageDao<TestNameLookupDTO>>();

        private ITestNameLookupService testNameLookupService;

        [TestInitialize]
        public void Setup()
        {
            testNameLookupService = new TestNameLookupService(mockTestNameLookupDao.Object);
        }

        [TestMethod]
        public void TestNameLookup_ShouldSaveANewTestNameLookup()
        {
            var testPackage = new TestPackage
            {
                publisher = "SBAC_PT",
                academicYear = "2017-2018",
                Blueprint = new BlueprintElement[]
                {
                    new BlueprintElement
                    {
                        id = "ICA-UNIT-TEST-GRADE-11-COMBINED",
                        type = "combined"
                    }
                }
            };

            mockTestNameLookupDao.Setup(dao => dao.Find(testPackage.GetTestPackageKey()))
                .Returns(null as List<TestNameLookupDTO>);
            mockTestNameLookupDao.Setup(dao => dao.Insert(It.IsAny<TestNameLookupDTO>()))
                .Verifiable();

            testNameLookupService.CreateTestNameLookup(testPackage);

            mockTestNameLookupDao.Verify(dao => dao.Find(testPackage.GetTestPackageKey()));
            mockTestNameLookupDao.Verify(dao => dao.Insert(It.Is<TestNameLookupDTO>(result =>
                result.InstanceName.Equals("OSS_TISService")
                && result.TestName.Equals(testPackage.GetTestPackageKey()))));
        }

        [TestMethod]
        public void TestNameLookup_ShoultNotTryToCreateANewTestNameLookupIfOneAlreadyExists()
        {
            var testPackage = new TestPackage
            {
                publisher = "SBAC_PT",
                academicYear = "2017-2018",
                Blueprint = new BlueprintElement[]
                {
                    new BlueprintElement
                    {
                        id = "ICA-UNIT-TEST-GRADE-11-COMBINED",
                        type = "combined"
                    }
                }
            };

            mockTestNameLookupDao.Setup(dao => dao.Find(testPackage.GetTestPackageKey()))
                .Returns(new List<TestNameLookupDTO> { new TestNameLookupDTO { InstanceName = "OSS_TISService", TestName = testPackage.GetTestPackageKey() } });
            mockTestNameLookupDao.Setup(dao => dao.Insert(It.IsAny<TestNameLookupDTO>()))
                .Verifiable();

            testNameLookupService.CreateTestNameLookup(testPackage);

            mockTestNameLookupDao.Verify(dao => dao.Find(testPackage.GetTestPackageKey()));
            mockTestNameLookupDao.Verify(dao => dao.Insert(It.IsAny<TestNameLookupDTO>()), Times.Never);
        }
    }
}
