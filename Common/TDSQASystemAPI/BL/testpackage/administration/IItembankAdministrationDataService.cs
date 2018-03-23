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
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        void SaveTestAdministration(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Insert a collection of administration subject records to describe the <code>TesPackage</code>'s
        /// Assessments and Segments.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        void CreateSetOfAdminSubjects(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Create a collection of <code>AdminStrandDTO</code> records, which associate an <code>Assessment</code>
        /// to a <code>Strand</code>.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        /// <param name="strandMap">A map of <code>StrandDTO</code>s that map to records in the 
        /// <code>OSS_Itembank..tblStrand</code> table.</param>
        void CreateAdminStrands(TestPackage.TestPackage testPackge, IDictionary<string, StrandDTO> strandMap);

        /// <summary>
        /// Create a collection of <code>AdminItemDTO</code>s, which associate <code>ItemGroupItem</code>s to an
        /// <code>Assessment</code>.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        /// <param name="strandMap">A map of <code>StrandDTO</code>s that map to records in the 
        /// <code>OSS_Itembank..tblStrand</code> table.</param>
        void CreateSetOfAdminItems(TestPackage.TestPackage testPackage, IDictionary<string, StrandDTO> strandMap);

        /// <summary>
        /// Create a collection of <code>ItemMeasurementParameterDTO</code>s that participate in scoring an
        /// item.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        void CeateItemMeasurementParameters(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Create a collection of <code>AdminStimuliDTO</code>s, which associate <code>ItemStimulus</code> to an
        /// <code>Assessment</code>.
        /// </summary>
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        void CreateAdminStimuli(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Create a collection of <code>TestFormDTO</code>s to associate forms to a fixed-formm 
        /// <code>AssessmentSegment</code>
        /// </summary>
        /// <remarks>
        /// Only <code>TestPackage</code>s with fixed-form <code>AssessmentSegment</code>s need to call this method.
        /// </remarks>
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        /// <returns>A collection of <code>TestFormDTO</code>s for every fixed-form <code>AssessmentSegment</code> in the
        /// <code>TestPackage</code>.</returns>
        List<TestFormDTO> CreateTestForms(TestPackage.TestPackage testPackage);

        /// <summary>
        /// Associate <code>ItemGroupItem</code>s to their parent <code>TestForm</code>s.
        /// </summary>
        /// <remarks>
        /// Only <code>TestPackage</code>s with fixed-form <code>AssessmentSegment</code>s need to call this method.
        /// </remarks>
        /// <param name="testPackage">The <code>TestPackage</code> being loaded.</param>
        /// <param name="testForms">The collection of <code>TestFormDTO</code>s for this <code>TestPackage</code>.</param>
        void CreateTestFormItems(TestPackage.TestPackage testPackage, IList<TestFormDTO> testForms);

        /// <summary>
        /// Create <code>AffinityGroup</code>s and thier associated items for the <code>TestPackage</code>.
        /// </summary>
        /// <param name="testPackage">TestPackage</code> being loaded.</param>
        void CreateAffinityGroups(TestPackage.TestPackage testPackage);
    }
}
