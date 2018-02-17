using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDSQASystemAPI.DAL.configs
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_Grade</code> table.
    /// </summary>
    public class GradeDTO
    {
        public string GradeCode { get; set; }
        public string Grade { get; set; }
        public string ClientName { get; set; }
        public string Origin { get; set; }
    }
}
