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

namespace TDSQASystemAPI.Routing.ItemScoring
{
    public class PretendItemScoringTarget : Target
    {
        public PretendItemScoringTarget(string targetName)
            : base(targetName, TargetClass.Handscoring, TargetType.Custom)
        {
        }

        public override ITargetResult Send(TestResults.TestResult testResult, Action<object> outputProcessor, params object[] inputArgs)
        {
            return new TargetResult() { Sent = true, Message = "pretend" };
        }
    }
}
