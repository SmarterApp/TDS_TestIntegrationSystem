namespace TDSQASystemAPI.DAL.configs
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_Test_ItemConstraint</code> table.
    /// </summary>
    public class TestItemConstraintDTO
    {
        public string ClientName { get; set; }
        public string TestId { get; set; }
        public string PropName { get; set; }
        public string PropValue { get; set; }
        public string ToolType { get; set; }
        public string ToolValue { get; set; }
        public bool ItemIn { get; set; }
    }
}
