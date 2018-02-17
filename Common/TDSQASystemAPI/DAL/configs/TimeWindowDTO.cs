using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDSQASystemAPI.DAL.configs
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
