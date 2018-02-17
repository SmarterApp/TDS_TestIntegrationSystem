namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_Subject</code> table
    /// </summary>
    public class SubjectDTO
    {
        public string SubjectCode { get; set; }
        public string Subject { get; set; }
        public string ClientName { get; set; }
        public string Origin { get; set; }
    }
}
