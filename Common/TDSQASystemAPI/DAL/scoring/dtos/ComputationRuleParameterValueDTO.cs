using System;

namespace TDSQASystemAPI.DAL.scoring.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TestScoringConfigs..ComputationRuleParameterValue</code> table
    /// </summary>
    public class ComputationRuleParameterValueDTO
    {
        public Guid TestScoreFeatureKey { get; set; } // maps to _fk_TestScoreFeature in ComputationRuleParameterValue
        public Guid ComputationRuleParameterKey { get; set; }
        public string Index { get; set; }
        public string Value { get; set; }
    }
}
