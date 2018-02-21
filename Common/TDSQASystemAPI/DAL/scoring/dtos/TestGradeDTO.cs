namespace TDSQASystemAPI.DAL.scoring.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TestScoringConfigs..TestGrades</code> table
    /// </summary>
    public class TestGradeDTO
    {
        public string ClientName { get; set; }
        public string TestId { get; set; }
        public string ReportingGrade { get; set; }
    }
}
