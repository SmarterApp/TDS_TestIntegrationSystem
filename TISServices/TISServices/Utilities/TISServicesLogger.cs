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
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using System.Text;

namespace TISServices.Utilities
{
    /// <summary>
    /// Thread safe logger
    /// </summary>
    internal static class TISServicesLogger
    {
        private static string _filePath
        {
            get
            {
                return ConfigurationManager.AppSettings["LogFilePath"];
            }
        }

        private static StreamWriter _sw;
        private static object _lock;

        static TISServicesLogger()
        {
            try
            {
                _sw = new StreamWriter(_filePath, true);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Could not instantiate TISServicesLogger. Filepath value = '{0}'. Error Message: {1}. Error Stacktrace: {2}", _filePath ?? "null", e.Message, e.StackTrace));
            }
            _lock = new object();
        }

        /// <summary>
        /// Log this message with the current time stamp
        /// </summary>
        /// <param name="message"></param>
        internal static void Log(string message)
        {
            lock (_lock)
            {
                _sw.WriteLine(string.Format("{0}==> {1}", DateTime.Now.ToString(), message));
                _sw.Flush();
            }
        }

        /// <summary>
        /// Log the message and StackTrace of the error
        /// </summary>
        /// <param name="e"></param>
        internal static void Log(Exception e)
        {
            LogException(e, new StringBuilder());
        }

        /// <summary>
        /// Handles nested exceptions
        /// </summary>
        /// <param name="e"></param>
        private static void LogException(Exception e, StringBuilder sb)
        {
            if (e != null)
            {
                sb.AppendLine(String.Format("Message: {0}, Stack Trace: {1}", e.Message, e.StackTrace));
                LogException(e.InnerException, sb);
            }
            else
            {
                Log(sb.ToString());
            }
        }
    }
}