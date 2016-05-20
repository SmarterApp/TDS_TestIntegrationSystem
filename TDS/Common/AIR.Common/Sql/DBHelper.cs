/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AIR.Common.Sql
{
    public static class DbHelper
    {
        public static string Join<T>(T[] list, char delim)
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

            return string.Join(delim.ToString(), strList) + delim;
        }

        public static object GetValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }

        }
        public static object Nullify(object value)
        {
            if (value == null || value.ToString().Length < 1)
                return DBNull.Value;
            else
                return value;
        }

        //save a table to a file
        public static bool SaveCSV(string file, IColumnReader reader)
        {
            try
            {
                if (reader == null)
                    return false;
                bool bHasRow = false;
                //generate the file  
                using (TextWriter csvFile = new StreamWriter(file, false))
                {
                    if (reader.Columns != null)
                    {
                        bHasRow = true;
                        foreach (string colname in reader.Columns)
                        {
                            csvFile.Write(colname);
                            csvFile.Write(',');
                        }
                        csvFile.WriteLine();
                    }
                    reader.FixNulls = true;
                    int colCount = reader.Columns.Count;
                    while (reader.Read())
                    {
                        for (int i = 0; i < colCount; i++)
                        {
                            csvFile.Write(reader[i].ToString());
                            csvFile.Write(',');
                        }
                        csvFile.WriteLine();
                    }
                }
                return bHasRow;
            }
            catch
            {
                throw;
            }           
        }

    }
}
