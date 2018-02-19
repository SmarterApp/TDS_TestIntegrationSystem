using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestToolTypeDTO</code>s to the <code>OSS_Configs..Client_TestToolType</code> table
    /// </summary>
    public class TestToolTypeDAO : TestPackageDaoBase<TestToolTypeDTO>
    {
        public TestToolTypeDAO()
        {
            DbConnectionStringName = "configs";
            TvpType = "TestToolTypeType";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TestToolType (" +
                "       ClientName, " +
                "       Context, " +
                "       ContextType, " +
                "       ToolName, " +
                "       AllowChange, " +
                "       IsSelectable, " +
                "       IsVisible, " +
                "       StudentControl, " +
                "       IsFunctional, " +
                "       RTSFieldName, " +
                "       IsRequired, " +
                "       TIDESelectable," +
                "       TIDESelectableBySubject) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   Context, \n" +
                "   ContextType, \n" +
                "   ToolName, \n" +
                "   AllowChange, \n" +
                "   IsSelectable, \n" +
                "   IsVisible, \n" +
                "   StudentControl, \n" +
                "   IsFunctional, \n" +
                "   RtsFieldName, \n" +
                "   IsRequired, \n" +
                "   TideSelectable, \n" +
                "   TideSelectableBySubject \n";
        }

        public override void Insert(IList<TestToolTypeDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
