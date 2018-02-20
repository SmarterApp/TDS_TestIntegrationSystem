namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblSetOfItemStimuli</code> table
    /// </summary>
    public class SetOfItemStimuliDTO
    {
        public string ItemKey { get; set; } // maps to _fk_item in tblSetOfItemStimuli
        public string StimulusKey { get; set; } // maps to _fk_stimulus in tblSetOfItemStimli
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in tblSetOfItemStimuli
        public long TestVersion { get; set; } // maps to loadconfig in tblSetOfItemStimuli
    }
}
