using System;

namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblStimulus</code> table
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..tblStimulus</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class StimulusDTO
    {
        public long ItemBankKey { get; set; } // maps to _efk_itembank in tblStimulus
        public long ItsKey { get; set; } // maps to _efk_ITSKey in tblStimulus
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime DateLastUpdated { get; set; } // logic from line 39 of spLoad_Stimuli sets this to "now"
        public string StimulusKey { get; set; } // maps to _key in tblStimulus
        public long TestVersion { get; set; } // maps to loadConfig in tblStimulus
    }
}
