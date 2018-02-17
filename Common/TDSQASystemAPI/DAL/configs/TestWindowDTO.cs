using System;

namespace TDSQASystemAPI.DAL.configs
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_TestWindow</code> table.
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Configs..Client_TestWindow</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class TestWindowDTO
    {
        public string ClientName { get; set; }
        public string TestId { get; set; }
        public string WindowID { get; set; }
        public int NumOpps { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
