using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class TestCohortDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestCohortDTO>
    {
        private readonly ITestPackageDao<TestCohortDTO> testPackageDao = new TestCohortDAO();
        private readonly string sql =
            "SELECT \n" +
                "   _fk_adminsubject AS SegmentKey, \n" +
                "   Cohort, \n" +
                "   ItemRatio \n" +
            "FROM \n" +
            "   TestCohort \n" +
            "WHERE \n" +
            "   _fk_adminsubject = 'unit-test-admin-subject' \n" +
            "   AND Cohort = 42";

        [TestMethod]
        public void ShouldSaveATestCohortRecord()
        {
            var cohortList = new List<TestCohortDTO>
            {
                new TestCohortDTO
                {
                    SegmentKey = "unit-test-admin-subject",
                    Cohort = 42,
                    ItemRatio = 13.0
                }
            };

            testPackageDao.Insert(cohortList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);
            CompareResults(cohortList[0], insertedRecords[0]);
        }
    }
}
