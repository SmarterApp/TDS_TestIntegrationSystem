using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.osstis.daos;
using TDSQASystemAPI.DAL.osstis.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.osstis
{
    [TestClass]
    public class TestNameLookupDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestNameLookupDTO>
    {
        private readonly ITestPackageDao<TestNameLookupDTO> testPackageDao = new TestNameLookupDAO();
        private readonly string sql =
            "SELECT \n" +
            "   InstanceName, \n" +
            "   TestName \n" +
            "FROM \n" +
            "   TestNameLookup \n" +
            "WHERE \n" +
            "   TestName = 'unit-test-test-name'";

        [TestMethod]
        public void ShouldSaveATestNameLookupRecord()
        {
            var testNameLookupList = new List<TestNameLookupDTO>
            {
                new TestNameLookupDTO
                {
                    InstanceName = "unit-test-instance-name",
                    TestName = "unit-test-test-name"
                }
            };

            testPackageDao.Insert(testNameLookupList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.OSS_TIS);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(testNameLookupList[0].InstanceName, result.InstanceName);
            Assert.AreEqual(testNameLookupList[0].TestName, result.TestName);
        }
    }
}