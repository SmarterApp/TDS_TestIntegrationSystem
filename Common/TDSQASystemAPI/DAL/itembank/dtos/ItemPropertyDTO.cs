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
    }
}
