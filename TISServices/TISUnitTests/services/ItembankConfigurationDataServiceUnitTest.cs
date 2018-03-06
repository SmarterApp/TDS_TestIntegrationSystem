﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void Subject_ShouldCreateANewSubject()
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.publisher, testPackage.subject);

            mockClientDao.Setup(dao => dao.Find(testPackage.publisher))
                .Returns(new List<ClientDTO> { new ClientDTO { Name = testPackage.publisher, ClientKey = 99 } });
            mockSubjectDao.Setup(dao => dao.Find(subjectKey))
                .Returns(new List<SubjectDTO>());
            mockSubjectDao.Setup(dao => dao.Insert(It.IsAny<IList<SubjectDTO>>()))
                .Verifiable();

            itembankConfigurationDataService.CreateSubject(testPackage);

            mockClientDao.Verify(dao => dao.Find(testPackage.publisher));
            mockSubjectDao.Verify(dao => dao.Find(subjectKey));
            mockSubjectDao.Verify(dao => dao.Insert(It.Is<List<SubjectDTO>>(subject => 
                subject.Count == 1 
                && subject[0].Name.Equals(testPackage.subject)
                && subject[0].Grade.Equals(string.Empty)
                && subject[0].ClientKey == 99
                && subject[0].TestVersion == 42L)));
        }

        [TestMethod]
        public void Subject_ShouldNotCreateASubjectIfItAlreadyExists()
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.publisher, testPackage.subject);

            mockClientDao.Setup(dao => dao.Find(testPackage.publisher))
                .Returns(new List<ClientDTO> { new ClientDTO { Name = testPackage.publisher, ClientKey = 99 } });
            mockSubjectDao.Setup(dao => dao.Find(subjectKey))
                .Returns(new List<SubjectDTO> { new SubjectDTO { Name = testPackage.subject, ClientKey = 99 } });

            itembankConfigurationDataService.CreateSubject(testPackage);

            mockSubjectDao.Verify(dao => dao.Find(subjectKey));
            mockClientDao.Verify(dao => dao.Find(testPackage.publisher), Times.Never);
            mockSubjectDao.Verify(dao => dao.Insert(It.IsAny<IList<SubjectDTO>>()), Times.Never);
        }

        [TestMethod]
        public void Subject_ShouldThrowInvalidOperationExceptionIfThereIsNoClientRecord()
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.publisher, testPackage.subject);

            mockClientDao.Setup(dao => dao.Find(testPackage.publisher))
                .Returns(new List<ClientDTO>());
            mockSubjectDao.Setup(dao => dao.Find(subjectKey))
                .Returns(new List<SubjectDTO>());

            var exception = 
                Assert.ThrowsException<InvalidOperationException>(() => itembankConfigurationDataService.CreateSubject(testPackage));

            mockClientDao.Verify(dao => dao.Find(testPackage.publisher));
            mockSubjectDao.Verify(dao => dao.Insert(It.IsAny<List<SubjectDTO>>()), Times.Never);

            Assert.AreEqual("Could not find a client record with name 'unit-test-publisher'", exception.Message);
        }

        [TestMethod]
        public void Strand_ShouldCreateACollectionOfStrands()
        {
            var loadedTestPackage = TestPackageAssembler.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
            var subjectKey = string.Format("{0}-{1}", loadedTestPackage.publisher, loadedTestPackage.subject);
            var mockClient = new ClientDTO { Name = loadedTestPackage.publisher, ClientKey = 99 };
            var mockSubject = new SubjectDTO { Name = loadedTestPackage.subject, ClientKey = 99 };

            mockClientDao.Setup(dao => dao.Find(loadedTestPackage.publisher))
                .Returns(new List<ClientDTO> { mockClient });
            mockSubjectDao.Setup(dao => dao.Find(subjectKey))
                .Returns(new List<SubjectDTO> { mockSubject });
            mockStrandDao.Setup(dao => dao.Insert(It.IsAny<IList<StrandDTO>>()))
                .Verifiable();

            IDictionary<string, StrandDTO> result = itembankConfigurationDataService.CreateStrands(loadedTestPackage);

            mockClientDao.Verify(dao => dao.Find(loadedTestPackage.publisher));
            mockSubjectDao.Verify(dao => dao.Find(subjectKey));
            mockStrandDao.Verify(dao => dao.Insert(It.Is<IList<StrandDTO>>(list => list.Count == 77))); // 77 nested BP elements in the example Test Package

            // Verify a leaf-level StrandDTO
            var leafLevelTargetStrand = result["1|F-IF|K|m|F-IF.1"];
            Assert.AreEqual("SBAC_PT-1|F-IF|K|m|F-IF.1", leafLevelTargetStrand.Key);
            Assert.IsTrue(leafLevelTargetStrand.IsLeafTarget);
            Assert.AreEqual(99, leafLevelTargetStrand.ClientKey);
            Assert.AreEqual("contentlevel", leafLevelTargetStrand.Type);
            Assert.AreEqual("SBAC_PT-1|F-IF|K|m", leafLevelTargetStrand.ParentId);
            Assert.AreEqual(5, leafLevelTargetStrand.TreeLevel); // TODO: should be 5?
            Assert.AreEqual(subjectKey, leafLevelTargetStrand.SubjectKey);

            // Verify a "claim" StrandDTO
            var claimStrand = result["1"];
            Assert.AreEqual("SBAC_PT-1", claimStrand.Key);
            Assert.IsFalse(claimStrand.IsLeafTarget);
            Assert.AreEqual(99, claimStrand.ClientKey);
            Assert.AreEqual("strand", claimStrand.Type);
            Assert.AreEqual(string.Empty, claimStrand.ParentId);
            Assert.AreEqual(1, claimStrand.TreeLevel);
            Assert.AreEqual(subjectKey, claimStrand.SubjectKey);
        }

        [TestMethod]
        public void Strand_ShouldThrowExceptionWhenSubjectCannotBeFound()
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.publisher, testPackage.subject);
            var mockClient = new ClientDTO { Name = testPackage.publisher, ClientKey = 99 };

            mockClientDao.Setup(dao => dao.Find(testPackage.publisher))
                .Returns(new List<ClientDTO> { mockClient });
            mockSubjectDao.Setup(dao => dao.Find(subjectKey))
                .Returns(new List<SubjectDTO>());
            mockStrandDao.Setup(dao => dao.Insert(It.IsAny<IList<StrandDTO>>()))
                .Verifiable();

            var exception = 
                Assert.ThrowsException<InvalidOperationException>(() => itembankConfigurationDataService.CreateStrands(testPackage));

            mockClientDao.Verify(dao => dao.Find(testPackage.publisher), Times.Never);
            mockSubjectDao.Verify(dao => dao.Find(subjectKey));
            mockStrandDao.Verify(dao => dao.Insert(It.IsAny<List<StrandDTO>>()), Times.Never);

            Assert.AreEqual(string.Format("Could not find a subject for '{0}'", subjectKey), exception.Message);
        }

        [TestMethod]
        public void Strand_ShouldThrowExceptionWhenClientCannotBeFound()
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.publisher, testPackage.subject);
            var mockSubject = new SubjectDTO { Name = testPackage.subject, ClientKey = 99 };

            mockClientDao.Setup(dao => dao.Find(testPackage.publisher))
                .Returns(new List<ClientDTO>());
            mockSubjectDao.Setup(dao => dao.Find(subjectKey))
                .Returns(new List<SubjectDTO> { mockSubject });
            mockStrandDao.Setup(dao => dao.Insert(It.IsAny<IList<StrandDTO>>()))
                .Verifiable();

            var exception =
                Assert.ThrowsException<InvalidOperationException>(() => itembankConfigurationDataService.CreateStrands(testPackage));

            mockClientDao.Verify(dao => dao.Find(testPackage.publisher));
            mockSubjectDao.Verify(dao => dao.Find(subjectKey));
            mockStrandDao.Verify(dao => dao.Insert(It.IsAny<List<StrandDTO>>()), Times.Never);

            Assert.AreEqual(string.Format("Could not find a client record with name '{0}'", testPackage.publisher), exception.Message);
        }
    }
}