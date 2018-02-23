namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..AA_ItemCL</code> table
    /// </summary>
    public class AaItemClDTO
    {
        public string SegmentKey { get; set; } // maps to _fk_AdminSubject in AA_ItemCL
        public string ItemKey { get; set; } // maps to _fk_item in AA_ItemCL
        public string ContentLevel { get; set; }
    }
}
