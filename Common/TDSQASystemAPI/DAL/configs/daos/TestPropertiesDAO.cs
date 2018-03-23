using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestPropertiesDTO</code>s to the <code>OSS_Configs..Client_TestProperties</code> table
    /// </summary>
    public class TestPropertiesDAO : TestPackageDaoBase<TestPropertiesDTO>
    {
        public TestPropertiesDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "TestPropertiesTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TestProperties (ClientName, TestID, IsSelectable, Label, SubjectName, MaxOpportunities, ScoreByTDS, AccommodationFamily,  ReportingInstrument, TIDE_ID, gradeText) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestId, \n" +
                "   IsSelectable, \n" +
                "   Label, \n" +
                "   SubjectName, \n" +
                "   MaxOpportunities, \n" +
                "   ScoreByTds, \n" +
                "   AccommodationFamily, \n" +
                "   ReportingInstrument, \n" +
                "   TideId, \n" +
                "   GradeText \n";
        }
    }
}
