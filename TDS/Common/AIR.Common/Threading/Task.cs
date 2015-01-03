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

namespace AIR.Common.Threading
{
    /// <summary>
    /// A generic task used for assigning an action.
    /// </summary>
    public class Task : ITask
    {
        private readonly Action _action;
        private DateTime enqueueTime;
        private DateTime dequeueTime;
        private TimeSpan executionTime;

        public Task(Action action)
        {
            _action = action;
        }

        #region ITask Members


        public virtual void Execute()
        {
            if(_action != null) _action();
        }

        public DateTime TimeEnqueued
        {
            get
            {
                return enqueueTime;
            }
            set
            {
                enqueueTime = value;
            }
        }

        public DateTime TimeDequeued
        {
            get
            {
                return dequeueTime;
            }
            set
            {
                dequeueTime = value;
            }
        }

        public TimeSpan ExecutionTime
        {
            get
            {
                return executionTime;
            }
            set
            {
                executionTime = value;
            }
        }

        #endregion
    }

}
