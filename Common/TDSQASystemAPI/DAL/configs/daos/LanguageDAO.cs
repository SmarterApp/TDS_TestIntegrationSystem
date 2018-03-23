using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>LanguageDTO</code>s to the <code>OSS_Configs..Client_Language</code> table
    /// </summary>
    public class LanguageDAO : TestPackageDaoBase<LanguageDTO>
    {
        public LanguageDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "LanguageTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_Language (clientname, [Language], LanguageCode)\n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   [Language], \n" +
                "   LanguageCode \n";
        }
    }
}
