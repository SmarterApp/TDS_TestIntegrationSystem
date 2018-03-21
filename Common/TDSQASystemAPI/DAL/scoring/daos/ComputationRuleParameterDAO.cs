using System.Collections.Generic;
using System.Data.SqlClient;
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
            ExistsSql = "SELECT count(*) FROM dbo.ComputationRuleParameters t WHERE " +
                "t.ComputationRule = @ComputationRule AND " +
                "t.ParameterName = @ParameterName AND " +
                "t.ParameterPosition = @ParameterPosition";
        }

        override protected void ExistsAddParameter(ComputationRuleParameterDTO computationRuleParameterDTO, SqlParameterCollection parameters)
        {
            parameters.AddWithValue("@ComputationRule", computationRuleParameterDTO.ComputationRule);
            parameters.AddWithValue("@ParameterName", computationRuleParameterDTO.ParameterName);
            parameters.AddWithValue("@ParameterPosition", computationRuleParameterDTO.ParameterPosition);
        }
    }
}
