using System.Collections.Generic;
using TDSQASystemAPI.DAL.scoring.dtos;

namespace TDSQASystemAPI.DAL.scoring.daos
{
    /// <summary>
    /// A class for saving <code>ComputationRuleParameterDTO</code>s to the <code>OSS_TestScoringConfigs..ComputationRuleParameter</code> table
    /// </summary>
    public class ComputationRuleParameterDAO : TestPackageDaoBase<ComputationRuleParameterDTO>
    {
        public ComputationRuleParameterDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.SCORING;
            TvpType = "ComputationRuleParameterTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.ComputationRuleParameters (_Key, ComputationRule, ParameterName, ParameterPosition, IndexType, Type) \n" +
                "SELECT \n" +
                "   ComputationRuleParameterKey, \n" +
                "   ComputationRule ,\n" +
                "   ParameterName, \n" +
                "   ParameterPosition, \n" +
                "   IndexType, \n" +
                "   [Type] \n";
        }

        public override void Insert(IList<ComputationRuleParameterDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
