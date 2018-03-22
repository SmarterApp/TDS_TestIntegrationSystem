namespace TDSQASystemAPI.DAL.osstis.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_TIS..QC_ProjectMetaData</code> table.
    /// </summary>
    public class QcProjectMetadataDTO
    {
        public int ProjectId { get; set; } // maps to _fk_ProjectID in OSS_TIS..QC_ProjectMetaData
        public string GroupName { get; set; }
        public string VarName { get; set; }
        public long IntValue { get; set; }
        public decimal FloatValue { get; set; }
        public string TextValue { get; set; }
        public string Comment { get; set; }
    }
}
