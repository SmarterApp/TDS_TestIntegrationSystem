namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..PerformanceLevel</code> table
    /// </summary>
    public class PerformanceLevelDTO
    {
        public string ContentKey { get; set; } // maps to _fk_content in PerformanceLevels
        public int PLevel { get; set; }
        public double ThetaLo { get; set; }
        public double ThetaHigh { get; set; }
        public double ScaledLo { get; set; }
        public double ScaledHi { get; set; }
    }
}
