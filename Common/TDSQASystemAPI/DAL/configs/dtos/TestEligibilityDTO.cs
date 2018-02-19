namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_TestEligibility</code> table.
    /// </summary>
    public class TestEligibilityDTO
    {
        // Default values from lines 239 - 242 of UpdateTDSConfigs
        public const string RTS_NAME_DEFAULT = "EnrlGrdCd";
        public const bool ENABLES_DEFAULT = true;
        public const bool DISABLES_DEFAULT = false;
        public const long ENTITY_TYPE_DEFAULT = 6L;
        public const string ELIGIBILITY_TYPE_DEFAULT = "ATTRIBUTE";
        public const int MATCH_TYPE_DEFAULT = 0;

        public string ClientName { get; set; }
        public string TestId { get; set; }
        public string RtsName { get; set; }
        public bool Enables { get; set; }
        public bool Disables { get; set; }
        public string RtsValue { get; set; }
        public long EntityType { get; set; } // _efk_EntityType
        public string EligibilityType { get; set; }
        public int MatchType { get; set; }

        public TestEligibilityDTO()
        {
            RtsName = RTS_NAME_DEFAULT;
            Enables = ENABLES_DEFAULT;
            Disables = DISABLES_DEFAULT;
            EntityType = ENTITY_TYPE_DEFAULT;
            EligibilityType = ELIGIBILITY_TYPE_DEFAULT;
            MatchType = MATCH_TYPE_DEFAULT;

            // TODO: set value for RtsValue based on logic in CASE statement of line 241
        }
    }
}
