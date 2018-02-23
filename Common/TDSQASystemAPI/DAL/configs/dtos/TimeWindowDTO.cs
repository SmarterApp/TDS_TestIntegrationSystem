using System;

namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_TimeWindow</code> table.
    /// </summary>
    public class TimeWindowDTO
    {
        public const string DEFAULT_WINDOW_ID = "ANNUAL"; // line 117 of UpdateTDSConfigs

        public TimeWindowDTO()
        {
            WindowId = DEFAULT_WINDOW_ID;
        }

        public string ClientName { get; set; }
        public string WindowId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
