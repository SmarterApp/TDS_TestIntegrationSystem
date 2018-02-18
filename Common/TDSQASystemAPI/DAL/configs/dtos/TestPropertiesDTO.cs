namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_TestProperties</code> table.
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Configs..Client_TestProperties</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class TestPropertiesDTO
    {
        public string ClientName { get; set; }
        public string TestId { get; set; }
        public bool IsSelectable { get; set; }
        public string Label { get; set; }
        public string SubjectName { get; set; }
        public int MaxOpportunities { get; set; }
        public bool ScoreByTds { get; set; }
        public string AccommodationFamily { get; set; }
        public string ReportingInstrument { get; set; }
        public string GradeText { get; set; }
        public string TideId => string.IsNullOrEmpty(ReportingInstrument)
            ? string.Format("{0}-{1}", ReportingInstrument, SubjectName)
            : null;
    }
}
