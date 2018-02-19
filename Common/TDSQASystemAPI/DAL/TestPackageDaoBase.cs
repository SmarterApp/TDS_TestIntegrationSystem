using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TDSQASystemAPI.Extensions;

namespace TDSQASystemAPI.DAL
{
    /// <summary>
    /// A base class to provide boilerplate implementation for inserting test package records into the <code>OSS_Configs</code> and <code>OSS_Itembank</code>
    /// databases.
    /// </summary>
    /// <remarks>
    /// This base class is designed to work with passing a collection of POCOs to the database via table-valued parameters.  This means a table-valued paramaeter 
    /// type must exist on the MSSQL server.  Refer to 8_create_tvp_types_for_configs_and_itembank.sql for examples of how to create a table-valued parameter.
    /// </remarks>
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/table-valued-parameters"/>
    /// <typeparam name="T">The type that should be persisted.</typeparam>
    public class TestPackageDaoBase<T> : ITestPackageDao<T>
    {
        private string insertSql = "";
        private const string DEFAULT_TVP_VARIABLE_NAME = "@tvpData";

        /// <summary>
        /// The SQL responsible for inserting the collection of records into the database.
        /// </summary>
        /// <example>
        /// This example shows how the insert SQL might look in a class that derives from this one:
        /// <code>
        /// InsertSql =
        ///        "INSERT \n" +
        ///        "   dbo.Client_TesteeRelationshipAttribute (ClientName, TDS_ID, RTSName, Label, ReportName, AtLogin, SortOrder, RelationshipType) \n" +
        ///        "SELECT \n" +
        ///        "   ClientName, \n" +
        ///        "   TDS_ID, \n" +
        ///        "   RTSName, \n" +
        ///        "   Label, \n" +
        ///        "   ReportName, \n" +
        ///        "   AtLogin, \n" +
        ///        "   SortOrder, \n" +
        ///        "   RelationshipType \n" +
        /// </code>
        /// </example>
        /// <remarks>
        /// The default behavior of this method is to build an SQL statement that ends with "FROM @tvpData", meaning that
        /// the source data to insert is stored in a table-valued parameter variable.  Consequently, the default
        /// implementation of the Insert method expects a single parameter of <code>SqlDbType.Structured</code>. If the
        /// SQL for this property needs to deviate from the pattern (i.e. if this property is overridden), then the
        /// <code>Insert</code> method should also be overridden.
        /// </remarks>
        protected internal virtual string InsertSql {
            get { return insertSql; }
            set {
                if (!value.EndsWith("\n"))
                {
                    value = value + "\n";
                }

                insertSql = string.Format("{0}FROM \n\t{1}", value, DEFAULT_TVP_VARIABLE_NAME);
            }
        }

        /// <summary>
        /// The type of the table-valued parameter used to pass the <code>IList<typeparamref name="T"/></code>.
        /// </summary>
        protected internal string TvpType { get; set; }

        /// <summary>
        /// The name of the connection string in the app.config file.
        /// </summary>
        protected internal string DbConnectionStringName { get; set; }

        /// <summary>
        /// Insert a collection of records into the database (which is specified by the connection string)
        /// </summary>
        /// <param name="recordsToSave">The <code>IList<typeparamref name="T"/></code> of records to persist.</param>
        public virtual void Insert(IList<T> recordsToSave)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DbConnectionStringName].ConnectionString))
            {
                using (var command = new SqlCommand(InsertSql, connection))
                {
                    command.CommandType = CommandType.Text;
                    var testeeAttributeParam = command.Parameters.AddWithValue(DEFAULT_TVP_VARIABLE_NAME, recordsToSave.ToDataTable());
                    testeeAttributeParam.SqlDbType = SqlDbType.Structured;
                    testeeAttributeParam.TypeName = TvpType;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
