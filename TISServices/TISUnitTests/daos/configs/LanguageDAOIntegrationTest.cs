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
    public class LanguageDAOIntegrationTest : TestPackageDaoIntegrationTestBase<LanguageDTO>
    {
        private readonly ITestPackageDao<LanguageDTO> testPackageDao = new LanguageDAO();
        private readonly string sql =
            "SELECT" +
            "   ClientName, " +
            "   [Language], " +
            "   LanguageCode " +
            "FROM " +
            "   Client_Language " +
            "WHERE" +
            "   ClientName = 'unit-test-client' " +
            "   AND [Language] = 'unit-test-language' " +
            "   AND LanguageCode = 'unit-test-language-code'";

        [TestMethod]
        public void ShouldSaveALanguageRecord()
        {
            var languageList = new List<LanguageDTO>
            {
                new LanguageDTO
                {
                    ClientName = "unit-test-client",
                    Language = "unit-test-language",
                    LanguageCode = "unit-test-language-code"
                }
            };

            testPackageDao.Insert(languageList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(languageList[0].ClientName, result.ClientName);
            Assert.AreEqual(languageList[0].Language, result.Language);
            Assert.AreEqual(languageList[0].LanguageCode, result.LanguageCode);
        }
    }
}
