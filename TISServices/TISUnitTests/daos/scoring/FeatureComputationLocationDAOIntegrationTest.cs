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
    public class FeatureComputationLocationDAOIntegrationTest : TestPackageDaoIntegrationTestBase<FeatureComputationLocationDTO>
    {
        private const string CLIENT_NAME = "SBAC_PT"; // from the 3_Confguration.sql script, used to create the OSS_TestScoringConfigs database
        private const string COMPUTATION_RULE_NAME = "ItemCount"; // from the 3_Confguration.sql script, used to create the OSS_TestScoringConfigs database
        private const string COMPUTATION_LOCATION_NAME = "TIS"; // from the 3_Confguration.sql script, used to create the OSS_TestScoringConfigs database

        private readonly ITestPackageDao<FeatureComputationLocationDTO> testPackageDao = new FeatureComputationLocationDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _fk_TestScoreFeature AS TestScoreFeatureKey, \n" +
            "   [Location] \n" +
            "FROM \n" +
            "   Feature_ComputationLocation \n" +
            "WHERE \n" +
            "   _fk_TestScoreFeature = '{0}'" +
            "   AND [Location] = '{1}'";

        private readonly ITestPackageDao<TestDTO> seedDataTestDao = new TestDAO();
        private readonly TestDTO seedDataTest = new TestDTO
        {
            ClientName = CLIENT_NAME,
            TestId = "unit-test-test-id",
            Subject = "unit-test-subject"
        };
        private readonly ITestPackageDao<TestScoreFeatureDTO> seedDataTestScoreFeatureDAO = new TestScoreFeatureDAO();
        private readonly Guid seedDataTestScoreFeatureKey = new Guid("cccd69f1-fd02-46fb-9e1e-45e0c6978ad4");

        /// <summary>
        /// Create seed data to satisfy foreign key constraints
        /// </summary>
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            seedDataTestDao.Insert(new List<TestDTO> { seedDataTest });

            seedDataTestScoreFeatureDAO.Insert(new List<TestScoreFeatureDTO>
            {
                new TestScoreFeatureDTO
                {
                    TestScoreFeatureKey = seedDataTestScoreFeatureKey,
                    ClientName = CLIENT_NAME,
                    TestId = seedDataTest.TestId,
                    MeasureOf = "unit-test-measure-of",
                    MeasureLabel = "unit-test-measure-label",
                    IsScaled = true,
                    ComputationRule = COMPUTATION_RULE_NAME,
                    ComputationOrder = 42
                }
            });
        }

        [TestMethod]
        public void ShouldSaveAFeatureComputationLocationRecord()
        {
            var locationList = new List<FeatureComputationLocationDTO>
            {
                new FeatureComputationLocationDTO
                {
                    TestScoreFeatureKey = seedDataTestScoreFeatureKey,
                    Location = COMPUTATION_LOCATION_NAME
                }
            };

            testPackageDao.Insert(locationList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, seedDataTestScoreFeatureKey, COMPUTATION_LOCATION_NAME), DatabaseConnectionStringNames.SCORING);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(locationList[0].TestScoreFeatureKey, result.TestScoreFeatureKey);
            Assert.AreEqual(locationList[0].Location, result.Location);

        }
    }
}
