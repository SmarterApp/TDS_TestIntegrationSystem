using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>AccommodationFamilyDTO</code>s to the <code>OSS_Configs..Client_AccommodationFamily</code> 
    /// table
    /// </summary>
    public class AccommodationFamilyDAO : TestPackageDaoBase<AccommodationFamilyDTO>
    {
        public AccommodationFamilyDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "AccommodationFamilyTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_AccommodationFamily (ClientName, Family, Label) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   Family, \n" +
                "   Label \n";
        }

        public override void Insert(IList<AccommodationFamilyDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
