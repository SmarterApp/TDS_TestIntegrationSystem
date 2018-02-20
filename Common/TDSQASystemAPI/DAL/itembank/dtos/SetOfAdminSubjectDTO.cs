namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblSetOfAdminSubjects</code> table
    /// </summary>
    /// /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Itembank..tblSetOfAdminSubjects</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class SetOfAdminSubjectDTO
    {
        public string SegmentKey { get; set; } // maps to _Key in tblSetOfAdminSubjects
        public string TestAdminKey { get; set; } // maps to _fk_testadmin in tblSetOfAdminSubjects
        public string SubjectKey { get; set; } // maps to _fk_subject in tblSetOfAdminSubjects
        public string TestId { get; set; }
        public double StartAbility { get; set; }
        public double StartInfo { get; set; }
        public int MinItems { get; set; }
        public int MaxItems { get; set; }
        public double Slope { get; set; }
        public double Intercept { get; set; }
        public int FieldTestStartPosition { get; set; } // maps to ftstartpos in tblSetOfAdminSubjects
        public int FieldTestEndPosition { get; set; } // maps to ftendpos in tblSetOfAdminSubjects
        public int FieldTestMinItems { get; set; } // maps to ftminitems in tblSetOfAdminSubjects
        public int FieldTestMaxItems { get; set; } // maps to ftmaxitems in tblSetOfAdminSubjects
        public string SelectionAlgorithm { get; set; }
        public double BlueprintWeight { get; set; }
        public int CSet1Size { get; set; }
        public int CSet2Random { get; set; }
        public int CSet2InitialRandom { get; set; }
        public string VirtualTest { get; set; }
        public int TestPosition { get; set; }
        public bool IsSegmented { get; set; }
        public double ItemWeight { get; set; } 
        public double AbilityOffset { get; set; }
        public string CSet1Order { get; set; }
        public long TestVersion { get; set; }  // maps to loadconfig in tblSetOfAdminSubjects
        public string Contract { get; set; }
        public string TestType { get; set; }
        public double PrecisionTarget { get; set; }
        public double AdaptiveCut { get; set; }
        public double TooCloseSEs { get; set; }
        public double AbilityWeight { get; set; }
        public bool ComputeAbilityWeights { get; set; }
        public double RcAbilityWeight { get; set; }
        public double PrecisionTargetMetWeight { get; set; }
        public double PrecisionTargetNotMetWeight { get; set; }
        public bool TerminationOverallInfo { get; set; }
        public bool TerminationRcInfo { get; set; }
        public bool TerminationMinCount { get; set; }
        public bool TerminationTooClose { get; set; }
        public bool TerminationFlagsAnd { get; set; }
        public string BlueprintMetricFunction { get; set; } // maps to bpmetricfunction in tblSetOfAdminSubjects
    }
}
