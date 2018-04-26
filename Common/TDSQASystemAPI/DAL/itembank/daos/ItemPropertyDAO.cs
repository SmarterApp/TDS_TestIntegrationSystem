using System.Collections.Generic;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>ItemPropertyDTO</code>s to the <code>OSS_Itembank..tblItemProps</code> table
    /// </summary>
    public class ItemPropertyDAO : TestPackageDaoBase<ItemPropertyDTO>
    {
        public ItemPropertyDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "ItemPropertyTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblItemProps (_fk_item, propname, propvalue, _fk_adminsubject, isactive) \n" +
                "SELECT \n" +
                "   ItemKey, \n" +
                "   PropertyName, \n" +
                "   PropertyValue, \n" +
                "   SegmentKey, \n" +
                "   IsActive \n";
            ExistsSql = "SELECT count(*) FROM dbo.tblItemProps t WHERE t._fk_item = @itemKey AND t.propname = @propname AND t.propvalue = @propvalue AND t._fk_adminsubject = @segmentKey";
        }

        override protected void AddNaturalKeys(ItemPropertyDTO propertyDTO, SqlParameterCollection parameters)
        {
            parameters.AddWithValue("@itemKey", propertyDTO.ItemKey);
            parameters.AddWithValue("@propname", propertyDTO.PropertyName);
            parameters.AddWithValue("@propvalue", propertyDTO.PropertyValue);
            parameters.AddWithValue("@segmentKey", propertyDTO.SegmentKey);
        }
    }
}
