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

namespace TISServices.Utilities
{
    internal class Statistics
    {
        private static volatile Statistics _instance;
        private static object _syncLoc = new object();

        private int _numRequestsReceived;
        private int _numRequestsInserted;
        private DateTime _startTime;

        internal static Statistics Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncLoc)
                    {
                        _instance = new Statistics();
                    }
                }
                return _instance;
            }
        }

        internal static int NumRequestsReceived
        {
            get
            {
                return Instance._numRequestsReceived;
            }
        }

        internal static int NumRequestsInserted
        {
            get
            {
                return Instance._numRequestsInserted;
            }
        }

        internal int UnauthenticatedRequests { get; private set; }

        internal static DateTime StartTime
        {
            get
            {
                return Instance._startTime;
            }
        }

        private Statistics()
        {
            this._numRequestsInserted = 0;
            this._numRequestsReceived = 0;
            this.UnauthenticatedRequests = 0;
            this._startTime = DateTime.Now;
        }

        internal static void AddToReceivedRequestCount()
        {
            Instance.AddReceivedRequest();
        }

        internal static void AddToInsertedRequestCount()
        {
            Instance.AddInsertedRequest();
        }

        internal static void AddToUnauthenticatedRequestCount()
        {
            lock (_syncLoc)
            {
                Instance.UnauthenticatedRequests++;
            }
        }

        /// <summary>
        /// dummy method to initialize the object the first time, this is so we can record the start time
        /// </summary>
        internal static void Initialize()
        {
            Instance.InitializeSelf();
        }

        #region private methods
        private void AddReceivedRequest()
        {
            lock (_syncLoc)
            {
                _numRequestsReceived++;
            }
        }

        private void AddInsertedRequest()
        {
            lock (_syncLoc)
            {
                _numRequestsInserted++;
            }
        }

        /// <summary>
        /// dummy method to initialize the object the first time, this is so we can record the start time
        /// </summary>
        private void InitializeSelf() { }

        #endregion
    }
}