using System.Collections.Generic;
using TDSQASystemAPI.DAL.scoring.dtos;

namespace TDSQASystemAPI.DAL.scoring.daos
{
    /// <summary>
    /// A class for saving <code>ConversionTableDTO</code>s to the <code>OSS_TestScoringConfigs..ConversionTables</code> table
    /// </summary>
    public class ConversionTableDAO : TestPackageDaoBase<ConversionTableDTO>
    {
        public ConversionTableDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.SCORING;
            TvpType = "ConversionTableTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.ConversionTables(tablename, invalue, outvalue, clientname) \n" +
                "SELECT \n" +
                "   TableName, \n" +
                "   InValue, \n" +
                "   OutValue, \n" +
                "   ClientName \n";
        }

        public override void Insert(IList<ConversionTableDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
