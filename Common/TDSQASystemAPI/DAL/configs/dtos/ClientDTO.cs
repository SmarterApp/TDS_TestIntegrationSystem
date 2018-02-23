namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client</code> table
    /// </summary>
    public class ClientDTO
    {
        public string Name { get; set; }
        public bool Internationalize { get; set; }
        public string DefaultLanguage { get; set; }
    }
}
