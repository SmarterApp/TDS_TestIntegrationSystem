using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

            ExistsSql = "SELECT count(*) FROM dbo.TestScoreFeature t WHERE " +
                "t.ClientName = @clientName AND t.TestID = @testID AND t.computationRule = @computationRule AND t.measureOf = @measureOf";
            FindByExampleSql =
                "SELECT \n" +
                "   _Key as TestScoreFeatureKey, \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   MeasureOf, \n" +
                "   MeasureLabel, \n" +
                "   IsScaled, \n" +
                "   ComputationRule, \n" +
                "   ComputationOrder \n" +
                "FROM dbo.TestScoreFeature t WHERE " +
                "t.ClientName = @clientName AND t.TestID = @testID AND t.computationRule = @computationRule AND t.measureOf = @measureOf";
        }

        override protected void AddNaturalKeys(TestScoreFeatureDTO testScoreFeature, SqlParameterCollection parameters) 
        { 
            parameters.AddWithValue("@clientName", testScoreFeature.ClientName);
            parameters.AddWithValue("@testID", testScoreFeature.TestId);
            parameters.AddWithValue("@computationRule", testScoreFeature.ComputationRule);
            parameters.AddWithValue("@measureOf", testScoreFeature.MeasureOf);
        }
    }
}
