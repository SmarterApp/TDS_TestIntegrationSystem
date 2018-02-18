using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    public class TestModeDAO : TestPackageDaoBase<TestModeDTO>
    {
        /// <summary>
        /// A class for saving <code>TestModeDTO</code>s to the <code>OSS_Configs..Client_TestMode</code> table
        /// </summary>
        public TestModeDAO()
        {
            DbConnectionStringName = "configs";
            TvpType = "TestModeType";
            TvpVariableName = "@tvpTestModes";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TestMode (ClientName, TestID, TestKey, Mode, SessionType)\n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   TestKey, \n" +
                "   Mode, \n" +
                "   SessionType \n" +
                "FROM \n" +
                TvpVariableName;
        }

        public override void Insert(IList<TestModeDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
