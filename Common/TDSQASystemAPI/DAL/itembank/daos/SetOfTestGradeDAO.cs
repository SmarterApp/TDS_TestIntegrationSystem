using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>SetOfTestGradeDTO</code>s to the <code>OSS_Itembank..SetOfTestGrades</code> table
    /// </summary>
    public class SetOfTestGradeDAO : TestPackageDaoBase<SetOfTestGradeDTO>
    {
        string DeleteSql = "DELETE from \n" +
                           "   dbo.SetOfTestGrades \n" +
                           "WHERE \n" +
                           "   _fk_AdminSubject = @TestKey \n";

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
            recordsToSave.ForEach(testGrade =>
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DbConnectionStringName].ConnectionString))
                {
                    using (var command = new SqlCommand(DeleteSql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@TestKey", testGrade.SegmentKey);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            });

            base.Insert(recordsToSave);
        }
    }
}
