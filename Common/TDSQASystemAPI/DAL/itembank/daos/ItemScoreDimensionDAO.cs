using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>SetOfAdminItemDTO</code>s to the <code>OSS_Itembank..tblSetOfAdminItems</code> table
    /// </summary>
    public class ItemScoreDimensionDAO : TestPackageDaoBase<ItemScoreDimensionDTO>
    {
        public ItemScoreDimensionDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "ItemScoreDimensionType";
            InsertSql =
                "INSERT \n" +
                "   dbo.ItemScoreDimension (_Key, _fk_Item, _fk_AdminSubject, Dimension, ScorePoints, Weight, _fk_MeasurementModel, RecodeRule) \n" +
                "SELECT \n" +
                "   ItemScoreDimensionKey, \n" +
                "   ItemKey, \n" +
                "   SegmentKey, \n" +
                "   Dimension, \n" +
                "   ScorePoints, \n" +
                "   [Weight], \n" +
                "   MeasurementModel, \n" +
                "   RecodeRule \n";
        }

        public override void Insert(IList<ItemScoreDimensionDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
