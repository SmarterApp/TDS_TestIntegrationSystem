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
        private static readonly String[] testPackageFiles = {
           @"..\..\resources\test-packages\tds\administration\(SBAC_PT)SBAC-IRP-CAT-MATH-3-Summer-2015-2016-UNIT-TEST.xml",
           @"..\..\resources\test-packages\tds\administration\(SBAC_PT)SBAC-IRP-Perf-MATH-3-Summer-2015-2016-UNIT-TEST.xml",
           @"..\..\resources\test-packages\tis\administration\(SBAC_PT)SBAC-IRP-MATH-3-COMBINED-Summer-2015-2016-UNIT-TEST.xml",
           @"..\..\resources\test-packages\tis\scoring\(SBAC_PT)SBAC-IRP-MATH-3-COMBINED-Summer-2015-2016-UNIT-TEST.xml",
           @"..\..\resources\test-packages\tds\administration\(SBAC_PT)IRP-Perf-ELA-11-Summer-2015-2016-UNIT-TEST.xml",
           @"..\..\resources\test-packages\tds\administration\(SBAC_PT)SBAC-IRP-CAT-ELA-11-Summer-2015-2016-UNIT-TEST.xml",
           @"..\..\resources\test-packages\tis\administration\(SBAC_PT)SBAC-IRP-ELA-11-COMBINED-Summer-2015-2016-UNIT-TEST.xml",
           @"..\..\resources\test-packages\tis\scoring\(SBAC_PT)SBAC-IRP-ELA-11-COMBINED-Summer-2015-2016-UNIT-TEST.xml"
        };

        private const String MATH_TEST_PACKAGE_KEY = "(SBAC_PT)SBAC-IRP-MATH-3-COMBINED-Summer-2015-2016-UNIT-TEST";
        private const String ELA_TEST_PACKAGE_KEY = "(SBAC_PT)SBAC-IRP-ELA-11-COMBINED-Summer-2015-2016-UNIT-TEST";

        private static readonly String[] combinationTestMapSqls =
        {
            @"INSERT INTO OSS_TIS.dbo.CombinationTestMap(ComponentTestName, ComponentSegmentName, CombinationTestName, CombinationSegmentName) VALUES('(SBAC_PT)SBAC-IRP-CAT-MATH-3-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-CAT-MATH-3-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-MATH-3-COMBINED-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-CAT-COMBINED-MATH-3-Summer-2015-2016-UNIT-TEST')",
            @"INSERT INTO OSS_TIS.dbo.CombinationTestMap(ComponentTestName, ComponentSegmentName, CombinationTestName, CombinationSegmentName) VALUES('(SBAC_PT)SBAC-IRP-Perf-MATH-3-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-Perf-MATH-3-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-MATH-3-COMBINED-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-CAT-COMBINED-MATH-3-Summer-2015-2016-UNIT-TEST' )",
            @"INSERT INTO OSS_TIS.dbo.CombinationTestMap(ComponentTestName, ComponentSegmentName, CombinationTestName, CombinationSegmentName) VALUES('(SBAC_PT)SBAC-IRP-CAT-ELA-11-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-CAT-ELA-11-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-ELA-11-COMBINED-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-CAT-COMBINED-ELA-11-Summer-2015-2016-UNIT-TEST')",
            @"INSERT INTO OSS_TIS.dbo.CombinationTestMap(ComponentTestName, ComponentSegmentName, CombinationTestName, CombinationSegmentName) VALUES('(SBAC_PT)IRP-Perf-ELA-11-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-Perf-S1-ELA-11-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-ELA-11-COMBINED-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-Perf-S1-COMBINED-ELA-11-Summer-2015-2016-UNIT-TEST')",
            @"INSERT INTO OSS_TIS.dbo.CombinationTestMap(ComponentTestName, ComponentSegmentName, CombinationTestName, CombinationSegmentName) VALUES('(SBAC_PT)IRP-Perf-ELA-11-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-Perf-S2-ELA-11-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-ELA-11-COMBINED-Summer-2015-2016-UNIT-TEST', '(SBAC_PT)SBAC-IRP-Perf-S2-COMBINED-ELA-11-Summer-2015-2016-UNIT-TEST')"
        };

        private static readonly String[] combinationTestFormMapSqls =
        {
            @"INSERT INTO OSS_TIS.dbo.CombinationTestFormMap(ComponentSegmentName, ComponentFormKey, CombinationFormKey) VALUES('(SBAC_PT)SBAC-IRP-Perf-MATH-3-Summer-2015-2016-UNIT-TEST', '187-764', '187-780')",
            @"INSERT INTO OSS_TIS.dbo.CombinationTestFormMap(ComponentSegmentName, ComponentFormKey, CombinationFormKey) VALUES('(SBAC_PT)SBAC-IRP-Perf-S1-ELA-11-Summer-2015-2016-UNIT-TEST', '187-762', '187-778')",
            @"INSERT INTO OSS_TIS.dbo.CombinationTestFormMap(ComponentSegmentName, ComponentFormKey, CombinationFormKey) VALUES('(SBAC_PT)SBAC-IRP-Perf-S2-ELA-11-Summer-2015-2016-UNIT-TEST', '187-763', '187-779')"
        };

        private readonly String[] mathCombinedValidationSqls =
        {
            @"SELECT COUNT(*) FROM OSS_Itembank.dbo.tblSetOfAdminSubjects WHERE _Key LIKE '%math%unit-test'",
            @"SELECT COUNT(*) FROM OSS_TIS.dbo.CombinationTestMap WHERE CombinationTestName LIKE '%math%%unit-test'",
            @"SELECT COUNT(*) FROM OSS_TIS.dbo.CombinationTestFormMap WHERE ComponentSegmentName LIKE '%math%%unit-test'",
            @"SELECT COUNT(*) FROM OSS_Itembank.dbo.tblAdminStimulus WHERE _fk_AdminSubject LIKE '%math%%unit-test'",
            @"SELECT COUNT(*) FROM OSS_Itembank.dbo.tblSetofAdminItems WHERE _fk_AdminSubject LIKE '%math%%unit-test'"
        };

        private readonly String[] elaCombinedValidationSqls =
        {
            @"SELECT COUNT(*) FROM OSS_Itembank.dbo.tblSetOfAdminSubjects WHERE _Key LIKE '%ela%unit-test'",
            @"SELECT COUNT(*) FROM OSS_TIS.dbo.CombinationTestMap WHERE CombinationTestName LIKE '%ela%%unit-test'",
            @"SELECT COUNT(*) FROM OSS_TIS.dbo.CombinationTestFormMap WHERE ComponentSegmentName LIKE '%ela%%unit-test'",
            @"SELECT COUNT(*) FROM OSS_Itembank.dbo.tblAdminStimulus WHERE _fk_AdminSubject LIKE '%ela%%unit-test'",
            @"SELECT COUNT(*) FROM OSS_Itembank.dbo.tblSetofAdminItems WHERE _fk_AdminSubject LIKE '%ela%%unit-test'"
        };
        
        /// <summary>
        /// Set up seed data for the integration tests prior to running them.  The <code>ClassInitialize</code> attribute ensures data will only be loaded
        /// once.
        /// </summary>
        /// <remarks>
        /// The signature for a method decorated with the <code>ClassInitialize</code> method must be static and must accept a <code>TestContext</code> argument, even if 
        /// the test context is never used.
        /// </remarks>
        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            // Read scoring package XML from each test package that needs to be loaded into TIS for testing the delete process.  According to the instructions on
            // the README for TIS, there are four files that need to be loaded into TIS.
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

                    // Insert records into the OSS_TIS.dbo.CombinationTestMap table
                    ExecuteInsertSqls(combinationTestMapSqls, connection);

                    // Insert records into the OSS_TIS.dbo.CombinationTestFormMap table
                    ExecuteInsertSqls(combinationTestFormMapSqls, connection);
                }

                trxScope.Complete();
            }
        }

        [TestMethod]
        public void ShouldDeleteTheSpecifiedAssessment()
        {
            var assessmentController = new AssessmentsController()
            {
                Request = new HttpRequestMessage(HttpMethod.Delete, "http://localhost:44444/api/assessments/" + MATH_TEST_PACKAGE_KEY)
            };

            using (var response = assessmentController.Delete(MATH_TEST_PACKAGE_KEY))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["itembank"].ConnectionString))
                {
                    connection.Open();
                    for (var i = 0; i < mathCombinedValidationSqls.Length; i++)
                    {
                        // Verify the records for the MATH assessment have been removed...
                        using (var verifyDeletedRecordsCommand = new SqlCommand(mathCombinedValidationSqls[i], connection))
                        {
                            verifyDeletedRecordsCommand.CommandType = CommandType.Text;
                            int result = (int)verifyDeletedRecordsCommand.ExecuteScalar();

                            Assert.AreEqual(0, result);
                        }

                        // ... then verify the records for the ELA assessment still remain in the database after the delete was executed.
                        using (var verifyRemainingRecordsCommand = new SqlCommand(elaCombinedValidationSqls[i], connection))
                        {
                            verifyRemainingRecordsCommand.CommandType = CommandType.Text;
                            int result = (int)verifyRemainingRecordsCommand.ExecuteScalar();

                            Assert.IsTrue(result > 0);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ShouldDeleteAMultiSegmentedAssessment()
        {
            var assessmentController = new AssessmentsController()
            {
                Request = new HttpRequestMessage(HttpMethod.Delete, "http://localhost:44444/api/assessments/" + ELA_TEST_PACKAGE_KEY)
            };

            using (var response = assessmentController.Delete(ELA_TEST_PACKAGE_KEY))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["itembank"].ConnectionString))
                {
                    connection.Open();
                    for (var i = 0; i < elaCombinedValidationSqls.Length; i++)
                    {
                        using (var verifyDeletedRecordsCommand = new SqlCommand(elaCombinedValidationSqls[i], connection))
                        {
                            verifyDeletedRecordsCommand.CommandType = CommandType.Text;
                            int result = (int)verifyDeletedRecordsCommand.ExecuteScalar();

                            Assert.AreEqual(0, result);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load test package XML into the TIS database(s).
        /// </summary>
        /// <param name="scoringPackageXml">The test package XML.</param>
        /// <param name="connection">The <code>SqlConnection</code> pointing to the database(s) that need to be loaded.</param>
        private static void LoadScoringPackage(String scoringPackageXml, SqlConnection connection)
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

        /// <summary>
        /// Insert records into the <code>OSS_TIS.dbo.CombinationTestMap</code> table.
        /// </summary>
        /// <param name="insertSqls">The SQL necessary to insert the required records into the </param>
        /// <param name="connection">The <code>SqlConnection</code> pointing to the database(s) that need to be loaded.</param>
        /// <remarks>The process that loads assessments into TIS does not write records to these tables; they have to be created
        /// manually.</remarks>
        private static void ExecuteInsertSqls(String[] insertSqls, SqlConnection connection)
        {
            for (var i = 0; i < insertSqls.Length; i++)
            {
                using (var combinationMapCommand = new SqlCommand(insertSqls[i], connection))
                {
                    combinationMapCommand.CommandType = CommandType.Text;
                    combinationMapCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
