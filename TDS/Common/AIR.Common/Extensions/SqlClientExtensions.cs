/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System.Data.SqlTypes;
using System.Xml;
using System.Xml.Linq;

namespace System.Data.SqlClient
{
    public static class SqlClientExtensions
    {
        /// <summary>
        /// Get a types value for use with SQL and deal with possible NULL's.
        /// </summary>
        private static object GetValue(object value)
        {
            if (value == null) return DBNull.Value;
            return value;
        }

        public static void AddParameter(this SqlCommand cmd, string parameterName, SqlDbType dbType, object value)
        {
            SqlParameter sqlParameter = new SqlParameter(parameterName, dbType);
            cmd.Parameters.Add(sqlParameter).Value = GetValue(value);
        }

        public static void AddParameter(this SqlCommand cmd, string parameterName, SqlDbType dbType, object value, int size)
        {
            SqlParameter sqlParameter = new SqlParameter(parameterName, dbType, size);
            cmd.Parameters.Add(sqlParameter).Value = GetValue(value);
        }

        public static void AddValue(this SqlCommand cmd, string parameterName, object value)
        {
            cmd.Parameters.AddWithValue(parameterName, GetValue(value));
        }

        public static void AddBit(this SqlCommand cmd, string parameterName, bool? value)
        {
            cmd.AddParameter(parameterName, SqlDbType.Bit, value, 1);
        }

        public static void AddInt(this SqlCommand cmd, string parameterName, int? value)
        {
            cmd.AddParameter(parameterName, SqlDbType.Int, value, 4);
        }

        public static void AddInt(this SqlCommand cmd, string parameterName, long? value)
        {
            cmd.AddParameter(parameterName, SqlDbType.Int, value, 4);
        }

        public static void AddBigInt(this SqlCommand cmd, string parameterName, long? value)
        {
            cmd.AddParameter(parameterName, SqlDbType.BigInt, value, 8);
        }

        public static void AddUniqueIdentifier(this SqlCommand cmd, string parameterName, Guid? value)
        {
            cmd.AddParameter(parameterName, SqlDbType.UniqueIdentifier, value);
        }

        public static void AddChar(this SqlCommand cmd, string parameterName, string value, int size)
        {
            cmd.AddParameter(parameterName, SqlDbType.Char, value, size);
        }

        public static void AddChar(this SqlCommand cmd, string parameterName, char value)
        {
            cmd.AddParameter(parameterName, SqlDbType.Char, value, 1);
        }

        public static void AddNChar(this SqlCommand cmd, string parameterName, string value, int size)
        {
            cmd.AddParameter(parameterName, SqlDbType.NChar, value, size);
        }

        public static void AddVarChar(this SqlCommand cmd, string parameterName, string value, int size)
        {
            cmd.AddParameter(parameterName, SqlDbType.VarChar, value, size);
        }

        public static void AddNVarChar(this SqlCommand cmd, string parameterName, string value, int size)
        {
            cmd.AddParameter(parameterName, SqlDbType.NVarChar, value, size);
        }

        public static void AddVarCharMax(this SqlCommand cmd, string parameterName, string value)
        {
            cmd.AddParameter(parameterName, SqlDbType.VarChar, value, -1);
        }

        public static void AddNVarCharMax(this SqlCommand cmd, string parameterName, string value)
        {
            cmd.AddParameter(parameterName, SqlDbType.NVarChar, value, -1);
        }

        public static void AddDateTime(this SqlCommand cmd, string parameterName, DateTime? value)
        {
            cmd.AddParameter(parameterName, SqlDbType.DateTime, value);
        }

        public static void AddXml(this SqlCommand cmd, string parameterName, SqlXml value)
        {
            cmd.AddParameter(parameterName, SqlDbType.Xml, value);
        }

        public static void AddXml(this SqlCommand cmd, string parameterName, XmlReader value)
        {
            cmd.AddParameter(parameterName, SqlDbType.Xml, new SqlXml(value));
        }

        public static void AddXml(this SqlCommand cmd, string parameterName, XDocument value)
        {
            cmd.AddParameter(parameterName, SqlDbType.Xml, new SqlXml(value.CreateReader()));
        }
    }
}
