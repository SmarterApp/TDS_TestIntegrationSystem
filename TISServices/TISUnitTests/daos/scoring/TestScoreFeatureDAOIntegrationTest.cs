using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.scoring.daos;
using TDSQASystemAPI.DAL.scoring.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.scoring
{
    [TestClass]
    public class TestScoreFeatureDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestScoreFeatureDTO>
    {
        private const string COMPUTATION_RULE_NAME = "ItemCount"; // from the 3_Confguration.sql script, used to create the OSS_TestScoringConfigs database
        private const string CLIENT_NAME = "SBAC_PT"; // from the 3_Confguration.sql script, used to create the OSS_TestScoringConfigs database

        private readonly ITestPackageDao<TestScoreFeatureDTO> testPackageDao = new TestScoreFeatureDAO();
        private readonly ITestPackageDao<TestDTO> seedDataTestDao = new TestDAO();
        private readonly TestDTO seedDataTest = new TestDTO
                {
                    ClientName = CLIENT_NAME,
                    TestId = "unit-test-test-id",
                    Subject = "unit-test-subject"
                };
    private readonly Guid testScoreFeatureKey = new Guid("9b812ce9-feb4-40b2-b7b7-7edbbf23fe40");
        private readonly string sql =
            "SELECT \n" +
            "   _Key AS TestScoreFeatureKey, \n" +
            "   ClientName, \n" +
            "   TestId, \n" +
            "   MeasureOf, \n" +
            "   MeasureLabel, \n" +
            "   IsScaled, \n" +
            "   ComputationRule, \n" +
            "   ComputationOrder \n" +
            "FROM \n" +
            "   TestScoreFeature \n" +
            "WHERE \n" +
            "   _Key = '{0}'" +
            "   AND ComputationRule = '{1}'" +
            "   AND ClientName = '{2}'";

        /// <summary>
        /// Create seed data to satisfy foreign key constraints
        /// </summary>
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            seedDataTestDao.Insert(new List<TestDTO> { seedDataTest });
        }

        [TestMethod]
        public void ShouldSaveATestScoreFeatureRecord()
        {
            var testScoreFeatureList = new List<TestScoreFeatureDTO>
            {
                new TestScoreFeatureDTO
                {
                    TestScoreFeatureKey = testScoreFeatureKey,
                    ClientName = CLIENT_NAME,
                    TestId = seedDataTest.TestId,
                    MeasureOf = "unit-test-measure-of",
                    MeasureLabel = "unit-test-measure-label",
                    IsScaled = true,
                    ComputationRule = COMPUTATION_RULE_NAME,
                    ComputationOrder = 42
                }
            };

            testPackageDao.Insert(testScoreFeatureList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, testScoreFeatureKey, COMPUTATION_RULE_NAME, CLIENT_NAME), DatabaseConnectionStringNames.SCORING);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(testScoreFeatureList[0].TestScoreFeatureKey, result.TestScoreFeatureKey);
            Assert.AreEqual(testScoreFeatureList[0].ClientName, result.ClientName);
            Assert.AreEqual(testScoreFeatureList[0].TestId, result.TestId);
            Assert.AreEqual(testScoreFeatureList[0].MeasureOf, result.MeasureOf);
            Assert.AreEqual(testScoreFeatureList[0].MeasureLabel, result.MeasureLabel);
            Assert.AreEqual(testScoreFeatureList[0].IsScaled, result.IsScaled);
            Assert.AreEqual(testScoreFeatureList[0].ComputationRule, result.ComputationRule);
            Assert.AreEqual(testScoreFeatureList[0].ComputationOrder, result.ComputationOrder);
        }
    }
}
