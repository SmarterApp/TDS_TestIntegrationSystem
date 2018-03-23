namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..AffinityGroupItem</code> table
    /// </summary>
    public class AffinityGroupItemDTO
    {
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in AffinityGroupItem

        /// <summary>
        /// The identifier of the <code>AffinityGroup</code> to which this item belongs.
        /// </summary>
        public string GroupId { get; set; } 
        public string ItemKey { get; set; }
    }
}
