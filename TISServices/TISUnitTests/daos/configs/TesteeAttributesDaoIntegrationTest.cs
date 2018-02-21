using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class TesteeAttributesDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TesteeAttributeDTO>
    {
        private readonly ITestPackageDao<TesteeAttributeDTO> testeeAttributeDao = new TesteeAttributeDAO();
        private string verificationSql = 
            "SELECT" +
            "   RTSName," +
            "   TDS_ID AS TdsId," +
            "   ClientName," +
            "   reportName, " +
            "   [Type]," +
            "   Label, " +
            "   AtLogin," +
            "   SortOrder " +
            "FROM " +
            "   Client_TesteeAttribute " +
            "WHERE " +
            "   ClientName = 'unit-test' " +
            "   AND TDS_ID LIKE 'tds-attribute-id%'";

        [TestMethod]
        public void ShouldSaveACollectionOfTesteeAttributes()
        {
            IList<TesteeAttributeDTO> testeeAttributesList = new List<TesteeAttributeDTO>
            {
                new TesteeAttributeDTO
                {
                    RtsName = "art-id-1",
                    AtLogin = "at-login-1",
                    ClientName = "unit-test",
                    Label = "label 1",
                    ReportName = "report name 1",
                    SortOrder = 1,
                    TdsId = "tds-attribute-id 1",
                    Type = "unit test"
                },
                new TesteeAttributeDTO
                {
                    RtsName = "art-id-2",
                    AtLogin = "at-login-2",
                    ClientName = "unit-test",
                    Label = "label 2",
                    ReportName = "report name 2",
                    SortOrder = 2,
                    TdsId = "tds-attribute-id 2",
                    Type = "unit test",
                    
                }
            };

            testeeAttributeDao.Insert(testeeAttributesList);

            var insertedRecords = GetInsertedRecords(verificationSql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(2, insertedRecords.Count);
            var firstResult = insertedRecords[0];
            Assert.AreEqual(testeeAttributesList[0].RtsName, firstResult.RtsName);
            Assert.AreEqual(testeeAttributesList[0].AtLogin, firstResult.AtLogin);
            Assert.AreEqual(testeeAttributesList[0].ClientName, firstResult.ClientName);
            Assert.AreEqual(testeeAttributesList[0].Label, firstResult.Label);
            Assert.AreEqual(testeeAttributesList[0].ReportName, firstResult.ReportName);
            Assert.AreEqual(testeeAttributesList[0].SortOrder, firstResult.SortOrder);
            Assert.AreEqual(testeeAttributesList[0].TdsId, firstResult.TdsId);
            Assert.AreEqual(testeeAttributesList[0].Type, firstResult.Type);

            var secondResult = insertedRecords[1];
            Assert.AreEqual(testeeAttributesList[1].RtsName, secondResult.RtsName);
            Assert.AreEqual(testeeAttributesList[1].AtLogin, secondResult.AtLogin);
            Assert.AreEqual(testeeAttributesList[1].ClientName, secondResult.ClientName);
            Assert.AreEqual(testeeAttributesList[1].Label, secondResult.Label);
            Assert.AreEqual(testeeAttributesList[1].ReportName, secondResult.ReportName);
            Assert.AreEqual(testeeAttributesList[1].SortOrder, secondResult.SortOrder);
            Assert.AreEqual(testeeAttributesList[1].TdsId, secondResult.TdsId);
            Assert.AreEqual(testeeAttributesList[1].Type, secondResult.Type);
        }
    }
}
