/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Data;
using System.Net.Mail;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using TDSQASystemAPI.Data;
using TDSQASystemAPI.Utilities;
using System.Text.RegularExpressions;
using AIR.Common;

namespace TDSQASystemAPI
{
    /// <summary>
    /// The main thread is responsible for managin source xml generalFiles by assigning them 
    /// to a worker thread for processing. It maintains the queue where file names are 
    /// kept and keeps track of all the worker threads currently alive.
    /// </summary>
    public abstract class QASystemMainThread : IDisposable
    {
        /// <summary>
        /// This is the queue of FileIds waiting to be processed.
        /// </summary>
        private Queue<XmlRepositoryItem> XMLRepositoryQ = new Queue<XmlRepositoryItem>();
        /// <summary>
        /// Instance of xmlRepositoryBL class
        /// </summary>
        private BL.XmlRepository xmlRepositoryBL;
        /// <summary>
        /// This counter keeps track of worker threads and makes sure they do not 
        /// go over the maximum number of threads threshold.
        /// </summary>
        private volatile Int32 generalThreadCounter;
        /// <summary>
        /// The amount of time the thread sleeps at each iteration of the loop
        /// </summary>
        private static int loopSleepTime = Int32.Parse(ConfigurationManager.AppSettings["LoopSleepTime"], CultureInfo.InvariantCulture);
        /// <summary>
        /// The amount of time to sleep when there is no work to be done, no generalFiles are being written to the source.
        /// </summary>
        private static int idleSleepTime = Int32.Parse(ConfigurationManager.AppSettings["IdleSleepTime"], CultureInfo.InvariantCulture);
        /// <summary>
        /// The total number of threads used for all subjects, except writing.
        /// </summary>
        private static int numberOfGeneralThreads = Int32.Parse(ConfigurationManager.AppSettings["NumberOfGeneralThreads"], CultureInfo.InvariantCulture);
        /// <summary>
        /// The Instance name of QA System.
        /// </summary>
        public static string ServiceName = ConfigurationManager.AppSettings["ServiceName"];

        /// <summary>
        /// The total number of worker threads spawned so far.
        /// </summary>
        private PerformanceCounter activeThreadCount = null;
        /// <summary>
        /// The number of generalFiles processed per second.
        /// </summary>
        public PerformanceCounter filesPerSecond = null;
        /// <summary>
        /// The number of files processed successfully since the last system restart.
        /// </summary>
        public PerformanceCounter totalFilesProcessedToday = null;

        // opportunity locking
        private object oppLock = new object();
        private HashSet<long> lockedStudents = new HashSet<long>();
        private HashSet<string> lockedOpps = new HashSet<string>();

        //Warnings summary variables
        private TimeSpan timeToSendWarnings;
        private static TimeSpan margin = TimeSpan.FromMinutes(10.0); // will send warnings email if within 10 minutes of timeToSendWarnings
        private bool sentWarnings;

        protected abstract void InitializeServices();

        /// <summary>
        /// Use this constructor to instantiate from the windows application and to start the multiple threads.
        /// </summary>
        /// <param name="startThreads"></param>
        public QASystemMainThread()
        {
            //ServiceLocator.Register<Routing.ITargetFactory>(() => new Routing.QATargetFactory());
            InitializeServices();
   
            QASystem qa = new QASystem("TDSQC", "TDSQC");
            //TODO: also make QASystemConfigSettings pluggable
            xmlRepositoryBL = new BL.XmlRepository(QASystemConfigSettings.Instance.LongDbCommandTimeout);

            Utilities.Logger.Log(true, "Initialized the QASystem", EventLogEntryType.Information, false, true);

            generalThreadCounter = 0;
           
            // Create performance counter category if it doesn't exist and add counters to it...
            if (!PerformanceCounterCategory.Exists("TDS QA System"))
            {
                // Create the collection container
                CounterCreationDataCollection counters = new
                    CounterCreationDataCollection();

                CounterCreationData threadCount = new CounterCreationData();
                threadCount.CounterName = "Total Active Thread Count";
                threadCount.CounterType = PerformanceCounterType.NumberOfItems32;
                counters.Add(threadCount);

                CounterCreationData filesPerSec = new CounterCreationData();
                filesPerSec.CounterName = "Files Processed per second";
                filesPerSec.CounterType = PerformanceCounterType.RateOfCountsPerSecond32;
                counters.Add(filesPerSec);

                CounterCreationData totalFilesProcessedSinceLastRestart = new CounterCreationData();
                totalFilesProcessedSinceLastRestart.CounterName = "Total Files Processed Since Last Restart";
                totalFilesProcessedSinceLastRestart.CounterType = PerformanceCounterType.NumberOfItems32;
                counters.Add(totalFilesProcessedSinceLastRestart);

                // Create the category and all of the counters.
                PerformanceCounterCategory.Create("TDS QA System", "Measures productivity of the QA System windows service.",
                    PerformanceCounterCategoryType.SingleInstance, counters);
            }

            // Initialize performance counters...
            activeThreadCount = new PerformanceCounter("TDS QA System", "Total Active Thread Count", false);
            activeThreadCount.MachineName = ".";

            filesPerSecond = new PerformanceCounter("TDS QA System", "Files Processed per second", false);
            filesPerSecond.MachineName = ".";

            //totalFilesProcessedToday = new PerformanceCounter("TDS QA System", "Total Files Processed Since Last Restart", false);
            //totalFilesProcessedToday.MachineName = ".";
            //totalFilesProcessedToday.RawValue = 0;
            DateTime todaysDate = DateTime.Now;
            string client = QASystemConfigSettings.Instance.Client;
            string environment = QASystemConfigSettings.Instance.Environment;
            timeToSendWarnings = TimeSpan.FromHours(QASystemConfigSettings.Instance.HourToSendWarningSummary);
            sentWarnings = false;
        }

        /// <summary>
        /// Checks for generalFiles in a directory and populate the queue,
        /// then assign a file for processing to a worker thread. 
        /// If the queue becomes empty, check again and give more work to worker threads.
        /// </summary>
        public void DoWork()
        {
            //@rem Regex regularExpression = new Regex(fileRegexPattern);

            while (true)
            {
                // if the settings say to send an email alert for warnings then we are not supposd to send a summary email as well.
                // If we are not supposed to send email alerts for warnings then we are supposed to send only a summary email
                if (!QASystemConfigSettings.Instance.EmailAlertForWarnings)
                { //Check if the current time is within `margin` of `timeToSendWarnings`. If it is, then send an email
                    // with a summary of all warnings since this time yesterday.
                    TimeSpan difference;
                    TimeSpan currentTime = TimeSpan.FromHours(DateTime.Now.Hour) + TimeSpan.FromMinutes(DateTime.Now.Minute) + TimeSpan.FromSeconds(DateTime.Now.Second);
                    if (currentTime <= timeToSendWarnings) // don't want negative values 
                        difference = timeToSendWarnings.Subtract(currentTime);
                    else difference = currentTime.Subtract(timeToSendWarnings);
                    if (difference <= margin && !sentWarnings)
                    {
                        Utilities.Logger.SendWarningSummaryEmail(DateTime.Now - TimeSpan.FromDays(1.0));
                        sentWarnings = true;
                    }
                    else if (difference > margin)
                    {
                        sentWarnings = false;
                    }
                }
                if (XMLRepositoryQ.Count > 0 && generalThreadCounter < numberOfGeneralThreads)
                {
                    // Get a file to process from the queue...
                    XmlRepositoryItem repositoryItem = XMLRepositoryQ.Dequeue();

                    // try to lock the opp.  If it's already locked, then move
                    //  on to the next file leaving this one in the repository to be
                    //  picked up next time.
                    if (!LockOpp(repositoryItem))
                    {
                        Utilities.Logger.Log(true, String.Format("FileID: {0}, OppID: {1}, TesteeKey: {2} could not be locked.  Will try again later...", repositoryItem.FileID, repositoryItem.OppID, repositoryItem.TesteeKey), EventLogEntryType.Information, false, true);
                        continue;
                    }

                    try
                    {
                        QASystemWorkerThread worker = new QASystemWorkerThread(repositoryItem);
                        Thread workerThread = new Thread(new ParameterizedThreadStart(worker.ProcessXmlFile));
                        workerThread.Name = "process XML file";
                        workerThread.Start(this);

                        // Increment the active thread count...
                        generalThreadCounter++;
                        activeThreadCount.Increment();
                    }
                    catch (Exception e)
                    {
                        Utilities.Logger.Log(true, "Error starting worker thread: " + e.StackTrace, EventLogEntryType.Error, false, true);
                        continue;
                    }
                }
                else if (XMLRepositoryQ.Count == 0) // If the file queues are empty...
                {
                    try
                    {
                        XMLRepositoryQ = xmlRepositoryBL.GetXMLRepository(ServiceName);
                    }
                    catch (Exception ex)
                    {
                        Utilities.Logger.Log(true, String.Format("Fatal Exception encountered while reading XmlRepository: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace), EventLogEntryType.Error, true, true);
                        throw ex;
                    }

                    if (XMLRepositoryQ.Count == 0)
                    {
                        Thread.Sleep(idleSleepTime);
                    }
                }
                Thread.Sleep(loopSleepTime);
            }
        }

        /// <summary>
        /// Decrements the counter
        /// </summary>
        public void DecrementCounter()
        {
            try
            {
                lock (this)
                {
                    activeThreadCount.Decrement();
                    generalThreadCounter--;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Unlock both the student and the opp.  When we process a test,
        /// both the student and the opp will be locked.  If either one cannot be locked,
        /// the opp will be left in the queue and picked up again on the next pass.  This
        /// will continue until both can be locked.
        /// </summary>
        /// <param name="file"></param>
        public void UnlockOpp(XmlRepositoryItem file)
        {
            lock (oppLock)
            {
                // first unlock the opp
                if (!lockedOpps.Remove(file.OppID))
                    throw new ApplicationException(String.Format("Could not unlock opp: {0}.  It's not locked!.", file.OppID));
                // then unlock the student.
                if (!lockedStudents.Remove(file.TesteeKey))
                    throw new ApplicationException(String.Format("Could not unlock student: {0}, but was able to unlock opp: {1}!.", file.TesteeKey, file.OppID));
            }
        }

        /// <summary>
        /// Both the student and the opp will be locked before processing a test.
        /// If either is already locked, the test will left in the queue for QA to 
        /// pick up again the next time it hits the xml repository.  So we will only
        /// process 1 test per student at a time.  If another opp comes in for the
        /// same student, it will be left in the queue.  It's possible that the same opp
        /// can come in for a different student in the case of a reassignment.  This is why
        /// we're locking on both SSID and OppID.  If the student is not locked but the opp is,
        /// the test will be left in the queue.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool LockOpp(XmlRepositoryItem file)
        {
            bool locked = false;
            lock (oppLock)
            {
                // see if we can lock the student.  If the SSID is empty (can happen with
                //  paper test discreps), then we'll skip locking on the student.
                locked = lockedStudents.Add(file.TesteeKey);
                if (locked)
                {
                    // we were able to lock the student (or the SSID was null/blank).  
                    //  Make sure we can also lock the opp.  For a reassignment, the opp could be locked 
                    //  by another student.
                    locked = lockedOpps.Add(file.OppID);
                    // if we weren't able to lock the opp, then unlock the student that we just 
                    //  locked above.
                    if (!locked)
                        lockedStudents.Remove(file.TesteeKey);
                }
            }
            return locked;
        }

        #region IDisposable Members

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (activeThreadCount != null)
                        activeThreadCount.Dispose();
                    if (filesPerSecond != null)
                        filesPerSecond.Dispose();
                    if (totalFilesProcessedToday != null)
                        totalFilesProcessedToday.Dispose();
                }
                disposed = true;
            }
        }

        #endregion
    }
}
