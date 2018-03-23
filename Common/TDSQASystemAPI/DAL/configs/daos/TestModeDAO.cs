using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestModeDTO</code>s to the <code>OSS_Configs..Client_TestMode</code> table
    /// </summary>
    public class TestModeDAO : TestPackageDaoBase<TestModeDTO>
    {
        public TestModeDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "TestModeTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TestMode (ClientName, TestID, TestKey, Mode, Algorithm, IsSegmented, SessionType)\n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   TestKey, \n" +
                "   Mode, \n" +
                "   Algorithm, \n" +
                "   IsSegmented, \n" +
                "   SessionType \n";
        }
    }
}
