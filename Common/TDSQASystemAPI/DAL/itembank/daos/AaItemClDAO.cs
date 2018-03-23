using System.Collections.Generic;
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
        }
    }
}
