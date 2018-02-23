using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class TesItemConstraintDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestItemConstraintDTO>
    {
        private readonly ITestPackageDao<TestItemConstraintDTO> testPackageDao = new TestItemConstraintDAO();
        private readonly string sql =
            "SELECT \n" +
            "   ClientName, \n" +
            "   TestId, \n" +
            "   PropName, \n" +
            "   PropValue, \n" +
            "   ToolType, \n" +
            "   ToolValue, \n" +
            "   item_in AS ItemIn \n" +
            "FROM \n" +
            "   Client_Test_ItemConstraint \n" +
            "WHERE \n" +
            "   ClientName = 'unit-test-client-name' \n" +
            "   AND TestId = 'unit-test-test-id' \n" +
            "   AND PropName = 'unit-test-prop-name' \n" +
            "   AND PropValue = 'unit-test-prop-value'";

        [TestMethod]
        public void ShouldSaveATestItemConstraintRecord()
        {
            var testItemConstraintList = new List<TestItemConstraintDTO>
            {
                new TestItemConstraintDTO
                {
                    ClientName = "unit-test-client-name",
                    TestId = "unit-test-test-id",
                    PropName = "unit-test-prop-name",
                    PropValue = "unit-test-prop-value",
                    ToolType = "unit-test-tool-type",
                    ToolValue = "unit-test-tool-value",
                    ItemIn = true
                }
            };

            testPackageDao.Insert(testItemConstraintList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(testItemConstraintList[0], insertedRecords[0]);
        }
    }
}
