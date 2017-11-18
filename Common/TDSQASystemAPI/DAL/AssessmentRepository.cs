
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TDSQASystemAPI.DAL
{
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly String connectionString;

        public AssessmentRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["itembank"].ConnectionString;
        }

        public void Delete(String testPackageKey)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("dbo.spDeleteAssessment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@testPackageKey", testPackageKey);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}