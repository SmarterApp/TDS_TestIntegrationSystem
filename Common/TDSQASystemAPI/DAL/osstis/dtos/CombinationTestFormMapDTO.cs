namespace TDSQASystemAPI.DAL.osstis.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TIS..CombinationTestFormMap</code> table.
    /// </summary>
    public class CombinationTestFormMapDTO
    {
        public string ComponentSegmentName { get; set; }
        public string ComponentFormKey { get; set; }
        public string CombinationFormKey { get; set; }
    }
}
