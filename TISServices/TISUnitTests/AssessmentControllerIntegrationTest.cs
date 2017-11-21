using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using TISServices.Services;
using System.Net.Http;
using System.Net;
using System.Transactions;
using System.Data;
using System.Data.SqlTypes;
using System.Xml;

namespace TISUnitTests
{
    /// <summary>
    /// Conduct integration tests for the <code>AssessmentsController</code>.  These tests are intended to be "destructive" (that is, they may affect the database).
    /// </summary>
    /// <remarks>
    /// <para>
    /// According to the instructions on the <see cref="https://github.com/SmarterApp/TDS_TestIntegrationSystem">REAADME for TIS</see>, there are four files required
    /// to load a combined assessment (sometimes referred to as an ICA) into TIS.  The four packages are included in the <code>resources</code> directory of this
    /// project.  The <code>Setup()</code> method will read the content of these files and try to load them into the database.  To minimize impact/dependency on whatever 
    /// database these integration tests run against, any integration tests should be written against the data contained within these files.
    /// </para>
    /// </remarks>
    [TestClass]
    public class AssessmentControllerIntegrationTest
    {
        private readonly String[] testPackageFiles = {
            "../../resources/test-packages/tds/administration/(SBAC_PT)SBAC-IRP-CAT-MATH-3-Summer-2015-2016-UNIT-TEST.xml",
            "../../resources/test-packages/tds/administration/(SBAC_PT)SBAC-IRP-Perf-MATH-3-Summer-2015-2016-UNIT-TEST.xml",
            "../../resources/test-packages/tis/administration/(SBAC_PT)SBAC-IRP-MATH-3-COMBINED-Summer-2015-2016-UNIT-TEST.xml",
            "../../resources/test-packages/tis/scoring/(SBAC_PT)SBAC-IRP-MATH-3-COMBINED-Summer-2015-2016-UNIT-TEST.xml"
        };

        private const String TEST_PACKAGE_KEY = "(SBAC_PT)SBAC-IRP-MATH-3-COMBINED-Summer-2015-2016-UNIT-TEST";

        [TestInitialize]
        public void Setup()
        {
            // Read scoring package XML from each test package that needs to be loaded into TIS for testing the delete process.  According to the instructions on
            // the README for TIS, there are four files that need to be loaded into the 
            var testPackageXmls = new String[testPackageFiles.Length];
            for (var i = 0; i < testPackageFiles.Length; i++)
            {
                if (File.Exists(testPackageFiles[i]))
                {
                    testPackageXmls[i] = File.ReadAllText(testPackageFiles[i]);             
                }
                else
                {
                    throw new FileNotFoundException(String.Format("Could not find scoring package XML at {0}\\{1}", Directory.GetCurrentDirectory(), testPackageFiles[i]));
                }
            }

            // Load the XML for each file into the database.  If any of the files fail to load, the entire transaction will be rolled back, leaving the database
            // in the state it was in prior to the load attempt.
            using (var trxScope = new TransactionScope())
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["itembank"].ConnectionString))
                {
                    connection.Open();

                    foreach (var testPackageXml in testPackageXmls)
                    {
                        LoadScoringPackage(testPackageXml, connection);
                    }

                    // Call the UpdateTDSConfigs stored procedure to move the data from the loader/staging tables into the production/"real" tables.
                    using (var updateConfigsCmd = new SqlCommand("dbo.UpdateTDSConfigs", connection))
                    {
                        updateConfigsCmd.CommandType = CommandType.StoredProcedure;
                        updateConfigsCmd.Parameters.AddWithValue("@doall", 1);

                        updateConfigsCmd.ExecuteNonQuery();
                    }
                }

                trxScope.Complete();
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

        /// <summary>
        /// Load test package XML into the TIS database(s).
        /// </summary>
        /// <param name="scoringPackageXml">The test package XML.</param>
        /// <param name="connection">The <code>SqlConnection</code> pointing to the database(s) that need to be loaded.</param>
        private void LoadScoringPackage(String scoringPackageXml, SqlConnection connection)
        {            
            using (var loaderCommand = new SqlCommand("tp.spLoader_Main", connection))
            {
                loaderCommand.CommandType = CommandType.StoredProcedure;
                loaderCommand.Parameters.Add(new SqlParameter("@XMLfile", SqlDbType.Xml)
                {
                    Value = new SqlXml(XmlReader.Create(new StringReader(scoringPackageXml)))
                });

                loaderCommand.ExecuteNonQuery();
            }
        }
    }
}
