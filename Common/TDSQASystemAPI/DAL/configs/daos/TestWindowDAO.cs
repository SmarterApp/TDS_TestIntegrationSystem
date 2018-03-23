using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestWindowDTO</code>s to the <code>OSS_Configs..Client_TestWindow</code> table
    /// </summary>
    public class TestWindowDAO : TestPackageDaoBase<TestWindowDTO>
    {
        public TestWindowDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "TestWindowTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TestWindow (ClientName, TestId, WindowId, NumOpps, StartDate, EndDate) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   WindowId, \n" +
                "   NumOpps, \n" +
                "   StartDate, \n" +
                "   EndDate \n";
        }
    }
}
