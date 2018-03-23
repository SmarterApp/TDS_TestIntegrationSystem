using System.Collections.Generic;
using TDSQASystemAPI.DAL.scoring.dtos;

namespace TDSQASystemAPI.DAL.scoring.daos
{
    /// <summary>
    /// A class for saving <code>TestDTO</code>s to the <code>OSS_TestScoringConfigs..Test</code> table
    /// </summary>
    public class FeatureComputationLocationDAO : TestPackageDaoBase<FeatureComputationLocationDTO>
    {
        public FeatureComputationLocationDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.SCORING;
            TvpType = "FeatureComputationLocationTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Feature_ComputationLocation (_fk_TestScoreFeature, [Location]) \n" +
                "SELECT \n" +
                "   TestScoreFeatureKey, \n" +
                "   [Location] \n";
        }
    }
}
