using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>ItemPropertyDTO</code>s to the <code>OSS_Itembank..tblItemProps</code> table
    /// </summary>
    public class ItemPropertyDAO : TestPackageDaoBase<ItemPropertyDTO>
    {
        public ItemPropertyDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "ItemPropertyTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblItemProps (_fk_item, propname, propvalue, _fk_adminsubject, isactive) \n" +
                "SELECT \n" +
                "   ItemKey, \n" +
                "   PropertyName, \n" +
                "   PropertyValue, \n" +
                "   SegmentKey, \n" +
                "   IsActive \n";
        }
    }
}
