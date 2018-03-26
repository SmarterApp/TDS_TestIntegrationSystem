namespace TDSQASystemAPI.Utilities
{
    /// <summary>
    /// A list of error/warning codes for the <code>ValidationError</code>.
    /// </summary>
    public class ValidationErrorCodes
    {
        /// <summary>
        /// Indicates some items were not loaded into TIS because they already exist.
        /// </summary>
        public const string EXISTING_ITEMS_NOT_LOADED = "existingItems";
    }
}
