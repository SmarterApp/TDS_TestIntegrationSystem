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
        private readonly ITestPackageDao<TimeWindowDTO> timeWindowDao = new TimeWindowDAO();
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
        private IList<TimeWindowDTO> timeWindowList = new List<TimeWindowDTO>
            {
                new TimeWindowDTO
                {
                    ClientName = "unit-test-client-name",
                    WindowId = "unit-test-window-id",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now
                }
            };

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            timeWindowDao.Insert(timeWindowList);
        }

        [TestMethod]
        public void ShouldSaveATestWindowRecord()
        {
            var testWindowList = new List<TestWindowDTO>
            {
                new TestWindowDTO
                {
                    ClientName = timeWindowList[0].ClientName,
                    TestId = "unit-test-test-id",
                    WindowID = timeWindowList[0].WindowId,
                    NumOpps = 42,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now
                }
            };

            testPackageDao.Insert(testWindowList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(testWindowList[0].ClientName, result.ClientName);
            Assert.AreEqual(testWindowList[0].TestId, result.TestId);
            Assert.AreEqual(testWindowList[0].WindowID, result.WindowID);
            Assert.AreEqual(testWindowList[0].NumOpps, result.NumOpps);
            Assert.AreEqual(testWindowList[0].StartDate.ToLongDateString(), result.StartDate.ToLongDateString());
            Assert.AreEqual(testWindowList[0].EndDate.ToLongDateString(), result.EndDate.ToLongDateString());
        }
    }
}
