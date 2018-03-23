using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>ItemMeasurementParameterDTO</code>s to the <code>OSS_Itembank..ItemMeasurementParameter</code> table
    /// </summary>
    public class ItemMeasurementParameterDAO : TestPackageDaoBase<ItemMeasurementParameterDTO>
    {
        public ItemMeasurementParameterDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "ItemMeasurementParameterTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.ItemMeasurementParameter(_fk_ItemScoreDimension, _fk_MeasurementParameter, parmvalue) \n" +
                "SELECT \n" +
                "   ItemScoreDimensionKey, \n" +
                "   MeasurementParameterKey, \n" +
                "   ParmValue \n";
        }
    }
}
