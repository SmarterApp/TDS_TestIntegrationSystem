using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
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
        /// <returns></returns>
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
