namespace TDSQASystemAPI.DAL.scoring.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TestScoringConfigs..Test</code> table
    /// </summary>
    public class TestDTO
    {
        public string ClientName { get; set; }
        public string TestId { get; set; }
        public string Subject { get; set; } // maps to _efk_Subject in Test
    }
}
