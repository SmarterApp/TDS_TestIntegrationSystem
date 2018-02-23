namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..tblItemSelectionParm</code> table
    /// </summary>
    /// <remarks>
    /// <para>
    /// Because there is a default that will automatically generate a new GUID when inserting into the
    /// <code>tblItemSelectionParm</code> table, the <code>_Key</code> field is not represented here.  The assumption is
    /// the default on the <code>_Key</code> column will handle generating the new primary key value.
    /// </para>
    /// <para>
    /// The records that get inserted into this table are only related to assessments that have the "adaptive" 
    /// selection algorithm (tp.spLoad_AdminSubjects, line 377)
    /// </para>
    /// </remarks>
    public class ItemSelectionParmDTO
    {
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in tblItemSelectionParm
        public string BlueprintElementId { get; set; } // maps to bpElementID in tblItemSelectionParm
        public string PropertyName { get; set; } // maps to name in tblItemSelectionParm
        public string PropertyValue { get; set; } // maps to value in tblItemSelectionParm
        public string PropertyLabel { get; set; } // maps to label in tblItemSelectionParm
    }
}
