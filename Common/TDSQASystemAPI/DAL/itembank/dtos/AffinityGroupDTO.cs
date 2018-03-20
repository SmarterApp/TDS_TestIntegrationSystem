namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..AffinityGroup</code> table
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..AffinityGroup</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class AffinityGroupDTO
    {
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in AffinityGroup
        public string GroupId { get; set; }
        public int MinItems { get; set; }
        public int MaxItems { get; set; }
        public double? BlueprintWeight { get; set; } // maps to weight in AffinityGroup
        public bool IsStrictMax { get; set; }
        public long TestVersion { get; set; } // maps to loadconfig in AffinityGroup
        public long UpdatedTestVersion { get; set; } // maps to updateconfig in AffinityGroup
        public double? AbilityWeight { get; set; }
        public double? PrecisionTarget { get; set; }
        public double? StartAbility { get; set; }
        public double? StartInfo { get; set; }
        public double? PrecisionTargetMetWeight { get; set; }
        public double? PrecisionTargetNotMetWeight { get; set; }
    }
}
