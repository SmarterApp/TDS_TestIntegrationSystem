using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.scoring.daos;
using TDSQASystemAPI.DAL.scoring.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.scoring
{
    [TestClass]
    public class ConversionTableDAOIntegrationTest : TestPackageDaoIntegrationTestBase<ConversionTableDTO>
    {
        private const string CLIENT_NAME = "SBAC_PT"; // from the 3_Confguration.sql script, used to create the OSS_TestScoringConfigs database

        private readonly ITestPackageDao<ConversionTableDTO> testPackageDao = new ConversionTableDAO();
        private readonly string sql =
            "SELECT \n" +
            "   TableName, \n" +
            "   InValue, \n" +
            "   OutValue, \n" +
            "   ClientName \n" +
            "FROM \n" +
            "   ConversionTables \n" +
            "WHERE \n" +
            "   ClientName = '{0}'" +
            "   AND InValue = {1}";

        [TestMethod]
        public void ShouldSaveAConversionTablesRecord()
        {
            var tableList = new List<ConversionTableDTO>
            {
                new ConversionTableDTO
                {
                    TableName = "unit-test-table-name",
                    InValue = 42,
                    OutValue = 13d,
                    ClientName = CLIENT_NAME
                }
            };

            testPackageDao.Insert(tableList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, CLIENT_NAME, tableList[0].InValue), DatabaseConnectionStringNames.SCORING);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(tableList[0].TableName, result.TableName);
            Assert.AreEqual(tableList[0].InValue, result.InValue);
            Assert.AreEqual(tableList[0].OutValue, result.OutValue);
            Assert.AreEqual(tableList[0].ClientName, result.ClientName);
        }
    }
}
