using System;

namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_TimeWindow</code> table.
    /// </summary>
    public class TimeWindowDTO
    {
        public string clientName { get; set; }
        public string windowId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}
