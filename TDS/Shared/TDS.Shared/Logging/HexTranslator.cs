/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Globalization;

namespace TDS.Shared.Logging
{
    internal static class HexTranslator
    {
        private static char[] hexCharSet = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        /// <summary>
        /// "Hexifies" a byte array
        /// </summary>
        /// <returns>A lower-case hex encoded string representation of the byte array</returns>
        public static string ToHex(byte[] buffer)
        {
            return ToHex(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// "Hexifies" a byte array
        /// </summary>
        /// <returns>A lower-case hex encoded string representation of the byte array</returns>
        public static string ToHex(byte[] buffer, int offset, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Offset cannot be less than 0");

            if (offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset", "Offset cannot be greater than buffer length");

            if (offset + length > buffer.Length)
                throw new ArgumentException("The offset and length values provided exceed buffer length");

            char[] charbuf = new char[checked(length * 2)];

            int c = -1;

            for (int i = offset; i < length + offset; i++)
            {
                charbuf[c += 1] = hexCharSet[buffer[i] >> 4];
                charbuf[c += 1] = hexCharSet[buffer[i] & 0x0F];
            }

            return new string(charbuf);
        }
    }
}
