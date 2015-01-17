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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using OSS.TIS;
using TDSQASystemAPI;

using TDSQASystemAPI.Utilities;

namespace TISService
{
    public partial class TISService : ServiceBase
    {
        /// <summary>
        /// 
        /// </summary>
        private string sleepTime = ConfigurationManager.AppSettings["IdleSleepTime"];
        /// <summary>
        /// The name of the event log running on the server to log issues with the QA System.
        /// </summary>
        private string eventLogName = ConfigurationManager.AppSettings["EventLogName"];

        private string eventLogSource = ConfigurationManager.AppSettings["EventLogSource"];

        /// <summary>
        /// The main thread started by the service.
        /// </summary>
        private QASystemMainThread mainQASystemThread;
        private Thread thread;

        public TISService()
        {
            InitializeComponent();

            this.ServiceName = ConfigurationManager.AppSettings.Get("ServiceName");

            if (!EventLog.SourceExists(eventLogName))
            {
                EventLog.CreateEventSource(eventLogSource, eventLogName);
            }
            eventLog.Source = eventLogSource;
            eventLog.Log = eventLogName;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                // Thread.Sleep(30000);
                
                mainQASystemThread = new TISMainThread();

                thread = new Thread(mainQASystemThread.DoWork);
                thread.Start();

                Logger.Log(true, ServiceName + " Service started.", EventLogEntryType.Information, true, true);
            }
            catch (Exception e)
            {
                Logger.Log(true, ServiceName + " failed: " + e.Message + e.StackTrace, EventLogEntryType.Error, true, true);
                throw;
            }
        }

        protected override void OnStop()
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Join(Int32.Parse(sleepTime));
            }
            else
            {
                this.Stop();
            }
            Logger.Log(true, ServiceName + " Stopped", EventLogEntryType.Information, true, true);
        }
    }
}
