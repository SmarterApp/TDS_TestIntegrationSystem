using System;

namespace TDSQASystemAPI.DAL.configs.dtos
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
        public const string MODE_ONLINE = "online"; // from line 162 of UpdateTDSConfigs
        public const string MODE_PAPER = "paper"; // from line 170 of UpdateTDSConfigs
        public const int SESSION_TYPE_ONLINE = 0; // from line 162 of UpdateTDSConfigs
        public const int SESSION_TYPE_PAPER = 1; // from line 170 of UpdateTDSConfigs

        public string ClientName { get; set; }
        public string TestId { get; set; }
        public string TestKey { get; set; }
        public string Mode => TestKey.IndexOf("online", StringComparison.OrdinalIgnoreCase) >= 0
                    ? MODE_ONLINE
                    : MODE_PAPER;
        public string Algorithm { get; set; }
        public bool IsSegmented { get; set; }
        public int SessionType => TestKey.IndexOf("online", StringComparison.OrdinalIgnoreCase) >= 0
            ? SESSION_TYPE_ONLINE
            : SESSION_TYPE_PAPER;
    }
}
