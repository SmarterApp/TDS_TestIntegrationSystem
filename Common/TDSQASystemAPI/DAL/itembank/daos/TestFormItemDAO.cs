using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>AdminStimulusDTO</code>s to the <code>OSS_Itembank..tblAdminStimulsu</code> table
    /// </summary>
    public class TestFormItemDAO : TestPackageDaoBase<TestFormItemDTO>
    {
        public TestFormItemDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "TestFormItemTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.TestFormItem (_fk_Item, _efk_ITSFormKey, FormPosition, _fk_AdminSubject, _fk_TestForm, isActive) \n" +
                "SELECT \n" +
                "   ItemKey, \n" +
                "   ITSFormKey, \n" +
                "   FormPosition, \n" +
                "   SegmentKey, \n" +
                "   TestFormKey, \n" +
                "   IsActive \n";
        }

        public override void Insert(IList<TestFormItemDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
