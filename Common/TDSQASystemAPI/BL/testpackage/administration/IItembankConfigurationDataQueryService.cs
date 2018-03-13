using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    public interface IItembankConfigurationDataQueryService
    {
        /// <summary>
        /// Look up the <code>TestPackage</code>'s client.
        /// </summary>
        /// <param name="clientName">The name of the client to find.  This will stored in the <code>TestPackage.publisher</code> field.</param>
        /// <returns>The <code>ClientDTO</code> representing the client that "owns" this <code>TestPackage</code>.  May return null.</returns>
        ClientDTO FindClientByName(string clientName);

        /// <summary>
        /// Look up a test pacakage's subject by its subject key.
        /// </summary>
        /// <param name="subjectKey">The identifier of the subject to find.  The pattern for the subject key is "[test package publisher]-[subject code]", e.g. "SBAC_PT-ELA"</param>
        /// <returns>A <code>SubjectDTO</code> representing the <code>TestPackage</code>'s subject.  May return null.</returns>
        SubjectDTO FindSubject(string subjectKey);

        /// <summary>
        /// Find the <code>TestPacakge</code>'s test administrator.
        /// </summary>
        /// <param name="clientName">The name of the client that owns this <code>TestPackage</code>.  May return null.</param>
        /// <returns>A <code>TestAdminDTO</code> representing the <code>TestPackage</code>'s test administrator.  May return null.</returns>
        TestAdminDTO FindTestAdmin(string clientName);
    }
}
