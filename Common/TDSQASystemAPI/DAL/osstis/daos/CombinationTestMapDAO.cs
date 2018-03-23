using TDSQASystemAPI.DAL.osstis.dtos;

namespace TDSQASystemAPI.DAL.osstis.daos
{
    public class CombinationTestMapDAO : TestPackageDaoBase<CombinationTestMapDTO>
    {
        public CombinationTestMapDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.OSS_TIS;
            TvpType = "CombinationTestMapTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.CombinationTestMap (ComponentTestName, ComponentSegmentName, CombinationTestName, CombinationSegmentName) \n" +
                "SELECT" +
                "   ComponentTestName, \n" +
                "   ComponentSegmentName, \n" +
                "   CombinationTestName, \n" +
                "   CombinationSegmentName \n";
        }
    }
}
