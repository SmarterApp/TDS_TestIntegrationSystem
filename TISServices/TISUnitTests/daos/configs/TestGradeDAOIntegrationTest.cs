using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.dtos;
using TDSQASystemAPI.DAL.configs.daos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class TestGradeDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestGradeDTO>
    {
        private readonly ITestPackageDao<TestGradeDTO> testPackageDao = new TestGradesDAO();
        private readonly string sql = 
            "SELECT " +
            "   ClientName, \n" +
            "   TestId, \n" +
            "   Grade \n" +
            "FROM " +
            "   Client_TestGrades " +
            "WHERE " +
            "   ClientName = 'unit-test-name' " +
            "   AND TestId = 'unit-test-test-id' " +
            "   AND Grade = 'unit-test-grade'";

        [TestMethod]
        public void ShouldSaveATestGradeRecord()
        {
            var testGradeList = new List<TestGradeDTO>
            {
                new TestGradeDTO
                {
                    ClientName = "unit-test-name",
                    TestId = "unit-test-test-id",
                    Grade = "unit-test-grade"
                }
            };

            testPackageDao.Insert(testGradeList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(testGradeList[0], insertedRecords[0]);
        }
    }
}
