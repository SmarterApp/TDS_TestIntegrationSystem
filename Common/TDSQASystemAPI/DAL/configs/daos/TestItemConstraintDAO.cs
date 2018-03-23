using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestItemConstraintDTO</code>s to the <code>OSS_Configs..Client_Test_ItemConstraint</code> table
    /// </summary>
    public class TestItemConstraintDAO : TestPackageDaoBase<TestItemConstraintDTO>
    {
        public TestItemConstraintDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "TestItemConstraintTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_Test_ItemConstraint (clientname, testID, propname, propvalue, tooltype, toolvalue, item_in) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   PropName, \n" +
                "   PropValue, \n" +
                "   ToolType, \n" +
                "   ToolValue, \n" +
                "   ItemIn \n";
        }
    }
}
