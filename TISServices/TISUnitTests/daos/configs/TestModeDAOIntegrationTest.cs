using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class TestModeDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestModeDTO>
    {
        private readonly ITestPackageDao<TestModeDTO> testPackageDao = new TestModeDAO();
        private readonly string sql =
            "SELECT \n" +
            "   ClientName, \n" +
            "   TestId, \n" +
            "   TestKey, \n" +
            "   Mode, \n" +
            "   Algorithm, \n" +
            "   IsSegmented, \n" +
            "   SessionType \n" +
            "FROM \n" +
            "   Client_TestMode \n" +
            "WHERE \n" +
            "   ClientName = 'unit-test-client-name-online' \n" +
            "   AND TestId = 'unit-test-test-id-online'";

        [TestMethod]
        public void ShouldSaveATestModeRecord()
        {
            var testModeList = new List<TestModeDTO>
            {
                new TestModeDTO
                {
                    ClientName = "unit-test-client-name-online",
                    TestId = "unit-test-test-id-online",
                    TestKey = "unit-test-test-key-online",
                    Mode = "unit-test",
                    Algorithm = "unit-test-algorithm",
                    IsSegmented = true,
                    SessionType = 42
                }
            };

            testPackageDao.Insert(testModeList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(testModeList[0].ClientName, result.ClientName);
            Assert.AreEqual(testModeList[0].TestId, result.TestId);
            Assert.AreEqual(testModeList[0].TestKey, result.TestKey);
            Assert.AreEqual(testModeList[0].Algorithm, result.Algorithm);
            Assert.AreEqual(testModeList[0].IsSegmented, result.IsSegmented);
            Assert.AreEqual(testModeList[0].Mode, result.Mode);
            Assert.AreEqual(testModeList[0].SessionType, result.SessionType);
        }
    }
}
