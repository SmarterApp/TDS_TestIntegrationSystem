/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Text;
using System.Web;
using System.Web.Security;

namespace AIR.Common.Web
{
    /// <summary>
    /// Helper for encrypting or decrypting data using the machine key.
    /// </summary>
    /// <remarks>
    /// Review this pattern:
    /// https://github.com/SignalR/SignalR/blob/bc9412bcab0f5ef097c7dc919e3ea1b37fc8718c/src/Microsoft.AspNet.SignalR.Core/Infrastructure/DefaultProtectedData.cs
    /// </remarks>
    public class MachineKeyData
    {
        /// <summary>
        /// Encrypt bytes into hex string.
        /// </summary>
        /// <returns>
        /// The bytes encrypted as a hex string.
        /// </returns>
        private static string Encode(byte[] bytes)
        {
            return MachineKey.Encode(bytes, MachineKeyProtection.Encryption);
        }

        /// <summary>
        /// Encrypt plain text string into base64.
        /// </summary>
        public static string Protect(string plainText, bool urlSafe = true)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            string encryptedText = Encode(bytes);
            return HexToBase64(encryptedText, urlSafe);
        }
        
        /// <summary>
        /// Descrypt hex string into bytes.
        /// </summary>
        /// <returns>
        /// The decrypted text in a byte array.
        /// </returns>
        private static byte[] Decode(string hex)
        {
            return MachineKey.Decode(hex, MachineKeyProtection.Encryption);
        }

        /// <summary>
        /// Decrypt base64 into plain text string. 
        /// </summary>
        public static string Unprotect(string base64, bool urlSafe = true)
        {
            string hex = Base64ToHex(base64, urlSafe);
            byte[] decryptedBytes = Decode(hex);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        // String transformation helpers

        private static char HexDigit(int value)
        {
            return (char)(value > 9 ? value + '7' : value + '0');
        }

        private static int HexValue(char digit)
        {
            return digit > '9' ? digit - '7' : digit - '0';
        }

        private static string Base64ToHex(string base64, bool urlSafe)
        {
            StringBuilder builder = new StringBuilder(base64.Length * 4);
            byte[] bytes;

            if (urlSafe)
            {
                bytes = HttpServerUtility.UrlTokenDecode(base64);
            }
            else
            {
                bytes = Convert.FromBase64String(base64);
            }

            if (bytes == null)
            {
                throw new ArgumentException("Cannot convert base64 to byte array.");
            }

            foreach (byte b in bytes)
            {
                builder.Append(HexDigit(b >> 4));
                builder.Append(HexDigit(b & 0x0F));
            }
            string result = builder.ToString();
            return result;
        }

        private static byte[] HexToBytes(string hex)
        {
            int size = hex.Length / 2;
            byte[] bytes = new byte[size];
            for (int idx = 0; idx < size; idx++)
            {
                bytes[idx] = (byte)((HexValue(hex[idx * 2]) << 4) + HexValue(hex[idx * 2 + 1]));
            }
            return bytes;
        }

        private static string HexToBase64(string hex, bool urlSafe)
        {
            byte[] bytes = HexToBytes(hex);

            if (urlSafe)
            {
                return HttpServerUtility.UrlTokenEncode(bytes);
            }
            else
            {
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
