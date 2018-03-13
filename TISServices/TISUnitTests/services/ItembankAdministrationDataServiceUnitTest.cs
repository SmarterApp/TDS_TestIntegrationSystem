using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.BL.testpackage.administration;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.TestPackage;

namespace TISUnitTests.services
{
    [TestClass]
    public class ItembankAdministrationDataServiceUnitTest
    {
        private Mock<ITestPackageDao<TestAdminDTO>> mockTestAdminDao = new Mock<ITestPackageDao<TestAdminDTO>>();
        private Mock<IItembankConfigurationDataQueryService> mockItembankConfigurationDataQueryService = new Mock<IItembankConfigurationDataQueryService>();

        private IItembankAdministrationDataService itembankAdministrationDataService;

        [TestInitialize]
        public void Setup()
        {
            itembankAdministrationDataService =
                new ItembankAdministrationDataService(mockItembankConfigurationDataQueryService.Object,
                    mockTestAdminDao.Object);
        }

        [TestMethod]
        public void TestAdmin_ShouldInsertANewTestAdminRecord()
        {
            var testPackage = new TestPackage
            {
                academicYear = "2017-2018",
                version = 99M,
                publisher = "UNIT_TEST"
            };

            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindTestAdmin(It.IsAny<string>()))
                .Returns((TestAdminDTO)null);
            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindClientByName(It.IsAny<string>()))
                .Returns(new ClientDTO { ClientKey = 42L, Name = testPackage.publisher });
            mockTestAdminDao.Setup(dao => dao.Insert(It.IsAny<List<TestAdminDTO>>()))
                .Verifiable();
            mockTestAdminDao.Setup(dao => dao.Update(It.IsAny<TestAdminDTO>()))
                .Verifiable();

            itembankAdministrationDataService.SaveTestAdministration(testPackage);

            mockItembankConfigurationDataQueryService.Verify(svc => svc.FindTestAdmin(testPackage.publisher));
            mockItembankConfigurationDataQueryService.Verify(svc => svc.FindClientByName(testPackage.publisher));
            mockTestAdminDao.Verify(dao => dao.Insert(It.Is<List<TestAdminDTO>>(recordInserted =>
                recordInserted.Count == 1
                    && recordInserted[0].AdminKey.Equals(testPackage.publisher)
                    && recordInserted[0].ClientKey == 42L
                    && recordInserted[0].SchoolYear.Equals(testPackage.academicYear)
                    && recordInserted[0].TestVersion == (long)testPackage.version)));
            mockTestAdminDao.Verify(dao => dao.Update(It.IsAny<TestAdminDTO>()), Times.Never);
        }

        [TestMethod]
        public void TestAdmin_ShouldUpdateAnExistingTestAdminRecord()
        {
            var testPackage = new TestPackage
            {
                academicYear = "2017-2018",
                version = 1099M,
                publisher = "UNIT_TEST"
            };
            var existingTestAdminDto = new TestAdminDTO
            {
                AdminKey = testPackage.publisher,
                ClientKey = 42L,
                SchoolYear = testPackage.academicYear,
                TestVersion = 99L
            };

            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindTestAdmin(It.IsAny<string>()))
                .Returns(existingTestAdminDto);
            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindClientByName(It.IsAny<string>()))
                .Returns(new ClientDTO { ClientKey = 42L, Name = testPackage.publisher });
            mockTestAdminDao.Setup(dao => dao.Insert(It.IsAny<List<TestAdminDTO>>()))
                .Verifiable();
            mockTestAdminDao.Setup(dao => dao.Update(It.IsAny<TestAdminDTO>()))
                .Verifiable();

            itembankAdministrationDataService.SaveTestAdministration(testPackage);

            mockItembankConfigurationDataQueryService.Verify(svc => svc.FindTestAdmin(testPackage.publisher));
            mockItembankConfigurationDataQueryService.Verify(svc => svc.FindClientByName(testPackage.publisher));
            mockTestAdminDao.Verify(dao => dao.Update(It.Is<TestAdminDTO>(recordInserted =>
                recordInserted.AdminKey.Equals(existingTestAdminDto.AdminKey)
                    && recordInserted.ClientKey == existingTestAdminDto.ClientKey
                    && recordInserted.SchoolYear.Equals(existingTestAdminDto.SchoolYear)
                    && recordInserted.TestVersion == (long)testPackage.version)));
            mockTestAdminDao.Verify(dao => dao.Insert(It.IsAny<List<TestAdminDTO>>()), Times.Never);
        }

        [TestMethod]
        public void TestAdmin_ShouldThrowExceptionWhenClientRecordCannotBeFound()
        {
            var testPackage = new TestPackage
            {
                academicYear = "2017-2018",
                version = 1099M,
                publisher = "UNIT_TEST"
            };

            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindTestAdmin(It.IsAny<string>()))
                .Verifiable();
            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindClientByName(It.IsAny<string>()))
                .Returns(null as ClientDTO);
            mockTestAdminDao.Setup(dao => dao.Insert(It.IsAny<List<TestAdminDTO>>()))
                .Verifiable();
            mockTestAdminDao.Setup(dao => dao.Update(It.IsAny<TestAdminDTO>()))
                .Verifiable();

            mockItembankConfigurationDataQueryService.Verify(svc => svc.FindTestAdmin(It.IsAny<string>()), Times.Never);
            mockTestAdminDao.Verify(dao => dao.Insert(It.IsAny<List<TestAdminDTO>>()), Times.Never);
            mockTestAdminDao.Verify(dao => dao.Update(It.IsAny<TestAdminDTO>()), Times.Never);

            var exception =
                Assert.ThrowsException<InvalidOperationException>(() => itembankAdministrationDataService.SaveTestAdministration(testPackage));
            Assert.AreEqual(string.Format("Could not find client for name '{0}'", testPackage.publisher), exception.Message);
        }
    }
}
