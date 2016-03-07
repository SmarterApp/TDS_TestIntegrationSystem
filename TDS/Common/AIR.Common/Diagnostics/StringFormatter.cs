/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System.Text;

namespace AIR.Common.Diagnostics
{
    /// <summary>
    /// Static type used to format a list of objects into a string. Each object has ToString called and
    /// is concatenated by a StringBuilder and seperated by a pipe (|).
    /// </summary>
    public static class StringFormatter
    { 
        private const string SEPERATOR = "|";

        /// <summary>
        /// Formats a list of objects into a string. Each object has ToString called and
        /// is concatenated by a StringBuilder and seperated by a pipe (|).
        /// </summary>
        /// <param name="data">The objects to be seperated</param>
        /// <returns>A pipe seperated string</returns>
        public static string FormatData(params object[] data)
        {
            string ret = null;

            if (null != data)
            {
                StringBuilder sb = new StringBuilder();

                foreach (object obj in data)
                {
                    if (null != obj)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(SEPERATOR);
                        }
                        sb.Append(obj);
                    }
                }

                ret = sb.ToString();
            }

            return ret;
        }

        /// <summary>
        /// Takes an object and checks for null before calling ToString().
        /// </summary>
        /// <param name="value">The object to be stringified</param>
        /// <returns>null or ToString of value</returns>
        public static string SafeToString(object value)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}