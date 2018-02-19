using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TimeWindowDTO</code>s to the <code>OSS_Configs..Client_TimeWindow</code> table
    /// </summary>
    public class TimeWindowDAO : TestPackageDaoBase<TimeWindowDTO>
    {
        public TimeWindowDAO()
        {
            DbConnectionStringName = "configs";
            TvpType = "TimeWindowType";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TimeWindow (ClientName, WindowID, StartDate, EndDate) \n" +
                "SELECT" +
                "   ClientName, \n" +
                "   WindowId, \n" +
                "   StartDate, \n" +
                "   EndDate \n";
        }

        public override void Insert(IList<TimeWindowDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
