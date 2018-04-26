namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblItemProps</code> table
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..tblItemProps</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class ItemPropertyDTO
    {
        public const string ITEM_TYPE_PROPERTY_NAME = "--ITEMTYPE--";

        public string ItemKey { get; set; } // maps to _fk_item in tblItemProps
        public string PropertyName { get; set; } // maps to propname in tblItemProps
        public string PropertyValue { get; set; } // maps to propvalue in tblItemProps
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in tblItemProps
        public bool IsActive { get; set; }

        public override bool Equals(object obj)
        {
            var itemPropertyDTO = obj as ItemPropertyDTO;

            if (itemPropertyDTO == null)
            {
                return false;
            }

            return this.SegmentKey.Equals(itemPropertyDTO.SegmentKey) &&
                this.ItemKey.Equals(itemPropertyDTO.ItemKey) &&
                this.PropertyName.Equals(itemPropertyDTO.PropertyName) &&
                this.PropertyValue.Equals(itemPropertyDTO.PropertyValue);
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
                if (PropertyName != null)
                {
                    hash = hash * 23 + PropertyName.GetHashCode();
                }
                if (PropertyValue != null)
                {
                    hash = hash * 23 + PropertyValue.GetHashCode();
                }
                return hash;
            }
        }
    }
}
