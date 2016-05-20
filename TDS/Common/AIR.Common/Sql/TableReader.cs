/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;

namespace AIR.Common.Sql
{
    /// <summary>
    /// Provides a way of reading a forward-only stream of rows from a list of DbTable objects. This is used to mimic
    /// a ADO.NET DataReader but is disconnected and much faster than a DataTable. Also using the Load() static method
    /// provides the best practices used for loading data as quickly as possible from a DB.  
    /// </summary>
    /// <remarks>This doesn't support GetSchemaTable so do not try to load into a DataSet or DataTable.</remarks>
    public class TableReader : IColumnReader, IEnumerable
    {
        private int rowIdx = -1; // start with row index of -1, the user has to call Read() first as part of the IDataReader API
        private int tableIdx = 0; // start with the first table, to go to the next one call NextResult()
        private readonly List<DbTable> tables;
        private bool fixNulls = false;
        private bool fixMissing = false;
        private bool forceTypes = false;

        /// <summary>
        /// Current readers columns
        /// </summary>
        public List<string> Columns
        {
            get
            {
                if (this.tables.Count == 0) return null;
                return this.tables[tableIdx].Columns;
            }
        }

        public List<DbTable> Tables
        {
            get { return tables; }
        }

        /// <summary>
        /// Current reader's table
        /// </summary>
        public DbTable Table
        {
            get
            {
                if (this.tables.Count == 0) return null;
                return this.tables[tableIdx];
            }
        }

        /// <summary>
        /// If set to true then for a missing column return its default value for the type.
        /// </summary>
        public bool FixMissing
        {
            get { return fixMissing; }
            set { fixMissing = value; }
        }

        /// <summary>
        /// If set to true then for NULL values return the default value for the type (e.x. a NULL for integer would return 0)
        /// </summary>
        public bool FixNulls
        {
            get { return fixNulls; }
            set { fixNulls = value; }
        }

        /// <summary>
        /// If set to true then if we are getting a value which doesn't match the type we requested force conversion into the type.
        /// </summary>
        public bool ForceTypes
        {
            get { return forceTypes; }
            set { forceTypes = value; }
        }

        public TableReader(List<DbTable> tables, bool fixNulls, bool fixMissing)
        {
            this.tables = tables;
            this.fixNulls = fixNulls;
            this.fixMissing = fixMissing;
        }

        public TableReader(List<DbTable> tables) : this(tables, false, false) { }

        public TableReader(DbTable result) : this(new List<DbTable>(new DbTable[] { result }), false, false) { }

        public bool IsClosed
        {
            get { return false; }
        }

        public void Close()
        {
            // since this is already disconnected just reset row and table indexes
            rowIdx = -1; 
            tableIdx = 0;
        }

        /// <summary>
        /// Is the reader currently on a row and data is available
        /// </summary>
        public bool IsReadable
        {
            get
            {
                if (Table != null)
                {
                    return Table.Rows.Count > 0 && rowIdx > -1;
                }

                return false;
            }
        }

        public bool Read()
        {
            if (rowIdx < this.tables[tableIdx].Rows.Count-1)
            {
                rowIdx++;
                return true;
            }
            
            return false;
        }

        public bool NextResult()
        {
            if (tables == null) return false;
            if (tables.Count == 0) return false;
            
            if (tableIdx < tables.Count - 1)
            {
                rowIdx = -1;
                tableIdx++;
                return true;
            }
            
            return false;
        }

        public void Dispose()
        {
            Close();
        }

        public string GetName(int i)
        {
            return this.tables[tableIdx].Columns[i];
        }

        /// <summary>
        /// Gets the type for the column
        /// </summary>
        /// <remarks>
        /// To get the correct type you need at least one row otherwise I return a string type. This is because
        /// for performance we do not get the schema of the table and infer this from the data returned. 
        /// </remarks>
        public Type GetFieldType(int i)
        {
            if (this.tables[tableIdx].Rows.Count == 0) return typeof (string);
            return this.tables[tableIdx].Rows[0][i].GetType();
        }

        public string GetDataTypeName(int i)
        {
            return GetFieldType(i).Name;
        }

        public int FieldCount
        {
            get { return this.tables[tableIdx].Columns.Count; }
        }

        public int RowCount
        {
            get
            {
                if (Table == null) return -1;
                return Table.Rows.Count;
            }
        }

        public int GetOrdinal(string name)
        {
            // check if name is null/empty and there are any tables
            if (string.IsNullOrEmpty(name) || this.tables.Count == 0) return -1;

            return tables[tableIdx].Columns.FindIndex(delegate(string column)
            {
                return string.Equals(name, column, StringComparison.CurrentCultureIgnoreCase);
            });
        }

/*
        public int GetOrdinal(string name)
        {
            if (string.IsNullOrEmpty(name) || this.tables.Count == 0) return -1;
            return this.tables[tableIdx].Columns.IndexOf(name.ToUpper());
        }
*/

        public bool IsDBNull(int i)
        {
            return (this[i] == DBNull.Value);
        }

        public bool IsDBNull(string name)
        {
            return IsDBNull(GetOrdinal(name));
        }

        public object GetValue(int i)
        {
            return this.tables[tableIdx].Rows[rowIdx][i];
        }

        public object GetValue(string name )
        {
            return GetValue(GetOrdinal(name));
        }

        public bool HasColumn(string name)
        {
            return GetOrdinal(name) != -1;
        } 

        public bool HasValue(string name)
        {
            return this.HasColumn(name) && !this.IsDBNull(name);
        }

        public int GetValues(object[] values)
        {
            this.tables[tableIdx].Rows[rowIdx].CopyTo(values, 0);
            return this.tables[tableIdx].Rows[rowIdx].Length;
        }

/*
        /// <summary>
        /// Gets a value of type <see cref="System.Object" /> from the datareader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public object GetValue(string name)
        {
            return GetValue(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a value of type <see cref="System.Object" /> from the datareader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual object GetValue(int i)
        {
            return GetGenericValue<object>(i);
        }
*/

        /// <summary>
        /// Gets a string value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns empty string for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public string GetString(string name)
        {
            return GetString(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a string value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns empty string for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual string GetString(int i)
        {
            return GetValue<string>(i) ?? String.Empty;
        }

        /// <summary>
        /// Gets an integer from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public int GetInt32(string name)
        {
            return GetInt32(GetOrdinal(name));
        }

        /// <summary>
        /// Gets an integer from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual int GetInt32(int i)
        {
            return GetValue<int>(i);
        }

        /// <summary>
        /// Gets a double from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public double GetDouble(string name)
        {
            return GetDouble(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a double from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual double GetDouble(int i)
        {
            return GetValue<double>(i);
        }

        /// <summary>
        /// Gets a Guid value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns Guid.Empty for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public Guid GetGuid(string name)
        {
            return GetGuid(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a Guid value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns Guid.Empty for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual Guid GetGuid(int i)
        {
            return GetValue<Guid>(i);
        }

        /// <summary>
        /// Gets a boolean value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns <see langword="false" /> for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public bool GetBoolean(string name)
        {
            return GetBoolean(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a boolean value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns <see langword="false" /> for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual bool GetBoolean(int i)
        {
            return GetValue<bool>(i);
        }

        /// <summary>
        /// Gets a date value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns DateTime.MinValue for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public virtual DateTime GetDateTime(string name)
        {
            return GetDateTime(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a date value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns DateTime.MinValue for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual DateTime GetDateTime(int i)
        {
            return GetValue<DateTime>(i);
        }

        /// <summary>
        /// Gets a decimal value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public decimal GetDecimal(string name)
        {
            return GetDecimal(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a decimal value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual decimal GetDecimal(int i)
        {
            return GetValue<decimal>(i);
        }

        /// <summary>
        /// Gets a Single value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public float GetFloat(string name)
        {
            return GetFloat(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a Single value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual float GetFloat(int i)
        {
            return GetValue<float>(i);
        }

        /// <summary>
        /// Gets a Short value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public short GetInt16(string name)
        {
            return GetInt16(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a Short value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual short GetInt16(int i)
        {
            return GetValue<short>(i);
        }

        /// <summary>
        /// Gets a Long value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public Int64 GetInt64(string name)
        {
            return GetInt64(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a Long value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual Int64 GetInt64(int i)
        {
            return GetValue<Int64>(i);
        }

        /// <summary>
        /// Gets a byte value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public byte GetByte(string name)
        {
            return GetByte(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a byte value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual byte GetByte(int i)
        {
            return GetValue<byte>(i);
        }

        /// <summary>
        /// Invokes the GetBytes method of the underlying datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        /// <param name="buffer">Array containing the data.</param>
        /// <param name="bufferOffset">Offset position within the buffer.</param>
        /// <param name="fieldOffset">Offset position within the field.</param>
        /// <param name="length">Length of data to read.</param>
        public Int64 GetBytes(string name, Int64 fieldOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Invokes the GetBytes method of the underlying datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        /// <param name="buffer">Array containing the data.</param>
        /// <param name="bufferOffset">Offset position within the buffer.</param>
        /// <param name="fieldOffset">Offset position within the field.</param>
        /// <param name="length">Length of data to read.</param>
        public virtual Int64 GetBytes(int i, Int64 fieldOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        public Type GetFieldType(string column)
        {
            return this.GetFieldType(GetOrdinal(column));
        }

        public object this[int i]
        {
            get
            {
                if (rowIdx < 0 || this.tables[tableIdx].Rows.Count == 0) return null;
                return this.tables[tableIdx].Rows[rowIdx][i];
            }
        }

        public object this[string name]
        {
            get { return this[GetOrdinal(name)]; }
        }

        public IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this);
        }

        #region Helper Functions

        public T GetValue<T>(int i)
        {
            return GetValue<T>(i, default(T));
        }

        /// <summary>
        /// A generic ordinal value reader
        /// </summary>
        public T GetValue<T>(int i, T nullValue)
        {
            // check if the column was found
            if (i == -1)
            {
                // if fixing missing is true then return default value for this type
                if (this.fixMissing) return default(T);

                throw new IndexOutOfRangeException("Could not find the column name. If you want to ignore missing columns then set the property FixMissing to true.");
            }

            object value = this[i];

            // check if the value is null
            if (value == DBNull.Value)
            {
                // if fixing nulls is true then return default value for this type
                if (fixNulls) return nullValue;

                throw new SqlNullValueException("The column '" + this.GetName(i) + "' contains a NULL value. If you want to ignore nulls then set the property FixNulls to true.");
            }

            // if the value is the correct type return it
            if (value is T)
            {
                return (T)value;
            }

            // if forcing types then return converted value for this type
            if (ForceTypes)
            {
                return value.ConvertTo<T>();
            }

            // if we get here then the columns data existed but the value didn't match the type
            throw new InvalidCastException("There was an error casting the column '" + this.GetName(i) + "' to the type " + typeof(T) + ", the actual type is " + value.GetType() + ".");
        }

        /// <summary>
        /// A generic name value reader
        /// </summary>
        public T GetValue<T>(string name)
        {
            return GetValue<T>(GetOrdinal(name));
        }
        
        public T GetValue<T>(string name, T nullValue)
        {
            return GetValue<T>(GetOrdinal(name), nullValue);
        }

        /// <summary>
        /// A nullable ordinal reader that returns a value
        /// </summary>
        public T? GetValueNullable<T>(int i) where T : struct
        {
            if (i == -1) return null;

            object value = this[i];

            if (value == DBNull.Value)
            {
                return null;
            }
            else
            {
                try
                {
                    return (T?)value;
                }
                catch (InvalidCastException ice)
                {
                    throw new InvalidCastException("There was an error casting the column '" + this.GetName(i) + "': " + ice.Message);
                }
            }
        }

        /// <summary>
        /// A nullable ordinal reader that returns a value
        /// </summary>
        public T? GetValueNullable<T>(string i) where T : struct
        {
            return GetValueNullable<T>(GetOrdinal(i));
        }

        #endregion

        #region Not Supported Functions

        public int Depth
        {
            get { return 0; }
        }

        public int RecordsAffected
        {
            get { return 0; }
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        /// <remarks>This information comes from SQL and we cannot support this function being a disconnected object</remarks>
        public DataTable GetSchemaTable()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a char value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns Char.MinValue for null. You cannot use GetChar to read a VarChar 
        /// column in chunks if CommandBehavior is set to SequentialAccess.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public char GetChar(string name)
        {
            throw new NotImplementedException("TODO");
            //return GetChar(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a char value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns Char.MinValue for null. You cannot use GetChar to read a VarChar 
        /// column in chunks if CommandBehavior is set to SequentialAccess.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual char GetChar(int i)
        {
            throw new NotImplementedException("TODO");
            //char[] myChar = new char[1];
            //long charRead = this.GetChars(i, 0, myChar, 0, 1);

            //// TODO: Look into what happens with NULL's situation here, I think it should return 0
            //if (charRead == 0)
            //{
            //    return char.MinValue;
            //}
            //else
            //{
            //    return myChar[0];
            //}
        }

        /// <summary>
        /// Invokes the GetChars method of the underlying datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        /// <param name="buffer">Array containing the data.</param>
        /// <param name="bufferOffset">Offset position within the buffer.</param>
        /// <param name="fieldOffset">Offset position within the field.</param>
        /// <param name="length">Length of data to read.</param>
        public Int64 GetChars(string name, Int64 fieldOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Invokes the GetChars method of the underlying datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        /// <param name="buffer">Array containing the data.</param>
        /// <param name="bufferOffset">Offset position within the buffer.</param>
        /// <param name="fieldOffset">Offset position within the field.</param>
        /// <param name="length">Length of data to read.</param>
        public virtual Int64 GetChars(int i, Int64 fieldOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        #endregion

        /// <summary>
        /// Gets a TableReader for all return sets and does not close the connection.
        /// </summary>
        public static TableReader Load(IDbCommand cmd)
        {
            List<DbTable> tables;

            using (IDataReader reader = cmd.ExecuteReader())
            {
                tables = DbTable.FillResults(reader);
            }

            return new TableReader(tables);
        }

    }
}