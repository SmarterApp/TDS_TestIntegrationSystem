using System.Collections.Generic;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL.scoring.dtos;

namespace TDSQASystemAPI.DAL.scoring.daos
{
    /// <summary>
    /// A class for saving <code>ComputationRuleParameterValueDTO</code>s to the <code>OSS_TestScoringConfigs..ComputationRuleParameterValue</code> table
    /// </summary>
    public class ComputationRuleParameterValueDAO : TestPackageDaoBase<ComputationRuleParameterValueDTO>
    {
        public ComputationRuleParameterValueDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.SCORING;
            TvpType = "ComputationRuleParameterValueTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.ComputationRuleParameterValue (_fk_TestScoreFeature, _fk_Parameter, [Index], [Value]) \n" +
                "SELECT \n" +
                "   TestScoreFeatureKey, \n" +
                "   ComputationRuleParameterKey, \n" +
                "   [Index], \n" +
                "   [Value] \n";
            ExistsSql = "SELECT count(*) FROM dbo.ComputationRuleParameterValue t WHERE " +
                "t._fk_TestScoreFeature = @TestScoreFeatureKey AND " +
                "t._fk_Parameter = @ComputationRuleParameterKey";
        }

        override protected void AddNaturalKeys(ComputationRuleParameterValueDTO computationRuleParameterValueDTO, SqlParameterCollection parameters)
        {
            parameters.AddWithValue("@TestScoreFeatureKey", computationRuleParameterValueDTO.TestScoreFeatureKey);
            parameters.AddWithValue("@ComputationRuleParameterKey", computationRuleParameterValueDTO.ComputationRuleParameterKey);
        }
    }
}
