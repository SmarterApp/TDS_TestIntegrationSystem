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
    public class TimeWindowDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TimeWindowDTO>
    {
        private readonly ITestPackageDao<TimeWindowDTO> testPackageDao = new TimeWindowDAO();
        private readonly string sql =
            "SELECT" +
            "   ClientName, \n" +
            "   WindowId, \n" +
            "   StartDate, \n" +
            "   EndDate \n" +
            "FROM \n" +
            "   Client_TimeWindow \n" +
            "WHERE \n" +
            "   ClientName = 'unit-test-client-name' \n" +
            "   AND WindowId = 'unit-test-window-id'";

        [TestMethod]
        public void ShouldSaveATimeWindowRecord()
        {
            var timeWindowList = new List<TimeWindowDTO>
            {
                new TimeWindowDTO
                {
                    ClientName = "unit-test-client-name",
                    WindowId = "unit-test-window-id",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now
                }
            };

            testPackageDao.Insert(timeWindowList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(timeWindowList[0], insertedRecords[0]);
        }
    }
}
