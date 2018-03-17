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
        /// <param name="strandMap">A map of <code>StrandDTO</code>s that map to records in the 
        /// <code>OSS_Itembank..tblStrand</code> table.</param>
        void CreateSetOfAdminItems(TestPackage.TestPackage testPackage, IDictionary<string, StrandDTO> strandMap);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testPackage"></param>
        void CeateItemMeasurementParameters(TestPackage.TestPackage testPackage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testPackage"></param>
        void CreateAdminStimuli(TestPackage.TestPackage testPackage);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Only <code>TestPackage</code>s with fixed-form <code>AssessmentSegment</code>s need to call this method.
        /// </remarks>
        /// <param name="testPackage"></param>
        /// <returns>A collection of <code>TestFormDTO</code>s for every fixed-form <code>AssessmentSegment</code> in the
        /// <code>TestPackage</code>.</returns>
        List<TestFormDTO> CreateTestForms(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Associate <code>ItemGroupItem</code>s to their parent <code>TestForm</code>s.
        /// </summary>
        /// <remarks>
        /// Only <code>TestPackage</code>s with fixed-form <code>AssessmentSegment</code>s need to call this method.
        /// </remarks>
        /// <param name="testPackage"></param>
        /// <param name="testForms">The collection of <code>TestFormDTO</code>s for this <code>TestPackage</code>.</param>
        void CreateTestFormItems(TestPackage.TestPackage testPackage, IList<TestFormDTO> testForms);
    }
}
