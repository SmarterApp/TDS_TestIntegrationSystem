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
    public class ThreadPoolStats
    {
        private long tasksQueued;
        private long tasksDeQueued;
        private int currentThreadCount;

        //Number of seconds task sits in queue     
        private TimeSpan minTaskQueueingDelay;
        private TimeSpan aveTaskQueueingDelay;
        private TimeSpan maxTaskQueueingDelay;

        //Number of seconds task takes to execute
        private TimeSpan minTaskExecutionDelay;
        private TimeSpan aveTaskExecutionDelay;
        private TimeSpan maxTaskExecutionDelay;

        public ThreadPoolStats(long tasksQueued, long tasksDeQueued, int threadcount, TimeSpan minTaskQDelay, TimeSpan aveTaskQDelay, TimeSpan maxTaskQDelay, TimeSpan minTaskExecutionTime, TimeSpan aveTaskExecutionTime, TimeSpan maxTaskExecutionTime)
        {
            this.tasksQueued = tasksQueued;
            this.tasksDeQueued = tasksDeQueued;
            this.currentThreadCount = threadcount;
            this.minTaskQueueingDelay = minTaskQDelay;
            this.aveTaskQueueingDelay = aveTaskQDelay;
            this.maxTaskQueueingDelay = maxTaskQDelay;
            this.minTaskExecutionDelay = minTaskExecutionTime;
            this.aveTaskExecutionDelay = aveTaskExecutionTime;
            this.maxTaskExecutionDelay = maxTaskExecutionTime;         
        }

        public long TasksExecuted { get { return tasksDeQueued; } }

        public long TasksInQCount { get { return tasksQueued - tasksDeQueued; } }

        public long CurrentThreadCount { get { return currentThreadCount; } }

        public TimeSpan MinQDelay { get { return minTaskQueueingDelay; } }

        public TimeSpan AveQDelay { get { return aveTaskQueueingDelay; } }

        public TimeSpan MaxQDelay { get { return maxTaskQueueingDelay; } }

        public TimeSpan MinTaskExecutionTime { get { return minTaskExecutionDelay; } }

        public TimeSpan AveTaskExecutionTime { get { return aveTaskExecutionDelay; } }

        public TimeSpan MaxTaskExecutionTime { get { return maxTaskExecutionDelay; } }

    }
}
