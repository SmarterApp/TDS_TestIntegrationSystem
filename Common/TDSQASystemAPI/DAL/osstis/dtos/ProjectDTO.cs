namespace TDSQASystemAPI.DAL.osstis.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TIS..Projects</code> table.
    /// </summary>
    public class ProjectDTO
    {
        public int ProjectKey { get; set; } // maps to _Key
        public string Description { get; set; }
    }
}
