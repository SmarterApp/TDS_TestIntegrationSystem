namespace TDSQASystemAPI.DAL.configs
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_FieldTestPriority</code> table
    /// </summary>
    public class FieldTestPriorityDTO
    {
        public string TdsId { get; set; }
        public string ClientName { get; set; }
        public int Priority { get; set; }
        public string TestId { get; set; }
    }
}
