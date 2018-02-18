using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>FieldTestPriorityDTO</code>s to the <code>OSS_Configs..Client_FieldTestPriority</code> table
    /// </summary>
    public class FieldTestPriorityDAO : TestPackageDaoBase<FieldTestPriorityDTO>
    {
        public FieldTestPriorityDAO()
        {
            DbConnectionStringName = "configs";
            TvpVariableName = "@tvpFieldTestPriorities";
            TvpType = "FieldTestPriorityType";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_FieldtestPriority (clientname, TDS_ID, priority, TestID) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TdsId, \n" +
                "   Priority, \n" +
                "   TestId \n" +
                "FROM \n" +
                TvpVariableName;
        }

        public override void Insert(IList<FieldTestPriorityDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
