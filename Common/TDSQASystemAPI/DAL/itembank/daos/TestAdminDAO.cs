using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>TestAdminDTO</code>s to the <code>OSS_Itembank..tblTestAdmin</code> table
    /// </summary>
    public class TestAdminDAO : TestPackageDaoBase<TestAdminDTO>
    {
        public TestAdminDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "TestAdminTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblTestAdmin (_Key, SchoolYear, Season, _fk_Client, [Description], LoadConfig) \n" +
                "SELECT \n" +
                "   AdminKey, \n" +
                "   SchoolYear, \n" +
                "   Season, \n" +
                "   ClientKey, \n" +
                "   [Description], \n" +
                "   TestVersion \n";
        }

        public override void Insert(IList<TestAdminDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
