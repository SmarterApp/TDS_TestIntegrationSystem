using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL.scoring.dtos;

namespace TDSQASystemAPI.DAL.scoring.daos
{
    /// <summary>
    /// A class for saving <code>TestDTO</code>s to the <code>OSS_TestScoringConfigs..Test</code> table
    /// </summary>
    public class TestDAO : TestPackageDaoBase<TestDTO>
    {
        public TestDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.SCORING;
            TvpType = "TestTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Test (ClientName, TestID, _efk_Subject) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   [Subject] \n";
            ExistsSql = "SELECT count(*) FROM dbo.Test t WHERE t.clientname = @clientName AND t.testID = @testID AND t._efk_Subject = @subject";
        }

        override protected void AddNaturalKeys(TestDTO testDTO, SqlParameterCollection parameters)
        {
            parameters.AddWithValue("@clientName", testDTO.ClientName);
            parameters.AddWithValue("@testID", testDTO.TestId);
            parameters.AddWithValue("@subject", testDTO.Subject);
        }    
    }
}
