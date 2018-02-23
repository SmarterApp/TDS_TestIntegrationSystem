using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>ItemSelectionParmDTO</code>s to the <code>OSS_Itembank..tblItemSelectionParm</code> table
    /// </summary>
    public class ItemSelectionParmDAO : TestPackageDaoBase<ItemSelectionParmDTO>
    {
        public ItemSelectionParmDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "ItemSelectionParmTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblItemSelectionParm (_fk_AdminSubject, bpElementID, [name], [value], label) \n" +
                "SELECT \n" +
                "   SegmentKey, \n" +
                "   BlueprintElementID, \n" +
                "   PropertyName, \n" +
                "   PropertyValue, \n" +
                "   PropertyLabel \n";
        }

        public override void Insert(IList<ItemSelectionParmDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
