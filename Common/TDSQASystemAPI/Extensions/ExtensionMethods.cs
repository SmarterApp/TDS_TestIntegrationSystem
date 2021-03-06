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
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace TDSQASystemAPI.Extensions
{
    public static class ExtensionMethods
    {
        public static string GetXmlFormattedString(this DateTime dateTime)
        {
            return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Unspecified);
        }

        public static void AddFormattedDateTimeParam(this XsltArgumentList args, string name, string namespaceUri, DateTime parameter)
        {
            args.AddParam(name, namespaceUri, parameter.GetXmlFormattedString());
        }

        public static string GetExceptionMessage(this Exception e, bool includeStackTrace)
        {
            StringBuilder message = new StringBuilder();
            Exception temp = e;
            while (temp != null)
            {
                message.Append(temp.Message);
                message.Append(" ");
                temp = temp.InnerException;
            }
            if (includeStackTrace && !String.IsNullOrEmpty(e.StackTrace))
            {
                message.Append(e.StackTrace);
            }
            return message.ToString();
        }

        /// <summary>
        /// Convert an <code>IList<typeparamref name="T"/></code> to a <code>DataTable</code>.
        /// </summary>
        /// <typeparam name="T">The type of object contained within the list</typeparam>
        /// <param name="list">The list of objects to convert to a <code>DataTable</code></param>
        /// <param name="tableName">The name of the <code>DataTable</code>.  Defaults to "dataTable" if not specified</param>
        /// <returns>A <code>DataTable</code> containing the list of T as rows/columns.</returns>
        /// <remarks>
        /// This method does not handle complex objects; the intent is to take a list of DTOs that map to database tables and
        /// create a <code>DataTable</code> from them.  This <code>DataTable</code> is ultimately passed to SQL Server as a 
        /// table-valued parameter, making saving lists of objects to the database easy.
        /// </remarks>
        public static DataTable ToDataTable<T>(this IList<T> list, string tableName = "dataTable")
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            var dataTable = new DataTable(tableName);

            if (typeof(T).IsValueType || typeof(T).Equals(typeof(string)))
            {
                var column = new DataColumn("Value");
                dataTable.Columns.Add(column);
                list.ForEach(l =>
                {
                    var dataRow = dataTable.NewRow();
                    dataRow[0] = l;
                    dataTable.Rows.Add(dataRow);
                });
            }
            else
            {
                var properties = TypeDescriptor.GetProperties(typeof(T));
                foreach (PropertyDescriptor prop in properties)
                {
                    dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

                list.ForEach(l =>
                {
                    var dataRow = dataTable.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                    {
                        try
                        {
                            dataRow[prop.Name] = prop.GetValue(l) ?? DBNull.Value;
                        }
                        catch (Exception e)
                        {
                            dataRow[prop.Name] = DBNull.Value;
                        }
                    }
                    dataTable.Rows.Add(dataRow);
                });
            }

            return dataTable;
        }

        public static DataTable ToDataTable<T>(this T item, string tableName = "dataTable")
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var dataTable = new DataTable(tableName);

            if (typeof(T).IsValueType || typeof(T).Equals(typeof(string)))
            {
                var column = new DataColumn("Value");
                dataTable.Columns.Add(column);
                var dataRow = dataTable.NewRow();
                dataRow[0] = item;
                dataTable.Rows.Add(dataRow);
            }
            else
            {
                var properties = TypeDescriptor.GetProperties(typeof(T));
                foreach (PropertyDescriptor prop in properties)
                {
                    dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

                var dataRow = dataTable.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    try
                    {
                        dataRow[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                    catch (Exception e)
                    {
                        dataRow[prop.Name] = DBNull.Value;
                    }
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        /// <summary>
        /// Get a value <code>V</code> for the specified key <code>K</code>.  If there is no value for the specified
        /// key, return a default value instead.
        /// </summary>
        /// <remarks>
        /// This code emulates the Java <code>GetOrDefault()</code> method on a <code>Map</code>.  It's also a convenient
        /// way to "inline" the <code>Dictionary#TryParse</code> and have it return some value in the event the key we're
        /// looking for doesn't exist.
        /// </remarks>
        /// <example>
        /// // Getting a value for the specified key:
        /// var dictionary = new Dictionary<string, string>
        /// {
        ///     { "firstKey", "foo" },
        ///     { "secondKey", "bar" }
        /// };
        ///
        /// var result = dictionary.GetOrDefault("firstKey", "default"); // result == "foo"
        /// 
        /// // Getting a default value when searching for a key that does not exist:
        /// var dictionary = new Dictionary<string, string>
        /// {
        ///     { "firstKey", "foo" },
        ///     { "secondKey", "bar" }
        /// };
        ///
        /// var result = dictionary.GetOrDefault("i-do-not-exist", "default"); // result == "default"
        /// </example>
        /// <typeparam name="K">The type of this dictionary's key.</typeparam>
        /// <typeparam name="V">The type of this dictionary's value.</typeparam>
        /// <param name="dictionary">The <code>Dictionary</code> that is being extended.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="defaultValue">The default value that should be returned in the event that a value cannot be
        /// found for the specified key.</param>
        /// <returns>The value <code>V</code> for the specified key <code>K</code>; otherwise the default value.</returns>
        public static V GetOrDefault<K, V>(this Dictionary<K, V> dictionary, K key, V defaultValue)
        {
            return dictionary.TryGetValue(key, out V value)
                ? value
                : defaultValue;
        }

        /// <summary>
        /// Convert a <code>string</code> to a nullable <code>int</code>.
        /// </summary>
        /// <param name="s">The <code>string</code> to convert</param>
        /// <returns>A nullable <code>int</code></returns>
        public static int? ToNullableInt(this string s)
        {
            return int.TryParse(s, out int i) ? i as int? : null;
        }

        /// <summary>
        /// Convert a <code>string</code> to a nullable <code>double</code>.
        /// </summary>
        /// <param name="s">The <code>string</code> to convert</param>
        /// <returns>A nullable <code>double</code></returns>
        public static double? ToNullableDouble(this string s)
        {
            return double.TryParse(s, out double d) ? d as double? : null;
        }
    }
}
