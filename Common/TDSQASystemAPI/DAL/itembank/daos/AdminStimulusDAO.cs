using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>AdminStimulusDTO</code>s to the <code>OSS_Itembank..tblAdminStimulus</code> table
    /// </summary>
    public class AdminStimulusDAO : TestPackageDaoBase<AdminStimulusDTO>
    {
        public AdminStimulusDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "AdminStimulusTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblAdminStimulus (_fk_stimulus, _fk_adminsubject, numitemsrequired, maxitems, loadconfig, groupid) \n" +
                "SELECT \n" +
                "   StimulusKey, \n" +
                "   SegmentKey, \n" +
                "   NumItemsRequired, \n" +
                "   MaxItems, \n" +
                "   TestVersion, \n" +
                "   GroupId \n";
        }
    }
}
