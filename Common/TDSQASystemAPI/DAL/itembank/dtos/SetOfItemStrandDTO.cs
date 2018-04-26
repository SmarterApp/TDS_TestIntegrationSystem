namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblSetOfItemStrands</code> table
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..tblSetOfItemStrands</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class SetOfItemStrandDTO
    {
        public string ItemKey { get; set; } // maps to _fk_item in tblSetOfItemStrands
        public string StrandKey { get; set; } // maps to _fk_strand in tblSetOfItemStrands
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in tblSetOfItemStrands
        public long TestVersion { get; set; } // maps to loadconfig in tblSetOfItemStrands

        public override bool Equals(object obj)
        {
            var setOfItemStrandDTO = obj as SetOfItemStrandDTO;

            if (setOfItemStrandDTO == null)
            {
                return false;
            }

            return this.ItemKey.Equals(setOfItemStrandDTO.ItemKey) &&
                this.StrandKey.Equals(setOfItemStrandDTO.StrandKey) &&
                this.SegmentKey.Equals(setOfItemStrandDTO.SegmentKey);
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
                if (StrandKey != null)
                {
                    hash = hash * 23 + StrandKey.GetHashCode();
                }
                return hash;
            }
        }
    }
}
