using System.Collections.Generic;
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
        }

        public override void Insert(IList<SetOfItemStimuliDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
