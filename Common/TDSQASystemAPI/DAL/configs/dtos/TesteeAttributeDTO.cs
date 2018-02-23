namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a record from the <code>OSS_Configs..Client_TesteeAttribute</code> table.
    /// </summary>
    public class TesteeAttributeDTO
    {
        public string RtsName { get; set; }
        public string TdsId { get; set; } // maps to TDS_ID in 
        public string ClientName { get; set; }
        public string ReportName { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string AtLogin { get; set; }
        public int SortOrder { get; set; }
    }
}
