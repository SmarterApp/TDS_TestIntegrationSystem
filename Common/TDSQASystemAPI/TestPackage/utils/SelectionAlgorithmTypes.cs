namespace TDSQASystemAPI.TestPackage.utils
{
    /// <summary>
    /// Describe the available selection algorithms for a <code>TestPackage</code>.
    /// </summary>
    public sealed class SelectionAlgorithmTypes
    {
        /// <summary>
        /// The descriptor for a test that serves items using the adaptive2 algorithm
        /// </summary>
        public const string ADAPTIVE = "adaptive2";

        /// <summary>
        /// The descriptor for a test that serves items using the fixed-form algorithm
        /// </summary>
        public const string FIXED_FORM = "fixedform";

        /// <summary>
        /// The descriptor for an assessment that contains mulitple segments, with each segment having its own selection algorithm definition.
        /// </summary>
        public const string VIRTUAL = "virtual";
        
        /// <summary>
        /// Find the appropriate selction algorithm descriptor based on a string value.
        /// </summary>
        /// <param name="algorithm">The <code>string</code> to evaluate.</param>
        /// <returns>The correct algorithm descriptor based on the string passed in.  If no definition matches, the original string will be returned, 
        /// in lower-case and trimmed.</returns>
        public static string GetSelectionAlgorithm(string algorithm)
        {
            var sanitizedAlgorithm = algorithm.ToLower().Trim();
            if (sanitizedAlgorithm.Contains("adaptive"))
            {
                return ADAPTIVE;
            }

            if (sanitizedAlgorithm.Equals(FIXED_FORM))
            {
                return FIXED_FORM;
            }

            if (sanitizedAlgorithm.Equals(VIRTUAL))
            {
                return VIRTUAL;
            }

            return sanitizedAlgorithm;
        }
    }
}
