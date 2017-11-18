using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using TISServices.Services;
using System.Net.Http;
using System.Net;

namespace TISUnitTests
{
    [TestClass]
    public class AssessmentControllerIntegrationTest
    {
        private const String TEST_PACKAGE_XML_FILEPATH = @"../../resources/scoring-package.xml";
        private const String TEST_PACKAGE_KEY = "foo";

        [TestInitialize]
        public void Setup()
        {
            // Read scoring package XML and insert it into the database to prepare for the 
            // integration test
            if (File.Exists(TEST_PACKAGE_XML_FILEPATH))
            {
                var scoringPackageXml = File.ReadAllText(TEST_PACKAGE_XML_FILEPATH);
                //LoadScoringPackage(scoringPackageXml);
            }
            else
            {
                throw new FileNotFoundException(String.Format("Could not find scoring package XML at {0}\\{1}", Directory.GetCurrentDirectory(), TEST_PACKAGE_XML_FILEPATH));
            }
        }

        [TestMethod]
        public void ShouldDeleteAnAssessment()
        {
            var assessmentController = new AssessmentsController()
            {
                Request = new HttpRequestMessage(HttpMethod.Delete, "http://localhost:44444/api/assessments/" + TEST_PACKAGE_KEY)
            };

            using (var response = assessmentController.Delete(TEST_PACKAGE_KEY))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        private void LoadScoringPackage(String scoringPackageXml)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["itembank"].ConnectionString))
            {
                using (var command = new SqlCommand("tp.spLoader_Main", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@XMLfile", scoringPackageXml);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
