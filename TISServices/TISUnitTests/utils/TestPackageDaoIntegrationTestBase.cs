using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Transactions;

namespace TISUnitTests.utils
{
    /// <summary>
    /// A class for rolling back database transactions after the test has completed.
    /// </summary>
    [TestClass]
    public abstract class TestPackageDaoIntegrationTestBase
    {
        private TransactionScope transactionScope;

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
    }
}
