using System;

namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblItem</code> table
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..tblItem</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class ItemDTO
    {
        public long ItemBankKey { get; set; } // maps to _efk_itembank in tblItem
        public long ItemKey { get; set; } // maps to _efk_item in tblItem
        public string ItemType { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime DateLastUpdated => DateTime.UtcNow;
        public string ItemId { get; set; }
        public string Key { get; set; } // maps to _key in tblItem
        public long TestVersion { get; set; } // maps to loadconfig in tblItem
    }
}
