using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class TestAdminDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestAdminDTO>
    {
        private const long SBAC_PT_CLIENT_ID = 2L;

        private readonly ITestPackageDao<TestAdminDTO> testPackageDao = new TestAdminDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _Key AS AdminKey, \n" +
            "   SchoolYear, \n" +
            "   Season, \n" +
            "   _fk_client AS ClientKey, \n" +
            "   [Description], \n" +
            "   LoadConfig AS TestVersion \n" +
            "FROM \n" +
            "   tblTestAdmin \n" +
            "WHERE \n" +
            "   _key = 'unit-test-admin-key'";

        [TestMethod]
        public void ShouldSaveATestAdminRecord()
        {
            var adminList = new List<TestAdminDTO>
            {
                new TestAdminDTO
                {
                    AdminKey = "unit-test-admin-key",
                    SchoolYear = "test-year",
                    Season = "season",
                    ClientKey = SBAC_PT_CLIENT_ID,
                    Description = "unit-test-desc",
                    TestVersion = 42L
                }
            };

            testPackageDao.Insert(adminList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(adminList[0], insertedRecords[0]);
        }
    }
}
