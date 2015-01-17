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
using System.Diagnostics;
using System.Text;

namespace TDS.Shared.Logging
{
    public interface ILogger
    {
        // try and remove start/stop

        void Start(string message);
        void Start(TraceLog log);

        void Stop(string message);
        void Stop(TraceLog log);

        // verbose is for serious debugging

        void Verbose(string message);
        void Verbose(string message, params string[] values);

        // everything below is for production

        void Info(string message);
        void Info(TraceLog log);
        
        void Warn(string message);
        void Warn(Exception exception);
        void Warn(TraceLog log);
        
        void Error(string message);
        void Error(Exception ex);
        void Error(TraceLog log);
        
        void Fatal(string message);
        void Fatal(Exception exception);
        void Fatal(TraceLog log);
    }
}
