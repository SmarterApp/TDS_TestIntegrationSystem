using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TDSQASystemAPI.BL.testpackage.scoring;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.DAL.scoring.dtos;
using TDSQASystemAPI.TestPackage;
using TDSQASystemAPI.TestPackage.utils;

namespace TISUnitTests.services
{
    [TestClass]
    public class ScoringConfigurationDataSeriveUnitTest
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\(SBAC_PT)SBAC-IRP-MATH-11-COMBINED-Summer-2015-2016-NEW.xml";
        private readonly Mock<ITestPackageDao<TestDTO>> mockTestDAO = new Mock<ITestPackageDao<TestDTO>>();
        private readonly Mock<ITestPackageDao<TestGradeDTO>> mockTestGradeDAO = new Mock<ITestPackageDao<TestGradeDTO>>();
        private readonly Mock<ITestPackageDao<TestScoreFeatureDTO>> mockTestScoreFeatureDAO = new Mock<ITestPackageDao<TestScoreFeatureDTO>>();
        private readonly Mock<ITestPackageDao<FeatureComputationLocationDTO>> mockFeatureComputationLocationDAO = new Mock<ITestPackageDao<FeatureComputationLocationDTO>>();
        private readonly Mock<ITestPackageDao<ComputationRuleParameterDTO>> mockComputationRuleParameterDAO = new Mock<ITestPackageDao<ComputationRuleParameterDTO>>();
        private readonly Mock<ITestPackageDao<ComputationRuleParameterValueDTO>> mockComputationRuleParameterRuleDAO = new Mock<ITestPackageDao<ComputationRuleParameterValueDTO>>();
        private readonly Mock<ITestPackageDao<PerformanceLevelDTO>> mockPerformanceLevelDAO = new Mock<ITestPackageDao<PerformanceLevelDTO>>();

        private ScoringConfigurationDataService scoringConfigurationDataService;
        private TestPackage testPackage;
        private string testKey;

        [TestInitialize]
        public void Setup()
        {
            testPackage = TestPackageMapper.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));
            testKey = ScoringConfigurationDataService.TestKey(testPackage);

            scoringConfigurationDataService =
                new ScoringConfigurationDataService(mockTestDAO.Object,
                    mockTestGradeDAO.Object,
                    mockTestScoreFeatureDAO.Object,
                    mockFeatureComputationLocationDAO.Object,
                    mockComputationRuleParameterDAO.Object,
                    mockComputationRuleParameterRuleDAO.Object,
                    mockPerformanceLevelDAO.Object);
        }

        [TestMethod]
        public void Test_ShouldCreateNewTest()
        {
            mockTestDAO.Setup(dao => dao.Exists(It.IsAny<TestDTO>()))
                .Returns(false);
            mockTestDAO.Setup(dao => dao.Insert(It.IsAny<IList<TestDTO>>()))
                .Verifiable();

            scoringConfigurationDataService.CreateTest(testPackage);

            mockTestDAO.Verify(dao => dao.Exists(It.IsAny<TestDTO>()));
            mockTestDAO.Verify(dao => dao.Insert(It.Is<TestDTO>(test =>
                    test.ClientName.Equals(testPackage.publisher)
                    && test.TestId.Equals(testKey)
                    && test.Subject.Equals(testPackage.subject))));
        }

        [TestMethod]
        public void Grade_ShouldCreateNewGrade()
        {
            mockTestGradeDAO.Setup(dao => dao.Exists(It.IsAny<TestGradeDTO>()))
                .Returns(false);
            mockTestGradeDAO.Setup(dao => dao.Insert(It.IsAny<IList<TestGradeDTO>>()))
                .Verifiable();

            scoringConfigurationDataService.CreateGrade(testPackage);
            mockTestGradeDAO.Verify(dao => dao.Exists(It.IsAny<TestGradeDTO>()));
            mockTestGradeDAO.Verify(dao => dao.Insert(It.Is<IList<TestGradeDTO>>(test =>
                    test.Count == 1 &&
                    test[0].ClientName.Equals(testPackage.publisher)
                    && test[0].TestId.Equals(testKey)
                    && test[0].ReportingGrade.Equals("11"))));
        }

        [TestMethod]
        public void ScoreFeature_ShouldCreateNewScoreFeature()
        {
            mockTestScoreFeatureDAO.Setup(dao => dao.Exists(It.IsAny<TestScoreFeatureDTO>()))
                .Returns(false);
            mockTestScoreFeatureDAO.Setup(dao => dao.Insert(It.IsAny<IList<TestScoreFeatureDTO>>()))
                .Verifiable();

            scoringConfigurationDataService.CreateScoreFeature(testPackage);

            mockTestScoreFeatureDAO.Verify(dao => dao.Exists(It.IsAny<TestScoreFeatureDTO>()));
            mockTestScoreFeatureDAO.Verify(dao => dao.Insert(It.Is<IList<TestScoreFeatureDTO>>(test =>
                test.Count == 25 &&
                test[0].ClientName.Equals(testPackage.publisher) &&
                test[0].TestId.Equals(testKey)
            )));
        }

        [TestMethod]
        public void ComputationRule_ShouldCreateNewComputationRule()
        {
            mockTestScoreFeatureDAO.Setup(dao => dao.Exists(It.IsAny<TestScoreFeatureDTO>()))
                .Returns(false);
            mockTestScoreFeatureDAO.Setup(dao => dao.Insert(It.IsAny<IList<TestScoreFeatureDTO>>()))
                .Verifiable();

            scoringConfigurationDataService.CreateScoreFeature(testPackage);

            mockTestScoreFeatureDAO.Verify(dao => dao.Exists(It.IsAny<TestScoreFeatureDTO>()));
            mockTestScoreFeatureDAO.Verify(dao => dao.Insert(It.Is<IList<TestScoreFeatureDTO>>(test =>
                test.Count == 25 &&
                test[0].ClientName.Equals(testPackage.publisher) &&
                test[0].TestId.Equals(testKey)
            )));
        }

    }
}