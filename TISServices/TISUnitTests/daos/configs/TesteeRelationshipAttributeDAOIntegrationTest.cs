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
            var result = insertedRecords[0];
            Assert.AreEqual(testeeRelationshipAttributeList[0].ClientName, result.ClientName);
            Assert.AreEqual(testeeRelationshipAttributeList[0].TdsId, result.TdsId);
            Assert.AreEqual(testeeRelationshipAttributeList[0].RtsName, result.RtsName);
            Assert.AreEqual(testeeRelationshipAttributeList[0].Label, result.Label);
            Assert.AreEqual(testeeRelationshipAttributeList[0].ReportName, result.ReportName);
            Assert.AreEqual(testeeRelationshipAttributeList[0].AtLogin, result.AtLogin);
            Assert.AreEqual(testeeRelationshipAttributeList[0].SortOrder, result.SortOrder);
            Assert.AreEqual(testeeRelationshipAttributeList[0].RelationshipType, result.RelationshipType);
        }
    }
}
