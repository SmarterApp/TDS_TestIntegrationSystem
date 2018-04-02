using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    public class UpdateConfigsDB
    {

        public UpdateConfigsDB()
        {
        }

        public void UpdateConfigs(string testKey)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DatabaseConnectionStringNames.ITEMBANK].ConnectionString))
            {
                using (var command = new SqlCommand("UpdateTDSConfigs", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@doall", "0"));
                    command.Parameters.Add(new SqlParameter("@testKey", testKey));

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public void LoadMeasurementParameters()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DatabaseConnectionStringNames.ITEMBANK].ConnectionString))
            {
                using (var command = new SqlCommand("Load_MeasurementParameters", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
