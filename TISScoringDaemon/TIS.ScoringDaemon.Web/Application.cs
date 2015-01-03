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
using System.Text;
using AIR.Common;
using TDS.ScoringDaemon.Abstractions;
using AIR.Common.Threading;

namespace TIS.ScoringDaemon.Web
{
    public class Application : TDS.ScoringDaemon.Web.Application
    {
        protected override void RegisterServices()
        {
            base.RegisterServices();
            ServiceLocator.Register<ItemScoreRequestFactory>(() => new ItemScoreRequestFactory());
        }

        //protected override Action CallbackInitAction
        //{
        //    get
        //    {
        //        return delegate
        //        {
        //            // create the thread pool and assign it to the callback handler
        //            ItemScoringCallbackHandler.WorkerPool = new BoundedThreadPool(
        //                ScoringDaemonSettings.CallbackThreadPoolCount, "Item Scoring Callback",
        //                ScoringDaemonSettings.CallbackThreadPoolHighWaterMark, ScoringDaemonSettings.CallbackThreadPoolLowWaterMark);
        //        };
        //    }
        //}
    }
}
