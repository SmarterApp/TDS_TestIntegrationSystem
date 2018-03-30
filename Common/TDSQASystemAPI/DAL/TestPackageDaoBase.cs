using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TDSQASystemAPI.DAL.utils;
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
        private const string DEFAULT_TVP_VARIABLE_NAME = "@tvpData";
        private const string DEFAULT_CRITERIA_VARIABLE_NAME = "@criteria";

        private string insertSql = string.Empty;
        private readonly ReflectionObjectPopulator<T> reflectionObjectPopulator = new ReflectionObjectPopulator<T>();

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
        /// The SQL responsible for fetching a collection of records from the database.
        /// </summary>
        protected internal string SelectSql { get; set; }

        /// <summary>
        /// The SQL responsible for updating a collection of existing records in the database.
        /// </summary>
        protected internal string UpdateSql { get; set; }

        protected internal string ExistsSql { get; set; }

        protected internal string FindByExampleSql { get; set; }

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
                    var tableParam = new SqlParameter(DEFAULT_TVP_VARIABLE_NAME, SqlDbType.Structured)
                    {
                        TypeName = TvpType,
                        Value = recordsToSave.ToDataTable()
                    };

                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(tableParam);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Insert a record into the database (which is specified by the connection string)
        /// </summary>
        /// <param name="recordToSave">The <code>typeparamref name="T"</code> of record to persist.</param>
        public virtual void Insert(T recordToSave)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DbConnectionStringName].ConnectionString))
            {
                using (var command = new SqlCommand(InsertSql, connection))
                {
                    command.CommandType = CommandType.Text;

                    var testeeAttributeParam = command.Parameters.AddWithValue(DEFAULT_TVP_VARIABLE_NAME, recordToSave.ToDataTable());
                    testeeAttributeParam.SqlDbType = SqlDbType.Structured;
                    testeeAttributeParam.TypeName = TvpType;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Insert a record into the database (which is specified by the connection string)
        /// </summary>
        /// <param name="recordToSave">The <code>typeparamref name="T"</code> of record to persist.</param>
        public virtual bool Exists(T recordToCheck)
        {
            if (string.IsNullOrEmpty(ExistsSql))
            {
                throw new System.InvalidOperationException("Exists SQL is not defined");
            }
            var exists = false;
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DbConnectionStringName].ConnectionString))
            {
                using (var command = new SqlCommand(ExistsSql, connection))
                {
                    command.CommandType = CommandType.Text;

                    AddNaturalKeys(recordToCheck, command.Parameters);

                    connection.Open();
                    var count = int.Parse(command.ExecuteScalar().ToString());
                    exists = count > 0;
                }
            }
            return exists;
        }

        virtual protected void AddNaturalKeys(T record, SqlParameterCollection parameters)
        {
        }


        /// <summary>
        /// Insert a record into the database (which is specified by the connection string)
        /// </summary>
        /// <param name="recordToSave">The <code>typeparamref name="T"</code> of record to persist.</param>
        public virtual List<T> FindByExample(T recordToCheck)
        {
            if (string.IsNullOrEmpty(FindByExampleSql))
            {
                throw new System.InvalidOperationException("FindByExample SQL is not defined");
            }

            var results = new List<T>();
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DbConnectionStringName].ConnectionString))
            {
                using (var command = new SqlCommand(FindByExampleSql, connection))
                {
                    command.CommandType = CommandType.Text;

                    AddNaturalKeys(recordToCheck, command.Parameters);

                    connection.Open();
                    results = reflectionObjectPopulator.GetListFromDataReader(command.ExecuteReader());
                }
            }
            return results;
        }

        virtual protected void FindByExampleAddParameter(T record, SqlParameterCollection parameters)
        {
        }

        /// <summary>
        /// Get a collection of records from the database.
        /// </summary>
        /// <remarks>
        /// The SQL columns must match the object's property names exactly.  Any database columns that do not match
        /// the property name exactly must be aliased.
        /// </remarks>
        /// <param name="sql">The SQL to execute against the database.</param>
        /// <param name="criteria">The value to look for</param>
        /// <returns>A <code>List<typeparamref name="T"/></code> built from the records contained in the <code>IDataReader</code>.</returns>
        public virtual List<T> Find(object criteria)
        {
            var results = new List<T>();

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[DbConnectionStringName].ConnectionString))
            {
                using (var command = new SqlCommand(SelectSql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue(DEFAULT_CRITERIA_VARIABLE_NAME, criteria);

                    connection.Open();
                    results = reflectionObjectPopulator.GetListFromDataReader(command.ExecuteReader());
                }
            }

            return results;
        }

        /// <summary>
        /// Update an existing record int he database (which is specified by the connection string)
        /// </summary>
        /// <remarks>
        /// Unlike the other methods in this class, the <code>Update()</code> method expects the entire UPDATE SQL statement 
        /// </remarks>
        /// <param name="recordToUpdate">The <code>T</code> record to update.</param>
        public virtual void Update(T recordToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
