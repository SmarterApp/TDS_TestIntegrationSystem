using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>StimulusDTO</code>s to the <code>OSS_Itembank..tblStimulus</code> table
    /// </summary>
    public class StimulusDAO : TestPackageDaoBase<StimulusDTO>
    {
        public StimulusDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "StimulusTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblStimulus (_efk_ItemBank, _efk_ITSKey, FilePath, FileName, DateLastUpdated, _Key, LoadConfig) \n" +
                "SELECT \n" +
                "   ItemBankKey, \n" +
                "   ItsKey, \n" +
                "   FilePath, \n" +
                "   [FileName], \n" +
                "   DateLastUpdated, \n" +
                "   StimulusKey, \n" +
                "   TestVersion \n";
        }

        public override void Insert(IList<StimulusDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
