namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..AA_ItemCL</code> table
    /// <remarks>
    /// Referred to as "Targets"
    /// </remarks>
    /// </summary>
    public class AaItemClDTO
    {
        public string SegmentKey { get; set; } // maps to _fk_AdminSubject in AA_ItemCL
        public string ItemKey { get; set; } // maps to _fk_item in AA_ItemCL
        public string ContentLevel { get; set; }

        public override bool Equals(object obj)
        {
            var aaItemClDTO = obj as AaItemClDTO;

            if (aaItemClDTO == null)
            {
                return false;
            }

            return this.SegmentKey.Equals(aaItemClDTO.SegmentKey) &&
                this.ItemKey.Equals(aaItemClDTO.ItemKey) &&
                this.ContentLevel.Equals(aaItemClDTO.ContentLevel);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                if (SegmentKey != null)
                {
                    hash = hash * 23 + SegmentKey.GetHashCode();
                }
                if (ItemKey != null)
                {
                    hash = hash * 23 + ItemKey.GetHashCode();
                }               
                if (ContentLevel != null)
                {
                    hash = hash * 23 + ContentLevel.GetHashCode();
                }
                return hash;
            }
        }
    }
}
