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
using System.Globalization;
using System.Linq;
using System.Text;

namespace TDS.Shared.Data
{
    /// <summary>
    /// Generic utilities to help you out with TDS SQL data.
    /// </summary>
    public class DataUtils
    {
        public static string MakeItemID(long itsBank, long itsItem)
        {
            return String.Format("{0}-{1}", itsBank, itsItem);
        }

        /// <summary>
        /// Get a nicely formatted date.
        /// </summary>
        /// <example>November 1st, 2001</example>
        /// <remarks>
        /// For more formatting go to: http://msdn.microsoft.com/en-us/library/az4se3k1.aspx
        /// </remarks>
        public static string GetDateFormatted(DateTime dateTime)
        {
            DateTimeFormatInfo mfi = new DateTimeFormatInfo();
            string birthdayMonth = mfi.GetMonthName(dateTime.Month);
            string birthdayDaySuffix = GetDayNumberSuffix(dateTime);
            return String.Format("{0} {1}{2}, {3}", birthdayMonth, dateTime.Day, birthdayDaySuffix, dateTime.Year);
        }

        /// <summary>
        /// Returns just the two character suffix text for the day number component in the given date value
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns>String of day number suffix (st, nd, rd or th)</returns>
        private static string GetDayNumberSuffix(DateTime date)
        {
            int day = date.Day;

            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";

                case 2:
                case 22:
                    return "nd";

                case 3:
                case 23:
                    return "rd";

                default:
                    return "th";
            }
        }
    }
}
