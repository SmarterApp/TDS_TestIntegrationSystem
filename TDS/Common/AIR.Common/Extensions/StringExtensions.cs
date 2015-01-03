/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Linq;
using System.Text;

/// <summary>
/// Extension methods for the string data type
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Check if this string matches another (ignoring case).
    /// </summary>
    public static bool IsMatch(this string stringA, string stringB)
    {
        return String.Equals(stringA, stringB, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Check if this string matches any other strings (ignoring case).
    /// </summary>
    public static bool IsMatch(this string stringA, params string[] strings)
    {
        return strings.Any(stringB => String.Equals(stringA, stringB, StringComparison.InvariantCultureIgnoreCase));
    }

    /*
    public static byte[] GetBytes(this string data)
    {
        return Encoding.Default.GetBytes(data);
    }

    public static byte[] GetBytes(this string data, Encoding encoding)
    {
        return encoding.GetBytes(data);
    }

    /// <summary>
    /// Encodes the input value to a Base64 string using the default encoding.
    /// </summary>
    /// <param name = "value">The input value.</param>
    /// <returns>The Base 64 encoded string</returns>
    public static string EncodeBase64(this string value)
    {
        return value.EncodeBase64(null);
    }

    /// <summary>
    /// Encodes the input value to a Base64 string using the supplied encoding.
    /// </summary>
    /// <param name = "value">The input value.</param>
    /// <param name = "encoding">The encoding.</param>
    /// <returns>The Base 64 encoded string</returns>
    public static string EncodeBase64(this string value, Encoding encoding)
    {
        encoding = (encoding ?? Encoding.UTF8);
        var bytes = encoding.GetBytes(value);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Decodes a Base 64 encoded value to a string using the default encoding.
    /// </summary>
    /// <param name = "encodedValue">The Base 64 encoded value.</param>
    /// <returns>The decoded string</returns>
    public static string DecodeBase64(this string encodedValue)
    {
        return encodedValue.DecodeBase64(null);
    }

    /// <summary>
    /// 	Decodes a Base 64 encoded value to a string using the supplied encoding.
    /// </summary>
    /// <param name = "encodedValue">The Base 64 encoded value.</param>
    /// <param name = "encoding">The encoding.</param>
    /// <returns>The decoded string</returns>
    public static string DecodeBase64(this string encodedValue, Encoding encoding)
    {
        encoding = (encoding ?? Encoding.UTF8);
        var bytes = Convert.FromBase64String(encodedValue);
        return encoding.GetString(bytes);
    }

    */

    /// <summary>
    /// Converts a string value to a type of enum.
    /// </summary>
    /// <remarks>
    /// C# help for bit mask: http://stackoverflow.com/questions/8447/enum-flags-attribute
    /// </remarks>
    public static T ConvertToEnum<T>(this string value) where T : struct
    {
        Type typeOfEnum = typeof (T);

        // make sure the generic base type is actually a enum
        if (typeOfEnum.BaseType != typeof(Enum))
        {
            throw new InvalidCastException();
        }

        // parse enum value
        T enumResult;
        Enum.TryParse(value, true, out enumResult);
        return enumResult;
    }

    /*
    /// <summary>
    /// Gets the string before the given string parameter.
    /// </summary>
    /// <param name = "value">The default value.</param>
    /// <param name = "x">The given string parameter.</param>
    /// <returns></returns>
    public static string GetBefore(this string value, string x)
    {
        var xPos = value.IndexOf(x);
        return xPos == -1 ? String.Empty : value.Substring(0, xPos);
    }

    /// <summary>
    /// Gets the string between the given string parameters.
    /// </summary>
    /// <param name = "value">The default value.</param>
    /// <param name = "left">The left string parameter.</param>
    /// <param name = "right">The right string parameter</param>
    /// <returns></returns>
    public static string GetBetween(this string value, string left, string right)
    {
        var xPos = value.IndexOf(left);
        var yPos = value.LastIndexOf(right);

        if (xPos == -1 || xPos == -1) return String.Empty;

        var startIndex = xPos + left.Length;
        return startIndex >= yPos ? String.Empty : value.Substring(startIndex, yPos - startIndex).Trim();
    }

    /// <summary>
    /// Gets the string after the given string parameter.
    /// </summary>
    /// <param name = "value">The default value.</param>
    /// <param name = "x">The given string parameter.</param>
    /// <returns></returns>
    public static string GetAfter(this string value, string x)
    {
        var xPos = value.LastIndexOf(x);

        if (xPos == -1) return String.Empty;

        var startIndex = xPos + x.Length;
        return startIndex >= value.Length ? String.Empty : value.Substring(startIndex).Trim();
    }
    */

}