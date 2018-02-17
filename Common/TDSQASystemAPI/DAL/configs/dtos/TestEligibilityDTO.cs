namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_TestEligibility</code> table.
    /// </summary>
    public class TestEligibilityDTO
    {
        public string ClientName { get; set; }
        public string TestId { get; set; }
        public string RtsName { get; set; }
        public bool Enables { get; set; }
        public bool Disables { get; set; }
        public string RtsValue { get; set; }
        public long EntityType { get; set; } // _efk_EntityType
        public string EligibilityType { get; set; }
        public int MatchType { get; set; }
    }
}
