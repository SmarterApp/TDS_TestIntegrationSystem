namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_FieldTestPriority</code> table
    /// </summary>
    public class FieldTestPriorityDTO
    {
        public const string DEFAULT_TEST_ID = "*"; // from line 124 of UpdateTDSConfigs

        public FieldTestPriorityDTO()
        {
            TestId = DEFAULT_TEST_ID;
        }

        public string ClientName { get; set; }
        public string TdsId { get; set; }
        public int Priority { get; set; }
        public string TestId { get; set; }
    }
}
