using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>ItemDTO</code>s to the <code>OSS_Itembank..tblItem</code> table
    /// </summary>
    public class ItemDAO : TestPackageDaoBase<ItemDTO>
    {
        public ItemDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "ItemTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblItem (" +
                "       _efk_ItemBank, " +
                "       _efk_Item, " +
                "       ItemType, " +
                "       ScorePoint," +
                "       FilePath, " +
                "       FileName, " +
                "       DateLastUpdated, " +
                "       _Key, " +
                "       LoadConfig) \n" +
                "SELECT \n" +
                "   ItemBankKey, \n" +
                "   ItemKey, \n" +
                "   ItemType, \n" +
                "   ScorePoint, \n" +
                "   FilePath, \n" +
                "   [FileName], \n" +
                "   DateLastUpdated, \n" +
                "   [Key], \n" +
                "   TestVersion \n";
        }

        public override void Insert(IList<ItemDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
