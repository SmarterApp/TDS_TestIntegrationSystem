using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.scoring.daos;
using TDSQASystemAPI.DAL.scoring.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.scoring
{
    [TestClass]
    public class ComputationRuleParameterDAOIntegrationTest : TestPackageDaoIntegrationTestBase<ComputationRuleParameterDTO>
    {
        private readonly ITestPackageDao<ComputationRuleParameterDTO> testPackageDao = new ComputationRuleParameterDAO();
        private readonly Guid mockPrimaryKey = new Guid("ec6133ce-4baa-41a9-a8ef-83e76153b93e");
        private readonly string sql =
            "SELECT \n" +
            "   _Key AS ComputationRuleParameterKey, \n" +
            "   ComputationRule ,\n" +
            "   ParameterName, \n" +
            "   ParameterPosition, \n" +
            "   IndexType, \n" +
            "   [Type] \n" +
            "FROM \n" +
            "   ComputationRuleParameters \n" +
            "WHERE \n" +
            "   _Key = '{0}'";

        [TestMethod]
        public void ShouldSaveAComputationRuleParameterRecord()
        {
            var ruleParamList = new List<ComputationRuleParameterDTO>
            {
                new ComputationRuleParameterDTO
                {
                    ComputationRuleParameterKey = mockPrimaryKey,
                    ComputationRule = "unit-test-rule-",
                    ParameterName = "unit-test-param-name",
                    ParameterPosition = 42,
                    IndexType = "unit-test-idx",
                    Type = "unit-test-type"
                }
            };

            testPackageDao.Insert(ruleParamList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, mockPrimaryKey), DatabaseConnectionStringNames.SCORING);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(ruleParamList[0], insertedRecords[0]);
        }
    }
}
