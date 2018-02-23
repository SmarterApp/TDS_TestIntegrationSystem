using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TDSQASystemAPI.DAL;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    /// <summary>
    /// Summary description for FieldTestPriorityDAOIntegrationTest
    /// </summary>
    [TestClass]
    public class FieldTestPriorityDAOIntegrationTest : TestPackageDaoIntegrationTestBase<FieldTestPriorityDTO>
    {
        private readonly ITestPackageDao<FieldTestPriorityDTO> testPackageDao = new FieldTestPriorityDAO();
        private readonly string sql =
            "SELECT " +
            "   ClientName, " +
            "   TDS_ID AS TdsId, " +
            "   [Priority], " +
            "   TestId " +
            "FROM " +
            "   Client_FieldTestPriority " +
            "WHERE " +
            "   clientname = 'unit-test-client' " +
            "   AND TDS_ID = 'unit-test-tds-id' " +
            "   AND [Priority] = 9 " +
            "   AND TestId = 'unit-test-test-id'";

        [TestMethod]
        public void ShouldSaveAFieldTestPriorityRecord()
        {
            var fieldPriorityList = new List<FieldTestPriorityDTO>
            {
                new FieldTestPriorityDTO
                {
                    ClientName = "unit-test-client",
                    TdsId = "unit-test-tds-id",
                    Priority = 9,
                    TestId = "unit-test-test-id"
                }
            };

            testPackageDao.Insert(fieldPriorityList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(fieldPriorityList[0], insertedRecords[0]);
        }
    }
}
