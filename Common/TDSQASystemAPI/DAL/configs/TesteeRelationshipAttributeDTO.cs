namespace TDSQASystemAPI.DAL.configs
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..TDS_TesteeRelationshipAttribute</code> table.
    /// </summary>
    public class TesteeRelationshipAttributeDTO
    {
        public string ClientName { get; set; }
        public string TtdsId { get; set; }
        public string RelationshipType { get; set; }
        public string ArtId { get; set; }
        public string Label { get; set; }
        public string AtLogin { get; set; }
        public string ReportName { get; set; }
    }
}
