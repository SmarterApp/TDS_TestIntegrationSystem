using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.BL.testpackage;
using TDSQASystemAPI.Utilities;
using TISServices.Services;

namespace TISUnitTests.controllers
{
    [TestClass]
    public class TestPackageControllerUnitTest
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml";

        private readonly Mock<ITestPackageLoaderService> mockTestPackageLoaderService = new Mock<ITestPackageLoaderService>();

        [TestMethod]
        public void ShouldReturnA201CreatedAfterSuccessfullyLoadingATestPackage()
        {
            mockTestPackageLoaderService.Setup(svc => svc.LoadTestPackage(It.IsAny<Stream>()))
                .Returns(new List<ValidationError>());

            var loadTestPackageRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost:44444/api/testpackage");

            using (var xmlStream = new StreamReader(TEST_PACKAGE_XML_FILE, Encoding.UTF8)) {
                loadTestPackageRequest.Content = new StringContent(xmlStream.ReadToEnd(), Encoding.UTF8, "application/xml");
            }

            var testPackageController = new TestPackageController(mockTestPackageLoaderService.Object)
            {
                Request = loadTestPackageRequest
            };

            using (var response = testPackageController.LoadTestPackage(loadTestPackageRequest))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }
        }

        [TestMethod]
        public void ShouldReturn201CreatedWithAWarningMessageOfItemsThatWereNotCreatedBecauseTheyAlreadyExist()
        {
            mockTestPackageLoaderService.Setup(svc => svc.LoadTestPackage(It.IsAny<Stream>()))
                .Returns(new List<ValidationError> {
                    new ValidationError(ValidationErrorCodes.EXISTING_ITEMS_NOT_LOADED,  "The following items were not created: \n187-1432\n187-1434")
                });

            var loadTestPackageRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost:44444/api/testpackage");

            using (var xmlStream = new StreamReader(TEST_PACKAGE_XML_FILE, Encoding.UTF8))
            {
                loadTestPackageRequest.Content = new StringContent(xmlStream.ReadToEnd(), Encoding.UTF8, "application/xml");
            }

            var testPackageController = new TestPackageController(mockTestPackageLoaderService.Object)
            {
                Request = loadTestPackageRequest
            };

            using (var response = testPackageController.LoadTestPackage(loadTestPackageRequest))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                if (!response.TryGetContentValue(out List<ValidationError> responseContent))
                {
                    Assert.Fail("ValidationError collection indicating which items already existed was not included in response");
                }

                Assert.AreEqual(1, responseContent.Count);
                Assert.AreEqual("The following items were not created: \n187-1432\n187-1434", responseContent[0].Message);
            }
        }

        [TestMethod]
        public void ShouldReturn500InternalServerErrorWhenAnLoadingATestPackageEncountersAnException()
        {
            mockTestPackageLoaderService.Setup(svc => svc.LoadTestPackage(It.IsAny<Stream>()))
                .Throws(new Exception("something bad happened"));

            var loadTestPackageRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost:44444/api/testpackage");

            using (var xmlStream = new StreamReader(TEST_PACKAGE_XML_FILE, Encoding.UTF8))
            {
                loadTestPackageRequest.Content = new StringContent(xmlStream.ReadToEnd(), Encoding.UTF8, "application/xml");
            }

            var testPackageController = new TestPackageController(mockTestPackageLoaderService.Object)
            {
                Request = loadTestPackageRequest
            };

            using (var response = testPackageController.LoadTestPackage(loadTestPackageRequest))
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }
    }
}
