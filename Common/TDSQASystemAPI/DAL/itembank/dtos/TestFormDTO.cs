namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..TestForm</code> table
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..TestForm</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class TestFormDTO
    {
        public const string COHORT_DEFAULT = "Default";

        public string SegmentKey { get; set; } // maps to _fk_adminsubject in TestForm
        public string Cohort => COHORT_DEFAULT; // logic from line 25 of spLoad_AdminForms
        public string Language { get; set; }
        public string TestFormKey { get; set; } // maps to _key in TestForm
        public string FormId { get; set; }
        public long ITSBankKey { get; set; } // maps to _efk_ITSBank in TestForm
        public long ITSKey { get; set; } // maps to _efk_ITSKey in TestForm
        public long TestVersion { get; set; }
    }
}
