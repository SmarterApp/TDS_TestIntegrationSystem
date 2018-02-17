using System;

namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..ClientTestFormProperties</code> table.
    /// </summary>
    public class TestFormPropertiesDTO
    {
        public string ClientName { get; set; }
        public string TestFormKey { get; set; } // _efk_TestForm
        public string FormId { get; set; }
        public string TestId { get; set; }
        public string Language { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Testkey { get; set; }
    }
}
