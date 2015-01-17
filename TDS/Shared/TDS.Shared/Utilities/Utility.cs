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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace TDS.Shared
{
    /// <summary>
    /// A class containing support methods used internally to TDS.Common
    /// </summary>
    public class Utility
    {
        private static bool? _isRunningInMediumTrust;

        /// <summary>
        /// Gets a value indicating whether this instance is running in medium trust.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is running in medium trust; otherwise, <c>false</c>.
        /// </value>
        public static bool IsRunningInMediumTrust
        {
            get 
            {
                if (!_isRunningInMediumTrust.HasValue)
                    _isRunningInMediumTrust = (GetCurrentTrustLevel() == AspNetHostingPermissionLevel.Medium);
                return _isRunningInMediumTrust.Value;
            }
        }

        private static void FullTrustTrace(string message)
        {
            if(Debug.Listeners.Count > 0)
            {
                message = String.Concat(DateTime.Now.ToString("H:mm:ss:fff"), " > ", message);
                Debug.WriteLine(message, "TDS.Common");
                Console.WriteLine(message);
            }
        }

        private static AspNetHostingPermissionLevel GetCurrentTrustLevel()
        {
            foreach (AspNetHostingPermissionLevel trustLevel in
                new AspNetHostingPermissionLevel[]
                {
                    AspNetHostingPermissionLevel.Unrestricted,
                    AspNetHostingPermissionLevel.High,
                    AspNetHostingPermissionLevel.Medium,
                    AspNetHostingPermissionLevel.Low,
                    AspNetHostingPermissionLevel.Minimal
                })
            {
                try
                {
                    new AspNetHostingPermission(trustLevel).Demand();
                }
                catch (SecurityException)
                {
                    continue;
                }

                return trustLevel;
            }

            return AspNetHostingPermissionLevel.None;
        }

        #region WebUtility

        /// <summary>
        /// Builds a simple HTML table from the passed-in datatable
        /// </summary>
        /// <param name="tbl">System.Data.DataTable</param>
        /// <param name="tableWidth">The width of the table</param>
        /// <returns>System.String</returns>
        public static string DataTableToHtmlTable(DataTable tbl, string tableWidth)
        {
            StringBuilder sb = new StringBuilder();
            if(String.IsNullOrEmpty(tableWidth))
                tableWidth = "70%";

            if(tbl != null)
            {
                sb.AppendFormat("<table style=\"width: {0}\" cellpadding=\"4\" cellspacing=\"0\"><thead style=\"background-color: #dcdcdc\">", tableWidth);

                //header
                foreach(DataColumn col in tbl.Columns)
                    sb.AppendFormat("<th><span style=\"font-weight: bold\">{0}</span></th>", col.ColumnName);
                sb.Append("</thead>");

                //rows
                bool isEven = false;
                foreach(DataRow dr in tbl.Rows)
                {
                    if(isEven)
                        sb.Append("<tr>");
                    else
                        sb.Append("<tr style=\"background-color: #f5f5f5\">");

                    foreach(DataColumn col in tbl.Columns)
                        sb.AppendFormat("<td>{0}</td>", dr[col]);
                    sb.Append("</tr>");

                    isEven = !isEven;
                }
                sb.Append("</table>");
            }
            return sb.ToString();
        }

        [Obsolete("Deprecated: Use DataTableToHtmlTable() instead")]
        public static string DataTableToHTML(DataTable tbl, string tableWidth)
        {
            return DataTableToHtmlTable(tbl, tableWidth);
        }

        #endregion


        #region Tests

        /// <summary>
        /// Performs a case-insensitive comparison of two passed strings.
        /// </summary>
        /// <param name="stringA">The string to compare with the second parameter</param>
        /// <param name="stringB">The string to compare with the first parameter</param>
        /// <returns>
        /// 	<c>true</c> if the strings match; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMatch(string stringA, string stringB)
        {
            return String.Equals(stringA, stringB, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Evaluates an array of strings to determine if at least one item is a match
        /// </summary>
        /// <param name="stringA">The base comparison string.</param>
        /// <param name="matchStrings">The match strings.</param>
        /// <returns></returns>
        public static bool MatchesOne(string stringA, params string[] matchStrings)
        {
            for(int i = 0; i < matchStrings.Length; i++)
            {
                if(IsMatch(stringA, matchStrings[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Performs a case-insensitive comparison of two passed strings, 
        /// with an option to trim the strings before comparison.
        /// </summary>
        /// <param name="stringA">The string to compare with the second parameter</param>
        /// <param name="stringB">The string to compare with the first parameter</param>
        /// <param name="trimStrings">if set to <c>true</c> strings will be trimmed before comparison.</param>
        /// <returns>
        /// 	<c>true</c> if the strings match; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMatch(string stringA, string stringB, bool trimStrings)
        {
            if(trimStrings)
                return String.Equals(stringA.Trim(), stringB.Trim(), StringComparison.InvariantCultureIgnoreCase);

            return String.Equals(stringA, stringB, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether the passed string matches the passed RegEx pattern.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="matchPattern">The RegEx match pattern.</param>
        /// <returns>
        /// 	<c>true</c> if the string matches the pattern; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRegexMatch(string inputString, string matchPattern)
        {
            return Regex.IsMatch(inputString, matchPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Strips any whitespace from the passed string using RegEx
        /// </summary>
        /// <param name="inputString">The string to strip of whitespace</param>
        /// <returns></returns>
        public static string StripWhitespace(string inputString)
        {
            if(!String.IsNullOrEmpty(inputString))
                return Regex.Replace(inputString, @"\s", String.Empty);

            return inputString;
        }

        /// <summary>
        /// Determine whether the passed string is numeric, by attempting to parse it to a double
        /// </summary>
        /// <param name="str">The string to evaluated for numeric conversion</param>
        /// <returns>
        /// 	<c>true</c> if the string can be converted to a number; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsStringNumeric(string str)
        {
            double result;
            return (double.TryParse(str, NumberStyles.Float, NumberFormatInfo.CurrentInfo, out result));
        }

        /// <summary>
        /// Determines whether the passed DbType supports null values.
        /// </summary>
        /// <param name="dbType">The DbType to evaluate</param>
        /// <returns>
        /// 	<c>true</c> if the DbType supports null values; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableDbType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.Binary:
                    //case DbType.Byte:
                case DbType.Object:
                case DbType.String:
                case DbType.StringFixedLength:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Determines whether the current web user is authenticated by evaluating the HTTP context.
        /// </summary>
        /// 	<c>true</c> if the user is authenticaed; otherwise, <c>false</c>.
        public static bool UserIsAuthenticated()
        {
            HttpContext context = HttpContext.Current;

            if(context.User != null && context.User.Identity != null && !String.IsNullOrEmpty(context.User.Identity.Name))
                return true;

            return false;
        }

        #endregion


        #region Types

        /// <summary>
        /// I can't remember why I created this method. :P
        /// </summary>
        /// <param name="dbType">The DbType to evaluate</param>
        /// <returns>
        /// 	<c>true</c> if the specified DbType is parsable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsParsable(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.Binary:
                case DbType.Guid:
                case DbType.Object:
                case DbType.String:
                case DbType.StringFixedLength:
                    return false;
                default:
                    return true;
            }
        }

 
        /// <summary>
        /// Returns the SqlDbType for a give DbType
        /// </summary>
        /// <param name="dbType">The DbType to evaluate</param>
        /// <returns></returns>
        public static SqlDbType GetSqlDBType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.AnsiString:
                    return SqlDbType.VarChar;
                case DbType.AnsiStringFixedLength:
                    return SqlDbType.Char;
                case DbType.Binary:
                    return SqlDbType.VarBinary;
                case DbType.Boolean:
                    return SqlDbType.Bit;
                case DbType.Byte:
                    return SqlDbType.TinyInt;
                case DbType.Currency:
                    return SqlDbType.Money;
                case DbType.Date:
                    return SqlDbType.DateTime;
                case DbType.DateTime:
                    return SqlDbType.DateTime;
                case DbType.Decimal:
                    return SqlDbType.Decimal;
                case DbType.Double:
                    return SqlDbType.Float;
                case DbType.Guid:
                    return SqlDbType.UniqueIdentifier;
                case DbType.Int16:
                    return SqlDbType.Int;
                case DbType.Int32:
                    return SqlDbType.Int;
                case DbType.Int64:
                    return SqlDbType.BigInt;
                case DbType.Object:
                    return SqlDbType.Variant;
                case DbType.SByte:
                    return SqlDbType.TinyInt;
                case DbType.Single:
                    return SqlDbType.Real;
                case DbType.String:
                    return SqlDbType.NVarChar;
                case DbType.StringFixedLength:
                    return SqlDbType.NChar;
                case DbType.Time:
                    return SqlDbType.DateTime;
                case DbType.UInt16:
                    return SqlDbType.Int;
                case DbType.UInt32:
                    return SqlDbType.Int;
                case DbType.UInt64:
                    return SqlDbType.BigInt;
                case DbType.VarNumeric:
                    return SqlDbType.Decimal;

                default:
                    return SqlDbType.VarChar;
            }
        }

        /// <summary>
        /// Gets the string representation of the .NET type for a given DbType 
        /// </summary>
        /// <param name="dbType">The DbType to evaluate</param>
        /// <returns></returns>
        public static string GetSystemType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.AnsiString:
                    return "System.String";
                case DbType.AnsiStringFixedLength:
                    return "System.String";
                case DbType.Binary:
                    return "System.Byte[]";
                case DbType.Boolean:
                    return "System.Boolean";
                case DbType.Byte:
                    return "System.Byte";
                case DbType.Currency:
                    return "System.Decimal";
                case DbType.Date:
                    return "System.DateTime";
                case DbType.DateTime:
                    return "System.DateTime";
                case DbType.Decimal:
                    return "System.Decimal";
                case DbType.Double:
                    return "System.Double";
                case DbType.Guid:
                    return "System.Guid";
                case DbType.Int16:
                    return "System.Int16";
                case DbType.Int32:
                    return "System.Int32";
                case DbType.Int64:
                    return "System.Int64";
                case DbType.Object:
                    return "System.Object";
                case DbType.SByte:
                    return "System.SByte";
                case DbType.Single:
                    return "System.Single";
                case DbType.String:
                    return "System.String";
                case DbType.StringFixedLength:
                    return "System.String";
                case DbType.Time:
                    return "System.TimeSpan";
                case DbType.UInt16:
                    return "System.UInt16";
                case DbType.UInt32:
                    return "System.UInt32";
                case DbType.UInt64:
                    return "System.UInt64";
                case DbType.VarNumeric:
                    return "System.Decimal";
                default:
                    return "System.String";
            }
        }

        /// <summary>
        /// Converts the passed byte array to a string
        /// </summary>
        /// <param name="arrInput">The byte array to convert to a string</param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] arrInput)
        {
            StringBuilder sOutput = new StringBuilder(arrInput.Length * 2);
            for(int i = 0; i < arrInput.Length; i++)
                sOutput.Append(arrInput[i].ToString("x2"));

            return sOutput.ToString();
        }

        /// <summary>
        /// Converts the passed string to a byte array.
        /// </summary>
        /// <param name="str">The string to convert to a byte array</param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string str)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            byte[] unicodeBytes = unicode.GetBytes(str);
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
            return asciiBytes;
        }

        /// <summary>
        /// Returns an Object with the specified Type and whose value is equivalent to the specified object.
        /// </summary>
        /// <param name="value">An Object that implements the IConvertible interface.</param>
        /// <param name="conversionType">The Type to which value is to be converted.</param>
        /// <returns>
        /// An object whose Type is conversionType (or conversionType's underlying type if conversionType
        /// is Nullable&lt;&gt;) and whose value is equivalent to value. -or- a null reference, if value is a null
        /// reference and conversionType is not a value type.
        /// </returns>
        /// <remarks>
        /// This method exists as a workaround to System.Convert.ChangeType(Object, Type) which does not handle
        /// nullables as of version 2.0 (2.0.50727.42) of the .NET Framework. The idea is that this method will
        /// be deleted once Convert.ChangeType is updated in a future version of the .NET Framework to handle
        /// nullable types, so we want this to behave as closely to Convert.ChangeType as possible.
        /// This method was written by Peter Johnson at:
        /// http://aspalliance.com/author.aspx?uId=1026.
        /// </remarks>
        public static object ChangeType(object value, Type conversionType)
        {
            // Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're
            // checking properties on conversionType below.
            if(conversionType == null)
                throw new ArgumentNullException("conversionType");

            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType

            if(conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // It's a nullable type, so instead of calling Convert.ChangeType directly which would throw a
                // InvalidCastException (per http://weblogs.asp.net/pjohnson/archive/2006/02/07/437631.aspx),
                // determine what the underlying type is
                // If it's null, it won't convert to the underlying type, but that's fine since nulls don't really
                // have a type--so just return null
                // Note: We only do this check if we're converting to a nullable type, since doing it outside
                // would diverge from Convert.ChangeType's behavior, which throws an InvalidCastException if
                // value is null and conversionType is a value type.
                if(value == null)
                    return null;

                // It's a nullable type, and not null, so that means it can be converted to its underlying type,
                // so overwrite the passed-in conversion type with this underlying type
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            else if (conversionType == typeof(Guid))
            {
                return new Guid(value.ToString());
            }
            else if (conversionType == typeof(Int64) && value.GetType() == typeof(int))
            {
                throw new InvalidOperationException("Can't convert an Int64 (long) to Int32(int). You will need to set your type to long.");
            }

            // Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
            // nullable type), pass the call on to Convert.ChangeType
            return Convert.ChangeType(value, conversionType);
        }

        #endregion


        #region String Manipulations

        /// <summary>
        /// Reformats the passed string from camelCase to ProperCase
        /// </summary>
        /// <param name="sIn">The string to reformat to proper case</param>
        /// <returns></returns>
        public static string ParseCamelToProper(string sIn)
        {
            //No transformation if string is alread all caps
            if(Validation.IsUpperCase(sIn))
                return sIn;

            char[] letters = sIn.ToCharArray();
            StringBuilder sOut = new StringBuilder();
            int index = 0;

            if(sIn.Contains("ID"))
            {
                //just upper the first letter
                sOut.Append(letters[0]);
                sOut.Append(sIn.Substring(1, sIn.Length - 1));
            }
            else
            {
                foreach(char c in letters)
                {
                    if(index == 0)
                    {
                        sOut.Append(" ");
                        sOut.Append(c.ToString().ToUpper());
                    }
                    else if(Char.IsUpper(c))
                    {
                        //it's uppercase, add a space
                        sOut.Append(" ");
                        sOut.Append(c);
                    }
                    else
                        sOut.Append(c);
                    index++;
                }
            }
            string strClean = sOut.ToString().Trim();
            return Regex.Replace(strClean, "(?<=[A-Z]) (?=[A-Z])", String.Empty);
            //return sOut.ToString().Trim();
        }

        /// <summary>
        /// Keys the word check.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="table">The table.</param>
        /// <param name="appendWith">The append with.</param>
        /// <returns></returns>
        public static string KeyWordCheck(string word, string table, string appendWith)
        {
            string newWord = String.Concat(word, appendWith);

            //if(word == "Schema")
            //    newWord =  word + appendWith;

            //Can't have a property with same name as class.
            if(word == table)
                return newWord;
            
            string evalWord = word.ToLower();

            switch(evalWord)
            {
                    // C# keywords
                case "abstract":
                case "as":
                case "base":
                case "bool":
                case "break":
                case "byte":
                case "case":
                case "catch":
                case "char":
                case "checked":
                case "class":
                case "const":
                case "continue":
                case "date":
                case "datetime":
                case "decimal":
                case "default":
                case "delegate":
                case "do":
                case "double":
                case "else":
                case "enum":
                case "event":
                case "explicit":
                case "extern":
                case "false":
                case "finally":
                case "fixed":
                case "float":
                case "for":
                case "foreach":
                case "goto":
                case "if":
                case "implicit":
                case "in":
                case "int":
                case "interface":
                case "internal":
                case "is":
                case "lock":
                case "long":
                case "namespace":
                case "new":
                case "null":
                case "object":
                case "operator":
                case "out":
                case "override":
                case "params":
                case "private":
                case "protected":
                case "public":
                case "readonly":
                case "ref":
                case "return":
                case "sbyte":
                case "sealed":
                case "short":
                case "sizeof":
                case "stackalloc":
                case "static":
                case "string":
                case "struct":
                case "switch":
                case "this":
                case "throw":
                case "true":
                case "try":
                case "typecode":
                case "typeof":
                case "uint":
                case "ulong":
                case "unchecked":
                case "unsafe":
                case "ushort":
                case "using":
                case "virtual":
                case "volatile":
                case "void":
                case "while":
                    // C# contextual keywords
                case "get":
                case "partial":
                case "set":
                case "value":
                case "where":
                case "yield":
                    // VB.NET keywords (commented out keywords that are the same as in C#)
                case "alias":
                case "addHandler":
                case "ansi":
                    //case "as": 
                case "assembly":
                case "auto":
                case "binary":
                case "byref":
                case "byval":
                case "custom":
                case "directcast":
                case "each":
                case "elseif":
                case "end":
                case "error":
                case "friend":
                case "global":
                case "handles":
                case "implements":
                case "lib":
                case "loop":
                case "me":
                case "module":
                case "mustinherit":
                case "mustoverride":
                case "mybase":
                case "myclass":
                case "narrowing":
                case "next":
                case "nothing":
                case "notinheritable":
                case "notoverridable":
                case "of":
                case "off":
                case "on":
                case "option":
                case "optional":
                case "overloads":
                case "overridable":
                case "overrides":
                case "paramarray":
                case "preserve":
                case "property":
                case "raiseevent":
                case "resume":
                case "shadows":
                case "shared":
                case "step":
                case "structure":
                case "text":
                case "then":
                case "to":
                case "trycast":
                case "unicode":
                case "until":
                case "when":
                case "widening":
                case "withevents":
                case "writeonly":
                    // VB.NET unreserved keywords
                case "compare":
                case "isfalse":
                case "istrue":
                case "mid":
                case "strict":
                case "schema":
                    return newWord;
                default:
                    return word;
            }
        }

        /// <summary>
        /// Fasts the replace.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns></returns>
        public static string FastReplace(string original, string pattern, string replacement)
        {
            return FastReplace(original, pattern, replacement, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Fasts the replace.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns></returns>
        public static string FastReplace(string original, string pattern, string replacement, StringComparison comparisonType)
        {
            if(original == null)
                return null;

            if(String.IsNullOrEmpty(pattern))
                return original;

            int lenPattern = pattern.Length;
            int idxPattern = -1;
            int idxLast = 0;

            StringBuilder result = new StringBuilder();

            while(true)
            {
                idxPattern = original.IndexOf(pattern, idxPattern + 1, comparisonType);

                if(idxPattern < 0)
                {
                    result.Append(original, idxLast, original.Length - idxLast);
                    break;
                }

                result.Append(original, idxLast, idxPattern - idxLast);
                result.Append(replacement);

                idxLast = idxPattern + lenPattern;
            }

            return result.ToString();
        }

        /// <summary>
        /// Strips the text.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="stripString">The strip string.</param>
        /// <returns></returns>
        public static string StripText(string inputString, string stripString)
        {
            if(!String.IsNullOrEmpty(stripString))
            {
                string[] replace = stripString.Split(new char[] {','});
                for(int i = 0; i < replace.Length; i++)
                {
                    if(!String.IsNullOrEmpty(inputString))
                        inputString = Regex.Replace(inputString, replace[i], String.Empty);
                }
            }
            return inputString;
        }

        /// <summary>
        /// Replaces most non-alpha-numeric chars
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <returns></returns>
        public static string StripNonAlphaNumeric(string sIn)
        {
			return StripNonAlphaNumeric(sIn, " ".ToCharArray()[0]);
        }

		/// <summary>
		/// Replaces most non-alpha-numeric chars
		/// </summary>
		/// <param name="sIn">The s in.</param>
		/// <param name="cReplace">The placeholder character to use for replacement</param>
		/// <returns></returns>
		public static string StripNonAlphaNumeric(string sIn, char cReplace)
		{
			StringBuilder sb = new StringBuilder(sIn);
			//these are illegal characters - remove zem
			const string stripList = ".'?\\/><$!@%^*&+,;:\"(){}[]|-#";

			for (int i = 0; i < stripList.Length; i++)
				sb.Replace(stripList[i], cReplace);

			sb.Replace(cReplace.ToString(), String.Empty);
			return sb.ToString();
		}


        /// <summary>
        /// Replaces any matches found in word from list.
        /// </summary>
        /// <param name="word">The string to check against.</param>
        /// <param name="find">A comma separated list of values to replace.</param>
        /// <param name="replaceWith">The value to replace with.</param>
        /// <param name="removeUnderscores">Whether or not underscores will be kept.</param>
        /// <returns></returns>
        public static string Replace(string word, string find, string replaceWith, bool removeUnderscores)
        {
            string[] findList = Split(find);
            string newWord = word;
            foreach(string f in findList)
            {
                if(f.Length > 0)
                    newWord = newWord.Replace(f, replaceWith);
            }

            if(removeUnderscores)
                return newWord.Replace(" ", String.Empty).Replace("_", String.Empty).Trim();
            return newWord.Replace(" ", String.Empty).Trim();
        }

        /// <summary>
        /// Finds a match in word using comma separted list.
        /// </summary>
        /// <param name="word">The string to check against.</param>
        /// <param name="list">A comma separted list of values to find.</param>
        /// <returns>
        /// true if a match is found or list is empty, otherwise false.
        /// </returns>
        public static bool StartsWith(string word, string list)
        {
            if(string.IsNullOrEmpty(list))
                return true;

            string[] find = Split(list);

            foreach(string f in find)
            {
                if(word.StartsWith(f, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// A custom split method
        /// </summary>
        /// <param name="list">A list of values separated by either ", " or ","</param>
        /// <returns></returns>
        public static string[] Split(string list)
        {
            string[] find;
            try
            {
                find = list.Split(new string[] {", ", ","}, StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                find = new string[] {String.Empty};
            }
            return find;
        }

        /// <summary>
        /// Shortens the text.
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string ShortenText(object sIn, int length)
        {
            string sOut = sIn.ToString();
            if(sOut.Length > length)
                sOut = String.Concat(sOut.Substring(0, length), " ...");

            return sOut;
        }

        /// <summary>
        /// Checks the length of the string.
        /// </summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <returns></returns>
        public static string CheckStringLength(string stringToCheck, int maxLength)
        {
            string checkedString;

            if(stringToCheck.Length <= maxLength)
                return stringToCheck;

            // If the string to check is longer than maxLength 
            // and has no whitespace we need to trim it down.
            if((stringToCheck.Length > maxLength) && (stringToCheck.IndexOf(" ") == -1))
                checkedString = String.Concat(stringToCheck.Substring(0, maxLength), "...");
            else if(stringToCheck.Length > 0)
                checkedString = String.Concat(stringToCheck.Substring(0, maxLength), "...");
            else
                checkedString = stringToCheck;

            return checkedString;
        }

        #endregion

        #region Conversions

        /// <summary>
        /// Strings to enum.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="Value">The value.</param>
        /// <returns></returns>
        public static object StringToEnum(Type t, string Value)
        {
            object oOut = null;

            foreach(FieldInfo fi in t.GetFields())
            {
                if(IsMatch(fi.Name, Value))
                    oOut = fi.GetValue(null);
            }

            return oOut;
        }

        public static T Parse<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string stringValueOf(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        #endregion


        #region URL Related

        /// <summary>
        /// Gets the site root.
        /// </summary>
        /// <returns></returns>
        public static string GetSiteRoot()
        {
            string port = HttpContext.Current.Request.ServerVariables[ServerVariable.SERVER_PORT];

            if(port == null || port == Ports.HTTP || port == Ports.HTTPS)
                port = String.Empty;
            else
                port = String.Concat(":", port);

            string protocol = HttpContext.Current.Request.ServerVariables[ServerVariable.SERVER_PORT_SECURE];
            if(protocol == null || protocol == "0")
                protocol = ProtocolPrefix.HTTP;
            else
                protocol = ProtocolPrefix.HTTPS;

            string appPath = HttpContext.Current.Request.ApplicationPath;

            if(appPath == "/")
                appPath = String.Empty;

            string sOut = protocol + HttpContext.Current.Request.ServerVariables[ServerVariable.SERVER_NAME] + port + appPath;

            return sOut;
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="sParam">The s param.</param>
        /// <returns></returns>
        public static string GetParameter(string sParam)
        {
            if(HttpContext.Current.Request.QueryString[sParam] != null)
                return HttpContext.Current.Request[sParam];
            return String.Empty;
        }

        /// <summary>
        /// Gets the int parameter.
        /// </summary>
        /// <param name="sParam">The s param.</param>
        /// <returns></returns>
        public static int GetIntParameter(string sParam)
        {
            int iOut = 0;
            if(HttpContext.Current.Request.QueryString[sParam] != null)
            {
                string sOut = HttpContext.Current.Request[sParam];
                if(!String.IsNullOrEmpty(sOut))
                    int.TryParse(sOut, out iOut);
            }
            return iOut;
        }

        /// <summary>
        /// Gets the GUID parameter.
        /// </summary>
        /// <param name="sParam">The s param.</param>
        /// <returns></returns>
        public static Guid GetGuidParameter(string sParam)
        {
            Guid gOut = Guid.Empty;
            if(HttpContext.Current.Request.QueryString[sParam] != null)
            {
                string sOut = HttpContext.Current.Request[sParam];
                if(Validation.IsGuid(sOut))
                    gOut = new Guid(sOut);
            }
            return gOut;
        }

        #endregion


        #region Random Generators

        /// <summary>
        /// Gets the random string.
        /// </summary>
        /// <returns></returns>
        public static string GetRandomString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, false));
            builder.Append(RandomInt(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        /// <summary>
        /// Randoms the string.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="lowerCase">if set to <c>true</c> [lower case].</param>
        /// <returns></returns>
        private static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for(int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(26 * random.NextDouble() + 65));
                builder.Append(ch);
            }
            if(lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        /// <summary>
        /// Randoms the int.
        /// </summary>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns></returns>
        private static int RandomInt(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        #endregion

        #region Lists




        /// <summary>
        /// Loads the drop down.
        /// </summary>
        /// <param name="ddl">The DDL.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="textField">The text field.</param>
        /// <param name="valueField">The value field.</param>
        /// <param name="initialSelection">The initial selection.</param>
        public static void LoadDropDown(ListControl ddl, ICollection collection, string textField, string valueField, string initialSelection)
        {
            ddl.DataSource = collection;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();

            ddl.SelectedValue = initialSelection;
        }

        /// <summary>
        /// Loads the drop down.
        /// </summary>
        /// <param name="ddl">The DDL.</param>
        /// <param name="rdr">The RDR.</param>
        /// <param name="closeReader">if set to <c>true</c> [close reader].</param>
        public static void LoadDropDown(ListControl ddl, IDataReader rdr, bool closeReader)
        {
            ddl.Items.Clear();

            if(closeReader)
            {
                using(rdr)
                {
                    while(rdr.Read())
                    {
                        string sText = rdr[1].ToString();
                        string sVal = rdr[0].ToString();
                        ddl.Items.Add(new ListItem(sText, sVal));
                    }
                    rdr.Close();
                }
            }
            else
            {
                while(rdr.Read())
                {
                    string sText = rdr[1].ToString();
                    string sVal = rdr[0].ToString();
                    ddl.Items.Add(new ListItem(sText, sVal));
                }
            }
        }

        /// <summary>
        /// Loads the list items.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="tblBind">The TBL bind.</param>
        /// <param name="tblVals">The TBL vals.</param>
        /// <param name="textField">The text field.</param>
        /// <param name="valField">The val field.</param>
        public static void LoadListItems(ListItemCollection list, DataTable tblBind, DataTable tblVals, string textField, string valField)
        {
            for(int i = 0; i < tblBind.Rows.Count; i++)
            {
                ListItem l = new ListItem(tblBind.Rows[i][textField].ToString(), tblBind.Rows[i][valField].ToString());

                for(int x = 0; x < tblVals.Rows.Count; x++)
                {
                    DataRow dr = tblVals.Rows[x];
                    if(IsMatch(dr[valField].ToString(), l.Value))
                        l.Selected = true;
                }
                list.Add(l);
            }
        }

        /// <summary>
        /// Loads the list items.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="rdr">The RDR.</param>
        /// <param name="textField">The text field.</param>
        /// <param name="valField">The val field.</param>
        /// <param name="selectedValue">The selected value.</param>
        /// <param name="closeReader">if set to <c>true</c> [close reader].</param>
        public static void LoadListItems(ListItemCollection list, IDataReader rdr, string textField, string valField, string selectedValue, bool closeReader)
        {
            list.Clear();

            if(closeReader)
            {
                using(rdr)
                {
                    while (rdr.Read())
                    {
                        string sText = rdr[textField].ToString();
                        string sVal = rdr[valField].ToString();

                        ListItem l = new ListItem(sText, sVal);
                        if (!String.IsNullOrEmpty(selectedValue))
                            l.Selected = IsMatch(selectedValue, sVal);
                        list.Add(l);
                    }
                    rdr.Close();
                }
            }
            else
            {
                while (rdr.Read())
                {
                    string sText = rdr[textField].ToString();
                    string sVal = rdr[valField].ToString();

                    ListItem l = new ListItem(sText, sVal);
                    if (!String.IsNullOrEmpty(selectedValue))
                        l.Selected = IsMatch(selectedValue, sVal);
                    list.Add(l);
                }
            }
        }

        /// <summary>
        /// Sets the list selection.
        /// </summary>
        /// <param name="lc">The lc.</param>
        /// <param name="Selection">The selection.</param>
        public static void SetListSelection(ListItemCollection lc, string Selection)
        {
            for(int i = 0; i < lc.Count; i++)
            {
                if(lc[i].Value == Selection)
                {
                    lc[i].Selected = true;
                    break;
                }
            }
        }

        #endregion

    }
}
