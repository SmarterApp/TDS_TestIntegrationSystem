using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using TDSQASystemAPI.BL;
using TDSQASystemAPI.DAL;
using TISServices.Services;

namespace TISUnitTests
{
    [TestClass]
    public class AssessmentsControllerUnitTest
    {
        [TestMethod]
        public void ShouldReturn200OKAfterSuccessfullyDeletingAnAssessment()
        {
            var mockRepository = new Mock<IAssessmentRepository>();
            mockRepository.Setup(mock => mock.Delete("success"));
            var assessmentService = new AssessmentService(mockRepository.Object);
            var assessmentsController = new AssessmentsController(assessmentService)
            {
                Request = new HttpRequestMessage(HttpMethod.Delete, "http://localhost:44444/api/assessments/success")
            };

            using (var response = assessmentsController.Delete("success"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public void ShouldReturn400BadRequestWhenNoAssessmentKeyIsProvided()
        {
            var mockRepository = new Mock<IAssessmentRepository>();
            mockRepository.Setup(mock => mock.Delete(null));
            var assessmentService = new AssessmentService(mockRepository.Object);
            var assessmentsController = new AssessmentsController(assessmentService)
            {
                Request = new HttpRequestMessage(HttpMethod.Delete, "http://localhost:44444/api/assessments")
            };

            using (var response = assessmentsController.Delete(null))
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }  
        }

        [TestMethod]
        public void ShouldReturn500InternalServerErrorWhenDeleteFails()
        {
            var mockRepository = new Mock<IAssessmentRepository>();
            mockRepository.Setup(mock => mock.Delete("failure"))
                .Throws(new Exception("Something bad happened"));
            var assessmentService = new AssessmentService(mockRepository.Object);
            var assessmentsController = new AssessmentsController(assessmentService)
            {
                Request = new HttpRequestMessage(HttpMethod.Delete, "http://localhost:44444/api/assessments/failure")
            };

            using (var response = assessmentsController.Delete("failure"))
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }
    }
}
