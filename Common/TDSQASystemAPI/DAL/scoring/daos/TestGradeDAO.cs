using System.Collections.Generic;
using TDSQASystemAPI.DAL.scoring.dtos;

namespace TDSQASystemAPI.DAL.scoring.daos
{
    /// <summary>
    /// A class for saving <code>TestGradeDTO</code>s to the <code>OSS_TestScoringConfigs..TestGrades</code> table
    /// </summary>
    public class TestGradeDAO : TestPackageDaoBase<TestGradeDTO>
    {
        public TestGradeDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.SCORING;
            TvpType = "TestGradeTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.TestGrades (ClientName, TestId, ReportingGrade) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   ReportingGrade \n";
        }

        public override void Insert(IList<TestGradeDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
