namespace TDSQASystemAPI.DAL.scoring.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TestScoringConfigs..ConversionTableDesc</code> table
    /// </summary>
    public class ConversionTableDescDTO
    {
        public string ConversionTableDescKey { get; set; } // maps to _Key in ConversionTableDesc
        public string TableName { get; set; }
        public string ClientName { get; set; } // maps to _fk_Client in ConversionTableDesc
    }
}
