using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>GradeDTO</code>s to the <code>OSS_Configs..Client_Grade</code> table
    /// </summary>
    public class GradeDAO : TestPackageDaoBase<GradeDTO>
    {
        public GradeDAO()
        {
            DbConnectionStringName = "configs";
            TvpVariableName = "@tvpClientGrades";
            TvpType = "GradeType";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_Grade (ClientName, GradeCode, Grade) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   GradeCode, \n" +
                "   Grade, \n" +
                "FROM \n" +
                TvpVariableName;
        }

        public override void Insert(IList<GradeDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
