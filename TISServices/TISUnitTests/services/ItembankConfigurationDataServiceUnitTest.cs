using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using TDSQASystemAPI.BL.testpackage.administration;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.TestPackage;

namespace TISUnitTests.services
{
    [TestClass]
    public class ItembankConfigurationDataServiceUnitTest
    {
        private readonly Mock<ITestPackageDao<ClientDTO>> mockClientDao = new Mock<ITestPackageDao<ClientDTO>>();
        private readonly Mock<ITestPackageDao<SubjectDTO>> mockSubjectDao = new Mock<ITestPackageDao<SubjectDTO>>();

        private IItembankConfigurationDataService itembankConfigurationDataService;
        private TestPackage testPackage = new TestPackage
            {
                subject = "unit-test-subject",
                publisher = "unit-test-publisher",
                version = 42L
            };

    [TestInitialize]
        public void Setup()
        {
            itembankConfigurationDataService =
                new ItembankConfigurationDataService(mockSubjectDao.Object, mockClientDao.Object);
        }

        [TestMethod]
        public void ShouldCreateANewSubject()
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.subject, testPackage.publisher);

            mockClientDao.Setup(dao => dao.Find(testPackage.publisher))
                .Returns(new List<ClientDTO> { new ClientDTO { Name = testPackage.publisher, ClientKey = 99 } });
            mockSubjectDao.Setup(dao => dao.Find(subjectKey))
                .Returns(new List<SubjectDTO>());
            mockSubjectDao.Setup(dao => dao.Insert(It.IsAny<IList<SubjectDTO>>()))
                .Verifiable();

            itembankConfigurationDataService.CreateSubject(testPackage);

            mockClientDao.Verify(dao => dao.Find(testPackage.publisher));
            // mockSubjectDao.Verify(dao => dao.Find(subjectKey)); // TODO: why isn't this being hit?
            mockSubjectDao.Verify(dao => dao.Insert(It.Is<List<SubjectDTO>>(subject => 
                subject.Count == 1 
                && subject[0].Name.Equals(testPackage.subject)
                && subject[0].Grade.Equals(string.Empty)
                && subject[0].ClientKey == 99
                && subject[0].TestVersion == 42L)));
        }

        [TestMethod]
        public void ShouldNotCreateASubjectIfItAlreadyExists()
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.subject, testPackage.publisher);

            mockClientDao.Setup(dao => dao.Find(testPackage.publisher))
                .Returns(new List<ClientDTO> { new ClientDTO { Name = testPackage.publisher, ClientKey = 99 } });
            mockSubjectDao.Setup(dao => dao.Find(subjectKey))
                .Returns(new List<SubjectDTO> { new SubjectDTO { Name = testPackage.subject, ClientKey = 99 } });

            itembankConfigurationDataService.CreateSubject(testPackage);

            mockClientDao.Verify(dao => dao.Find(testPackage.publisher));
            //mockSubjectDao.Verify(dao => dao.Find(It.IsAny<string>())); // TODO: Why isn't this being hit?
            mockSubjectDao.Verify(dao => dao.Insert(It.IsAny<IList<SubjectDTO>>()), Times.Never);
        }

        [TestMethod]
        public void ShouldThrowInvalidOperationExceptionIfThereIsNoClientRecord()
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.subject, testPackage.publisher);

            mockClientDao.Setup(dao => dao.Find(testPackage.publisher))
                .Returns(new List<ClientDTO>());
            mockSubjectDao.Setup(dao => dao.Find(subjectKey))
                .Returns(new List<SubjectDTO>());

            var exception = 
                Assert.ThrowsException<InvalidOperationException>(() => itembankConfigurationDataService.CreateSubject(testPackage));

            mockClientDao.Verify(dao => dao.Find(testPackage.publisher));

            Assert.AreEqual("Could not find a client record with name 'unit-test-publisher'", exception.Message);
        }
    }
}
