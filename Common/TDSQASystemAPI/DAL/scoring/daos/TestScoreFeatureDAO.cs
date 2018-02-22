using System.Collections.Generic;
using TDSQASystemAPI.DAL.scoring.dtos;

namespace TDSQASystemAPI.DAL.scoring.daos
{
    /// <summary>
    /// A class for saving <code>TestDTO</code>s to the <code>OSS_TestScoringConfigs..TestScoreFeature</code> table
    /// </summary>
    public class TestScoreFeatureDAO : TestPackageDaoBase<TestScoreFeatureDTO>
    {
        public TestScoreFeatureDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.SCORING;
            TvpType = "TestScoreFeatureTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.TestScoreFeature(_Key, ClientName, TestID, MeasureOf, MeasureLabel, IsScaled, ComputationRule, ComputationOrder) \n" +
                "SELECT \n" +
                "   TestScoreFeatureKey, \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   MeasureOf, \n" +
                "   MeasureLabel, \n" +
                "   IsScaled, \n" +
                "   ComputationRule, \n" +
                "   ComputationOrder \n";
        }

        public override void Insert(IList<TestScoreFeatureDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
