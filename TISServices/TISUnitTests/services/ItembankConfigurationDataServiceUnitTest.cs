using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Xml;
using TDSQASystemAPI.BL.testpackage.administration;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.TestPackage;
using TDSQASystemAPI.TestPackage.utils;

namespace TISUnitTests.services
{
    [TestClass]
    public class ItembankConfigurationDataServiceUnitTest
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml";

        private readonly Mock<ITestPackageDao<ClientDTO>> mockClientDao = new Mock<ITestPackageDao<ClientDTO>>();
        private readonly Mock<ITestPackageDao<SubjectDTO>> mockSubjectDao = new Mock<ITestPackageDao<SubjectDTO>>();
        private readonly Mock<ITestPackageDao<StrandDTO>> mockStrandDao = new Mock<ITestPackageDao<StrandDTO>>();

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
                new ItembankConfigurationDataService(mockSubjectDao.Object, 
                    mockClientDao.Object, 
                    mockStrandDao.Object);
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
            mockSubjectDao.Verify(dao => dao.Find(subjectKey)); // TODO: why isn't this being hit?
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

        [TestMethod]
        public void ShouldCreateACollectionOfStrandDTOs()
        {
            var loadedTestPackage = TestPackageAssembler.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
            var subjectKey = string.Format("{0}-{1}", loadedTestPackage.subject, loadedTestPackage.publisher);

            mockClientDao.Setup(dao => dao.Find(It.IsAny<string>()))
                .Returns(new List<ClientDTO> { new ClientDTO { Name = loadedTestPackage.publisher, ClientKey = 99 } });
            mockSubjectDao.Setup(dao => dao.Find(It.IsAny<string>()))
                .Returns(new List<SubjectDTO> { new SubjectDTO { Name = loadedTestPackage.subject, ClientKey = 99 } });
            mockStrandDao.Setup(dao => dao.Insert(It.IsAny<IList<StrandDTO>>()))
                .Verifiable();

            IDictionary<string, StrandDTO> result = itembankConfigurationDataService.CreateStrands(loadedTestPackage);

            mockClientDao.Verify(dao => dao.Find(It.IsAny<string>()));
            mockSubjectDao.Verify(dao => dao.Find(It.IsAny<string>()));
            mockStrandDao.Verify(dao => dao.Insert(It.Is<IList<StrandDTO>>(list => list.Count == 77))); // 77 nested BP elements in the example Test Package

            // Verify a leaf-level StrandDTO
            var leafLevelTargetStrand = result["1|F-IF|K|m|F-IF.1"];
            Assert.AreEqual("SBAC_PT-1|F-IF|K|m|F-IF.1", leafLevelTargetStrand.Key);
            Assert.IsTrue(leafLevelTargetStrand.IsLeafTarget);
            Assert.AreEqual(loadedTestPackage.publisher, leafLevelTargetStrand.ClientKey);
            Assert.AreEqual("contentlevel", leafLevelTargetStrand.Type);
            Assert.AreEqual("SBAC_PT-1|F-IF|K|m", leafLevelTargetStrand.ParentId);
            Assert.AreEqual(5, leafLevelTargetStrand.TreeLevel);
            Assert.AreEqual(subjectKey, leafLevelTargetStrand.SubjectKey);

            // Verify a "claim" StrandDTO
            var claimStrand = result["1"];
            Assert.AreEqual("SBAC_PT-1", claimStrand.Key);
            Assert.IsFalse(claimStrand.IsLeafTarget);
            Assert.AreEqual(loadedTestPackage.publisher, claimStrand.ClientKey);
            Assert.AreEqual("strand", claimStrand.Type);
            Assert.AreEqual(string.Empty, claimStrand.ParentId);
            Assert.AreEqual(1, claimStrand.TreeLevel);
            Assert.AreEqual(subjectKey, claimStrand.SubjectKey);
        }
    }
}
