namespace TDSQASystemAPI.TestPackage.utils
{
    /// <summary>
    /// Identify and cateogrize the various types of <code>BlueprintElement</code>s that can be specified in a 
    /// <code>TestPackage</code>.
    /// </summary>
    public class BlueprintElementTypes
    {
        public const string STRAND = "strand";
        public const string CONTENT_LEVEL = "contentlevel";
        public const string TARGET = "target";
        public const string CLAIM = "claim";
        public const string SOCK = "sock";
        public const string AFFINITY_GROUP = "affinitygroup";

        /// <summary>
        /// A superset of all types of <code>BlueprintElement</code>s that describe either a claim or a target.
        /// </summary>
        public static readonly string[] CLAIM_AND_TARGET_TYPES = new string[] { STRAND, CONTENT_LEVEL, TARGET, CLAIM };

        /// <summary>
        /// A collection of all types of <code>BlueprintElement</code>s that descrbie a claim.
        /// </summary>
        public static readonly string[] CLAIM_TYPES = new string[] { STRAND, CLAIM };

        /// <summary>
        /// A collection of all types of <code>BlueprintElement</code>s that descrbie a claim.
        /// </summary>
        public static readonly string[] TARGET_TYPES = new string[] { CONTENT_LEVEL, TARGET };
    }
}
