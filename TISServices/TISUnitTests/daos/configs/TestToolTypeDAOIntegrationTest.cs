using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class TestToolTypeDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestToolTypeDTO>
    {
        private readonly ITestPackageDao<TestToolTypeDTO> testPackageDao = new TestToolTypeDAO();
        private readonly string sql =
            "SELECT \n" +
            "   ClientName, \n" +
            "   Context, \n" +
            "   ContextType, \n" +
            "   ToolName, \n" +
            "   AllowChange, \n" +
            "   IsSelectable, \n" +
            "   IsVisible, \n" +
            "   StudentControl, \n" +
            "   IsFunctional, \n" +
            "   RtsFieldName, \n" +
            "   IsRequired, \n" +
            "   TideSelectable, \n" +
            "   TideSelectableBySubject \n" +
            "FROM \n" +
            "   Client_TestToolType \n" +
            "WHERE \n" +
            "   ClientName = 'unit-test-client-name' \n" +
            "   AND Context = 'unit-test-context' \n" +
            "   AND ContextType = 'unit-test-context-type' \n" +
            "   AND ToolName = 'unit-test-tool-name'";

        [TestMethod]
        public void ShouldSaveATestToolTypeRecord()
        {
            var toolTypeList = new List<TestToolTypeDTO>
            {
                new TestToolTypeDTO
                {
                    ClientName = "unit-test-client-name",
                    Context = "unit-test-context",
                    ContextType = "unit-test-context-type",
                    ToolName = "unit-test-tool-name",
                    AllowChange = true,
                    IsSelectable = true,
                    IsVisible = true,
                    StudentControl = true,
                    IsFunctional = true,
                    RtsFieldName = "unit-test-rts-field-name",
                    IsRequired = true,
                    TideSelectable = true,
                    TideSelectableBySubject = true
                }
            };

            testPackageDao.Insert(toolTypeList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(toolTypeList[0].ClientName, result.ClientName);
            Assert.AreEqual(toolTypeList[0].Context, result.Context);
            Assert.AreEqual(toolTypeList[0].ContextType, result.ContextType);
            Assert.AreEqual(toolTypeList[0].ToolName, result.ToolName);
            Assert.AreEqual(toolTypeList[0].AllowChange, result.AllowChange);
            Assert.AreEqual(toolTypeList[0].IsSelectable, result.IsSelectable);
            Assert.AreEqual(toolTypeList[0].IsVisible, result.IsVisible);
            Assert.AreEqual(toolTypeList[0].StudentControl, result.StudentControl);
            Assert.AreEqual(toolTypeList[0].IsFunctional, result.IsFunctional);
            Assert.AreEqual(toolTypeList[0].RtsFieldName, result.RtsFieldName);
            Assert.AreEqual(toolTypeList[0].IsRequired, result.IsRequired);
            Assert.AreEqual(toolTypeList[0].TideSelectable, result.TideSelectable);
            Assert.AreEqual(toolTypeList[0].TideSelectableBySubject, result.TideSelectableBySubject);

        }
    }
}
