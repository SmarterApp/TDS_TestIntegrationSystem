using System;

namespace TDSQASystemAPI.DAL.scoring.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TestScoringConfigs..Feature_ComputationLocation</code> table
    /// </summary>
    public class FeatureComputationLocationDTO
    {
        public Guid TestScoreFeatureKey { get; set; } // maps to _fk_testscorefeature in Feature_ComputationLocation
        public string Location { get; set; }
    }
}
