using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL.configs.dtos;
using TDSQASystemAPI.Extensions;

namespace TDSQASystemAPI.DAL.configs.daos
{
    public class TesteeAttributeDAO : ITestPackageDao<TesteeAttributeDTO>
    {
        private const string SQL_INSERT = 
            "INSERT \n" +
            "   dbo.Client_TesteeAttribute (clientname, TDS_ID, RTSName, type, Label, reportName, atLogin, SortOrder) \n" +
            "SELECT \n" +
            "   ClientName, \n" +
            "   TDS_ID, \n" +
            "   RTSName, \n" +
            "   [Type], \n" +
            "   Label, \n" +
            "   ReportName, \n" +
            "   AtLogin, \n" +
            "   SortOrder \n" +
            "FROM \n" +
            "   @tvpTesteeAttributes";

        public void Insert(IList<TesteeAttributeDTO> recordsToSave)
        {
            using(var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["configs"].ConnectionString))
            {
                using (var command = new SqlCommand(SQL_INSERT, connection))
                {
                    command.CommandType = CommandType.Text;
                    var testeeAttributeParam = command.Parameters.AddWithValue("@tvpTesteeAttributes", recordsToSave.ToDataTable());
                    testeeAttributeParam.SqlDbType = SqlDbType.Structured;
                    testeeAttributeParam.TypeName = "dbo.TesteeAttributeType";

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
