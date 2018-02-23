using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class TestToolDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestToolDTO>
    {
        private readonly ITestPackageDao<TestToolDTO> testPackageDao = new TestToolDAO();
        private readonly string sql =
            "SELECT \n" +
            "   ClientName, \n" +
            "   [Type], \n" +
            "   Code, \n" +
            "   [Value], \n" +
            "   IsDefault, \n" +
            "   AllowCombine, \n" +
            "   ValueDescription, \n" +
            "   Context, \n" +
            "   ContextType \n" +
            "FROM \n" +
            "   Client_TestTool \n" +
            "WHERE \n" +
            "   ClientName = 'unit-test-client-name' \n" +
            "   AND [Type] = 'unit-test-tool-name' \n" +
            "   AND Code = 'unit-test-code' \n" +
            "   AND Context = 'unit-test-context' \n" +
            "   AND ContextType = 'unit-test-context-type'";
        private readonly TestToolTypeDTO toolTypeListSeedData = new TestToolTypeDTO
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
        };

        /// <summary>
        /// Seed the database with a record in <code>OSS_Configs..Client_TestToolType</code> so foreign key constraints can be satisfied.
        /// </summary>
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            new TestToolTypeDAO().Insert(new List<TestToolTypeDTO> { toolTypeListSeedData });
        }

        [TestMethod]
        public void ShouldSaveATestToolRecord()
        {
            var testToolList = new List<TestToolDTO>
            {
                new TestToolDTO
                {
                    ClientName = toolTypeListSeedData.ClientName,
                    Context = toolTypeListSeedData.Context,
                    ContextType = toolTypeListSeedData.ContextType,
                    Type = toolTypeListSeedData.ToolName,
                    Code = "unit-test-code",
                    Value = "unit-code-value",
                    IsDefault = true,
                    AllowCombine = true,
                    ValueDescription = "unit-test-description"
                }
            };

            testPackageDao.Insert(testToolList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(testToolList[0], insertedRecords[0]);
        }
    }
}
