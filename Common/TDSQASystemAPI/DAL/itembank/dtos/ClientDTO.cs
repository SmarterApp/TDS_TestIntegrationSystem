namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblClient</code> table.
    /// </summary>
    public class ClientDTO
    {
        public long ClientKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HomePath { get; set; }
    }
}
