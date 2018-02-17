namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a record from the <code>OSS_Configs..TDS_TesteeAttribute</code> table.
    /// </summary>
    public class TesteeAttributeDTO
    {
        public string TdsAttributeId { get; set; }
        public string ClientName { get; set; }
        public string ArtId { get; set; }
        public string ReportName { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string AtLogin { get; set; }
        public int SortOrder { get; set; }
    }
}
