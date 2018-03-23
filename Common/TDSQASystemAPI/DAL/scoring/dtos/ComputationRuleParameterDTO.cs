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
        public string Type { get; set; }
        public override bool Equals(object obj)
        {
            var computationRuleParameterDTO = obj as ComputationRuleParameterDTO;

            if (computationRuleParameterDTO == null)
            {
                return false;
            }

            return this.ComputationRule.Equals(computationRuleParameterDTO.ComputationRule) &&
                this.ParameterName.Equals(computationRuleParameterDTO.ParameterName) && 
                this.ParameterPosition.Equals(computationRuleParameterDTO.ParameterPosition) &&
                this.IndexType.Equals(computationRuleParameterDTO.IndexType) &&
                this.Type.Equals(computationRuleParameterDTO.Type);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                if (ComputationRule != null)
                {
                    hash = hash * 23 + ComputationRule.GetHashCode();
                }
                if (ParameterName != null)
                {
                    hash = hash * 23 + ParameterName.GetHashCode();
                }
                hash = hash * 23 + ParameterPosition.GetHashCode();
                if (IndexType != null)
                {
                    hash = hash * 23 + IndexType.GetHashCode();
                }
                if (Type != null)
                {
                    hash = hash * 23 + Type.GetHashCode();
                }
                return hash;
            }
        }
    }
}
