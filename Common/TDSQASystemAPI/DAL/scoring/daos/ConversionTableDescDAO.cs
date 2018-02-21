using System.Collections.Generic;
using TDSQASystemAPI.DAL.scoring.dtos;

namespace TDSQASystemAPI.DAL.scoring.daos
{
    /// <summary>
    /// A class for saving <code>ConversionTableDescDTO</code>s to the <code>OSS_TestScoringConfigs..ConversionTableDesc</code> table
    /// </summary>
    public class ConversionTableDescDAO : TestPackageDaoBase<ConversionTableDescDTO>
    {
        public ConversionTableDescDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.SCORING;
            TvpType = "ConversionTableDescTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.ConversionTableDesc (_Key, TableName, _fk_Client) \n" +
                "SELECT \n" +
                "   ConversionTableDescKey, \n" +
                "   TableName, \n" +
                "   ClientName \n";
        }

        public override void Insert(IList<ConversionTableDescDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
