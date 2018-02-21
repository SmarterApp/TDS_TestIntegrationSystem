using System.Collections.Generic;
using TDSQASystemAPI.DAL.scoring.dtos;

namespace TDSQASystemAPI.DAL.scoring.daos
{
    /// <summary>
    /// A class for saving <code>TestDTO</code>s to the <code>OSS_TestScoringConfigs..Test</code> table
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
        }

        public override void Insert(IList<ComputationRuleParameterValueDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
