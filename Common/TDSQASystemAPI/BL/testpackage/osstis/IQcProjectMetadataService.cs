using TDSQASystemAPI.DAL.osstis.dtos;

namespace TDSQASystemAPI.BL.testpackage.osstis
{
    /// <summary>
    /// An inteface for interacting with QC Project Metadata.
    /// </summary>
    public interface IQcProjectMetadataService
    {
        /// <summary>
        /// Insert new project metadata records for the <code>Assessment</code>s being loaded.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackge</code> being loaded.</param>
        void CreateQcProjectMetadata(TestPackage.TestPackage testPackage);
    }
}
