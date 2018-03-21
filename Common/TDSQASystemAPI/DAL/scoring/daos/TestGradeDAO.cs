using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
            ExistsSql = "SELECT count(*) FROM dbo.TestGrades t WHERE t.ClientName = @clientName AND t.TestID = @testID AND t.reportingGrade = @reportingGrade";
        }

        override protected void ExistsAddParameter(TestGradeDTO testGradeDTO, SqlParameterCollection parameters)
        {
            parameters.AddWithValue("@clientName", testGradeDTO.ClientName);
            parameters.AddWithValue("@testID", testGradeDTO.TestId);
            parameters.AddWithValue("@reportingGrade", testGradeDTO.ReportingGrade);
        }
    }
}
