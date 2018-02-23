using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.scoring.daos;
using TDSQASystemAPI.DAL.scoring.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.scoring
{
    [TestClass]
    public class ConversionTableDescDAOIntegrationTest : TestPackageDaoIntegrationTestBase<ConversionTableDescDTO>
    {
        private const string CLIENT_NAME = "SBAC_PT"; // from the 3_Confguration.sql script, used to create the OSS_TestScoringConfigs database

        private readonly ITestPackageDao<ConversionTableDescDTO> testPackageDao = new ConversionTableDescDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _Key AS ConversionTableDescKey, \n" +
            "   TableName, \n" +
            "   _fk_client AS ClientName \n" +
            "FROM \n" +
            "   ConversionTableDesc \n" +
            "WHERE \n" +
            "   _Key = 'unit-test-key' \n" +
            "   AND TableName = 'unit-test-table-name' \n" +
            "   AND _fk_client = '{0}'";

        [TestMethod]
        public void ShouldSaveAConversionTableDescRecord()
        {
            var tableList = new List<ConversionTableDescDTO>
            {
                new ConversionTableDescDTO
                {
                    ConversionTableDescKey = "unit-test-key",
                    TableName = "unit-test-table-name",
                    ClientName = CLIENT_NAME
                }
            };

            testPackageDao.Insert(tableList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, CLIENT_NAME), DatabaseConnectionStringNames.SCORING);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(tableList[0], insertedRecords[0]);
        }
    }
}
