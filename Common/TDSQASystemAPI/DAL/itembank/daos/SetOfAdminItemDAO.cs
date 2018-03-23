using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>SetOfAdminItemDTO</code>s to the <code>OSS_Itembank..tblSetOfAdminItems</code> table
    /// </summary>
    public class SetOfAdminItemDAO : TestPackageDaoBase<SetOfAdminItemDTO>
    {
        public SetOfAdminItemDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "SetOfAdminItemTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblSetOfAdminItems(" +
                "       _fk_item, " +
                "       _fk_adminsubject, " +
                "       loadconfig, " +
                "       _fk_strand, " +
                "       _fk_testadmin," +
                "       groupid, " +
                "       itemposition, " +
                "       isfieldtest, " +
                "       isactive, " +
                "       blockid, " +
                "       isrequired, " +
                "       groupkey, " +
                "       strandname, " +
                "       irt_a, " +
                "       irt_b, " +
                "       irt_c, " +
                "       irt_model, " +
                "       clstring, " +
                "       updateconfig) \n" +
                "SELECT \n" +
                "   ItemKey, \n" +
                "   SegmentKey, \n" +
                "   TestVersion, \n" +
                "   StrandKey, \n" +
                "   TestAdminKey, \n" +
                "   GroupId, \n" +
                "   ItemPosition, \n" +
                "   IsFieldTest, \n" +
                "   IsActive, \n" +
                "   BlockId, \n" +
                "   IsRequired, \n" +
                "   GroupKey, \n" +
                "   StrandName, \n" +
                "   IrtA, \n" +
                "   IrtB, \n" +
                "   IrtC, \n" +
                "   IrtModel, \n" +
                "   ClString, \n" +
                "   UpdatedTestVersion \n";
        }
    }
}
