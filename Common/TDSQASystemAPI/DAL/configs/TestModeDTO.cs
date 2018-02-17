namespace TDSQASystemAPI.DAL.configs
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_TestMode</code> table.
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Configs..Client_TestMode</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class TestModeDTO
    {
        public string ClientName { get; set; }
        public string TestId { get; set; }
        public string TestKey { get; set; }
        public string Mode { get; set; }
        public string Algorithm { get; set; }
        public bool isSegmented { get; set; }
        public int SessionType { get; set; }
    }
}
