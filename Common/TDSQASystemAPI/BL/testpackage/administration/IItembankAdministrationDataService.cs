namespace TDSQASystemAPI.BL.testpackage.administration
{
    /// <summary>
    /// 
    /// </summary>
    public interface IItembankAdministrationDataService
    {
        /// <summary>
        /// Insert or update the <code>TestPackage</code>'s test administration record.
        /// <remarks>
        /// If a test admin record already exists, the <code>OSS_Itembank..tblTestAdmin.updateConfig</code> column should be updated to the
        /// <code>TestPacakge</code>'s version number.
        /// </remarks>
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage</code> containing the test administration data.</param>
        void SaveTestAdministration(TestPackage.TestPackage testPackage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage</code> containing the administration subject data.</param>
        void CreateSetOfAdminSubjects(TestPackage.TestPackage testPackage);
    }
}
