/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;

namespace AIR.Common.Threading
{
    public interface ITask
    {
        void Execute();

        DateTime TimeEnqueued { get; set; }

        DateTime TimeDequeued { get; set; }

        TimeSpan ExecutionTime { get; set; }

    }
}