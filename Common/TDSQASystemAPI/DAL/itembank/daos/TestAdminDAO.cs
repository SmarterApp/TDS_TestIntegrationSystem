using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>TestAdminDTO</code>s to the <code>OSS_Itembank..tblTestAdmin</code> table
    /// </summary>
    public class TestAdminDAO : TestPackageDaoBase<TestAdminDTO>
    {
        public TestAdminDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "TestAdminTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblTestAdmin (_Key, SchoolYear, Season, _fk_Client, [Description], LoadConfig) \n" +
                "SELECT \n" +
                "   AdminKey, \n" +
                "   SchoolYear, \n" +
                "   Season, \n" +
                "   ClientKey, \n" +
                "   [Description], \n" +
                "   TestVersion \n";
            SelectSql =
                "SELECT \n" +
                "   _Key AS AdminKey, \n" +
                "   SchoolYear, \n" +
                "   Season, \n" +
                "   _fk_client AS ClientKey, \n" +
                "   [Description], \n" +
                "   LoadConfig AS TestVersion \n" +
                "FROM \n" +
                "   tblTestAdmin \n" +
                "WHERE \n" +
                "   _key = @criteria";
            UpdateSql =
                "UPDATE \n" +
                "   tblTestAdmin \n" +
                "SET \n" +
                "   updateConfig = @updateConfig \n" +
                "WHERE \n" +
                "   _Key = @key";
        }

        public override void Update(TestAdminDTO recordToUpdate)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DbConnectionStringName].ConnectionString))
            {
                using (var command = new SqlCommand(UpdateSql, connection))
                {
                    command.CommandType = CommandType.Text;
                    var versionParam = command.Parameters.AddWithValue("@updateConfig", recordToUpdate.TestVersion);
                    versionParam.SqlDbType = SqlDbType.BigInt;
                    var keyParam = command.Parameters.AddWithValue("@key", recordToUpdate.AdminKey);
                    keyParam.SqlDbType = SqlDbType.VarChar;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
