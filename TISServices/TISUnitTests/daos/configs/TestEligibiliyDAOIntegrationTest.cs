using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class TestEligibiliyDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestEligibilityDTO>
    {
        private readonly ITestPackageDao<TestEligibilityDTO> testPackageDao = new TestEligibilityDAO();
        private readonly string sql =
            "SELECT " +
            "   ClientName, " +
            "   TestId, " +
            "   RtsName, " +
            "   Enables, " +
            "   Disables, " +
            "   RtsValue, " +
            "   _efk_entityType AS EntityType, " +
            "   EligibilityType, " +
            "   MatchType " +
            "FROM " +
            "   Client_TestEligibility " +
            "WHERE" +
            "   ClientName = 'unit-test-client-name' " +
            "   AND TestId = 'unit-test-test-id'";

        [TestMethod]
        public void ShouldSaveATestEligibilityRecord()
        {
            var testEligibilityList = new List<TestEligibilityDTO>
            {
                new TestEligibilityDTO
                {
                    ClientName = "unit-test-client-name",
                    TestId = "unit-test-test-id",
                    RtsName = "unit-test-rts-name",
                    Enables = true,
                    Disables = true,
                    RtsValue = "unit-test-rts-value",
                    EntityType = 99L,
                    EligibilityType = "unit-test-eligibility-type",
                    MatchType = 42
                }
            };

            testPackageDao.Insert(testEligibilityList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(testEligibilityList[0], insertedRecords[0]);
        }
    }
}
