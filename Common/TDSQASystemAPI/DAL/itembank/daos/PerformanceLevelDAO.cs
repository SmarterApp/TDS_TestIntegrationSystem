using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>TestDTO</code>s to the <code>OSS_TestScoringConfigs..Test</code> table
    /// </summary>
    public class PerformanceLevelDAO : TestPackageDaoBase<PerformanceLevelDTO>
    {
        public PerformanceLevelDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "PerformanceLevelTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.PerformanceLevels (_fk_content, PLevel, ThetaLo, ThetaHi, ScaledLo, ScaledHi) \n" +
                "SELECT \n" +
                "   ContentKey, \n" +
                "   PLevel, \n" +
                "   ThetaLo, \n" +
                "   ThetaHi, \n" +
                "   ScaledLo, \n" +
                "   ScaledHi \n";
            ExistsSql = "SELECT count(*) FROM dbo.PerformanceLevels t WHERE t._fk_content = @content AND t.PLevel = @pLevel";
        }

        override protected void AddNaturalKeys(PerformanceLevelDTO performanceLevelDTO, SqlParameterCollection parameters)
        {
            parameters.AddWithValue("@content", performanceLevelDTO.ContentKey);
            parameters.AddWithValue("@pLevel", performanceLevelDTO.PLevel);
        }
    }
}
