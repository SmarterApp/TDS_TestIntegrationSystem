using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestWindowDTO</code>s to the <code>OSS_Configs..Client_TestWindow</code> table
    /// </summary>
    public class TestGradesDAO : TestPackageDaoBase<TestGradeDTO>
    {
        public TestGradesDAO()
        {
            DbConnectionStringName = "configs";
            TvpType = "TestGradeType";
            TvpVariableName = "@tvpTestGrades";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TestGrades (ClientName, TestId, Grade) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   Grade, \n" +
                "FROM \n" +
                TvpVariableName;
        }

        public override void Insert(IList<TestGradeDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
