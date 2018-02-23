using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TesteeRelationshipAttributeDTO</code>s to the <code>OSS_Configs..Client_TesteeRelationshipAttribute</code> table
    /// </summary>
    public class TesteeRelationshipAttributeDAO : TestPackageDaoBase<TesteeRelationshipAttributeDTO>
    {
        public TesteeRelationshipAttributeDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "TesteeRelationshipAttributeTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TesteeRelationshipAttribute (ClientName, TDS_ID, RTSName, Label, ReportName, AtLogin, SortOrder, RelationshipType) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TdsId, \n" +
                "   RtsName, \n" +
                "   Label, \n" +
                "   ReportName, \n" +
                "   AtLogin, \n" +
                "   SortOrder, \n" +
                "   RelationshipType \n";
        }

        public override void Insert(IList<TesteeRelationshipAttributeDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
