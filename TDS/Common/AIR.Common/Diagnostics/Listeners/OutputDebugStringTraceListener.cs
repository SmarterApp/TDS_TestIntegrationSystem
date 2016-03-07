/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Diagnostics;

namespace AIR.Common.Diagnostics
{
    /// <summary>
    /// A TraceListener that supports formatted output and writes using OutputDebugString
    /// </summary>
    public class OutputDebugStringTraceListener : CustomTraceListener
    {
        /// <summary>
        /// Creates a new <see cref="OutputDebugStringTraceListener"/> />
        /// </summary>
        public OutputDebugStringTraceListener() : base("OutputDebugStringTraceListener")
        {
        }

        protected override void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            SafeNativeMethods.OutputDebugString(String.Format("{0} {1}({2}) {3} ", eventType, source, id, message));
        }

        protected override void TraceDataCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            SafeNativeMethods.OutputDebugString(String.Format("{0} {1}({2}) {3} ", eventType, source, id, StringFormatter.FormatData(data)));
        }
    }
}