using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class ClientDAOIntegrationTest : TestPackageDaoIntegrationTestBase<ClientDTO>
    {
        private readonly ITestPackageDao<ClientDTO> testPackageDao = new ClientDAO();
        private readonly string sql =
            "SELECT " +
            "   Name, " +
            "   Internationalize, " +
            "   DefaultLanguage " +
            "FROM" +
            "   dbo.Client " +
            "WHERE" +
            "   Name = 'unit-test'";

        [TestMethod]
        public void ShouldSaveAClientRecord()
        {
            var clientList = new List<ClientDTO>
            {
                new ClientDTO
                {
                    Name = "unit-test",
                    Internationalize = true,
                    DefaultLanguage = "ENU"
                }
            };

            testPackageDao.Insert(clientList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(clientList[0], insertedRecords[0]);
        }
    }
}
