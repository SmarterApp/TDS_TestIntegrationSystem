using System.Collections.Generic;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>SetOfItemStimuliDTO</code>s to the <code>OSS_Itembank..tblSetOfItemStimuli</code> table
    /// </summary>
    public class SetOfItemStimuliDAO : TestPackageDaoBase<SetOfItemStimuliDTO>
    {
        public SetOfItemStimuliDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "SeOfItemStimuliTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblSetOfItemStimuli (_fk_item, _fk_stimulus, _fk_adminsubject, loadconfig) \n" +
                "SELECT \n" +
                "   ItemKey, \n" +
                "   StimulusKey, \n" +
                "   SegmentKey, \n" +
                "   TestVersion \n";
            ExistsSql = "SELECT count(*) FROM dbo.tblSetOfItemStimuli t WHERE t._fk_item = @itemKey AND t._fk_stimulus = @stimulus AND t._fk_adminsubject = @segmentKey";
        }

        override protected void AddNaturalKeys(SetOfItemStimuliDTO stimuliDTO, SqlParameterCollection parameters)
        {
            parameters.AddWithValue("@itemKey", stimuliDTO.ItemKey);
            parameters.AddWithValue("@stimulus", stimuliDTO.StimulusKey);
            parameters.AddWithValue("@segmentKey", stimuliDTO.SegmentKey);
        }
    }    
}
