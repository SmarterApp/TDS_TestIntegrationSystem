namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_TestScoreFeatures</code> table.
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Configs..Client_TestScoreFeatures</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class TestScoreFeatureDTO
    {
        public const string MEASURE_LABEL_THETA_SCORE = "thetascore"; // from line 47 of UpdateCOnfigs_Scorefeatures

        public string ClientName { get; set; }
        public string TestId { get; set; }
        public string MeasureOf { get; set; }
        public string MeasureLabel { get; set; }
        public bool UseForAbility { get; set; }
    }
}
