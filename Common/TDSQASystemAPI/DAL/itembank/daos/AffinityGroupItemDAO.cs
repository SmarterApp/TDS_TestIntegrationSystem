using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>AffinityGroupItemDTO</code>s to the <code>OSS_Itembank..AffinityGroupItem</code> table
    /// </summary>
    public class AffinityGroupItemDAO : TestPackageDaoBase<AffinityGroupItemDTO>
    {
        public AffinityGroupItemDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "AffinityGroupItemTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.AffinityGroupItem (_fk_adminsubject, groupid, _fk_item) \n" +
                "SELECT" +
                "   SegmentKey, \n" +
                "   GroupId, \n" +
                "   ItemKey \n";
        }

        public override void Insert(IList<AffinityGroupItemDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
