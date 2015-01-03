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
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using AIR.Common;
using AIR.Common.Threading;
using TDS.ScoringDaemon.Abstractions;
using TDS.Shared.Logging;

namespace TDS.ScoringDeamon.Web
{
    public class AdminMonitor : IDisposable
    {
        private Timer _cloudTimer;     
        private bool _started = false;

        private static volatile AdminMonitor _instance;
        private static object _lock = new Object();

        private AdminMonitor()
        {
        }
   
        public bool IsStarted
        {
            get { return _started; }
        }

        public static AdminMonitor GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AdminMonitor();
                    }
                }
            }
            return _instance;
        }

        public void Dispose()
        {
            try
            {
                if (_cloudTimer != null)
                {
                    _cloudTimer.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Start(WindowsIdentity identity, Action initializeCallback)
        {
            if (_started) return;

            ScoringDaemonSettings.AppStartTime = DateTime.Now;

            if (initializeCallback == null)
                // create the thread pool and assign it to the callback handler
                ItemScoringCallbackHandler.WorkerPool = new BoundedThreadPool(
                    ScoringDaemonSettings.CallbackThreadPoolCount, "Item Scoring Callback",
                    ScoringDaemonSettings.CallbackThreadPoolHighWaterMark, ScoringDaemonSettings.CallbackThreadPoolLowWaterMark);
            else
                initializeCallback();

            // Check that the cloud DB connection string is provisioned. If not, nothing we can do here
            if (ScoringDaemonSettings.CloudConnectionString == null)
            {
                throw new InvalidDataException("Connection String for the Cloud DB not provided");
            }

            // Setup the timer to scan the cloud db for hubs to manage
            long interval = (ScoringDaemonSettings.CloudTimerIntervalSeconds * 1000);
            const long startup = 100;
            _cloudTimer = new Timer(delegate
            {
                WindowsImpersonationContext wi = null;
                try
                {
                    wi = identity.Impersonate();

                    IAdminRepository cloudRepository = ServiceLocator.Resolve<IAdminRepository>();
                    List<DataStoreInfo> hubs = cloudRepository.GetMonitoredDataStores(ScoringDaemonSettings.MachineName);
                    ScoringDaemonSettings.CloudLastPollTime = DateTime.Now;

                    // Dispose of any hubs that are no longer present
                    foreach (ReponseRepoMonitor hub in MonitorCollection.GetAll())
                    {
                        bool found = false;
                        foreach (var cloudServiceInfo in hubs)
                        {
                            if (cloudServiceInfo.ClientName == hub.ClientName &&
                                cloudServiceInfo.Environment == hub.Environment &&
                                cloudServiceInfo.DBName == hub.DBName &&
                                ScoringDaemonSettings.HubIP(cloudServiceInfo) == hub.DBIP)
                            {
                                found = true; // OK - so this hub should still be retained
                            }
                        }
                        if (!found)
                        {
                            // Now we have a hub that used to be registered in the cloud and is no longer there. Get rid of it.
                            MonitorCollection.Remove(hub.ClientName, hub.Environment);
                            hub.Dispose();
                            TDSLogger.Application.Info(String.Format("Disposing ReponseRepoMonitor {0}:{1}:{2}:{3}", hub.ClientName, hub.Environment, hub.DBIP, hub.DBName));
                        }
                    }

                    // Add any new hubs that need to be added
                    foreach (DataStoreInfo hubInfo in hubs)
                    {
                        ReponseRepoMonitor reponseRepoMonitor = MonitorCollection.Lookup(hubInfo.ClientName, hubInfo.Environment);
                        if (reponseRepoMonitor == null)
                        {
                            reponseRepoMonitor = new ReponseRepoMonitor(hubInfo.ClientName, hubInfo.Environment, ScoringDaemonSettings.HubIP(hubInfo), hubInfo.DBName);
                            MonitorCollection.Add(reponseRepoMonitor);
                            reponseRepoMonitor.Activate();
                            TDSLogger.Application.Info(String.Format("Adding ReponseRepoMonitor {0}:{1}:{2}:{3}", reponseRepoMonitor.ClientName, reponseRepoMonitor.Environment, reponseRepoMonitor.DBIP, reponseRepoMonitor.DBName));
                        }
                    }

                }
                catch (Exception ex)
                {
                    string errorMessage = String.Format("CloudTimer: Fatal exception occured on Cloud Timer thread");
                    TDSLogger.Application.Fatal(new TraceLog("Application: " + errorMessage, ex));
                }
                finally
                {
                    if (wi != null) wi.Undo();
                }

            }, null, startup, interval);

            _started = true;
        }

        // AM: maintaining old signature in case there's a test harness that requires it.
        //  added a new param to the primary method
        public void Start(WindowsIdentity identity)
        {
            Start(identity, null);
        }
    }
}
