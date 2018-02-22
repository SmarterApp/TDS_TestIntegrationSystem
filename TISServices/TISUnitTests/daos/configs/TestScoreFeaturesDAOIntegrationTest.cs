using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.dtos;
using TDSQASystemAPI.DAL.configs.daos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class TestScoreFeaturesDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestScoreFeatureDTO>
    {
        private readonly ITestPackageDao<TestScoreFeatureDTO> testPackageDao = new TestScoreFeaturesDAO();
        private readonly string sql =
            "SELECT \n" +
            "   ClientName, \n" +
            "   TestId, \n" +
            "   MeasureOf, \n" +
            "   MeasureLabel, \n" +
            "   UseForAbility \n" +
            "FROM \n" +
            "   Client_TestScoreFeatures \n" +
            "WHERE \n" +
            "   ClientName = 'unit-test-client-name' \n" +
            "   AND TestId = 'unit-test-test-id' \n" +
            "   AND MeasureOf = 'unit-test-measure-of' \n" +
            "   AND MeasureLabel = 'unit-test-measure-label'";

        [TestMethod]
        public void ShouldSaveATestScoreFeaturesRecord()
        {
            var testScoreFeaturesList = new List<TestScoreFeatureDTO>
            {
                new TestScoreFeatureDTO
                {
                    ClientName = "unit-test-client-name",
                    TestId = "unit-test-test-id",
                    MeasureOf = "unit-test-measure-of",
                    MeasureLabel = "unit-test-measure-label",
                    UseForAbility = true
                }
            };

            testPackageDao.Insert(testScoreFeaturesList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(testScoreFeaturesList[0].ClientName, result.ClientName);
            Assert.AreEqual(testScoreFeaturesList[0].TestId, result.TestId);
            Assert.AreEqual(testScoreFeaturesList[0].MeasureOf, result.MeasureOf);
            Assert.AreEqual(testScoreFeaturesList[0].MeasureLabel, result.MeasureLabel);
            Assert.AreEqual(testScoreFeaturesList[0].UseForAbility, result.UseForAbility);
        }
    }
}
