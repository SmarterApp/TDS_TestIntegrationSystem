namespace TDSQASystemAPI.BL.testpackage.osstis
{
    /// <summary>
    /// An interface for interacting with test package lookup data.
    /// </summary>
    public interface ITestNameLookupService
    {
        /// <summary>
        /// Create a new test package lookup record.
        /// </summary>
        /// <param name="testPackage">The <coode>TestPackage</coode> being loaded.</param>
        void CreateTestNameLookup(TestPackage.TestPackage testPackage);
    }
}
