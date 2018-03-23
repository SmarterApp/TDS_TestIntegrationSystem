using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.osstis.daos;
using TDSQASystemAPI.DAL.osstis.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.osstis
{
    [TestClass]
    public class CombinationTestMapDAOIntegrationTest : TestPackageDaoIntegrationTestBase<CombinationTestMapDTO>
    {
        private readonly ITestPackageDao<CombinationTestMapDTO> testPackageDao = new CombinationTestMapDAO();
        private readonly string sql =
            "SELECT \n" +
            "   ComponentTestName, \n" +
            "   ComponentSegmentName, \n" +
            "   CombinationTestName, \n" +
            "   CombinationSegmentName \n" +
            "FROM \n" +
            "   CombinationTestMap \n" +
            "WHERE \n" +
            "   ComponentTestName = 'unit-test-component-test-name' \n" +
            "   AND ComponentSegmentName = 'unit-test-component-segment-name' \n" +
            "   AND CombinationTestName = 'unit-test-combination-test-name' \n" +
            "   AND CombinationSegmentName = 'unit-test-combination-segment-name'";

        [TestMethod]
        public void ShouldSaveACombinationTestMapRecord()
        {
            var combinationTestMapRecord = new CombinationTestMapDTO
            {
                ComponentTestName = "unit-test-component-test-name",
                ComponentSegmentName = "unit-test-component-segment-name",
                CombinationTestName = "unit-test-combination-test-name",
                CombinationSegmentName = "unit-test-combination-segment-name"
            };

            testPackageDao.Insert(new List<CombinationTestMapDTO> { combinationTestMapRecord });

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.OSS_TIS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(combinationTestMapRecord, insertedRecords[0]);
        }
    }
}
