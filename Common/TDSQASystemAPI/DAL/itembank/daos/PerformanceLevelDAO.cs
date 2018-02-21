using System.Collections.Generic;
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
                "   PLeveln \n" +
                "   ThetaLo, \n" +
                "   ThetaHi, \n" +
                "   ScaledLo, \n" +
                "   ScaledHi \n";
        }

        public override void Insert(IList<PerformanceLevelDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
