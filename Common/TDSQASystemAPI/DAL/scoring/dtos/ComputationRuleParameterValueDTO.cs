using System;

namespace TDSQASystemAPI.DAL.scoring.dtos
{
    /// <summary>
    /// A class for saving <code>TestDTO</code>s to the <code>OSS_TestScoringConfigs..Test</code> table
    /// </summary>
    public class ComputationRuleParameterValueDTO
    {
        public Guid TestScoreFeatureKey { get; set; }
        public Guid ComputationRuleParameterKey { get; set; }
        public string Index { get; set; }
        public string Value { get; set; }
    }
}
