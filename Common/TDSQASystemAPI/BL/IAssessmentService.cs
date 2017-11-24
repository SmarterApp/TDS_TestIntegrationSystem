using System;

namespace TDSQASystemAPI.BL
{
    /// <summary>
    /// Service for interacting with assessment data in TIS.
    /// </summary>
    public interface IAssessmentService
    {
        /// <summary>
        /// Delete an assessment from the TIS databases.
        /// </summary>
        void Delete(String testPackageKey);
    }
}
