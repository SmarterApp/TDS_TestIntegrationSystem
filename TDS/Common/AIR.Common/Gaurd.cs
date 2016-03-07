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

namespace AIR.Common
{
    /// <summary>
    /// Helper class with guard checks.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Checks an argument to ensure it isn't null.
        /// </summary>
        /// <param name="value">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNull(object value, string argumentName)
        {
            if (value == null) throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty.
        /// </summary>
        /// <param name="value">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNullOrEmptyString(string value, string argumentName)
        {
            ArgumentNotNull(value, argumentName);
            if (value.Length == 0) throw new ArgumentException("Value cannot be null or an empty string.");
        }
    }
}
