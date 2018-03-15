using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

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
        /// Insert a collection of administration subject records to describe the <code>TesPackage</code>'s
        /// Assessments and Segments.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage</code> containing the administration subject data.</param>
        void CreateSetOfAdminSubjects(TestPackage.TestPackage testPackage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testPackge">The <code>TestPackage</code> containing the administration strand data.</param>
        /// <param name="strandMap">A map of <code>StrandDTO</code>s that map to records in the 
        /// <code>OSS_Itembank..tblStrand</code> table.</param>
        void CreateAdminStrands(TestPackage.TestPackage testPackge, IDictionary<string, StrandDTO> strandMap);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testPackage"></param>
        /// <param name="strandMap"></param>
        void CreateSetOfAdminItems(TestPackage.TestPackage testPackage, IDictionary<string, StrandDTO> strandMap);
    }
}
