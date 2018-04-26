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
        public int NumItemsRequired { get; set; }
        public int MaxItems { get; set; } 
        public long TestVersion { get; set; } // maps to loadconfig in tblAdminStimulus
        public long UpdatedTestVersion { get; set; }
        public string GroupId { get; set; }
        public override bool Equals(object obj)
        {
            var adminStimulusDTO = obj as AdminStimulusDTO;

            if (adminStimulusDTO == null)
            {
                return false;
            }

            return this.SegmentKey.Equals(adminStimulusDTO.SegmentKey) &&                
                this.StimulusKey.Equals(adminStimulusDTO.StimulusKey);
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
                if (StimulusKey != null)
                {
                    hash = hash * 23 + StimulusKey.GetHashCode();
                }
                return hash;
            }
        }

    }
}
