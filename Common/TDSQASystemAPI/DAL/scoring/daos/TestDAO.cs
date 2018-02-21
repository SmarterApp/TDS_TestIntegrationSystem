using System.Collections.Generic;
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
        }

        public override void Insert(IList<TestDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
