using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>TestCohortDTO</code>s to the <code>OSS_Itembank..TestCohort</code> table
    /// </summary>
    public class TestCohortDAO : TestPackageDaoBase<TestCohortDTO>
    {
        public TestCohortDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "TestCohortTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.TestCohort (_fk_AdminSubject, Cohort, ItemRatio) \n" +
                "SELECT \n" +
                "   SegmentKey, \n" +
                "   Cohort, \n" +
                "   ItemRatio \n";
        }

        public override void Insert(IList<TestCohortDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
