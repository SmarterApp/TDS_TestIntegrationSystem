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
    public class FileTraceListener : FileTraceListenerBase
    {
        public override string GetTraceEventString(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            return String.Format("{0}|{1}: {2} - {3} - {4}", Trace.CorrelationManager.ActivityId, eventType, source, message, eventCache.DateTime.ToLocalTime());
        }

        public override string GetTraceDataString(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            return String.Format("{0}|{1}: {2} - {3} - {4}", Trace.CorrelationManager.ActivityId, eventType, source, StringFormatter.FormatData(data), eventCache.DateTime.ToLocalTime());
        }
    }
}
