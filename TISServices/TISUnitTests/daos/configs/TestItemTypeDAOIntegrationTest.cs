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
    public class TestItemTypeDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestItemTypeDTO>
    {
        private readonly ITestPackageDao<TestItemTypeDTO> testPackageDao = new TestItemTypeDAO();
        private readonly string sql =
            "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   ItemType \n" +
            "FROM \n" +
            "   Client_Test_ItemTypes \n" +
            "WHERE \n" +
            "   ClientName = 'unit-test-client-name' \n" +
            "   AND TestId = 'unit-test-test-id' \n" +
            "   AND ItemType = 'unit-test-item-type'";

        [TestMethod]
        public void ShouldSaveATestItemTypeRecord()
        {
            var testItemTypeList = new List<TestItemTypeDTO>
            {
                new TestItemTypeDTO
                {
                    ClientName = "unit-test-client-name",
                    TestId = "unit-test-test-id",
                    ItemType = "unit-test-item-type"
                }
            };

            testPackageDao.Insert(testItemTypeList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(testItemTypeList[0].ClientName, result.ClientName);
            Assert.AreEqual(testItemTypeList[0].TestId, result.TestId);
            Assert.AreEqual(testItemTypeList[0].ItemType, result.ItemType);
        }
    }
}
