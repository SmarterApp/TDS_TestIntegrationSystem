using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>SetOfItemStrandDTO</code>s to the <code>OSS_Itembank..tblSetOfItemStrands</code> table
    /// </summary>
    public class SetOfItemStrandDAO : TestPackageDaoBase<SetOfItemStrandDTO>
    {
        public SetOfItemStrandDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "SetOfItemStrandsTable";
            InsertSql = 
                "INSERT \n" +
                "   dbo.tblSetOfItemStrands (_fk_item, _fk_strand, _fk_adminsubject, loadconfig) \n" +
                "SELECT \n" +
                "   ItemKey, \n" +
                "   StrandKey, \n" +
                "   SegmentKey, \n" +
                "   TestVersion \n";
        }

        public override void Insert(IList<SetOfItemStrandDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
