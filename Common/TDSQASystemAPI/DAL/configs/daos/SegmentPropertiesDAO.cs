using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>SegmentPropertiesDTO</code>s to the <code>OSS_Configs..Client_SegmentProperties</code> table
    /// </summary>
    public class SegmentPropertiesDAO : TestPackageDaoBase<SegmentPropertiesDTO>
    {
        public SegmentPropertiesDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "SegmentPropertiesTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_SegmentProperties (ClientName, SegmentID, SegmentPosition, ParentTest, IsPermeable, EntryApproval, ExitApproval, ItemReview, Label, ModeKey) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   SegmentId, \n" +
                "   SegmentPosition, \n" +
                "   ParentTest, \n" +
                "   IsPermeable, \n" +
                "   EntryApproval, \n" +
                "   ExitApproval, \n" +
                "   ItemReview, \n" +
                "   Label, \n" +
                "   ModeKey \n";
        }
    }
}
