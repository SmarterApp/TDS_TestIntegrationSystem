using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.BL.testpackage;
using TDSQASystemAPI.TestPackage;
using TISUnitTests.utils;

namespace TISUnitTests.services
{
    /// <summary>
    /// Integration tests for the <code>TestPackageLoaderService</code>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The vast majority of the integration tests in this project have been conducted using the 
    /// "V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml" file, so that's what is used here.  Other integration tests could
    /// easily be created using the other files included in the <code>resources</code> directory.
    /// </para>
    /// <para>
    /// Because test <code>TestPackageLoaderService</code> needs to query MSSQL and MySQL databases, this test cannot
    /// be wrapped in a database transaction**.  Therefore, the <code>TestInitialize</code> and <code>TestCleanup</code>
    /// methods will delete all assessment(s) associated with the <code>TestPackage</code> being loaded during the test.
    /// 
    /// ** = setting up distributed transactions is possible, but probably not worth the effort solely for an integration
    /// test.
    /// </para>
    /// </remarks>
    [TestClass]
    public class TestPackageLoaderServiceIntegrationTest// : TestPackageDaoIntegrationTestBase<TestPackage>
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml";
        private const string TEST_PACKAGE_KEY = "(SBAC_PT)ICA-UNIT-TEST-GRADE-11-COMBINED-2017-2018";

        private readonly ITestPackageLoaderService testPackageLoaderService = new TestPackageLoaderService();

        [TestInitialize]
        public void Setup()
        {
            DeleteTestPackage(TEST_PACKAGE_KEY);
        }

        /// <summary>
        /// Load a test package into the TIS databases.
        /// </summary>
        [TestMethod]
        [Ignore("This integration test has a dependency on test form data for the test package existing in TDS")]
        public void LOADER_INTEGRATION_ShouldLoadTestPackage()
        {
            using (var testPackageStream = File.OpenRead(TEST_PACKAGE_XML_FILE))
            {
                var result = testPackageLoaderService.LoadTestPackage(testPackageStream);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            DeleteTestPackage(TEST_PACKAGE_KEY);
        }

        /// <summary>
        /// Delete Assessment(s) from the database for a specified test package key.
        /// </summary>
        /// <remarks>
        /// The intent of this method is to "clear the runway" for loading the test package XML file(s) that are included
        /// in the <code>resources</code> directory.  This method calls the <code>spDeleteAssessment</code> stored procedure
        /// to delete any assessment(s) that are associated with the specified test package key.
        /// </remarks>
        /// <param name="testPackgeKey">The identifier of the test package to delete.</param>
        private void DeleteTestPackage(string testPackgeKey)
        {
            var spName = "spDeleteAssessment";

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["itembank"].ConnectionString))
            {
                using (var command = new SqlCommand(spName, connection))
                {
                    var testPackageKeyParameter = new SqlParameter("@testPackageKey", testPackgeKey)
                    {
                        DbType = System.Data.DbType.String
                    };

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(testPackageKeyParameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
