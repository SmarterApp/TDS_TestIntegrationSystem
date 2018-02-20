using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>AdminStimulusDTO</code>s to the <code>OSS_Itembank..tblAdminStimulsu</code> table
    /// </summary>
    public class TestFormDAO : TestPackageDaoBase<TestFormDTO>
    {
        public TestFormDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "TestFormTable";
            InsertSql = 
                "INSERT \n" +
                "   dbo.TestForm (_fk_AdminSubject, Cohort, Language, _Key, FormID, _efk_ITSBank, _efk_ITSKey, LoadConfig) \n" +
                "SELECT \n" +
                "   SegmentKey, \n" +
                "   Cohort, \n" +
                "   [Language], \n" +
                "   TestFormKey, \n" +
                "   FormId, \n" +
                "   ITSBankKey, \n" +
                "   ITSKey, \n" +
                "   TestVersion \n"
        }

        public override void Insert(IList<TestFormDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
