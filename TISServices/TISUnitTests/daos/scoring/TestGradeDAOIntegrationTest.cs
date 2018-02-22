using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.scoring.daos;
using TDSQASystemAPI.DAL.scoring.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.scoring
{
    [TestClass]
    public class TestGradeDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestGradeDTO>
    {
        private const string CLIENT_NAME = "SBAC_PT"; // from the 3_Confguration.sql script, used to create the OSS_TestScoringConfigs database

        private readonly ITestPackageDao<TestGradeDTO> testPackageDao = new TestGradeDAO();
        private readonly ITestPackageDao<TestDTO> seedDataTestDao = new TestDAO();
        private readonly TestDTO seedDataTest = new TestDTO
        {
            ClientName = CLIENT_NAME,
            TestId = "unit-test-test-id",
            Subject = "unit-test-subject"
        };
        private readonly string sql =
            "SELECT \n" +
            "   ClientName, \n" +
            "   TestId, \n" +
            "   ReportingGrade \n" +
            "FROM \n" +
            "   TestGrades \n" +
            "WHERE \n" +
            "   ClientName = '{0}' \n" +
            "   AND TestId = '{1}' \n" +
            "   AND ReportingGrade = '{2}'";

        /// <summary>
        /// Create seed data to satisfy foreign key constraints
        /// </summary>
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            seedDataTestDao.Insert(new List<TestDTO> { seedDataTest });
        }

        [TestMethod]
        public void ShouldSaveATestGradeRecord()
        {
            var gradeList = new List<TestGradeDTO>
            {
                new TestGradeDTO
                {
                    ClientName = CLIENT_NAME,
                    TestId = seedDataTest.TestId,
                    ReportingGrade = "unit-test"
                }
            };

            testPackageDao.Insert(gradeList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, CLIENT_NAME, seedDataTest.TestId, "unit-test"), DatabaseConnectionStringNames.SCORING);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(gradeList[0].ClientName, result.ClientName);
            Assert.AreEqual(gradeList[0].TestId, result.TestId);
            Assert.AreEqual(gradeList[0].ReportingGrade, result.ReportingGrade);
        }
    }
}
