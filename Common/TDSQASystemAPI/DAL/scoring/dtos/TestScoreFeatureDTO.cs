using System;

namespace TDSQASystemAPI.DAL.scoring.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TestScoringConfigs..TestScoreFeature</code> table
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_TestScoringConfigs..TestScoreFeature</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class TestScoreFeatureDTO
    {
        public Guid TestScoreFeatureKey { get; set; } // maps to _Key in TestScoreFeature
        public string ClientName { get; set; }
        public string TestId { get; set; }
        public string MeasureOf { get; set; }
        public string MeasureLabel { get; set; }
        public bool IsScaled { get; set; }
        public string ComputationRule { get; set; }
        public int ComputationOrder { get; set; }
    }
}
