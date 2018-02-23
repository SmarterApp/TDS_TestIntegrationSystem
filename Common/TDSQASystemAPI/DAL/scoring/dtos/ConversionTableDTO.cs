namespace TDSQASystemAPI.DAL.scoring.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TestScoringConfigs..ConversionTables</code> table
    /// </summary>
    public class ConversionTableDTO
    {
        public string TableName { get; set; }
        public int InValue { get; set; }
        public double OutValue { get; set; }
        public string ClientName { get; set; }
    }
}
