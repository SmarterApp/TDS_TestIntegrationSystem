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
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AIR.Common.Web
{
    /// <summary>
    /// A grab-bag of utility methods useful for performing hashing.
    /// </summary>
    public static class HashingUtilities
    {
        /// <summary>
        /// The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

        /// <summary>
        /// The cryptographically strong random data generator used for creating secrets.
        /// </summary>
        /// <remarks>The random number generator is thread-safe.</remarks>
        internal static readonly RandomNumberGenerator CryptoRandomDataGenerator = new RNGCryptoServiceProvider();

        /// <summary>
        /// A pseudo-random data generator (NOT cryptographically strong random data)
        /// </summary>
        internal static readonly Random NonCryptoRandomDataGenerator = new Random();

        /// <summary>
        /// The uppercase alphabet.
        /// </summary>
        internal const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// The lowercase alphabet.
        /// </summary>
        internal const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// The set of base 10 digits.
        /// </summary>
        internal const string Digits = "0123456789";

        /// <summary>
        /// The set of digits and alphabetic letters (upper and lowercase).
        /// </summary>
        internal const string AlphaNumeric = UppercaseLetters + LowercaseLetters + Digits;

        /// <summary>
        /// All the characters that are allowed for use as a base64 encoding character.
        /// </summary>
        internal const string Base64Characters = AlphaNumeric + "+" + "/";

        /// <summary>
        /// All the characters that are allowed for use as a base64 encoding character
        /// in the "web safe" context.
        /// </summary>
        internal const string Base64WebSafeCharacters = AlphaNumeric + "-" + "_";

        /// <summary>
        /// The set of digits, and alphabetic letters (upper and lowercase) that are clearly
        /// visually distinguishable.
        /// </summary>
        internal const string AlphaNumericNoLookAlikes = "23456789abcdefghjkmnpqrstwxyzABCDEFGHJKMNPQRSTWXYZ";

        /// <summary>
        /// Adds a name-value pair to the end of a given URL
        /// as part of the querystring piece.  Prefixes a ? or &amp; before
        /// first element as necessary.
        /// </summary>
        /// <param name="builder">The UriBuilder to add arguments to.</param>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the argument.</param>
        /// <remarks>
        /// If the parameters to add match names of parameters that already are defined
        /// in the query string, the existing ones are <i>not</i> replaced.
        /// </remarks>
        public static void AppendQueryArgument(this UriBuilder builder, string name, string value)
        {
            AppendQueryArgs(builder, new[] { new KeyValuePair<string, string>(name, value) });
        }

        /// <summary>
        /// Adds a set of name-value pairs to the end of a given URL
        /// as part of the querystring piece.  Prefixes a ? or &amp; before
        /// first element as necessary.
        /// </summary>
        /// <param name="builder">The UriBuilder to add arguments to.</param>
        /// <param name="args">
        /// The arguments to add to the query.  
        /// If null, <paramref name="builder"/> is not changed.
        /// </param>
        /// <remarks>
        /// If the parameters to add match names of parameters that already are defined
        /// in the query string, the existing ones are <i>not</i> replaced.
        /// </remarks>
        public static void AppendQueryArgs(this UriBuilder builder, IEnumerable<KeyValuePair<string, string>> args)
        {
            if (args != null && args.Count() > 0)
            {
                StringBuilder sb = new StringBuilder(50 + (args.Count() * 10));
                if (!string.IsNullOrEmpty(builder.Query))
                {
                    sb.Append(builder.Query.Substring(1));
                    sb.Append('&');
                }
                sb.Append(CreateQueryString(args));

                builder.Query = sb.ToString();
            }
        }

        /// <summary>
        /// Concatenates a list of name-value pairs as key=value&amp;key=value,
        /// taking care to properly encode each key and value for URL
        /// transmission according to RFC 3986.  No ? is prefixed to the string.
        /// </summary>
        /// <param name="args">The dictionary of key/values to read from.</param>
        /// <returns>The formulated querystring style string.</returns>
        public static string CreateQueryString(IEnumerable<KeyValuePair<string, string>> args)
        {
            if (!args.Any())
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder(args.Count() * 10);

            foreach (var p in args)
            {
                sb.Append(EscapeUriDataStringRfc3986(p.Key));
                sb.Append('=');
                sb.Append(EscapeUriDataStringRfc3986(p.Value));
                sb.Append('&');
            }
            sb.Length--; // remove trailing &

            return sb.ToString();
        }

        /// <summary>
        /// Escapes a string according to the URI data string rules given in RFC 3986.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        /// <remarks>
        /// The <see cref="Uri.EscapeDataString"/> method is <i>supposed</i> to take on
        /// RFC 3986 behavior if certain elements are present in a .config file.  Even if this
        /// actually worked (which in my experiments it <i>doesn't</i>), we can't rely on every
        /// host actually having this configuration element present.
        /// </remarks>
        public static string EscapeUriDataStringRfc3986(string value)
        {
            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

            // Upgrade the escaping to RFC 3986, if necessary.
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }

            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }

        /// <summary>
        /// Gets a random string made up of a given set of allowable characters.
        /// </summary>
        /// <param name="length">The length of the desired random string.</param>
        /// <param name="allowableCharacters">The allowable characters.</param>
        /// <returns>A random string.</returns>
        public static string GetRandomString(int length, string allowableCharacters)
        {
            char[] randomString = new char[length];
            
            for (int i = 0; i < length; i++)
            {
                randomString[i] = allowableCharacters[NonCryptoRandomDataGenerator.Next(allowableCharacters.Length)];
            }

            return new string(randomString);
        }

        /// <summary>
        /// Computes the hash of a string.
        /// </summary>
        /// <param name="algorithm">The hash algorithm to use.</param>
        /// <param name="value">The value to hash.</param>
        /// <param name="encoding">The encoding to use when converting the string to a byte array.</param>
        /// <returns>A base64 encoded string.</returns>
        public static string ComputeHash(this HashAlgorithm algorithm, string value, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            byte[] bytesToHash = encoding.GetBytes(value);
            byte[] hash = algorithm.ComputeHash(bytesToHash);
            string base64Hash = Convert.ToBase64String(hash);
            return base64Hash;
        }

        /// <summary>
        /// Computes the hash of a sequence of key=value pairs.
        /// </summary>
        /// <param name="algorithm">The hash algorithm to use.</param>
        /// <param name="data">The data to hash.</param>
        /// <param name="encoding">The encoding to use when converting the string to a byte array.</param>
        /// <returns>A base64 encoded string.</returns>
        public static string ComputeHash(this HashAlgorithm algorithm, IDictionary<string, string> data, Encoding encoding = null)
        {
            // Assemble the dictionary to sign, taking care to remove the signature itself
            // in order to accurately reproduce the original signature (which of course didn't include
            // the signature).
            // Also we need to sort the dictionary's keys so that we sign in the same order as we did
            // the last time.
            var sortedData = new SortedDictionary<string, string>(data, StringComparer.OrdinalIgnoreCase);
            return ComputeHash(algorithm, (IEnumerable<KeyValuePair<string, string>>)sortedData, encoding);
        }

        /// <summary>
        /// Computes the hash of a sequence of key=value pairs.
        /// </summary>
        /// <param name="algorithm">The hash algorithm to use.</param>
        /// <param name="sortedData">The data to hash.</param>
        /// <param name="encoding">The encoding to use when converting the string to a byte array.</param>
        /// <returns>A base64 encoded string.</returns>
        internal static string ComputeHash(this HashAlgorithm algorithm, IEnumerable<KeyValuePair<string, string>> sortedData, Encoding encoding = null)
        {
            return ComputeHash(algorithm, CreateQueryString(sortedData), encoding);
        }

        /// <summary>
        /// Converts to data buffer to a base64-encoded string, using web safe characters and with the padding removed.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <returns>A web-safe base64-encoded string without padding.</returns>
        public static string ConvertToBase64WebSafeString(byte[] data)
        {
            var builder = new StringBuilder(Convert.ToBase64String(data));

            // Swap out the URL-unsafe characters, and trim the padding characters.
            builder.Replace('+', '-').Replace('/', '_');
            while (builder[builder.Length - 1] == '=')
            { // should happen at most twice.
                builder.Length -= 1;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Decodes a (web-safe) base64-string back to its binary buffer form.
        /// </summary>
        /// <param name="base64WebSafe">The base64-encoded string.  May be web-safe encoded.</param>
        /// <returns>A data buffer.</returns>
        public static byte[] FromBase64WebSafeString(string base64WebSafe)
        {
            // Restore the padding characters and original URL-unsafe characters.
            int missingPaddingCharacters;
            switch (base64WebSafe.Length % 4)
            {
                case 3:
                    missingPaddingCharacters = 1;
                    break;
                case 2:
                    missingPaddingCharacters = 2;
                    break;
                case 0:
                    missingPaddingCharacters = 0;
                    break;
                default:
                    throw new Exception("No more than two padding characters should be present for base64.");
            }
            var builder = new StringBuilder(base64WebSafe, base64WebSafe.Length + missingPaddingCharacters);
            builder.Replace('-', '+').Replace('_', '/');
            builder.Append('=', missingPaddingCharacters);

            return Convert.FromBase64String(builder.ToString());
        }

        /// <summary>
        /// Converts a <see cref="NameValueCollection"/> to an IDictionary&lt;string, string&gt;.
        /// </summary>
        /// <param name="nvc">The NameValueCollection to convert.  May be null.</param>
        /// <param name="throwOnNullKey">
        /// A value indicating whether a null key in the <see cref="NameValueCollection"/> should be silently skipped since it is not a valid key in a Dictionary.  
        /// Use <c>true</c> to throw an exception if a null key is encountered.
        /// Use <c>false</c> to silently continue converting the valid keys.
        /// </param>
        /// <returns>The generated dictionary, or null if <paramref name="nvc"/> is null.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="throwOnNullKey"/> is <c>true</c> and a null key is encountered.</exception>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection nvc)
        {
            if (nvc == null) return null;

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
        
            foreach (string key in nvc)
            {
                // NameValueCollection supports a null key, but Dictionary<K,V> does not.
                if (key != null)
                {
                    dictionary.Add(key, nvc[key]);
                }
            }

            return dictionary;
        }

    }

}
