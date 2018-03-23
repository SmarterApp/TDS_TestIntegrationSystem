using TDSQASystemAPI.DAL.osstis.dtos;

namespace TDSQASystemAPI.DAL.osstis.daos
{
    public class CombinationTestFormMapDAO : TestPackageDaoBase<CombinationTestFormMapDTO>
    {
        public CombinationTestFormMapDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.OSS_TIS;
            TvpType = "CombinationTestFormMapTable";
            InsertSql =
                "INSERT CombinationTestFormMap(ComponentSegmentName, ComponentFormKey, CombinationFormKey) \n" +
                "SELECT \n" +
                "   ComponentSegmentName, \n" +
                "   ComponentFormKey, \n" +
                "   CombinationFormKey";
        }
    }
}
