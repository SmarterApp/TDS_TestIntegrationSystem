/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using AIR.Common.Configuration;

namespace AIR.Common.Sql
{
    /// <summary>
    /// A base class for creating simple SQL repositories.
    /// </summary>
    public abstract class RepositoryBase
    {
        private string _connectionString;

        public virtual string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        protected RepositoryBase()
        {
        }

        protected RepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected string GetConnectionString(string key)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[key];
            return (connectionString == null) ? null : connectionString.ToString();
        }

        protected SqlCommand CreateCommand(string connectionString, CommandType cmdType, string cmdText)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand(cmdText, connection);
            cmd.CommandType = cmdType;
            cmd.CommandTimeout = AppSettingsHelper.GetInt32("SqlCommandTimeout", 60);

            return cmd;
        }

        protected SqlCommand CreateCommand(CommandType cmdType, string cmdText)
        {
            return CreateCommand(ConnectionString, cmdType, cmdText);
        }

        protected int ExecuteNonQuery(SqlCommand cmd)
        {
            int count;

            try
            {
                cmd.Connection.Open();
                count = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                LogException(ex, cmd);
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }

            LogVerbose(cmd);

            return count;
        }

        protected IColumnReader ExecuteTableReader(SqlCommand cmd)
        {
            IColumnReader reader;

            try
            {
                cmd.Connection.Open();
                reader = TableReader.Load(cmd);
            }
            catch (Exception ex)
            {
                LogException(ex, cmd);
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }

            LogVerbose(cmd);

            return reader;
        }

        protected void ExecuteReader(SqlCommand cmd, Action<IColumnReader> readerProcessing)
        {
            try
            {
                cmd.Connection.Open();

                using (IColumnReader reader = new ColumnReader(cmd.ExecuteReader()))
                {
                    readerProcessing(reader);
                }
            }
            catch (Exception ex)
            {
                LogException(ex, cmd);
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }

            // log SP's
            LogVerbose(cmd);
        }

        /// <summary>
        /// Check if the DB has the SP
        /// </summary>
        /// <remarks>
        /// Avoid usage of this function as it has to query to figure out info.
        /// Currently this is NOT used in any code.
        /// </remarks>
        protected bool HasSP(string name)
        {
            string sql = string.Format("select OBJECT_ID('[dbo].{0}') AS ID", name);

            SqlCommand cmd = CreateCommand(CommandType.Text, sql);

            using (IColumnReader reader = ExecuteTableReader(cmd))
            {
                if (reader.Read())
                {
                    return !reader.IsDBNull("ID");
                }
            }

            return false;
        }

        public string Join<T>(T[] list)
        {
            string[] strList;

            if (list is string[])
            {
                strList = list as string[];
            }
            else
            {
                strList = new string[list.Length];
                for (int i = 0; i < list.Length; i++)
                {
                    strList[i] = list[i].ToString();
                }
            }

            return string.Join(Delim.ToString(), strList);
        }

        protected static readonly char Delim = '|';

        /// <summary>
        /// Get a types value for use with SQL and deal with possible NULL's.
        /// </summary>
        public object GetValue(object value)
        {
            if (value == null) return DBNull.Value;
            return value;
        }

        protected DataTable GetDataTable(SqlDataReader sqlDataReader)
        {
            DataTable schemaTable = sqlDataReader.GetSchemaTable();
            DataTable dataTable = new DataTable();

            int count = schemaTable.Rows.Count;

            for (int i = 0; i < count; i++)
            {
                DataRow dataRow = schemaTable.Rows[i];
                string columnName = dataRow["ColumnName"] as string;
                Type columnType = dataRow["DataType"] as Type;
                DataColumn dataColumn = new DataColumn(columnName, columnType);
                dataTable.Columns.Add(dataColumn);
            }

            try
            {
                dataTable.BeginLoadData();

                while (sqlDataReader.Read())
                {
                    object[] values = new object[count];
                    sqlDataReader.GetValues(values);
                    dataTable.LoadDataRow(values, true);
                }
            }
            finally
            {
                dataTable.EndLoadData();
            }

            return dataTable;
        }

        protected DataSet GetDataSet(SqlDataReader sqlDataReader)
        {
            DataSet dataSet = new DataSet();

            if (sqlDataReader.HasRows)
            {
                dataSet.Tables.Add(GetDataTable(sqlDataReader));
            }

            while (sqlDataReader.NextResult())
            {
                dataSet.Tables.Add(GetDataTable(sqlDataReader));
            }

            return dataSet;
        }

        #region Debugging

        private static bool IsNumericType(object obj)
        {
            if (obj == null) return false;

            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get SQL parameters in readable string format
        /// </summary>
        protected string GetTraceParameters(SqlCommand cmd)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                SqlParameter parameter = cmd.Parameters[i];

                if (i > 0) sb.Append(", ");

                // add parameter name
                sb.AppendFormat("@{0}=", parameter.ParameterName);

                // add parameter value
                if (parameter.Value == null || parameter.Value == DBNull.Value)
                {
                    // null's
                    sb.Append("null");
                }
                else if (IsNumericType(parameter.Value))
                {
                    // numbers
                    sb.Append(parameter.Value);
                }
                else if (parameter.Value is bool)
                {
                    // boolean
                    sb.Append(((bool)parameter.Value) ? "1" : "0");
                }
                else if (parameter.Value is DateTime)
                {
                    // dates
                    SqlDateTime sqlDateTime = new SqlDateTime((DateTime)parameter.Value);
                    sb.AppendFormat("'{0}'", sqlDateTime);
                }
                else
                {
                    // everything else
                    sb.AppendFormat("'{0}'", parameter.Value);
                }
            }

            return sb.ToString();
        }

        protected virtual void LogVerbose(SqlCommand cmd)
        {
        }

        protected virtual void LogException(Exception ex, SqlCommand cmd)
        {
        }

        #endregion

    }
}