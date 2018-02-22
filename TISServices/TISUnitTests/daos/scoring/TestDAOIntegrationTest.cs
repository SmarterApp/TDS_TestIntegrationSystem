using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.scoring.daos;
using TDSQASystemAPI.DAL.scoring.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.scoring
{
    [TestClass]
    public class TestDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestDTO>
    {
        private readonly ITestPackageDao<TestDTO> testPackageDao = new TestDAO();
        private readonly string sql =
            "SELECT \n" +
            "   ClientName, \n" +
            "   TestId, \n" +
            "   _efk_subject AS [Subject] \n" +
            "FROM \n" +
            "   Test \n" +
            "WHERE \n" +
            "   ClientName = 'unit-test-client-name' \n" +
            "   AND TestId = 'unit-test-test-id'";

        [TestMethod]
        public void ShouldSaveATestRecord()
        {
            var testList = new List<TestDTO>
            {
                new TestDTO
                {
                    ClientName = "unit-test-client-name",
                    TestId = "unit-test-test-id",
                    Subject = "unit-test-subject"
                }
            };

            testPackageDao.Insert(testList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.SCORING);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(testList[0].ClientName, result.ClientName);
            Assert.AreEqual(testList[0].TestId, result.TestId);
            Assert.AreEqual(testList[0].Subject, result.Subject);
        }
    }
}
