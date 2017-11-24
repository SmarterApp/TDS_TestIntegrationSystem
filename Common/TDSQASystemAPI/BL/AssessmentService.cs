using System;
using TDSQASystemAPI.DAL;

namespace TDSQASystemAPI.BL
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IAssessmentRepository assessmentRepository;

        public AssessmentService()
        {
            assessmentRepository = new AssessmentRepository();
        }
        
        public AssessmentService(IAssessmentRepository assessmentRepository)
        {
            this.assessmentRepository = assessmentRepository;
        }
        
        /// <param name="testPackageKey">The unique identifier of the assessment to delete</param>
        public void Delete(String testPackageKey)
        {
            assessmentRepository.Delete(testPackageKey);
        }
    }
}