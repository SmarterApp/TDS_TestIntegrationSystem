using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class TestFormPropertyDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestFormPropertiesDTO>
    {
        private readonly ITestPackageDao<TestFormPropertiesDTO> testPackageDao = new TestFormPropertiesDAO();
        private readonly string sql =
            "SELECT " +
            "   ClientName, " +
            "   _efk_TestForm AS TestFormKey, " +
            "   FormId, " +
            "   TestId, " +
            "   [Language], " +
            "   StartDate, " +
            "   EndDate, " +
            "   TestKey " +
            "FROM " +
            "   Client_TestFormProperties " +
            "WHERE " +
            "   ClientName = 'unit-test-client-name' " +
            "   AND _efk_TestForm = 'unit-test-test-form'";

        [TestMethod]
        public void ShouldSaveATestFormPropertyRecord()
        {
            var testFormPropertyList = new List<TestFormPropertiesDTO>
            {
                new TestFormPropertiesDTO
                {
                    ClientName = "unit-test-client-name",
                    TestFormKey = "unit-test-test-form",
                    FormId = "unit-test-form-id",
                    TestId = "unit-test-test-id",
                    Language = "unit-test-language",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Testkey = "unit-test-test-key"
                }
            };

            testPackageDao.Insert(testFormPropertyList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(testFormPropertyList[0].ClientName, result.ClientName);
            Assert.AreEqual(testFormPropertyList[0].TestFormKey, result.TestFormKey);
            Assert.AreEqual(testFormPropertyList[0].FormId, result.FormId);
            Assert.AreEqual(testFormPropertyList[0].TestId, result.TestId);
            Assert.AreEqual(testFormPropertyList[0].Language, result.Language);
            Assert.AreEqual(testFormPropertyList[0].StartDate.ToLongDateString(), result.StartDate.ToLongDateString());
            Assert.AreEqual(testFormPropertyList[0].EndDate.ToLongDateString(), result.EndDate.ToLongDateString());
            Assert.AreEqual(testFormPropertyList[0].Testkey, result.Testkey);
        }
    }
}
