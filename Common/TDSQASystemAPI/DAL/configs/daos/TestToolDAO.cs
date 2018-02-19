using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestToolDTO</code>s to the <code>OSS_Configs..Client_TestTooly</code> table
    /// </summary>
    public class TestToolDAO : TestPackageDaoBase<TestToolDTO>
    {
        public TestToolDAO()
        {
            DbConnectionStringName = "configs";
            TvpType = "TestToolType";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TestTool (" +
                "       ClientName, " +
                "       [Type], " +
                "       Code, " +
                "       [Value], " +
                "       IsDefault, " +
                "       AllowCombine, " +
                "       ValueDescription, " +
                "       Context, " +
                "       ContextType)\n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   [Type], \n" +
                "   Code, \n" +
                "   [Value], \n" +
                "   IsDefault, \n" +
                "   AllowCombine, \n" +
                "   ValueDescription, \n" +
                "   Context, \n" +
                "   ContextType \n";
        }

        public override void Insert(IList<TestToolDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
