using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TDSQASystemAPI.BL;
using TISServices.Utilities;

namespace TISServices.Services
{
    /// <summary>
    /// Endpoints for inteacting with assessment data in TIS.
    /// </summary>
    public class AssessmentsController : ApiController
    {
        private readonly IAssessmentService assessmentService;

        public AssessmentsController()
        {
            assessmentService = new AssessmentService();
        }

        public AssessmentsController(IAssessmentService assessmentService)
        {
            this.assessmentService = assessmentService;
        }

        /// <summary>
        /// Delete an assessment from TIS.
        /// </summary>
        /// <param name="testPackageKey">The unique identifier of the assessment.  Typically this will be the <code>uniqueId</code> from the scoring test package.</param>
        /// <returns>An <code>HttpResponseMessage</code> with one of the following statuses:
        /// 200 - OK if the delete was successful
        /// 400 - Bad Request if the <code>key</code> is null or empty
        /// 500 - Internal Server Error if the delete operation failed.
        /// </returns>
        [HttpDelete] // DELETE api/assessments/(SBAC_PT)MSB-Multiform-Mathematics-3
        public HttpResponseMessage Delete(String testPackageKey)
        {
            try
            {
                if (String.IsNullOrEmpty(testPackageKey))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "An assessment key must be provided");
                }

                assessmentService.Delete(testPackageKey);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                TISServicesLogger.Log(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, String.Format("Could not delete assessment {0} from TIS", testPackageKey));
            }
        }
    }
}