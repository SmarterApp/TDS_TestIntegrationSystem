using System.Collections.Generic;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>SetOfTestGradeDTO</code>s to the <code>OSS_Itembank..SetOfTestGrades</code> table
    /// </summary>
    public class SetOfTestGradeDAO : TestPackageDaoBase<SetOfTestGradeDTO>
    {
        public SetOfTestGradeDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "SetOfTestGradeTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.SetOfTestGrades (TestId, grade, RequireEnrollment, _fk_adminsubject, EnrolledSubject) \n" +
                "SELECT \n" +
                "   TestId, \n" +
                "   Grade, \n" +
                "   RequireEnrollment, \n" +
                "   SegmentKey, \n" +
                "   EnrolledSubject \n";
        }

        public override void Insert(IList<SetOfTestGradeDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
