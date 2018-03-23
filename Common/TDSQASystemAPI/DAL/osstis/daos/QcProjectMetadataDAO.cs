using TDSQASystemAPI.DAL.osstis.dtos;

namespace TDSQASystemAPI.DAL.osstis.daos
{
    public class QcProjectMetadataDAO : TestPackageDaoBase<QcProjectMetadataDTO>
    {
        public QcProjectMetadataDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.OSS_TIS;
            TvpType = "QcProjectMetadataTable";
            InsertSql =
                "INSERT \n" +
                "   QC_ProjectMetaData (_fk_ProjectID, GroupName, VarName, IntValue, FloatValue, TextValue, Comment) \n" +
                "SELECT \n" +
                "   ProjectId, \n" +
                "   GroupName, \n" +
                "   VarName, \n" +
                "   IntValue, \n" +
                "   FloatValue, \n" +
                "   TextValue, \n" +
                "   Comment \n";
        }
    }
}
