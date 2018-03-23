namespace TDSQASystemAPI.DAL.osstis.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TIS..CombinationTestMap</code> table.
    /// </summary>
    public class CombinationTestMapDTO
    {
        public string ComponentTestName { get; set; }
        public string ComponentSegmentName { get; set; }
        public string CombinationTestName { get; set; }
        public string CombinationSegmentName { get; set; }
    }
}
