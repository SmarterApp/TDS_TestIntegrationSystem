/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace TDS.Shared
{
    /// <summary>
    /// Summary for the Numbers class
    /// </summary>
    public static class Numbers
    {
        /// <summary>
        /// Determines whether [is natural number] [the specified s item].
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if [is natural number] [the specified s item]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNaturalNumber(string sItem)
        {
            Regex notNaturalPattern = new Regex("[^0-9]");
            Regex naturalPattern = new Regex("0*[1-9][0-9]*");

            return !notNaturalPattern.IsMatch(sItem) && naturalPattern.IsMatch(sItem);
        }

        /// <summary>
        /// Determines whether [is whole number] [the specified s item].
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if [is whole number] [the specified s item]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWholeNumber(string sItem)
        {
            Regex notWholePattern = new Regex("[^0-9]");
            return !notWholePattern.IsMatch(sItem);
        }

        /// <summary>
        /// Determines whether the specified s item is integer.
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if the specified s item is integer; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInteger(string sItem)
        {
            Regex notIntPattern = new Regex("[^0-9-]");
            Regex intPattern = new Regex("^-[0-9]+$|^[0-9]+$");

            return !notIntPattern.IsMatch(sItem) && intPattern.IsMatch(sItem);
        }

        /// <summary>
        /// Determines whether the specified s item is number.
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if the specified s item is number; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumber(string sItem)
        {
            double result;
            return (double.TryParse(sItem, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.CurrentInfo, out result));
        }

        /// <summary>
        /// Determines whether the specified value is an even number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is even; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEven(int value)
        {
            return ((value & 1) == 0);
        }

        /// <summary>
        /// Determines whether the specified value is an odd number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is odd; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOdd(int value)
        {
            return ((value & 1) == 1);
        }

        /// <summary>
        /// Generates a random number
        /// </summary>
        /// <param name="noZeros"></param>
        /// <returns></returns>
        public static int Random(bool noZeros)
        {
            byte[] random = new Byte[1];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            if(noZeros)
                rng.GetNonZeroBytes(random);
            else
                rng.GetBytes(random);

            return random[0];
        }

        /// <summary>
        /// Generates a random number with an upper bound
        /// </summary>
        /// <param name="high">The high.</param>
        /// <returns></returns>
        public static int Random(int high)
        {
            byte[] random = new Byte[4];
            new RNGCryptoServiceProvider().GetBytes(random);
            int randomNumber = BitConverter.ToInt32(random, 0);

            return Math.Abs(randomNumber % high);
        }

        /// <summary>
        /// Generates a random number between the specified bounds
        /// </summary>
        /// <param name="low">The low.</param>
        /// <param name="high">The high.</param>
        /// <returns></returns>
        public static int Random(int low, int high)
        {
            return new Random().Next(low, high);
        }

        /// <summary>
        /// Generates a random double
        /// </summary>
        /// <returns></returns>
        public static double Random()
        {
            return new Random().NextDouble();
        }

        public static string GetOrdinal(int number)
        {
            const string th = "th";
            string s = number.ToString();

            number %= 100;

            if ((number >= 11) && (number <= 13))
            {
                return string.Format("{0}{1}", s, th);
            }
            switch (number % 10)
            {
                case 1:
                    return string.Format("{0}st", s);
                case 2:
                    return string.Format("{0}nd", s);
                case 3:
                    return string.Format("{0}rd", s);
                default:
                    return string.Format("{0}{1}", s, th);
            }
        }
    }
}