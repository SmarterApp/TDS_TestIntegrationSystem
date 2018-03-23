using System.Collections.Generic;
using TDSQASystemAPI.DAL.osstis.dtos;

namespace TDSQASystemAPI.DAL.osstis.daos
{
    /// <summary>
    /// A class for saving <code>TestNameLookupDTO</code>s to the <code>OSS_TIS..TestNameLookup</code> table
    /// </summary>
    public class TestNameLookupDAO : TestPackageDaoBase<TestNameLookupDTO>
    {
        public TestNameLookupDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.OSS_TIS;
            TvpType = "TestNameLookupTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.TestNameLookup (InstanceName, TestName) \n" +
                "SELECT \n" +
                "   InstanceName, \n" +
                "   TestName \n";
        }
    }
}
