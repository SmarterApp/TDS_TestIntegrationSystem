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

namespace TISUnitTests.services
{
    [TestClass]
    public class ItembankAdministrationDataServiceUnitTest
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml";
        private const string MULTI_SEGMENT_TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-MULTI-SEGMENT-EXAMPLE.xml";

        private Mock<ITestPackageDao<TestAdminDTO>> mockTestAdminDao = new Mock<ITestPackageDao<TestAdminDTO>>();
        private Mock<ITestPackageDao<SetOfAdminSubjectDTO>> mockSetOfAdminDao = new Mock<ITestPackageDao<SetOfAdminSubjectDTO>>();
        private Mock<IItembankConfigurationDataQueryService> mockItembankConfigurationDataQueryService = new Mock<IItembankConfigurationDataQueryService>();

        private IItembankAdministrationDataService itembankAdministrationDataService;

        [TestInitialize]
        public void Setup()
        {
            itembankAdministrationDataService =
                new ItembankAdministrationDataService(mockItembankConfigurationDataQueryService.Object,
                    mockTestAdminDao.Object,
                    mockSetOfAdminDao.Object);
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

            mockSetOfAdminDao.Setup(dao => dao.Insert(It.IsAny<List<SetOfAdminSubjectDTO>>()))
                .Verifiable();

            itembankAdministrationDataService.CreateSetOfAdminSubjects(testPackage);

            mockSetOfAdminDao.Verify(dao => dao.Insert(It.Is<IList<SetOfAdminSubjectDTO>>(result => 
                EvaluateShouldInsertSetOfAdminSubjectRecordsForAllSegments(result.ToList()))));
        }

        [TestMethod]
        public void SetOfAdminSubject_ShouldCreateSetOfAdminRecordsForAMultiSegmentedAssessment()
        {
            // This test package has one assessment with tw segments
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(MULTI_SEGMENT_TEST_PACKAGE_XML_FILE));

            mockSetOfAdminDao.Setup(dao => dao.Insert(It.IsAny<List<SetOfAdminSubjectDTO>>()))
                .Verifiable();

            itembankAdministrationDataService.CreateSetOfAdminSubjects(testPackage);

            mockSetOfAdminDao.Verify(dao => dao.Insert(It.Is<List<SetOfAdminSubjectDTO>>(result =>
                EvaluateShouldCreateSetOfAdminRecordsForAMultiSegmentedAssessment(result))));
        }

        private bool EvaluateShouldInsertSetOfAdminSubjectRecordsForAllSegments(List<SetOfAdminSubjectDTO> setOfAdminSubjectDtos)
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

        private bool EvaluateShouldCreateSetOfAdminRecordsForAMultiSegmentedAssessment(List<SetOfAdminSubjectDTO> setOfAdminSubjectDtos)
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
    }
}
