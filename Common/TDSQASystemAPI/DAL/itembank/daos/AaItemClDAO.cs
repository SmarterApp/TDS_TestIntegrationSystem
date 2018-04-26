using System.Collections.Generic;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>AaItemClDTO</code>s to the <code>OSS_Itembank..AA_ItemCL</code> table
    /// </summary>
    public class AaItemClDAO : TestPackageDaoBase<AaItemClDTO>
    {
        public AaItemClDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "AaItemClTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.AA_ItemCL (_fk_AdminSubject, _fk_Item, ContentLevel) \n" +
                "SELECT \n" +
                "   SegmentKey, \n" +
                "   ItemKey, \n" +
                "   ContentLevel \n";
            ExistsSql = "SELECT count(*) FROM dbo.AA_ItemCL t WHERE t._fk_AdminSubject = @segmentKey AND t._fk_Item = @itemKey AND t.ContentLevel = @contentLevel";
        }

        override protected void AddNaturalKeys(AaItemClDTO aaItemClDTO, SqlParameterCollection parameters)
        {
            parameters.AddWithValue("@segmentKey", aaItemClDTO.SegmentKey);
            parameters.AddWithValue("@itemKey", aaItemClDTO.ItemKey);
            parameters.AddWithValue("@contentLevel", aaItemClDTO.ContentLevel);
        } 
    }
}
