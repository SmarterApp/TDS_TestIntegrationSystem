using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestItemTypeDTO</code>s to the <code>OSS_Configs..Client_Test_ItemType</code> table
    /// </summary>
    public class TestItemTypeDAO : TestPackageDaoBase<TestItemTypeDTO>
    {
        public TestItemTypeDAO()
        {
            DbConnectionStringName = "configs";
            TvpType = "TestItemTypeType (ClientName, TestID, ItemType)";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_Test_ItemTypes \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   ItemType \n";
        }

        public override void Insert(IList<TestItemTypeDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
