using System.Collections.Generic;
using System.Data.SqlClient;
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
            ExistsSql = "SELECT count(*) FROM dbo.tblSetOfItemStrands t WHERE t._fk_item = @itemKey AND t._fk_strand = @strand AND t._fk_adminsubject = @segmentKey";
        }

        override protected void AddNaturalKeys(SetOfItemStrandDTO strandDTO, SqlParameterCollection parameters)
        {
            parameters.AddWithValue("@itemKey", strandDTO.ItemKey);
            parameters.AddWithValue("@strand", strandDTO.StrandKey);
            parameters.AddWithValue("@segmentKey", strandDTO.SegmentKey);
        }
    }
}
