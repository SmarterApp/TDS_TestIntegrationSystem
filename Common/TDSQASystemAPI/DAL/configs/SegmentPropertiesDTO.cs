namespace TDSQASystemAPI.DAL.configs
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_SegmentProperties</code> table.
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Configs..Client_SegmentProperties</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class SegmentPropertiesDTO
    {
        public string ClientName { get; set; }
        public string SegmentId { get; set; }
        public string SegmentPosition { get; set; }
        public string ParentTest { get; set; }
        public int IsPermeable { get; set; }
        public int EntryApproval { get; set; }
        public int ExitApproval { get; set; }
        public bool ItemReview { get; set; }
        public string Label { get; set; }
        public string ModeKey { get; set; }
    }
}
