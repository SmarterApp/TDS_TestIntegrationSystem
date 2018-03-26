namespace TDSQASystemAPI.Utilities
{
    /// <summary>
    /// Describe potential severity levels for a <code>ValidationError</code>.
    /// </summary>
    public class ValidationErrorSeverityLevels
    {
        /// <summary>
        /// A warning indicates the load into TIS did not fail, but some data was not loaded into TIS
        /// because it already exists.
        /// </summary>
        public const string WARN = "warning";

        /// <summary>
        /// An error indicates a part of loading the <code>TestPackage</code> into TIS failed due to some rule
        /// violation.
        /// </summary>
        public const string ERROR = "error";
    }
}
