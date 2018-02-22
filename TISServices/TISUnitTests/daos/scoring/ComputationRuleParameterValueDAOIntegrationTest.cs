using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.scoring.daos;
using TDSQASystemAPI.DAL.scoring.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.scoring
{
    [TestClass]
    public class ComputationRuleParameterValueDAOIntegrationTest : TestPackageDaoIntegrationTestBase<ComputationRuleParameterValueDTO>
    {
        private const string COMPUTATION_RULE_NAME = "ItemCount"; // from the 3_Confguration.sql script, used to create the OSS_TestScoringConfigs database
        private const string CLIENT_NAME = "SBAC_PT"; // from the 3_Confguration.sql script, used to create the OSS_TestScoringConfigs database

        private readonly ITestPackageDao<ComputationRuleParameterValueDTO> testPackageDao = new ComputationRuleParameterValueDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _fk_TestScoreFeature AS TestScoreFeatureKey, \n" +
            "   _fk_Parameter AS ComputationRuleParameterKey, \n" +
            "   [Index], \n" +
            "   [Value] \n" +
            "FROM \n" +
            "   ComputationRuleParameterValue \n" +
            "WHERE \n" +
            "   _fk_TestScoreFeature = '{0}' \n" +
            "   AND _fk_Parameter = '{1}' \n" +
            "   AND [Index] = 'unit-test-index'";

        private readonly TestDTO seedDataTest = new TestDTO
        {
            ClientName = CLIENT_NAME,
            TestId = "unit-test-test-id",
            Subject = "unit-test-subject"
        };
        private readonly TestScoreFeatureDTO seedDataTestScoreFeature = new TestScoreFeatureDTO
        {
            TestScoreFeatureKey = new Guid(),
            ClientName = CLIENT_NAME,
            TestId = "unit-test-test-id",
            MeasureOf = "unit-test-measure-of",
            MeasureLabel = "unit-test-measure-label",
            IsScaled = true,
            ComputationRule = COMPUTATION_RULE_NAME,
            ComputationOrder = 42
        };
        private readonly ComputationRuleParameterDTO seedDataComputationRuleParameter = new ComputationRuleParameterDTO
        {
            ComputationRuleParameterKey = new Guid(),
            ComputationRule = "unit-test-rule",
            ParameterName = "unit-test-param-name",
            ParameterPosition = 42,
            IndexType = "unit-test-idx",
            Type = "unit-test-type"
        };

        /// <summary>
        /// Create seed data to satisfy foreign key constraints
        /// </summary>
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            new TestDAO().Insert(new List<TestDTO> { seedDataTest });
            new TestScoreFeatureDAO().Insert(new List<TestScoreFeatureDTO> { seedDataTestScoreFeature });
            new ComputationRuleParameterDAO().Insert(new List<ComputationRuleParameterDTO> { seedDataComputationRuleParameter });
        }

        [TestMethod]
        public void ShouldSaveAComputationRuleParameterValueDescRecord()
        {
            var valueList = new List<ComputationRuleParameterValueDTO>
            {
                new ComputationRuleParameterValueDTO
                {
                    TestScoreFeatureKey = seedDataTestScoreFeature.TestScoreFeatureKey,
                    ComputationRuleParameterKey = seedDataComputationRuleParameter.ComputationRuleParameterKey,
                    Index = "unit-test-index",
                    Value = "unit-test-value"
                }
            };

            testPackageDao.Insert(valueList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, seedDataTestScoreFeature.TestScoreFeatureKey, seedDataComputationRuleParameter.ComputationRuleParameterKey),
                DatabaseConnectionStringNames.SCORING);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(valueList[0].TestScoreFeatureKey, result.TestScoreFeatureKey);
            Assert.AreEqual(valueList[0].ComputationRuleParameterKey, result.ComputationRuleParameterKey);
            Assert.AreEqual(valueList[0].Index, result.Index);
            Assert.AreEqual(valueList[0].Value, result.Value);

        }
    }
}
