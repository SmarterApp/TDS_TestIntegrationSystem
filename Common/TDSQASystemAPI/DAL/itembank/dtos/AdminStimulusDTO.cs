namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblAdminStimulus</code> table.
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..tblAdminStimulus</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class AdminStimulusDTO
    {
        public string StimulusKey { get; set; } // maps to _fk_stimulus in tblAdminStimulus
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in tblAdminStimulus
        public int NumOfItemsRequired { get; set; }
        public int MaxItems { get; set; } 
        public long TestVersion { get; set; } // maps to loadconfig in tblAdminStimulus
        public long UpdatedTestVersion { get; set; }
    }
}
