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
using Microsoft.VisualBasic.FileIO;

namespace AIR.Common.Sql
{
    /// <summary>
    /// A class used to hold the data returned from a SQL query.
    /// </summary>
    [Serializable]
    public sealed class DbTable : IEnumerable<object[]>
    {
        private List<string> columns;
        private List<object[]> rows;

        public List<string> Columns
        {
            get
            {
                if (columns == null) columns = new List<string>();
                return columns;
            }
            set { columns = value; }
        }

        public List<object[]> Rows
        {
            get
            {
                if (rows == null) rows = new List<object[]>();
                return rows;
            }
            set { rows = value; }
        }

        public DbTable()
        {
        }

        public DbTable(List<string> columns, List<object[]> rows)
        {
            this.columns = columns;
            this.rows = rows;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (object[] row in rows)
            {
                yield return row;
            }
        }

        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator()
        {
            foreach (object[] row in rows)
            {
                yield return row;
            }
        }

        /// <summary>
        /// Loads all the records for a result set (single query).
        /// </summary>
        public static DbTable FillRecords(IDataReader reader)
        {
            DbTable result = new DbTable();

            int fieldCount = reader.FieldCount;
            result.Columns = new List<string>(fieldCount);

            for (int i = 0; i < fieldCount; i++)
            {
                result.Columns.Add(reader.GetName(i)); // .ToUpper()
            }

            while (reader.Read())
            {
                object[] objects = new object[reader.FieldCount];
                reader.GetValues(objects);
                result.Rows.Add(objects);
            }

            return result;
        }

        /// <summary>
        /// Loads all records for each result set (multiple queries).
        /// </summary>
        public static List<DbTable> FillResults(IDataReader reader)
        {
            List<DbTable> tables = new List<DbTable>();

            do
            {
                DbTable result = FillRecords(reader);
                tables.Add(result);

            } while (reader.NextResult());

            return tables;
        }

        public static DbTable ParseCSV(string file)
        {
            DbTable csvTable = new DbTable();

            using (TextFieldParser parser = FileSystem.OpenTextFieldParser(file))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // parse columns
                string[] rowData = parser.ReadFields(); // first row is assumed to be columns
                int columnCount = rowData.Length;
                csvTable.Columns = new List<string>(rowData);

                // parse rows
                while (!parser.EndOfData)
                {
                    rowData = parser.ReadFields();
                    object[] objects = new object[columnCount];

                    for (int i = 0; i < columnCount; i++)
                    {
                        objects[i] = rowData[i];
                    }

                    csvTable.Rows.Add(objects);
                }

                parser.Close();
            }

            return csvTable;
        }

    }
}
