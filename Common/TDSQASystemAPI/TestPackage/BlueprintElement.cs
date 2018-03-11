using System.Linq;
using TDSQASystemAPI.TestPackage.utils;

namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>BlueprintElement</code> with additional 
    /// data.
    /// </summary>
    public partial class BlueprintElement
    {
        /// <summary>
        /// Indicate if this <code>BlueprintElement</code> is a "claim".
        /// </summary>
        /// <returns>True if the <code>BlueprintElement's</code> is one of the "claim" types; otherwise false.</returns>
        public bool IsClaim()
        {
            return BlueprintElementTypes.CLAIM_TYPES.Contains(type);
        }

        /// <summary>
        /// Indicate if this <code>BlueprintElement</code> is a "target".
        /// </summary>
        /// <returns>True if the <code>BlueprintElement's</code> is one of the "target" types; otherwise false.</returns>
        public bool IsTarget()
        {
            return BlueprintElementTypes.TARGET_TYPES.Contains(type);
        }

        /// <summary>
        /// Indicate if this <code>BlueprintElement</code> is a "claim" or a "target".
        /// </summary>
        /// <returns>True if the <code>BlueprintElement's</code> is one of the "claim" or "target" types; otherwise false.</returns>
        public bool IsClaimOrTarget()
        {
            return BlueprintElementTypes.CLAIM_AND_TARGET_TYPES.Contains(type);
        }
    }
}
