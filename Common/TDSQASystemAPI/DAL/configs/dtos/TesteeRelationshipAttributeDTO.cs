namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..TDS_TesteeRelationshipAttribute</code> table.
    /// </summary>
    public class TesteeRelationshipAttributeDTO
    {
        public string ClientName { get; set; }
        public string TdsId { get; set; }
        public string RtsName { get; set; }
        public string Label { get; set; }
        public string ReportName { get; set; }
        public string AtLogin { get; set; }
        public int SortOrder { get; set; }
        public string RelationshipType { get; set; }
    }
}
