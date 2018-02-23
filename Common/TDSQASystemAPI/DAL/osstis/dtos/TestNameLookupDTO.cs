namespace TDSQASystemAPI.DAL.osstis.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TIS..TestNameLookup</code> table.
    /// </summary>
    public class TestNameLookupDTO
    {
        public string InstanceName { get; set; }
        public string TestName { get; set; }
    }
}
