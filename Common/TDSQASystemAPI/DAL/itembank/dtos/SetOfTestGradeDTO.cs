namespace TDSQASystemAPI.DAL.itembank.dtos
{
    /// <summary>
    /// Represents a single record in the <code>OSS_Itembank..SetOfTestGrades</code> table
    /// </summary>
    /// <remarks>
    /// Because there is a default that will automatically generate a new GUID when inserting into the
    /// <code>SetOfTestGrades</code> table, the <code>_Key</code> field is not represented here.  The assumption is
    /// the default on the <code>_Key</code> column will handle generating the new primary key value.
    /// </remarks>
    public class SetOfTestGradeDTO
    {
        public string TestId { get; set; }
        public string Grade { get; set; }
        public bool RequireEnrollment { get; set; } // logic on line 62 of tp.spLoad_TestGrades always sets this to false
        public string SegmentKey { get; set; } // maps to _fk_adminsubject in dbo.SetOfTestGrades
        public string EnrolledSubject { get; set; } // logic on line 64 of tp.spLoad_TestGrades always sets this to null
    }
}
