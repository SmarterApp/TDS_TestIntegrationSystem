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
using System.Text;
using System.Diagnostics;

namespace AIR.Common.Diagnostics
{
    /// <summary>
    /// Log to Win32
    /// </summary>
    public class InternalLogger
    {
        /// <summary>
        /// Logs any internal failures to an attached debugger or output debug string if no debugger is attached
        /// </summary>
        /// <param name="exc"></param>
        public static void LogException(Exception exc)
        {
            if (Debugger.IsLogging())
            {
                Debugger.Log(5, "Error", exc.ToString());
            }
            else
            {
                SafeNativeMethods.OutputDebugString(exc.ToString());
            }
        }

        public static void LogInformation(string message)
        {
            if (Debugger.IsLogging())
            {
                Debugger.Log(3, "Information", message);
            }
            else
            {
                SafeNativeMethods.OutputDebugString(message);
            }
        }
    }
}
