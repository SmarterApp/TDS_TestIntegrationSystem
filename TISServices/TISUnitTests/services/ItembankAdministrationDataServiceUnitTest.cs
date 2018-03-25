using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ItembankAdministrationDataServiceUnitTest
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml";
        private const string MULTI_SEGMENT_TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-MULTI-SEGMENT-EXAMPLE.xml";

        private Mock<ITestPackageDao<TestAdminDTO>> mockTestAdminDao = new Mock<ITestPackageDao<TestAdminDTO>>();
        private Mock<ITestPackageDao<SetOfAdminSubjectDTO>> mockSetOfAdminSubjectDao = new Mock<ITestPackageDao<SetOfAdminSubjectDTO>>();
        private Mock<ITestPackageDao<AdminStrandDTO>> mockAdminStrandDao = new Mock<ITestPackageDao<AdminStrandDTO>>();
        private Mock<ITestPackageDao<SetOfAdminItemDTO>> mockSetOfAdminItemDao = new Mock<ITestPackageDao<SetOfAdminItemDTO>>();
        private Mock<ITestPackageDao<ItemScoreDimensionDTO>> mockItemScoreDimensionDao = new Mock<ITestPackageDao<ItemScoreDimensionDTO>>();
        private Mock<ITestPackageDao<ItemMeasurementParameterDTO>> mockItemMeasurementParameterDao = new Mock<ITestPackageDao<ItemMeasurementParameterDTO>>();
        private Mock<ITestPackageDao<AdminStimulusDTO>> mockSetOfAdminStimuliDao = new Mock<ITestPackageDao<AdminStimulusDTO>>();
        private Mock<ITestPackageDao<TestFormDTO>> mockTestFormDao = new Mock<ITestPackageDao<TestFormDTO>>();
        private Mock<ITestPackageDao<TestFormItemDTO>> mockTestFormItemDao = new Mock<ITestPackageDao<TestFormItemDTO>>();
        private Mock<ITestPackageDao<AffinityGroupDTO>> mockAffinityGroupDao = new Mock<ITestPackageDao<AffinityGroupDTO>>();
        private Mock<ITestPackageDao<AffinityGroupItemDTO>> mockAffinityGroupItemDao = new Mock<ITestPackageDao<AffinityGroupItemDTO>>();

        private Mock<IItembankConfigurationDataQueryService> mockItembankConfigurationDataQueryService = new Mock<IItembankConfigurationDataQueryService>();

        private IItembankAdministrationDataService itembankAdministrationDataService;

        [TestInitialize]
        public void Setup()
        {
            itembankAdministrationDataService =
                new ItembankAdministrationDataService(mockItembankConfigurationDataQueryService.Object,
                    mockTestAdminDao.Object,
                    mockSetOfAdminSubjectDao.Object,
                    mockAdminStrandDao.Object,
                    mockSetOfAdminItemDao.Object,
                    mockItemScoreDimensionDao.Object,
                    mockItemMeasurementParameterDao.Object,
                    mockSetOfAdminStimuliDao.Object,
                    mockTestFormDao.Object,
                    mockTestFormItemDao.Object,
                    mockAffinityGroupDao.Object,
                    mockAffinityGroupItemDao.Object);
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

            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindTestAdmin(testPackage.publisher))
                .Returns((TestAdminDTO)null);
            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindClientByName(testPackage.publisher))
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

            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindTestAdmin(testPackage.publisher))
                .Returns(existingTestAdminDto);
            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindClientByName(testPackage.publisher))
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

            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindTestAdmin(testPackage.publisher))
                .Verifiable();
            mockItembankConfigurationDataQueryService.Setup(svc => svc.FindClientByName(testPackage.publisher))
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

        [TestMethod]
        public void SetOfAdminSubject_ShouldCreateSetOfAdminSubjectRecordsForAllSegments()
        {
            // This test package has two assessments, each with one segment
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));

            mockSetOfAdminSubjectDao.Setup(dao => dao.Insert(It.IsAny<List<SetOfAdminSubjectDTO>>()))
                .Verifiable();

            itembankAdministrationDataService.CreateSetOfAdminSubjects(testPackage);

            mockSetOfAdminSubjectDao.Verify(dao => dao.Insert(It.Is<IList<SetOfAdminSubjectDTO>>(result => 
                EvaluateShouldInsertSetOfAdminSubjectRecordsForAllSegments(result.ToList()))));
        }

        [TestMethod]
        public void SetOfAdminSubject_ShouldCreateSetOfAdminRecordsForAMultiSegmentedAssessment()
        {
            // This test package has one assessment with tw segments
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(MULTI_SEGMENT_TEST_PACKAGE_XML_FILE));

            mockSetOfAdminSubjectDao.Setup(dao => dao.Insert(It.IsAny<List<SetOfAdminSubjectDTO>>()))
                .Verifiable();

            itembankAdministrationDataService.CreateSetOfAdminSubjects(testPackage);

            mockSetOfAdminSubjectDao.Verify(dao => dao.Insert(It.Is<List<SetOfAdminSubjectDTO>>(result =>
                EvaluateShouldCreateSetOfAdminRecordsForAMultiSegmentedAssessment(result))));
        }

        [TestMethod]
        public void AdminStrand_ShouldCreateACollectionOfAdminStrands()
        {
            // This test package has two assessments, each with one segment
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
            var strandMap = StrandBuilder.GetStrandDTODictionary(testPackage);

            mockAdminStrandDao.Setup(dao => dao.Insert(It.IsAny<List<AdminStrandDTO>>()))
                .Verifiable();

            itembankAdministrationDataService.CreateAdminStrands(testPackage, strandMap);

            mockAdminStrandDao.Verify(dao => dao.Insert(It.Is<List<AdminStrandDTO>>(result =>
                EvaluateShouldCreateACollectionOfAdminStrands(result))));
        }

        [TestMethod]
        public void SetOfAdminItem_ShouldCreateASetOfAdminItemsCollection()
        {
            // This test package has two assessments, each with one segment
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
            var strandMap = StrandBuilder.GetStrandDTODictionary(testPackage);

            mockSetOfAdminItemDao.Setup(dao => dao.Insert(It.IsAny<List<SetOfAdminItemDTO>>()))
                .Verifiable();

            itembankAdministrationDataService.CreateSetOfAdminItems(testPackage, strandMap);

            mockSetOfAdminItemDao.Verify(dao => dao.Insert(It.Is<List<SetOfAdminItemDTO>>(result =>
                EvaluateShouldCreateASetOfAdminItemsCollection(result))));
        }

        [TestMethod]
        public void ItemMeasurementParameter_ShouldCreateAnItemMeasurementParameterCollection()
        {
            // This test package has two assessments, each with one segment
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));

            mockItemScoreDimensionDao.Setup(dao => dao.Insert(It.IsAny<List<ItemScoreDimensionDTO>>()))
                .Verifiable();
            mockItemMeasurementParameterDao.Setup(dao => dao.Insert(It.IsAny<List<ItemMeasurementParameterDTO>>()))
                .Verifiable();

            itembankAdministrationDataService.CreateItemMeasurementParameters(testPackage);

            mockItemScoreDimensionDao.Verify(dao => dao.Insert(It.Is<List<ItemScoreDimensionDTO>>(result => 
                EvaluateShouldCreateAnItemMeasurementParameterCollection_ItemScoreDimensions(result))));

            mockItemMeasurementParameterDao.Verify(dao => dao.Insert(It.Is<List<ItemMeasurementParameterDTO>>(result =>
                EvaluateShouldCreateAnItemMeasurementParameterCollection_ItemMeasurementParameters(result))));
        }

        [TestMethod]
        public void AdminStimuli_ShouldCreateACollectionOfAdminStimuli()
        {
            // This test package has two assessments, each with one segment
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));

            mockSetOfAdminStimuliDao.Setup(dao => dao.Insert(It.IsAny<List<AdminStimulusDTO>>()))
                .Verifiable();

            itembankAdministrationDataService.CreateAdminStimuli(testPackage);

            mockSetOfAdminStimuliDao.Verify(dao => dao.Insert(It.Is<List<AdminStimulusDTO>>(result =>
                EvaluateShouldCreateACollectionOfAdminStimuli(result))));
        }

        [TestMethod]
        public void TestForm_ShouldCreateACollectionOfTestForms()
        {
            // This test package has two assessments, each with one segment
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
            var fixedFormSegmentKey = "(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018";
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

            mockTestFormDao.Setup(dao => dao.Find(It.IsAny<string>()))
                .Returns(new List<TestFormDTO> { mockTdsTesForm });
            mockTestFormDao.Setup(dao => dao.Insert(It.IsAny<List<TestFormDTO>>()))
                .Verifiable();

            var result = itembankAdministrationDataService.CreateTestForms(testPackage);

            mockTestFormDao.Verify(dao => dao.Find(fixedFormSegmentKey));
            mockTestFormDao.Verify(dao => dao.Insert(It.IsAny<List<TestFormDTO>>()));

            Assert.AreEqual(1, result.Count);

            var insertedTestForm = result[0];
            Assert.AreEqual(mockTdsTesForm.TestFormKey, insertedTestForm.TestFormKey);
            Assert.AreEqual(mockTdsTesForm.SegmentKey, insertedTestForm.SegmentKey);
            Assert.AreEqual(mockTdsTesForm.ITSBankKey, insertedTestForm.ITSBankKey);
            Assert.AreEqual(mockTdsTesForm.ITSKey, insertedTestForm.ITSKey);
            Assert.AreEqual(mockTdsTesForm.Language, insertedTestForm.Language);
            Assert.AreEqual(mockTdsTesForm.FormId, insertedTestForm.FormId);
            Assert.AreEqual(mockTdsTesForm.Cohort, insertedTestForm.Cohort);
        }

        [TestMethod]
        public void TestFormItem_ShouldCreateACollectionOfTestFormItemDtos()
        {
            // This test package has two assessments, each with one segment
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
            var fixedFormSegmentKey = "(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018";
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

            mockTestFormItemDao.Setup(dao => dao.Insert(It.IsAny<List<TestFormItemDTO>>()))
                .Verifiable();

            itembankAdministrationDataService.CreateTestFormItems(testPackage, new List<TestFormDTO> { mockTdsTesForm });

            mockTestFormItemDao.Verify(dao => dao.Insert(It.Is<List<TestFormItemDTO>>(result =>
                EvaluateShouldCreateACollectionOfTestFormItemDtos(result))));
        }

        [TestMethod]
        public void AffinityGroup_ShouldCreateACollectionOfAffinityGroupsAndItems()
        {
            // This test package has two assessments, each with one segment
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));

            mockAffinityGroupDao.Setup(dao => dao.Insert(It.IsAny<IList<AffinityGroupDTO>>()))
                .Verifiable();
            mockAffinityGroupItemDao.Setup(dao => dao.Insert(It.IsAny<IList<AffinityGroupItemDTO>>()))
                .Verifiable();

            itembankAdministrationDataService.CreateAffinityGroups(testPackage);

            mockAffinityGroupDao.Verify(dao => dao.Insert(It.Is<IList<AffinityGroupDTO>>(result => 
                EvaluateShouldCreateACollectionOfAffinityGroups(result))));
            mockAffinityGroupItemDao.Verify(dao => dao.Insert(It.Is<IList<AffinityGroupItemDTO>>(result =>
                EvaluateShouldCreateACollectionOfAffinityGroupItems(result))));
        }

        private bool EvaluateShouldInsertSetOfAdminSubjectRecordsForAllSegments(IList<SetOfAdminSubjectDTO> setOfAdminSubjectDtos)
        {
            Assert.AreEqual(2, setOfAdminSubjectDtos.Count);

            var firstSetOfAdminDto = setOfAdminSubjectDtos[0];

            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", firstSetOfAdminDto.SegmentKey);
            Assert.AreEqual("SBAC_PT", firstSetOfAdminDto.TestAdminKey);
            Assert.AreEqual("SBAC_PT-MATH", firstSetOfAdminDto.SubjectKey);
            Assert.AreEqual("SBAC-IRP-CAT-MATH-11", firstSetOfAdminDto.TestId);
            Assert.AreEqual(8, firstSetOfAdminDto.MinItems);
            Assert.AreEqual(8, firstSetOfAdminDto.MaxItems);
            Assert.AreEqual(0, firstSetOfAdminDto.FieldTestMinItems);
            Assert.AreEqual(0, firstSetOfAdminDto.FieldTestMaxItems);
            Assert.AreEqual("adaptive2", firstSetOfAdminDto.SelectionAlgorithm);
            Assert.IsNull(firstSetOfAdminDto.FieldTestStartPosition);
            Assert.IsNull(firstSetOfAdminDto.FieldTestEndPosition);
            Assert.IsFalse(firstSetOfAdminDto.IsSegmented);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", firstSetOfAdminDto.VirtualTest);
            Assert.AreEqual(1, firstSetOfAdminDto.TestPosition);
            Assert.AreEqual("summative", firstSetOfAdminDto.TestType);
            Assert.AreEqual("bp1", firstSetOfAdminDto.BlueprintMetricFunction);
            Assert.AreEqual(8185L, firstSetOfAdminDto.TestVersion);
            Assert.AreEqual(1D, firstSetOfAdminDto.BlueprintWeight);
            Assert.AreEqual(0.506053D, firstSetOfAdminDto.StartAbility);
            Assert.AreEqual(0.2D, firstSetOfAdminDto.StartInfo);
            Assert.AreEqual(5, firstSetOfAdminDto.CSet1Size);
            Assert.AreEqual(3, firstSetOfAdminDto.CSet2Random);
            Assert.AreEqual(18, firstSetOfAdminDto.CSet2InitialRandom);
            Assert.AreEqual(0, firstSetOfAdminDto.AbilityOffset);
            Assert.AreEqual(1D, firstSetOfAdminDto.ItemWeight);
            Assert.AreEqual("ABILITY", firstSetOfAdminDto.CSet1Order);
            Assert.AreEqual(79.3D, firstSetOfAdminDto.Slope);
            Assert.AreEqual(2514.9D, firstSetOfAdminDto.Intercept);
            Assert.AreEqual(1D, firstSetOfAdminDto.PrecisionTarget);
            Assert.AreEqual(0D, firstSetOfAdminDto.AdaptiveCut);
            Assert.AreEqual(0D, firstSetOfAdminDto.TooCloseSEs);
            Assert.AreEqual(1D, firstSetOfAdminDto.AbilityWeight);
            Assert.IsFalse(firstSetOfAdminDto.ComputeAbilityEstimates);
            Assert.AreEqual(0D, firstSetOfAdminDto.RcAbilityWeight);
            Assert.AreEqual(1D, firstSetOfAdminDto.PrecisionTargetMetWeight);
            Assert.AreEqual(1D, firstSetOfAdminDto.PrecisionTargetNotMetWeight);
            Assert.IsFalse(firstSetOfAdminDto.TerminationOverallInfo);
            Assert.IsTrue(firstSetOfAdminDto.TerminationMinCount);
            Assert.IsFalse(firstSetOfAdminDto.TerminationTooClose);
            Assert.IsFalse(firstSetOfAdminDto.TerminationFlagsAnd);

            var secondSetOfAdminDto = setOfAdminSubjectDtos[1];

            Assert.AreEqual("(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018", secondSetOfAdminDto.SegmentKey);
            Assert.AreEqual("SBAC_PT", secondSetOfAdminDto.TestAdminKey);
            Assert.AreEqual("SBAC_PT-MATH", secondSetOfAdminDto.SubjectKey);
            Assert.AreEqual("SBAC-IRP-Perf-MATH-11", secondSetOfAdminDto.TestId);
            Assert.AreEqual(7, secondSetOfAdminDto.MinItems);
            Assert.AreEqual(7, secondSetOfAdminDto.MaxItems);
            Assert.AreEqual(0, secondSetOfAdminDto.FieldTestMinItems);
            Assert.AreEqual(0, secondSetOfAdminDto.FieldTestMaxItems);
            Assert.AreEqual("fixedform", secondSetOfAdminDto.SelectionAlgorithm);
            Assert.IsNull(secondSetOfAdminDto.FieldTestStartPosition);
            Assert.IsNull(secondSetOfAdminDto.FieldTestEndPosition);
            Assert.IsFalse(secondSetOfAdminDto.IsSegmented);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018", secondSetOfAdminDto.VirtualTest);
            Assert.AreEqual(1, secondSetOfAdminDto.TestPosition);
            Assert.AreEqual("summative", secondSetOfAdminDto.TestType);
            Assert.AreEqual("bp1", secondSetOfAdminDto.BlueprintMetricFunction);
            Assert.AreEqual(8185L, secondSetOfAdminDto.TestVersion);
            Assert.AreEqual(5D, secondSetOfAdminDto.BlueprintWeight);
            Assert.AreEqual(0D, secondSetOfAdminDto.StartAbility);
            Assert.AreEqual(1D, secondSetOfAdminDto.StartInfo);
            Assert.AreEqual(20, secondSetOfAdminDto.CSet1Size);
            Assert.AreEqual(1, secondSetOfAdminDto.CSet2Random);
            Assert.AreEqual(5, secondSetOfAdminDto.CSet2InitialRandom);
            Assert.AreEqual(0, secondSetOfAdminDto.AbilityOffset);
            Assert.AreEqual(5D, secondSetOfAdminDto.ItemWeight);
            Assert.AreEqual("ABILITY", secondSetOfAdminDto.CSet1Order);
            Assert.AreEqual(79.3D, secondSetOfAdminDto.Slope);
            Assert.AreEqual(2514.9D, secondSetOfAdminDto.Intercept);
            Assert.IsNull(secondSetOfAdminDto.PrecisionTarget);
            Assert.IsNull(secondSetOfAdminDto.AdaptiveCut);
            Assert.IsNull(secondSetOfAdminDto.TooCloseSEs);
            Assert.AreEqual(1D, secondSetOfAdminDto.AbilityWeight);
            Assert.IsTrue(secondSetOfAdminDto.ComputeAbilityEstimates);
            Assert.AreEqual(1D, secondSetOfAdminDto.RcAbilityWeight);
            Assert.AreEqual(1D, secondSetOfAdminDto.PrecisionTargetMetWeight);
            Assert.AreEqual(1D, secondSetOfAdminDto.PrecisionTargetNotMetWeight);
            Assert.IsFalse(secondSetOfAdminDto.TerminationOverallInfo);
            Assert.IsFalse(secondSetOfAdminDto.TerminationMinCount);
            Assert.IsFalse(secondSetOfAdminDto.TerminationTooClose);
            Assert.IsFalse(secondSetOfAdminDto.TerminationFlagsAnd);

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }

        private bool EvaluateShouldCreateSetOfAdminRecordsForAMultiSegmentedAssessment(IList<SetOfAdminSubjectDTO> setOfAdminSubjectDtos)
        {
            Assert.AreEqual(3, setOfAdminSubjectDtos.Count);

            var firstSetOfAdminDto = setOfAdminSubjectDtos[0];

            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-S1-2017-2018", firstSetOfAdminDto.SegmentKey);
            Assert.AreEqual("SBAC_PT", firstSetOfAdminDto.TestAdminKey);
            Assert.AreEqual("SBAC_PT-MATH", firstSetOfAdminDto.SubjectKey);
            Assert.AreEqual("SBAC-IRP-CAT-MATH-11-S1", firstSetOfAdminDto.TestId);
            Assert.AreEqual(8, firstSetOfAdminDto.MinItems);
            Assert.AreEqual(8, firstSetOfAdminDto.MaxItems);
            Assert.AreEqual(0, firstSetOfAdminDto.FieldTestMinItems);
            Assert.AreEqual(0, firstSetOfAdminDto.FieldTestMaxItems);
            Assert.AreEqual("adaptive2", firstSetOfAdminDto.SelectionAlgorithm);
            Assert.IsNull(firstSetOfAdminDto.FieldTestStartPosition);
            Assert.IsNull(firstSetOfAdminDto.FieldTestEndPosition);
            Assert.IsFalse(firstSetOfAdminDto.IsSegmented);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", firstSetOfAdminDto.VirtualTest);
            Assert.AreEqual(1, firstSetOfAdminDto.TestPosition);
            Assert.AreEqual("summative", firstSetOfAdminDto.TestType);
            Assert.AreEqual("bp1", firstSetOfAdminDto.BlueprintMetricFunction);
            Assert.AreEqual(8185L, firstSetOfAdminDto.TestVersion);
            Assert.AreEqual(1D, firstSetOfAdminDto.BlueprintWeight);
            Assert.AreEqual(0.506053D, firstSetOfAdminDto.StartAbility);
            Assert.AreEqual(0.2D, firstSetOfAdminDto.StartInfo);
            Assert.AreEqual(5, firstSetOfAdminDto.CSet1Size);
            Assert.AreEqual(3, firstSetOfAdminDto.CSet2Random);
            Assert.AreEqual(18, firstSetOfAdminDto.CSet2InitialRandom);
            Assert.AreEqual(0, firstSetOfAdminDto.AbilityOffset);
            Assert.AreEqual(1D, firstSetOfAdminDto.ItemWeight);
            Assert.AreEqual("ABILITY", firstSetOfAdminDto.CSet1Order);
            Assert.AreEqual(79.3D, firstSetOfAdminDto.Slope);
            Assert.AreEqual(2514.9D, firstSetOfAdminDto.Intercept);
            Assert.AreEqual(1D, firstSetOfAdminDto.PrecisionTarget);
            Assert.AreEqual(0D, firstSetOfAdminDto.AdaptiveCut);
            Assert.AreEqual(0D, firstSetOfAdminDto.TooCloseSEs);
            Assert.AreEqual(1D, firstSetOfAdminDto.AbilityWeight);
            Assert.IsFalse(firstSetOfAdminDto.ComputeAbilityEstimates);
            Assert.AreEqual(0D, firstSetOfAdminDto.RcAbilityWeight);
            Assert.AreEqual(1D, firstSetOfAdminDto.PrecisionTargetMetWeight);
            Assert.AreEqual(1D, firstSetOfAdminDto.PrecisionTargetNotMetWeight);
            Assert.IsFalse(firstSetOfAdminDto.TerminationOverallInfo);
            Assert.IsTrue(firstSetOfAdminDto.TerminationMinCount);
            Assert.IsFalse(firstSetOfAdminDto.TerminationTooClose);
            Assert.IsFalse(firstSetOfAdminDto.TerminationFlagsAnd);

            var secondSetOfAdminDto = setOfAdminSubjectDtos[1];

            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-S2-2017-2018", secondSetOfAdminDto.SegmentKey);
            Assert.AreEqual("SBAC_PT", secondSetOfAdminDto.TestAdminKey);
            Assert.AreEqual("SBAC_PT-MATH", secondSetOfAdminDto.SubjectKey);
            Assert.AreEqual("SBAC-IRP-CAT-MATH-11-S2", secondSetOfAdminDto.TestId);
            Assert.AreEqual(7, secondSetOfAdminDto.MinItems);
            Assert.AreEqual(7, secondSetOfAdminDto.MaxItems);
            Assert.AreEqual(0, secondSetOfAdminDto.FieldTestMinItems);
            Assert.AreEqual(0, secondSetOfAdminDto.FieldTestMaxItems);
            Assert.AreEqual("adaptive2", secondSetOfAdminDto.SelectionAlgorithm);
            Assert.IsNull(secondSetOfAdminDto.FieldTestStartPosition);
            Assert.IsNull(secondSetOfAdminDto.FieldTestEndPosition);
            Assert.IsFalse(secondSetOfAdminDto.IsSegmented);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", secondSetOfAdminDto.VirtualTest);
            Assert.AreEqual(2, secondSetOfAdminDto.TestPosition);
            Assert.AreEqual("summative", secondSetOfAdminDto.TestType);
            Assert.AreEqual("bp1", secondSetOfAdminDto.BlueprintMetricFunction);
            Assert.AreEqual(8185L, secondSetOfAdminDto.TestVersion);
            Assert.AreEqual(5D, secondSetOfAdminDto.BlueprintWeight);
            Assert.AreEqual(0D, secondSetOfAdminDto.StartAbility);
            Assert.AreEqual(1D, secondSetOfAdminDto.StartInfo);
            Assert.AreEqual(20, secondSetOfAdminDto.CSet1Size);
            Assert.AreEqual(1, secondSetOfAdminDto.CSet2Random);
            Assert.AreEqual(5, secondSetOfAdminDto.CSet2InitialRandom);
            Assert.AreEqual(0, secondSetOfAdminDto.AbilityOffset);
            Assert.AreEqual(5D, secondSetOfAdminDto.ItemWeight);
            Assert.AreEqual("ABILITY", secondSetOfAdminDto.CSet1Order);
            Assert.AreEqual(79.3D, secondSetOfAdminDto.Slope);
            Assert.AreEqual(2514.9D, secondSetOfAdminDto.Intercept);
            Assert.IsNull(secondSetOfAdminDto.PrecisionTarget);
            Assert.IsNull(secondSetOfAdminDto.AdaptiveCut);
            Assert.IsNull(secondSetOfAdminDto.TooCloseSEs);
            Assert.AreEqual(1D, secondSetOfAdminDto.AbilityWeight);
            Assert.IsTrue(secondSetOfAdminDto.ComputeAbilityEstimates);
            Assert.AreEqual(1D, secondSetOfAdminDto.RcAbilityWeight);
            Assert.AreEqual(1D, secondSetOfAdminDto.PrecisionTargetMetWeight);
            Assert.AreEqual(1D, secondSetOfAdminDto.PrecisionTargetNotMetWeight);
            Assert.IsFalse(secondSetOfAdminDto.TerminationOverallInfo);
            Assert.IsFalse(secondSetOfAdminDto.TerminationMinCount);
            Assert.IsFalse(secondSetOfAdminDto.TerminationTooClose);
            Assert.IsFalse(secondSetOfAdminDto.TerminationFlagsAnd);

            var virtualTestSetOfAdminDto = setOfAdminSubjectDtos[2];

            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", virtualTestSetOfAdminDto.SegmentKey);
            Assert.AreEqual("SBAC_PT", virtualTestSetOfAdminDto.TestAdminKey);
            Assert.AreEqual("SBAC_PT-MATH", virtualTestSetOfAdminDto.SubjectKey);
            Assert.AreEqual("SBAC-IRP-CAT-MATH-11", virtualTestSetOfAdminDto.TestId);
            Assert.AreEqual(15, virtualTestSetOfAdminDto.MinItems);
            Assert.AreEqual(15, virtualTestSetOfAdminDto.MaxItems);
            Assert.AreEqual(0, virtualTestSetOfAdminDto.FieldTestMinItems);
            Assert.AreEqual(0, virtualTestSetOfAdminDto.FieldTestMaxItems);
            Assert.AreEqual("virtual", virtualTestSetOfAdminDto.SelectionAlgorithm);
            Assert.IsNull(virtualTestSetOfAdminDto.FieldTestStartPosition);
            Assert.IsNull(virtualTestSetOfAdminDto.FieldTestEndPosition);
            Assert.IsTrue(virtualTestSetOfAdminDto.IsSegmented);
            Assert.IsNull(virtualTestSetOfAdminDto.VirtualTest);
            Assert.AreEqual(0, virtualTestSetOfAdminDto.TestPosition);
            Assert.AreEqual("summative", virtualTestSetOfAdminDto.TestType);
            Assert.AreEqual("bp1", virtualTestSetOfAdminDto.BlueprintMetricFunction);
            Assert.AreEqual(8185L, virtualTestSetOfAdminDto.TestVersion);
            Assert.AreEqual(5D, virtualTestSetOfAdminDto.BlueprintWeight);
            Assert.AreEqual(0D, virtualTestSetOfAdminDto.StartAbility);
            Assert.AreEqual(1D, virtualTestSetOfAdminDto.StartInfo);
            Assert.AreEqual(20, virtualTestSetOfAdminDto.CSet1Size);
            Assert.AreEqual(1, virtualTestSetOfAdminDto.CSet2Random);
            Assert.AreEqual(5, virtualTestSetOfAdminDto.CSet2InitialRandom);
            Assert.AreEqual(0, virtualTestSetOfAdminDto.AbilityOffset);
            Assert.AreEqual(5D, virtualTestSetOfAdminDto.ItemWeight);
            Assert.AreEqual("ABILITY", virtualTestSetOfAdminDto.CSet1Order);
            Assert.AreEqual(1D, virtualTestSetOfAdminDto.Slope);
            Assert.AreEqual(1D, virtualTestSetOfAdminDto.Intercept);
            Assert.IsNull(virtualTestSetOfAdminDto.PrecisionTarget);
            Assert.IsNull(virtualTestSetOfAdminDto.AdaptiveCut);
            Assert.IsNull(virtualTestSetOfAdminDto.TooCloseSEs);
            Assert.AreEqual(1D, virtualTestSetOfAdminDto.AbilityWeight);
            Assert.IsTrue(virtualTestSetOfAdminDto.ComputeAbilityEstimates);
            Assert.AreEqual(1D, virtualTestSetOfAdminDto.RcAbilityWeight);
            Assert.AreEqual(1D, virtualTestSetOfAdminDto.PrecisionTargetMetWeight);
            Assert.AreEqual(1D, virtualTestSetOfAdminDto.PrecisionTargetNotMetWeight);
            Assert.IsFalse(virtualTestSetOfAdminDto.TerminationOverallInfo);
            Assert.IsFalse(virtualTestSetOfAdminDto.TerminationMinCount);
            Assert.IsFalse(virtualTestSetOfAdminDto.TerminationTooClose);
            Assert.IsFalse(virtualTestSetOfAdminDto.TerminationFlagsAnd);

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }

        private bool EvaluateShouldCreateACollectionOfAdminStrands(IList<AdminStrandDTO> adminStrandDtos)
        {
            Assert.AreEqual(62, adminStrandDtos.Count);

            var savedContentLevel = adminStrandDtos.First(x => x.StrandKey.Equals("SBAC_PT-1|F-IF|K|m|F-IF.1"));

            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", savedContentLevel.SegmentKey);
            Assert.AreEqual(0, savedContentLevel.MinItems);
            Assert.AreEqual(1, savedContentLevel.MaxItems);
            Assert.IsNull(savedContentLevel.AdaptiveCut);
            Assert.AreEqual(1D, savedContentLevel.BlueprintWeight);
            Assert.AreEqual(8185L, savedContentLevel.TestVersion);

            var savedStrand = adminStrandDtos.First(dto => dto.StrandKey.Equals("SBAC_PT-1"));

            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", savedStrand.SegmentKey);
            Assert.AreEqual(string.Format("{0}-{1}", savedStrand.SegmentKey, savedStrand.StrandKey), savedStrand.AdminStrandKey);
            Assert.AreEqual(4, savedStrand.MinItems);
            Assert.AreEqual(4, savedStrand.MaxItems);
            Assert.AreEqual(-31.7137D, savedStrand.AdaptiveCut);
            Assert.AreEqual(-31.7137D, savedStrand.StartAbility);
            Assert.AreEqual(0, savedStrand.StartInfo);
            Assert.AreEqual(5D, savedStrand.Scalar);
            Assert.AreEqual(1D, savedStrand.BlueprintWeight);
            Assert.AreEqual(8185L, savedStrand.TestVersion);
            Assert.IsNull(savedStrand.LoadMin);
            Assert.IsNull(savedStrand.LoadMax);

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }

        private bool EvaluateShouldCreateASetOfAdminItemsCollection(IList<SetOfAdminItemDTO> setOfAdminItemDtos)
        {
            Assert.AreEqual(20, setOfAdminItemDtos.Count);

            var itemToVerify = setOfAdminItemDtos.First(dto => dto.ItemKey.Equals("187-2029"));

            Assert.AreEqual("I-187-2029_A", itemToVerify.GroupKey);
            Assert.AreEqual("I-187-2029", itemToVerify.GroupId);
            Assert.AreEqual(1, itemToVerify.ItemPosition);
            Assert.IsFalse(itemToVerify.IsFieldTest);
            Assert.IsTrue(itemToVerify.IsActive);
            Assert.AreEqual("0.678030000000000", itemToVerify.IrtB);
            Assert.AreEqual("A", itemToVerify.BlockId);
            Assert.IsTrue(itemToVerify.IsRequired);
            Assert.AreEqual("SBAC_PT", itemToVerify.TestAdminKey);
            Assert.AreEqual("SBAC_PT-3|G-SRT|A|NA|NA", itemToVerify.StrandKey);
            Assert.AreEqual(8185L, itemToVerify.TestVersion);
            Assert.AreEqual("SBAC_PT-3", itemToVerify.StrandName);
            Assert.AreEqual(0.53279F, itemToVerify.IrtA);
            Assert.AreEqual(0F, itemToVerify.IrtC);
            Assert.AreEqual("IRT3PLn", itemToVerify.IrtModel);
            Assert.AreEqual("0.678030000000000", itemToVerify.BVector);
            Assert.AreEqual("G11Math_DOK2;SBAC_PT-3|G-SRT|A|NA|NA", itemToVerify.ClString);
            
            var itemGroupMemberToVerify = setOfAdminItemDtos.First(dto => dto.ItemKey.Equals("187-1432"));
            Assert.AreEqual("G-187-3688-0_A", itemGroupMemberToVerify.GroupKey);
            Assert.AreEqual("G-187-3688-0", itemGroupMemberToVerify.GroupId);
            Assert.AreEqual(2, itemGroupMemberToVerify.ItemPosition);
            Assert.AreEqual("-0.401550000000000;0.762370000000000", itemGroupMemberToVerify.BVector);

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }

        private bool EvaluateShouldCreateAnItemMeasurementParameterCollection_ItemScoreDimensions(IList<ItemScoreDimensionDTO> itemScoreDimensionDtos)
        {
            Assert.AreEqual(20, itemScoreDimensionDtos.Count);

            var anItemScoreDimensionDto = itemScoreDimensionDtos.First(dto => dto.ItemKey.Equals("187-1930"));
            Assert.IsNotNull(anItemScoreDimensionDto.ItemScoreDimensionKey);
            Assert.AreEqual(string.Empty, anItemScoreDimensionDto.Dimension);
            Assert.AreEqual(string.Empty, anItemScoreDimensionDto.RecodeRule);
            Assert.AreEqual(1, anItemScoreDimensionDto.ScorePoints);
            Assert.AreEqual(1D, anItemScoreDimensionDto.Weight);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", anItemScoreDimensionDto.SegmentKey);
            Assert.AreEqual(4, anItemScoreDimensionDto.MeasurementModel);

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }

        private bool EvaluateShouldCreateAnItemMeasurementParameterCollection_ItemMeasurementParameters(IList<ItemMeasurementParameterDTO> itemMeasurementParameterDtos)
        {
            Assert.AreEqual(61, itemMeasurementParameterDtos.Count);

            var firstItemMeasurementParameter = itemMeasurementParameterDtos[0];
            Assert.AreEqual(0, firstItemMeasurementParameter.MeasurementParameterKey);
            Assert.AreEqual(1, firstItemMeasurementParameter.ParmValue);


            var secondItemMeasurementParameter = itemMeasurementParameterDtos[1];
            Assert.AreEqual(1, secondItemMeasurementParameter.MeasurementParameterKey);
            Assert.AreEqual(1E-15F, secondItemMeasurementParameter.ParmValue);

            var lastItemMeasurementParameter = itemMeasurementParameterDtos.Last();
            Assert.AreEqual(2, lastItemMeasurementParameter.MeasurementParameterKey);
            Assert.AreEqual(0.76237F, lastItemMeasurementParameter.ParmValue);

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }

        private bool EvaluateShouldCreateACollectionOfAdminStimuli(IList<AdminStimulusDTO> adminStimuliDtos)
        {
            Assert.AreEqual(1, adminStimuliDtos.Count);

            var firstStimuli = adminStimuliDtos[0];
            Assert.AreEqual("187-3688", firstStimuli.StimulusKey);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-Perf-MATH-11-2017-2018", firstStimuli.SegmentKey);
            Assert.AreEqual(-1, firstStimuli.NumItemsRequired);
            Assert.AreEqual(-1, firstStimuli.MaxItems);
            Assert.AreEqual(8185L, firstStimuli.TestVersion);
            Assert.AreEqual(8185L, firstStimuli.UpdatedTestVersion);
            Assert.AreEqual("G-187-3688-0", firstStimuli.GroupId);

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }

        private bool EvaluateShouldCreateACollectionOfTestFormItemDtos(IList<TestFormItemDTO> testFormItemDtos)
        {
            Assert.AreEqual(2, testFormItemDtos.Count);

            var firstTestFormItem = testFormItemDtos[0];
            Assert.IsTrue(firstTestFormItem.IsActive);
            Assert.AreEqual(2112L, firstTestFormItem.ITSFormKey);
            Assert.AreEqual(1, firstTestFormItem.FormPosition);
            Assert.AreEqual("187-2112", firstTestFormItem.TestFormKey);
            Assert.AreEqual("187-1434", firstTestFormItem.ItemKey);

            var secondTestFormItem = testFormItemDtos[1];
            Assert.IsTrue(secondTestFormItem.IsActive);
            Assert.AreEqual(2112L, secondTestFormItem.ITSFormKey);
            Assert.AreEqual(2, secondTestFormItem.FormPosition);
            Assert.AreEqual("187-2112", secondTestFormItem.TestFormKey);
            Assert.AreEqual("187-1432", secondTestFormItem.ItemKey);
                
            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }

        private bool EvaluateShouldCreateACollectionOfAffinityGroups(IList<AffinityGroupDTO> affinityGroups)
        {
            // Verify AffinityGroups
            Assert.AreEqual(3, affinityGroups.Count);

            var firstAffinityGroup = affinityGroups.First(ag => ag.GroupId.Equals("G11Math_DOK2", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsNull(firstAffinityGroup.AbilityWeight);
            Assert.AreEqual(2, firstAffinityGroup.MinItems);
            Assert.AreEqual(8, firstAffinityGroup.MaxItems);
            Assert.IsFalse(firstAffinityGroup.IsStrictMax);
            Assert.AreEqual(1D, firstAffinityGroup.BlueprintWeight);
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", firstAffinityGroup.SegmentKey);

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }

        private bool EvaluateShouldCreateACollectionOfAffinityGroupItems(IList<AffinityGroupItemDTO> affinityGroupItems)
        {
            Assert.AreEqual(18, affinityGroupItems.Count);

            var affinityGroupItem = affinityGroupItems.First(agi => agi.ItemKey.Equals("187-2029"));
            Assert.AreEqual("(SBAC_PT)SBAC-IRP-CAT-MATH-11-2017-2018", affinityGroupItem.SegmentKey);
            Assert.AreEqual("G11Math_DOK2", affinityGroupItem.GroupId);

            // If all the Assertions pass, this method passes.  Otherwise, the Assert will throw an
            // exception before this line is hit.
            return true;
        }
    }
}
