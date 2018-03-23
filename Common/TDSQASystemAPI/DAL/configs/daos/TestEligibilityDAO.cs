using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestEligibilityDTO</code>s to the <code>OSS_Configs..Client_TestEligibility</code> table
    /// </summary>
    public class TestEligibilityDAO : TestPackageDaoBase<TestEligibilityDTO>
    {
        public TestEligibilityDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "TestEligibilityTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TestEligibility (Clientname, TestID, RTSName, enables, disables, RTSValue, _efk_EntityType, eligibilityType, matchType) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   RtsName, \n" +
                "   Enables, \n" +
                "   Disables, \n" +
                "   RtsValue, \n" +
                "   EntityType, \n" +
                "   EligibilityType, \n" +
                "   MatchType \n";
        }
    }
}
