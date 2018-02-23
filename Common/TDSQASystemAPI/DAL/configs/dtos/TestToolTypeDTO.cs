namespace TDSQASystemAPI.DAL.configs.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Configs..Client_TestToolType</code> table.
    /// </summary>
    /// <remarks>
    /// This class is a subset of all the fields on the <code>OSS_Configs..Client_TestToolType</code> table; fields that are not referenced by an INSERT/UPDATE 
    /// in <code>OSS_Itembank.tp.spLoader_Main</code> (or the procedures it calls) aare not included.
    /// </remarks>
    public class TestToolTypeDTO
    {
        // Constants come from lines 338 - 339, 358 of UpdateTDSConfigs
        public const string GLOBAL_CONTEXT = "*"; // from line 358 of UpdateTDSConfigs
        public const string CONTEXT_TYPE_DEFAULT = "TEST";
        public const bool IS_SELECTABLE_DEFAULT = false;
        public const bool IS_VISIBLE_DEFAULT = false;
        public const bool STUDENT_CONTROL_DEFAULT = false;
        public const bool IS_FUNCTIONAL_DEFAULT = false;
        public const bool IS_REQUIRED_DEFAULT = false;
        public const bool TIDE_SELECTABLE_DEFAULT = false;
        public const bool TIDE_SELECTABLE_BY_SUBJECT_DEFAULT = false;

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

        /// <summary>
        /// When <code>UpdateTDSConfigs</code> stored procedure runs, these are the values that are hard-coded into
        /// the record that is ultimately inserted into <code>OSS_Configs..Client_TestToolType</code>.  This constructor
        /// builds a <code>TestToolTypeDTO</code> that will look like the records that are inserted on lines 338 - 339.
        /// </summary>
        public TestToolTypeDTO()
        {
            ContextType = CONTEXT_TYPE_DEFAULT;
            IsSelectable = IS_SELECTABLE_DEFAULT;
            IsVisible = IS_VISIBLE_DEFAULT;
            StudentControl = STUDENT_CONTROL_DEFAULT;
            IsFunctional = IS_FUNCTIONAL_DEFAULT;
            IsRequired = IS_REQUIRED_DEFAULT;
            TideSelectable = TIDE_SELECTABLE_DEFAULT;
            TideSelectableBySubject = TIDE_SELECTABLE_BY_SUBJECT_DEFAULT;
        }
    }
}
