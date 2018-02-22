using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestScoreFeatureDTO</code>s to the <code>OSS_Configs..Client_TestScoreFeatures</code> table
    /// </summary>
    public class TestScoreFeaturesDAO : TestPackageDaoBase<TestScoreFeatureDTO>
    {
        public TestScoreFeaturesDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "TestScoreFeatureTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TestScoreFeatures (ClientName, TestID, MeasureOf, MeasureLabel, UseForAbility) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   MeasureOf, \n" +
                "   MeasureLabel, \n" +
                "   UseForAbility \n";
        }

        public override void Insert(IList<TestScoreFeatureDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
