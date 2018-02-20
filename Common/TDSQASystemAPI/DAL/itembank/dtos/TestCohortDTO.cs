namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..TestCohort</code> table
    /// </summary>
    public class TestCohortDTO
    {
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in TestCohort
        public string Cohort { get; set; }
        public double ItemRatio { get; set; }
    }
}
