using System.Collections.Generic;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>StrandDTO</code>s to the <code>OSS_Itembank..tblStrand</code> table
    /// </summary>
    public class StrandDAO : TestPackageDaoBase<StrandDTO>
    {
        public StrandDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "StrandTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblStrand (_fk_Subject, Name, _fk_Parent, _Key, _fk_Client, TreeLevel, LoadConfig) \n" +
                "SELECT \n" +
                "   SubjectKey, \n" +
                "   [Name], \n" +
                "   ParentId, \n" +
                "   BlueprintElementId, \n" +
                "   ClientKey, \n" +
                "   TreeLevel, \n" +
                "   TestVersion \n";

            ExistsSql = "SELECT count(*) FROM dbo.tblStrand t WHERE t._fk_Subject = @subject AND t._Key = @key AND t._fk_Client = @client";
        }

        override protected void AddNaturalKeys(StrandDTO strandDTO, SqlParameterCollection parameters)
        {
            parameters.AddWithValue("@subject", strandDTO.SubjectKey);
            parameters.AddWithValue("@key", strandDTO.Key);
            parameters.AddWithValue("@client", strandDTO.ClientKey);
        }
    }
}
