/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;

namespace TDS.Shared.Logging
{
    public class TraceLog
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public string Details { get; set; }

        public TraceLog(string message)
        {
            Message = message;
        }

        public TraceLog(string message, string details)
        {
            Message = message;
            Details = details;
        }

        public TraceLog(Exception exception)
        {
            Exception = exception;
        }

        public TraceLog(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        public override string ToString()
        {
            if (Message != null) return Message;
            if (Exception != null) return Exception.Message;
            return "NULL";
        }
    }
}