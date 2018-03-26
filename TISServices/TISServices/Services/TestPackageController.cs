using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using TDSQASystemAPI.BL.testpackage;
using TDSQASystemAPI.Utilities;

namespace TISServices.Services
{
    /// <summary>
    /// Endpoints for inteacting with test package data in TIS.
    /// </summary>
    public class TestPackageController : ApiController
    {
        private readonly ITestPackageLoaderService testPackageLoaderService;

        /// <summary>
        /// Default constructor creates a new instance of the <code>TestPackageLoaderService</code>; there is no
        /// DI/IoC container implemented in TIS.
        /// </summary>
        public TestPackageController()
        {
            testPackageLoaderService = new TestPackageLoaderService();
        }

        /// <summary>
        /// Constructor for unit/integration testing.
        /// </summary>
        /// <param name="testPackageLoaderService">The <code>ITestPackageLoaderService</code> that should be used by this
        /// controller.</param>
        public TestPackageController(ITestPackageLoaderService testPackageLoaderService)
        {
            this.testPackageLoaderService = testPackageLoaderService ?? throw new ArgumentNullException(nameof(testPackageLoaderService));
        }

        /// <summary>
        /// Load a test package XML into TIS.
        /// </summary>
        /// <example>
        /// // the body of the POST should contain the test package XML
        /// POST http://localhost:[port number]/api/testpackage
        /// </example>
        /// <remarks>
        /// The test package XML should conform to the new 
        /// </remarks>
        /// <param name="loadTestPackageRequest">The <code>HttpRequestMessage</code> containing the test package XML payload
        /// in the body of the POST.</param>
        /// <returns>An <code>HttpResponseMessage</code> indicating success or failure.</returns>
        [HttpPost]
        public HttpResponseMessage LoadTestPackage(HttpRequestMessage loadTestPackageRequest)
        {
            try { 
                loadTestPackageRequest.Content.ReadAsStreamAsync().Result.Seek(0, SeekOrigin.Begin);
                var testPackageXmlStream = loadTestPackageRequest.Content.ReadAsStreamAsync().Result;
                var validationErrors = testPackageLoaderService.LoadTestPackage(testPackageXmlStream);

                if (validationErrors.Any())
                {
                    //var responseContent = new ObjectContent<List<ValidationError>>(validationErrors.ToList(), new JsonMediaTypeFormatter());
                    return Request.CreateResponse(HttpStatusCode.Created, validationErrors, new JsonMediaTypeFormatter(), "application/json");
                }

                return Request.CreateResponse(HttpStatusCode.Created);
            } catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
