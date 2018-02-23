using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class TestWindowDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestWindowDTO>
    {
        private readonly ITestPackageDao<TestWindowDTO> testPackageDao = new TestWindowDAO();
        private readonly string sql =
            "SELECT \n" +
            "   ClientName, \n" +
            "   TestId, \n" +
            "   WindowId, \n" +
            "   NumOpps, \n" +
            "   StartDate, \n" +
            "   EndDate \n" +
            "FROM \n" +
            "   Client_TestWindow \n" +
            "WHERE \n" +
            "   ClientName = 'unit-test-client-name'";
        private readonly TimeWindowDTO timeWindowSeedData = new TimeWindowDTO
        {
            ClientName = "unit-test-client-name",
            WindowId = "unit-test-window-id",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now
        };

        /// <summary>
        /// Create seed data to satisfy foreign key constraints
        /// </summary>
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            new TimeWindowDAO().Insert(new List<TimeWindowDTO> { timeWindowSeedData });
        }

        [TestMethod]
        public void ShouldSaveATestWindowRecord()
        {
            var testWindowList = new List<TestWindowDTO>
            {
                new TestWindowDTO
                {
                    ClientName = timeWindowSeedData.ClientName,
                    TestId = "unit-test-test-id",
                    WindowID = timeWindowSeedData.WindowId,
                    NumOpps = 42,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now
                }
            };

            testPackageDao.Insert(testWindowList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            CompareResults(testWindowList[0], insertedRecords[0]);
        }
    }
}
