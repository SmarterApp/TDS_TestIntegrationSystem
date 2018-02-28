using System.Collections.Generic;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>PresentationsPresentation</code> with additional 
    /// data.
    /// </summary>
    public partial class PresentationsPresentation
    {
        private readonly IDictionary<string, string> labelMap = new Dictionary<string, string>
        {
            { "ENU", "English" },
            { "ESN", "Spanish" },
            { "ENU-Braille", "Braille" }
        };

        /// <summary>
        /// Get the correct display label based on this presentation's label or code.
        /// </summary>
        /// <remarks>
        /// <para>
        /// THIS METHOD SHOULD BE USED INSTEAD OF THE label PROPERTY.
        /// </para>
        /// <para>
        /// There is no easy way to override a property in a partial class, so this method is used to 
        /// fetch the correct label.
        /// </para>
        /// </remarks>
        /// <returns>The correct display label</returns>
        public string GetLabel()
        {
            return label ?? labelMap[code];
        }
    }
}
