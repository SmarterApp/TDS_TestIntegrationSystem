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

namespace TDS.Shared.Logging
{
    public class TraceLogger : ILogger
    {
        private readonly TraceSource _traceSource;

        public TraceLogger(string sourceName)
        {
            _traceSource = new TraceSource(sourceName);
        }

        public void Start(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Start, 0, message);
        }

        public void Start(TraceLog log)
        {
            _traceSource.TraceData(TraceEventType.Start, 0, log);
        }

        public void Stop(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Stop, 0, message);
        }

        public void Stop(TraceLog log)
        {
            _traceSource.TraceData(TraceEventType.Stop, 0, log);
        }

        public void Verbose(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, message);
        }

        public void Verbose(string message, params string[] values)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, string.Format(message, values));
        }

        public void Info(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, message);
        }

        public void Info(TraceLog log)
        {
            _traceSource.TraceData(TraceEventType.Information, 0, log);
        }

        public void Warn(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Warning, 0, message);
        }

        public void Warn(Exception ex)
        {
            _traceSource.TraceData(TraceEventType.Warning, 0, ex);
        }

        public void Warn(TraceLog log)
        {
            _traceSource.TraceData(TraceEventType.Warning, 0, log);
        }

        public void Error(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Error, 0, message);
        }

        public void Error(Exception ex)
        {
            _traceSource.TraceData(TraceEventType.Error, 0, ex);
        }

        public void Error(TraceLog log)
        {
            _traceSource.TraceData(TraceEventType.Error, 0, log);
        }

        public void Fatal(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Critical, 0, message);
        }

        public void Fatal(Exception ex)
        {
            _traceSource.TraceData(TraceEventType.Critical, 0, ex);
        }

        public void Fatal(TraceLog log)
        {
            _traceSource.TraceData(TraceEventType.Critical, 0, log);
        }
    }
}