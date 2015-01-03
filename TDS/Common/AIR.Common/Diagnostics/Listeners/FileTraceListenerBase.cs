/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using Timer=System.Timers.Timer;

namespace AIR.Common.Diagnostics
{
    public abstract class FileTraceListenerBase : CustomTraceListener
    {
        private const string FILE_PATH_ATTR = "filePath";
        private const string FILE_PREFIX_ATTR = "filePrefix";
        private const string ENABLE_USER_LOGGING_ATTR = "enableUserLogging";
        private const string ENABLE_MACHINE_LOGGING_ATTR = "enableMachineLogging";
        private const string ENABLE_DATE_LOGGING_ATTR = "enableDateLogging";
        private const string CLEAN_INTERVAL_ATTR = "cleanInterval";

        private readonly TimeSpan DEFAULT_CLEAN_INTERVAL = TimeSpan.FromHours(1);
        private StreamWriterCache _writerCache;

        private TimeSpan _cleanInterval;
        private readonly object _initlock = new object();
        private bool initialized = false;
        
        protected string filePath;
        protected string filePrefix;
        protected bool enableUserLogging;
        protected bool enableMachineLogging;
        protected bool enableDateLogging; 

        protected override string[] GetSupportedAttributes()
        {
            return new string[] { FILE_PATH_ATTR, FILE_PREFIX_ATTR, CLEAN_INTERVAL_ATTR, ENABLE_USER_LOGGING_ATTR, ENABLE_MACHINE_LOGGING_ATTR, ENABLE_DATE_LOGGING_ATTR };
        }

        public FileTraceListenerBase() : base("FileTraceListener")
        {
        }

        private void Init()
        {
            if (!initialized)
            {
                lock (_initlock)
                {
                    if (!initialized)
                    {
                        initialized = true;
                        _writerCache = new StreamWriterCache();

                        // get interval for cleaning out writers from config attribute
                        if (string.IsNullOrEmpty(Attributes[CLEAN_INTERVAL_ATTR]))
                        {
                            _cleanInterval = DEFAULT_CLEAN_INTERVAL;
                        }
                        else if (!TimeSpan.TryParse(Attributes[CLEAN_INTERVAL_ATTR], out _cleanInterval))
                        {
                            throw new ConfigurationErrorsException(string.Format("clean interval invalid '{0}'", Attributes[CLEAN_INTERVAL_ATTR]));
                        }

                        // get file path from config attribute
                        if (string.IsNullOrEmpty(Attributes[FILE_PATH_ATTR]))
                        {
                            throw new ArgumentException("Must provide a \"filePath\" attribute to FileListener.");
                        }
                        else
                        {
                            filePath = Attributes[FILE_PATH_ATTR];
                        }

                        filePrefix = Attributes[FILE_PREFIX_ATTR];
                        bool.TryParse(Attributes[ENABLE_USER_LOGGING_ATTR] ?? "false", out enableUserLogging);
                        bool.TryParse(Attributes[ENABLE_MACHINE_LOGGING_ATTR] ?? "false", out enableMachineLogging);
                        bool.TryParse(Attributes[ENABLE_DATE_LOGGING_ATTR] ?? "false", out enableDateLogging);

                        // this is a timer user to check for cleaning out old writers
                        Timer timer = new Timer(_cleanInterval.TotalMilliseconds);
                        timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                        timer.Start();
                    }
                }
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            InternalLogger.LogInformation("FileListener: Cleaning old streams");
            _writerCache.ClearOldStreams(e.SignalTime - _cleanInterval);
        }

        public virtual string GetFileName(TraceEventCache eventCache)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(filePrefix))
            {
                sb.Append(filePrefix);
            }

            if (enableMachineLogging)
            {
                if (sb.Length > 0) sb.Append('_');
                sb.Append(Environment.MachineName);
            }
            
            if (enableUserLogging)
            {
                if (sb.Length > 0) sb.Append('_');

                if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    sb.Append(Thread.CurrentPrincipal.Identity.Name);
                }
                else
                {
                    sb.Append(Environment.UserName);
                }
            }

            if (enableDateLogging)
            {
                if (sb.Length > 0) sb.Append('_');
                sb.AppendFormat("{0}-{1}-{2}", eventCache.DateTime.Year, eventCache.DateTime.Month, eventCache.DateTime.Day);
            }

            if (sb.Length == 0)
            {
                sb.Append("default");
            }

            sb.Append(".log");

            return sb.ToString();
        }
        
        public abstract string GetTraceEventString(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message);
        public abstract string GetTraceDataString(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data);

        protected sealed override void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            InternalWrite(filePath + GetFileName(eventCache), GetTraceEventString(eventCache, source, eventType, id, message));
        }

        protected sealed override void TraceDataCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            InternalWrite(filePath + GetFileName(eventCache), GetTraceDataString(eventCache, source, eventType, id, data));
        }

        private void InternalWrite(string path, string output)
        {
            Init();

            // TODO: what if null - need to do some sensible checking and error throwing here!
            StreamWriter sw = _writerCache.GetStreamWriter(path);
            sw.WriteLine(output);
            
            if (Trace.AutoFlush)
            {
                sw.Flush();
            }
        }
    }
}