using System;

namespace TDSQASystemAPI.DAL.scoring.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TestScoringConfigs..ComputationRuleParameters</code> table
    /// </summary>
    public class ComputationRuleParameterDTO
    {
        public Guid ComputationRuleParameterKey { get; set; } // maps to _Key in ComputationRuleParameters
        public string ComputationRule { get; set; }
        public string ParameterName { get; set; }
        public int ParameterPosition { get;  set; }
        public string IndexType { get; set; }
        public String Type { get; set; }
    }
}
