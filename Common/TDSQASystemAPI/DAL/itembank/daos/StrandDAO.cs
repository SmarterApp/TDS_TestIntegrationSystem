using System.Collections.Generic;
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
        }

        public override void Insert(IList<StrandDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
