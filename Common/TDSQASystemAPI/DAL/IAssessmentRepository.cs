using System;

namespace TDSQASystemAPI.DAL
{
    /// <summary>
    /// Interface for interacting with assessment data within TIS.
    /// </summary>
    public interface IAssessmentRepository
    {
        /// <summary>
        /// Call <code>spDeleteAssessment</code> to remove the assessment from all TIS databases.
        /// </summary>
        /// <param name="testPackageKey">The unique identifier of the assessment</param>
        void Delete(String testPackageKey);
    }
}
