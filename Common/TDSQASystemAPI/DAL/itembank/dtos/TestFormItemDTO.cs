namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..TestFormItem</code> table
    /// </summary>
    public class TestFormItemDTO
    {
        public string ItemKey { get; set; } // maps to _fk_item in TestFormItem
        public string ITSFormKey { get; set; } // maps to _efk_ITSFormKey in TestFormItem
        public int FormPosition { get; set; }
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in TestFormItem
        public string TestFormKey { get; set; } // maps to _fk_testform in TestFormItem
        public bool IsActive { get; set; }
    }
}
