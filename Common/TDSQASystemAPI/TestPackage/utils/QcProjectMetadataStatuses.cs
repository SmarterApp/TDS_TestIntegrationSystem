namespace TDSQASystemAPI.TestPackage.utils
{
    /// <summary>
    /// Represents the various statuses for the records in the <code>OSS_TIS..QC_ProjectMetaData</code>.
    /// </summary>
    /// <remarks>
    /// These status values are appeneded to the end of the <code>VarName</code> field.
    /// </remarks>
    public class QcProjectMetadataStatuses
    {
        public const string COMPLETED = "completed";
        public const string EXPIRED = "expired";
        public const string INVALIDATED = "invalidated";
        public const string REPORTED = "reported";
        public const string RESET = "reset";
        public const string SCORED = "scored";
        public const string SUBMITTED = "submitted";
    }
}
