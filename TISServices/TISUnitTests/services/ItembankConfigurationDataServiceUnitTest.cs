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
using TISUnitTests.utils;

namespace TISUnitTests.services
{
    [TestClass]
    public class ItembankConfigurationDataServiceUnitTest
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml";

        private readonly Mock<ITestPackageDao<ClientDTO>> mockClientDao = new Mock<ITestPackageDao<ClientDTO>>();
        private readonly Mock<ITestPackageDao<SubjectDTO>> mockSubjectDao = new Mock<ITestPackageDao<SubjectDTO>>();
        private readonly Mock<ITestPackageDao<StrandDTO>> mockStrandDao = new Mock<ITestPackageDao<StrandDTO>>();
        private readonly Mock<ITestPackageDao<ItemDTO>> mockItemDao = new Mock<ITestPackageDao<ItemDTO>>();
        private readonly Mock<ITestPackageDao<StimulusDTO>> mockStimulusDao = new Mock<ITestPackageDao<StimulusDTO>>();
        private readonly Mock<ITestPackageDao<AaItemClDTO>> mockAaItemClDao = new Mock<ITestPackageDao<AaItemClDTO>>();
        private readonly Mock<ITestPackageDao<SetOfItemStrandDTO>> mockSetOfItemStrandDao = new Mock<ITestPackageDao<SetOfItemStrandDTO>>();
        private readonly Mock<ITestPackageDao<SetOfItemStimuliDTO>> mockSetOfItemStimuliDao = new Mock<ITestPackageDao<SetOfItemStimuliDTO>>();
        private readonly Mock<ITestPackageDao<ItemPropertyDTO>> mockItemPropertyDao = new Mock<ITestPackageDao<ItemPropertyDTO>>();
        private readonly Mock<IItembankConfigurationDataQueryService> mockItembankConfigurationQueryService = new Mock<IItembankConfigurationDataQueryService>();

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
                new ItembankConfigurationDataService(mockItembankConfigurationQueryService.Object, 
                    mockSubjectDao.Object,
                    mockStrandDao.Object,
                    mockItemDao.Object,
                    mockStimulusDao.Object,
                    mockAaItemClDao.Object,
                    mockSetOfItemStrandDao.Object,
                    mockSetOfItemStimuliDao.Object,
                    mockItemPropertyDao.Object);
        }

        [TestMethod]
        public void Subject_ShouldCreateANewSubject()
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.publisher, testPackage.subject);

            mockItembankConfigurationQueryService.Setup(svc => svc.FindClientByName(testPackage.publisher))
                .Returns(new ClientDTO { Name = testPackage.publisher, ClientKey = 99 });
            mockItembankConfigurationQueryService.Setup(svc => svc.FindSubject(subjectKey))
                .Returns(null as SubjectDTO);
            mockSubjectDao.Setup(dao => dao.Insert(It.IsAny<IList<SubjectDTO>>()))
                .Verifiable();

            itembankConfigurationDataService.CreateSubject(testPackage);

            mockItembankConfigurationQueryService.Verify(svc => svc.FindClientByName(testPackage.publisher));
            mockItembankConfigurationQueryService.Verify(svc => svc.FindSubject(subjectKey));
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

            mockItembankConfigurationQueryService.Setup(svc => svc.FindClientByName(testPackage.publisher))
                .Returns(new ClientDTO { Name = testPackage.publisher, ClientKey = 99L });
            mockItembankConfigurationQueryService.Setup(svc => svc.FindSubject(It.IsAny<string>()))
                .Returns(new SubjectDTO { Name = testPackage.subject, ClientKey = 99 });
            mockSubjectDao.Setup(dao => dao.Insert(It.IsAny<IList<SubjectDTO>>()))
                .Verifiable();

            itembankConfigurationDataService.CreateSubject(testPackage);

            mockItembankConfigurationQueryService.Verify(svc => svc.FindSubject(subjectKey));
            mockItembankConfigurationQueryService.Verify(svc => svc.FindClientByName(testPackage.publisher), Times.Never);
            mockSubjectDao.Verify(dao => dao.Insert(It.IsAny<IList<SubjectDTO>>()), Times.Never);
        }

        [TestMethod]
        public void Subject_ShouldThrowInvalidOperationExceptionIfThereIsNoClientRecord()
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.publisher, testPackage.subject);

            mockItembankConfigurationQueryService.Setup(svc => svc.FindClientByName(testPackage.publisher))
                .Returns(null as ClientDTO);
            mockItembankConfigurationQueryService.Setup(svc => svc.FindSubject(subjectKey))
                .Returns(null as SubjectDTO);

            var exception = 
                Assert.ThrowsException<InvalidOperationException>(() => itembankConfigurationDataService.CreateSubject(testPackage));

            mockItembankConfigurationQueryService.Verify(svc => svc.FindClientByName(testPackage.publisher));
            mockSubjectDao.Verify(dao => dao.Insert(It.IsAny<List<SubjectDTO>>()), Times.Never);

            Assert.AreEqual("Could not find a client record with name 'unit-test-publisher'", exception.Message);
        }

        [TestMethod]
        public void Strand_ShouldCreateACollectionOfStrands()
        {
            var loadedTestPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
            var subjectKey = string.Format("{0}-{1}", loadedTestPackage.publisher, loadedTestPackage.subject);
            var mockClient = new ClientDTO { Name = loadedTestPackage.publisher, ClientKey = 99 };
            var mockSubject = new SubjectDTO { Name = loadedTestPackage.subject, ClientKey = 99 };

            mockItembankConfigurationQueryService.Setup(svc => svc.FindClientByName(loadedTestPackage.publisher))
                .Returns(mockClient);
            mockItembankConfigurationQueryService.Setup(svc => svc.FindSubject(subjectKey))
                .Returns(mockSubject);
            mockStrandDao.Setup(dao => dao.Insert(It.IsAny<IList<StrandDTO>>()))
                .Verifiable();

            IDictionary<string, StrandDTO> result = itembankConfigurationDataService.CreateStrands(loadedTestPackage);

            mockItembankConfigurationQueryService.Setup(svc => svc.FindClientByName(loadedTestPackage.publisher));
            mockItembankConfigurationQueryService.Setup(svc => svc.FindSubject(subjectKey));
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

            mockItembankConfigurationQueryService.Setup(svc => svc.FindClientByName(testPackage.publisher))
                .Returns(mockClient);
            mockItembankConfigurationQueryService.Setup(svc => svc.FindSubject(subjectKey))
                .Returns(null as SubjectDTO);
            mockStrandDao.Setup(dao => dao.Insert(It.IsAny<IList<StrandDTO>>()))
                .Verifiable();

            var exception = 
                Assert.ThrowsException<InvalidOperationException>(() => itembankConfigurationDataService.CreateStrands(testPackage));

            mockItembankConfigurationQueryService.Verify(svc => svc.FindClientByName(testPackage.publisher), Times.Never);
            mockItembankConfigurationQueryService.Verify(svc => svc.FindSubject(subjectKey));
            mockStrandDao.Verify(dao => dao.Insert(It.IsAny<List<StrandDTO>>()), Times.Never);

            Assert.AreEqual(string.Format("Could not find a subject for '{0}'", subjectKey), exception.Message);
        }

        [TestMethod]
        public void Strand_ShouldThrowExceptionWhenClientCannotBeFound()
        {
            var subjectKey = string.Format("{0}-{1}", testPackage.publisher, testPackage.subject);
            var mockSubject = new SubjectDTO { Name = testPackage.subject, ClientKey = 99 };

            mockItembankConfigurationQueryService.Setup(svc => svc.FindClientByName(testPackage.publisher))
                .Returns(null as ClientDTO);
            mockItembankConfigurationQueryService.Setup(svc => svc.FindSubject(subjectKey))
                .Returns(mockSubject);
            mockStrandDao.Setup(dao => dao.Insert(It.IsAny<IList<StrandDTO>>()))
                .Verifiable();

            var exception =
                Assert.ThrowsException<InvalidOperationException>(() => itembankConfigurationDataService.CreateStrands(testPackage));

            mockItembankConfigurationQueryService.Verify(svc => svc.FindClientByName(testPackage.publisher));
            mockItembankConfigurationQueryService.Verify(svc => svc.FindSubject(subjectKey));
            mockStrandDao.Verify(dao => dao.Insert(It.IsAny<List<StrandDTO>>()), Times.Never);

            Assert.AreEqual(string.Format("Could not find a client record with name '{0}'", testPackage.publisher), exception.Message);
        }

        [TestMethod]
        public void Stimuli_ShoulsCreateaACollectionOfStimuli()
        {
            var loadedTestPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));

            mockStimulusDao.Setup(dao => dao.Insert(It.IsAny<List<StimulusDTO>>()));

            itembankConfigurationDataService.CreateStimuli(loadedTestPackage);

            mockStimulusDao.Verify(dao => dao.Insert(It.Is<List<StimulusDTO>>(stimuli =>
                stimuli.Count == 1
                    && stimuli[0].ItemBankKey == 187
                    && stimuli[0].ItsKey == 3688
                    && stimuli[0].StimulusKey.Equals("187-3688")
                    && stimuli[0].FilePath.Equals("stim-187-3688/")
                    && stimuli[0].FileName.Equals("stim-187-3688.xml")
                    && stimuli[0].TestVersion == (long)loadedTestPackage.version)));
        }

        [TestMethod]
        public void Item_SouldCreateACollectionOfItems()
        {
            var loadedTestPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));

            mockItemDao.Setup(dao => dao.Insert(It.IsAny<List<ItemDTO>>()));

            itembankConfigurationDataService.CreateItems(loadedTestPackage);

            mockItemDao.Verify(dao => dao.Insert(It.Is<List<ItemDTO>>(items => 
                items.Count == 20)));

        }

        [TestMethod]
        public void SetOfItemStrand_ShouldCreateASetOfItemStrandCollection()
        {
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
            var strandMap = StrandBuilder.GetStrandDTODictionary(testPackage);

            mockAaItemClDao.Setup(dao => dao.Insert(It.IsAny<List<AaItemClDTO>>()))
                .Verifiable();
            mockSetOfItemStrandDao.Setup(dao => dao.Insert(It.IsAny<IList<SetOfItemStrandDTO>>()))
                .Verifiable();

            itembankConfigurationDataService.LinkItemToStrands(testPackage, strandMap);

            mockAaItemClDao.Verify(dao => dao.Insert(It.Is<List<AaItemClDTO>>(result =>
                result.Count == 108)));
            mockSetOfItemStrandDao.Verify(dao => dao.Insert(It.Is<IList<SetOfItemStrandDTO>>(result =>
            result.Count == 20)));
        }

        [TestMethod]
        public void SetOfItemStimuli_ShouldCreateASetOfItemStimuli()
        {
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));

            mockSetOfItemStimuliDao.Setup(dao => dao.Insert(It.IsAny<List<SetOfItemStimuliDTO>>()))
                .Verifiable();

            itembankConfigurationDataService.LinkItemsToStimuli(testPackage);

            mockSetOfItemStimuliDao.Verify(dao => dao.Insert(It.Is<List<SetOfItemStimuliDTO>>(result =>
                result.Count == 2
                    && result[0].SegmentKey.Equals("(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018")
                    && result[0].StimulusKey.Equals("187-3688")
                    && result[0].ItemKey.Equals("187-1434")
                    && result[1].SegmentKey.Equals("(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018")
                    && result[1].StimulusKey.Equals("187-3688")
                    && result[1].ItemKey.Equals("187-1432")
                )));
        }

        [TestMethod]
        public void ItemProperties_ShouldCreateItemPropertiesForTestPackage()
        {
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));

            mockItemPropertyDao.Setup(dao => dao.Insert(It.IsAny<List<ItemPropertyDTO>>()));

            itembankConfigurationDataService.CreateItemProperties(testPackage);

            mockItemPropertyDao.Verify(dao => dao.Insert(It.Is<List<ItemPropertyDTO>>(result =>
                result.Count == 60 // 3 properties per item; 20 items in testPackage
                    && result.FindAll(r => r.ItemKey.Equals("187-2029")).Count == 3
                    && result.Find(r => r.PropertyName.Equals("language", StringComparison.InvariantCultureIgnoreCase)).PropertyValue.Equals("ENU")
                )));
        }
    }
}
