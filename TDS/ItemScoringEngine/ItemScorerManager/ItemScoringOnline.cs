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
using AIR.Common.Threading;

namespace TDS.ItemScoringEngine
{
    /// <summary>
    /// A base class for ItemScoringServer and ItemScoringClient
    /// </summary>
    public abstract class ItemScoringOnline : IDisposable
    {
        static TraceSwitch _traceSwitchCore = new TraceSwitch("ItemScoringEngine", "Item Scoring Engine");
        protected readonly BoundedThreadPool _threadPool;

        protected ItemScoringOnline()
        {
            _threadPool = new BoundedThreadPool(10, "OnlineScoring", 500, 400);
        }

        protected ItemScoringOnline(BoundedThreadPool threadPool)
        {
            _threadPool = threadPool;
        }

        protected void Log(string message)
        {
            if (true || _traceSwitchCore.TraceInfo)
            {
                Trace.WriteLine("ItemScoringEngine: " + message);
            }
        }

        public BoundedThreadPool WorkerPool
        {
            get { return _threadPool; }
        }

        public void Dispose()
        {
            if(_threadPool != null) 
                _threadPool.Dispose();
        }
    }
}