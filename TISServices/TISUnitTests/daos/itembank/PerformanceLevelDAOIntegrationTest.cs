using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class PerformanceLevelDAOIntegrationTest : TestPackageDaoIntegrationTestBase<PerformanceLevelDTO>
    {
        private readonly ITestPackageDao<PerformanceLevelDTO> testPackageDao = new PerformanceLevelDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _fk_content AS ContentKey, \n" +
            "   PLevel, \n" +
            "   ThetaLo, \n" +
            "   ThetaHi, \n" +
            "   ScaledLo, \n" +
            "   ScaledHi \n" +
            "FROM \n" +
            "   PerformanceLevels \n" +
            "WHERE \n" +
            "   _fk_content = 'unit-test-content' \n" +
            "   AND plevel = 42";

        [TestMethod]
        public void ShouldSaveAPerformanceLevelRecord()
        {
            var perfLvlList = new List<PerformanceLevelDTO>
            {
                new PerformanceLevelDTO
                {
                    ContentKey = "unit-test-content",
                    PLevel = 42,
                    ThetaLo = 4D,
                    ThetaHi = 6D,
                    ScaledLo = 8D,
                    ScaledHi = 10D
                }
            };

            testPackageDao.Insert(perfLvlList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(perfLvlList[0], insertedRecords[0]);
        }
    }
}
