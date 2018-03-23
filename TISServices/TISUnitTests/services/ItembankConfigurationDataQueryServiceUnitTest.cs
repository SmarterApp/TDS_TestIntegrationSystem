using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using TDSQASystemAPI.BL.testpackage.administration;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TISUnitTests.services
{
    [TestClass]
    public class ItembankConfigurationDataQueryServiceUnitTest
    {
        private readonly Mock<ITestPackageDao<ClientDTO>> mockClientDao = new Mock<ITestPackageDao<ClientDTO>>();
        private readonly Mock<ITestPackageDao<SubjectDTO>> mockSubjectDao = new Mock<ITestPackageDao<SubjectDTO>>();
        private readonly Mock<ITestPackageDao<TestAdminDTO>> mockTestAdminDao = new Mock<ITestPackageDao<TestAdminDTO>>();


        private IItembankConfigurationDataQueryService itembankConfigurationDataQueryService;

        [TestInitialize]
        public void Setup()
        {
            itembankConfigurationDataQueryService =
                new ItembankConfigurationDataQueryService(mockSubjectDao.Object, 
                    mockClientDao.Object, 
                    mockTestAdminDao.Object);
        }

        [TestMethod]
        public void Client_ShouldFindAClientForTheSpecifiedPublisher()
        {
            mockClientDao.Setup(dao => dao.Find(It.IsAny<string>()))
                .Returns(new List<ClientDTO> { new ClientDTO { ClientKey = 42L, Name = "UNIT-TEST" } });

            var result = itembankConfigurationDataQueryService.FindClientByName("UNIT-TEST");

            Assert.AreEqual("UNIT-TEST", result.Name);
            Assert.AreEqual(42L, result.ClientKey);
        }

        [TestMethod]
        public void Client_ShouldReturnNullWhenNoClientIsFound()
        {
            mockClientDao.Setup(dao => dao.Find(It.IsAny<string>()))
                .Returns((List<ClientDTO>)null);

            var result = itembankConfigurationDataQueryService.FindClientByName("UNIT-TEST");

            Assert.IsNull(result);
        }
    }
}
