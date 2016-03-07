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
    public class ThreadPoolStatsRecorder
    {
        private static readonly object lockObject = new Object();

        // Items currently commented out are stats that would be useful to collect in the future but leaving for now due to time constraints
        long tasksQueued = 0;
        long tasksDeQueued = 0;
        int currentThreadCount = 0;        

        //Number of tasks being queued/dequeued per second
        //float aveTaskArrivalRate;       
        //float aveTaskServiceRate;       

        //Number of seconds task sits in queue     
        TimeSpan minTaskQueueingDelay = TimeSpan.MaxValue;
        long aveTaskQueueingDelayinTicks = 0;
        TimeSpan maxTaskQueueingDelay = TimeSpan.MinValue;

        //Number of seconds task takes to execute
        TimeSpan minTaskExecutionDelay = TimeSpan.MaxValue;
        long aveTaskExecutionDelayinTicks = 0;
        TimeSpan maxTaskExecutionDelay = TimeSpan.MinValue;

        public ThreadPoolStatsRecorder(int threadCount)
        {
            currentThreadCount = threadCount;   // Todo: when we implement self expanding/contracting pools, this needs to be fixed.
        }        

        public void RecordTaskStats(ITask task, ThreadPoolActivity step) 
        {
            lock (lockObject)
            {
                switch (step)
                {
                    case ThreadPoolActivity.ENQUEUED:
                        {
                            tasksQueued++;
                        }
                        break;
                    case ThreadPoolActivity.DEQUEUED:
                        {
                            tasksDeQueued++;
                            TimeSpan taskinQTime = new TimeSpan(task.TimeDequeued.Ticks - task.TimeEnqueued.Ticks);
                            if (taskinQTime.Ticks < minTaskQueueingDelay.Ticks) minTaskQueueingDelay = taskinQTime;
                            if (taskinQTime.Ticks > maxTaskQueueingDelay.Ticks) maxTaskQueueingDelay = taskinQTime;
                            aveTaskQueueingDelayinTicks = ((aveTaskQueueingDelayinTicks * (tasksDeQueued - 1)) + taskinQTime.Ticks) / tasksDeQueued;

                        }
                        break;

                    case ThreadPoolActivity.EXECUTED:
                        {
                            if (task.ExecutionTime.Ticks < minTaskExecutionDelay.Ticks) minTaskExecutionDelay = task.ExecutionTime;
                            if (task.ExecutionTime.Ticks > maxTaskExecutionDelay.Ticks) maxTaskExecutionDelay = task.ExecutionTime;
                            aveTaskExecutionDelayinTicks = ((aveTaskExecutionDelayinTicks * (tasksDeQueued - 1)) + task.ExecutionTime.Ticks) / tasksDeQueued;
                        }
                        break;
                }
            }

        }

        public ThreadPoolStats GetStats()
        {
            return new ThreadPoolStats(tasksQueued, tasksDeQueued, currentThreadCount, minTaskQueueingDelay, new TimeSpan(aveTaskQueueingDelayinTicks), maxTaskQueueingDelay, minTaskExecutionDelay, new TimeSpan(aveTaskExecutionDelayinTicks), maxTaskExecutionDelay);
        }

    }
}
