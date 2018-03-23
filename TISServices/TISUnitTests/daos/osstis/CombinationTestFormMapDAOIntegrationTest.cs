using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.osstis.daos;
using TDSQASystemAPI.DAL.osstis.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.osstis
{
    [TestClass]
    public class CombinationTestFormMapDAOIntegrationTest : TestPackageDaoIntegrationTestBase<CombinationTestFormMapDTO>
    {
        private readonly ITestPackageDao<CombinationTestFormMapDTO> testPackageDao = new CombinationTestFormMapDAO();
        private readonly string sql =
            "SELECT \n" +
            "   ComponentSegmentName, \n" +
            "   ComponentFormKey, \n" +
            "   CombinationFormKey \n" +
            "FROM \n" +
            "   CombinationTestFormMap \n" +
            "WHERE \n" +
            "   ComponentSegmentName = 'unit-test-component-segment-name' \n" +
            "   AND ComponentFormKey = 'unit-test-component-form-key' \n" +
            "   AND CombinationFormKey = 'unit-test-combination-form-key'";

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            // Create seed data
            var combinationTestMapDao = new CombinationTestMapDAO();
            var combinationTestMapRecord = new CombinationTestMapDTO
            {
                ComponentTestName = "unit-test-component-test-name",
                ComponentSegmentName = "unit-test-component-segment-name",
                CombinationTestName = "unit-test-combination-test-name",
                CombinationSegmentName = "unit-test-combination-segment-name"
            };

            combinationTestMapDao.Insert(new List<CombinationTestMapDTO> { combinationTestMapRecord });
        }

        [TestMethod]
        public void ShouldCreateACombinationTestFormMapRecord()
        {
            var combinationTestFormMap = new CombinationTestFormMapDTO
            {
                ComponentSegmentName = "unit-test-component-segment-name",
                ComponentFormKey = "unit-test-component-form-key",
                CombinationFormKey = "unit-test-combination-form-key"
            };

            testPackageDao.Insert(new List<CombinationTestFormMapDTO> { combinationTestFormMap });

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.OSS_TIS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(combinationTestFormMap, insertedRecords[0]);
        }
    }
}
