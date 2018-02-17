namespace TDSQASystemAPI.DAL.configs
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_TestToolType</code> table.
    /// </summary>
    /// /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Configs..Client_TestToolType</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class TestToolType
    {
        public string ClientName { get; set; }
        public string Context { get; set; }
        public string ContextType { get; set; }
        public string ToolName { get; set; }
        public bool AllowChange { get; set; }
        public bool IsSelectable { get; set; }
        public bool IsVisible { get; set; }
        public bool StudentControl { get; set; }
        public bool IsFunctional { get; set; }
        public string RtsFieldName { get; set; }
        public bool IsRequired { get; set; }
        public bool TideSelectable { get; set; }
        public bool TideSelectableBySubject { get; set; }
    }
}
