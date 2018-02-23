using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Transactions;

namespace TISUnitTests.utils
{
    /// <summary>
    /// A class for rolling back database transactions after the test has completed.
    /// </summary>
    [TestClass]
    public abstract class TestPackageDaoIntegrationTestBase<T>
    {
        private TransactionScope transactionScope;
        private ReflectionObjectPopulator<T> reflectionReader = new ReflectionObjectPopulator<T>();
        private CompareLogic compareLogic;

        /// <summary>
        /// Get an instance of the <code>CompareLogic</code> to simplify object comparison.
        /// </summary>
        /// <remarks>
        /// The <code>Config.MaxMillisecondsDateDifference</code> is to allow some leeway to compare dates.  Even if .NET <code>DateTime</code>s match to the
        /// millisecond, the number of ticks might be slightly different.  As of right now, it does not look like there's a way to implement a custom comparison
        /// on just a particular type.
        /// </remarks>
        protected CompareLogic Comparer {
            get
            {
                if (this.compareLogic != null)
                {
                    return this.compareLogic;
                }

                var compareLogic = new CompareLogic();
                compareLogic.Config.MaxMillisecondsDateDifference = 10;
                this.compareLogic = compareLogic;

                return compareLogic;
            }
        }

        /// <summary>
        /// Compare an expected result (e.g. a mock object or some sort of "control group" object) to an actual result (e.g. the results of a test).  If the 
        /// comparison fails, <code>Assert.Fail</code> is called to indicate the test is a failure.
        /// </summary>
        /// <param name="expected">A type <code>T</code> containing the values that are expected to be present.</param>
        /// <param name="actual">A type <code>T</code> that should be compared to the <code>expected</code> argument.</param>
        protected void CompareResults(T expected, T actual)
        {
            var comparisonResult = Comparer.Compare(expected, actual);
            if (!comparisonResult.AreEqual)
            {
                Assert.Fail(string.Format("Comparison failure!  The following differences were recorded:\n{0}", comparisonResult.DifferencesString));
            }
        }

        /// <summary>
        /// Create the transaction scope to enforce creating a new transaction prior to the test executing.
        /// </summary>
        [TestInitialize]
        public virtual void Setup()
        {
            transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { Timeout = new TimeSpan(0, 10, 0) });
        }

        /// <summary>
        /// Rollback the transaction and cleanup the transaction scope after test execution completes.
        /// </summary>
        [TestCleanup]
        public virtual void Teardown()
        {
            Transaction.Current.Rollback();
            transactionScope.Dispose();
        }

        /// <summary>
        /// Get a collection of records from the database.
        /// </summary>
        /// <remarks>
        /// The SQL columns must match the object's property names exactly.  Any database columns that do not match
        /// the property name exactly must be aliased.
        /// </remarks>
        /// <param name="sql">The SQL to execute against the database.</param>
        /// <param name="connectionStringName">The name of the connection string in app.config to use when connecting 
        /// to the database server.</param>
        /// <returns>A <code>List<typeparamref name="T"/></code> built from the records contained in the <code>IDataReader</code>.</returns>
        public virtual List<T> GetInsertedRecords(string sql, string connectionStringName)
        {
            var results = new List<T>();

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    results = reflectionReader.GetListFromDataReader(command.ExecuteReader());
                }
            }

            return results;
        }
    }
}
