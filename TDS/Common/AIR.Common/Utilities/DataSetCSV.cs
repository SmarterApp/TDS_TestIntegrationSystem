/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Data;
using System.IO;

namespace AIR.Common.Utilities
{
    /// <summary>
    /// Write out a dataset as a CSV.
    /// </summary>
    public class DataSetCSV
    {
        public static void Write(DataTable dt, StreamWriter file, bool WriteHeader)
        {
            if (WriteHeader)
            {
                string[] arr = new String[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    arr[i] = dt.Columns[i].ColumnName;
                    arr[i] = GetWriteableValue(arr[i]);
                }
                file.WriteLine(string.Join(",", arr));
            }
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                string[] dataArr = new String[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    object o = dt.Rows[j][i];
                    dataArr[i] = GetWriteableValue(o);
                }
                file.WriteLine(string.Join(",", dataArr));
            }
        }

        public static void Write(DataSet ds, StreamWriter file, bool WriteHeader)
        {
            if (ds == null || ds.Tables.Count < 1)
                return;
            for (int tbl = 0; tbl < ds.Tables.Count; tbl++)
            {
                DataTable dt = ds.Tables[tbl];
                if (WriteHeader)
                {
                    string[] arr = new String[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        arr[i] = dt.Columns[i].ColumnName;
                        arr[i] = GetWriteableValue(arr[i]);
                    }
                    file.WriteLine(string.Join(",", arr));
                }
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    string[] dataArr = new String[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        object o = dt.Rows[j][i];
                        dataArr[i] = GetWriteableValue(o);
                    }
                    file.WriteLine(string.Join(",", dataArr));
                }

                file.WriteLine();
            }
        }

        private static string GetWriteableValue(object o)
        {
            if (o == null || o == Convert.DBNull)
                return "";
            else if (o.ToString().IndexOf(",") == -1)
                return o.ToString();
            else
                return "\"" + o.ToString() + "\"";
        }
    }
}
