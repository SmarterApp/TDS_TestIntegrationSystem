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
    public class TesteeRelationshipAttributeDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TesteeRelationshipAttributeDTO>
    {
        private readonly ITestPackageDao<TesteeRelationshipAttributeDTO> testPackageDao = new TesteeRelationshipAttributeDAO();
        private readonly string sql =
            "SELECT " +
            "   ClientName, " +
            "   TDS_ID AS TdsId, " +
            "   RtsName, " +
            "   Label, " +
            "   ReportName, " +
            "   AtLogin, " +
            "   SortOrder, " +
            "   RelationshipType " +
            "FROM " +
            "   Client_TesteeRelationshipAttribute " +
            "WHERE " +
            "   ClientName = 'unit-test-client-name' " +
            "   AND TDS_ID = 'unit-test-tds-id'";

        [TestMethod]
        public void ShouldSaveATesteeRelationshipAttributeRecord()
        {
            var testeeRelationshipAttributeList = new List<TesteeRelationshipAttributeDTO>
            {
                new TesteeRelationshipAttributeDTO
                {
                    ClientName = "unit-test-client-name",
                    TdsId = "unit-test-tds-id",
                    RtsName = "unit-test-rts-name",
                    Label = "unit-test-label",
                    ReportName = "unit-test-report-name",
                    AtLogin = "unit-test-at-login",
                    SortOrder = 50,
                    RelationshipType = "unit-test-relationship-type"
                }
            };

            testPackageDao.Insert(testeeRelationshipAttributeList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(testeeRelationshipAttributeList[0], insertedRecords[0]);
        }
    }
}
