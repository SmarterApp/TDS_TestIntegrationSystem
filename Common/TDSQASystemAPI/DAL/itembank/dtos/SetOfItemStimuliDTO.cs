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

        public override bool Equals(object obj)
        {
            var setOfItemStimuliDTO = obj as SetOfItemStimuliDTO;

            if (setOfItemStimuliDTO == null)
            {
                return false;
            }

            return this.SegmentKey.Equals(setOfItemStimuliDTO.SegmentKey) &&
                this.ItemKey.Equals(setOfItemStimuliDTO.ItemKey) &&
                this.StimulusKey.Equals(setOfItemStimuliDTO.StimulusKey);
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
                if (StimulusKey != null)
                {
                    hash = hash * 23 + StimulusKey.GetHashCode();
                }
                return hash;
            }
        }
    }
}
