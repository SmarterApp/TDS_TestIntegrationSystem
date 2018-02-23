using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class GradeDAOIntegrationTest : TestPackageDaoIntegrationTestBase<GradeDTO>
    {
        private readonly ITestPackageDao<GradeDTO> testPackageDao = new GradeDAO();
        private readonly string sql =
            "SELECT " +
            "   GradeCode, " +
            "   Grade, " +
            "   ClientName " +
            "FROM " +
            "   Client_Grade " +
            "WHERE " +
            "   GradeCode = 'unit-test-gradecode'" +
            "   AND Grade = 'unit-test-grade'" +
            "   AND ClientName = 'unit-test-client'";

        [TestMethod]
        public void ShouldSaveAGradeRecord()
        {
            var gradeList = new List<GradeDTO>
            {
                new GradeDTO
                {
                    GradeCode = "unit-test-gradecode",
                    Grade = "unit-test-grade",
                    ClientName = "unit-test-client"
                }
            };

            testPackageDao.Insert(gradeList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(gradeList[0], insertedRecords[0]);  
        }
    }
}
