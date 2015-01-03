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
using System.Threading.Tasks;
using TDSQASystemAPI;
using TDSQASystemAPI.Routing;
using TDSQASystemAPI.Routing.ItemScoring;
using TDSQASystemAPI.Acknowledgement;
using OSS.TIS.Routing;
using OSS.TIS.Routing.ItemScoring;
using AIR.Common;
using OSS.TIS.Acknowledgement;
using TDSQASystemAPI.TestResults;

namespace OSS.TIS
{
    public class TISMainThread : QASystemMainThread
    {
        protected override void InitializeServices()
        {
            ServiceLocator.Register<ITargetFactory>(() => new TISTargetFactory());
            ServiceLocator.Register<IItemScoringTargetFactory>(() => new ItemScoringTargetFactory());
            ServiceLocator.Register<IAcknowledgementTargetFactory>(() => new AcknowledgementTargetFactory());
            ServiceLocator.Register<ITestResultSerializerFactory>(() => new TestResultSerializerFactory());
            ServiceLocator.Register<TDSQASystemAPI.Config.ISystemConfigurationManager>(() => new Config.SystemConfigurationManager());
        }
    }
}
