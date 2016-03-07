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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Text;
using AIR.Common.Diagnostics;

namespace AIR.Common.Threading
{
    public delegate void  ThreadPoolExceptionHandler(ITask task, Exception exception);

    /// <summary>
    /// A thread pool implementation which allows the queue to be bounded.
    /// </summary>
    /// <remarks>
    /// Threading references: 
    /// * http://www.albahari.com/threading/part2.aspx
    /// * http://www.bluebytesoftware.com/blog/2008/07/29/BuildingACustomThreadPoolSeriesPart1.aspx
    /// * http://smartthreadpool.codeplex.com/ 
    /// </remarks>
    public class BoundedThreadPool : IThreadPool
    {
        private static int _numberOfProcessors = GetNumberOfProcessors();

        private readonly Queue<ITask> taskQueue = new Queue<ITask>();       
        private readonly int numWorkerThreads;
        private Thread[] workerThreads;
        private int taskQHighWaterMark;
        private int taskQLowWaterMark;
        private bool suspendQUntilLowWaterMarkisReached = false;
        private ThreadPoolStatus status = ThreadPoolStatus.Uninitialized;
        private string name;
        private ThreadPoolStatsRecorder statsRecorder;

        private int threadsWaiting;

        enum ThreadPoolStatus
        {
            Uninitialized,
            Active,
            ShuttingDown,
            Closed
        }

        public static int NumberOfProcessors
        {
            get { return _numberOfProcessors; }
        }

        public int Count
        {
            get { return this.taskQueue.Count; }
        }

        public int ThreadCount
        {
            get { return this.numWorkerThreads; }
        }

        public ThreadPoolExceptionHandler ExceptionHandler
        {
            private get;
            set;
        }

        public BoundedThreadPool(int threadCount, string threadPoolName, int highWaterMark, int lowWaterMark)
            : this(threadCount, threadPoolName, highWaterMark, lowWaterMark, true)
        {
        }

        public BoundedThreadPool(int threadCount, string threadPoolName, int highWaterMark, int lowWaterMark, bool captureStats) 
        {
            if (threadCount <= 0)
            {
                threadCount = Math.Max(_numberOfProcessors, 2);
            }

            this.numWorkerThreads = threadCount;

            if (threadPoolName == null)
            {
                this.name = string.Empty;
            }
            else
            {
                this.name = threadPoolName;
            }

            // if the high/low watermarks are 0 then basically the queue is unbounded
            if (highWaterMark <= 0) highWaterMark = Int32.MaxValue;
            if (lowWaterMark <= 0) lowWaterMark = Int32.MaxValue;

            if (lowWaterMark > highWaterMark)
            {
                throw new ArgumentException("The low watermark cannot be larger than the high watermark", "lowWaterMark");
            }

            this.taskQHighWaterMark = highWaterMark;
            this.taskQLowWaterMark = lowWaterMark;

            if (captureStats)
            {
                statsRecorder = new ThreadPoolStatsRecorder(threadCount);
            } 

            InitWorkerThreads();
        }

        /// <summary>
        /// Initialize and kick off the required number of worker threads
        /// </summary>
        private void InitWorkerThreads()
        {
            // TODO: This should be locked so someone cannot initialize a threadpool while it is shutting down

            workerThreads = new Thread[numWorkerThreads];

            for (int i = 0; i < numWorkerThreads; i++)
            {
                Thread thread = new Thread(WorkerLoop);
                thread.Name = string.IsNullOrEmpty(this.name) ? this.name : this.name + " #" + (i + 1);

                // "Pooled threads are always background threads."
                // http://stackoverflow.com/questions/3646901/thread-pool-and-isbackground-in-net
                thread.IsBackground = true;

                workerThreads[i] = thread;
                workerThreads[i].Start();
            }

            this.status = ThreadPoolStatus.Active;
        }

        public bool IsShutdown()
        {
            return (status == ThreadPoolStatus.ShuttingDown || status == ThreadPoolStatus.Closed);
        }

        /// <summary>
        /// Main Loop that all worker threads run
        /// </summary>
        private void WorkerLoop() 
        {
            ITask task = null;
            
            while (true)
            {
                try
                {
                    lock (taskQueue)
                    {
                        // If shutdown was requested, exit the thread.
                        if (IsShutdown()) return;

                        // Find a new work item to execute.
                        while (taskQueue.Count == 0)
                        {
                            threadsWaiting++;
                            try { Monitor.Wait(taskQueue); }
                            finally { threadsWaiting--; }

                            // If we were signaled due to shutdown, exit the thread.
                            if (IsShutdown()) return;
                        }

                        // We found a work item! Grab it
                        task = taskQueue.Dequeue();
                        
                        // Record some statistics
                        if (task != null)
                        {
                            task.TimeDequeued = DateTime.UtcNow;  //Marking the time of dequeue
                            if (statsRecorder != null) statsRecorder.RecordTaskStats(task, ThreadPoolActivity.DEQUEUED);
                        }
                    }

                    if (task != null)
                    {
                        Stopwatch stopWatch = Stopwatch.StartNew();
                        
                        try
                        {
                            task.Execute();
                        }
                        finally
                        {
                            stopWatch.Stop();
                            task.ExecutionTime = stopWatch.Elapsed;
                            if (statsRecorder != null) statsRecorder.RecordTaskStats(task, ThreadPoolActivity.EXECUTED);
                        }
                    }
                }
                catch (Exception exception)
                {
                    InternalLogger.LogException(exception); // log to win32

                    // if this is a critical exception (e.x., out of memory) then we need to shut down this thread
                    if (IsCriticalException(exception))
                    {
                        throw;
                    }

                    // ThreadAbortException is automagically reraised, so don't try and stop it
                    // http://blogs.msdn.com/b/clrteam/archive/2009/04/28/threadabortexception.aspx
                    if (exception is ThreadAbortException) return;

                    // Handle the exception that has occured
                    HandleException(task, exception);

                    // we will now restart the worker while loop
                }
            }
        }

        // Simple handling here. If an extended class wants to do something special, override this method
        public virtual void HandleException(ITask task, Exception exception)
        {
            if (ExceptionHandler != null)
            {
                try
                {
                    ExceptionHandler(task, exception);
                }
                catch (Exception exp)
                {
                    // We can't do anything here... silently absorb it.
                    InternalLogger.LogException(exception);
                }
            }
        }

        /// <summary>
        /// Primary entry point to add tasks to this thread pool. 
        /// The thread pool maintains a high water mark which when reached, suspends further enqueing of tasks.
        /// The pool is unlocked only with the queue capacity dips below the low threshold.
        /// This is done primarily to prevent the queue from growing unbounded.
        /// </summary>
        /// <param name="task"></param>
        /// <returns>true if task was added, false otherwise</returns>
        public bool Enqueue(ITask task)
        {
            // Check to make sure we are initialized and running.
            if (status != ThreadPoolStatus.Active) return false;

            lock(taskQueue)
            {
                // Lets check our watermarks to see if we should add this task or not
                int queueCount = taskQueue.Count;

                // Case 1: We hit the high water mark. Tasks cannot be enqueued right now (likely due to high load)
                // Case 2: We are currently in the process of bleeding our queue till we hit the low water mark 
                if ((queueCount >= taskQHighWaterMark) || (queueCount >= taskQLowWaterMark && suspendQUntilLowWaterMarkisReached)) 
                {
                    suspendQUntilLowWaterMarkisReached = true;
                    return false;
                }

                // Case 3: We can enqueue (possibly waking a thread)
                suspendQUntilLowWaterMarkisReached = false;
                taskQueue.Enqueue(task);
                task.TimeEnqueued = DateTime.UtcNow;  //Marking the time of enqueue
                if (statsRecorder != null) statsRecorder.RecordTaskStats(task, ThreadPoolActivity.ENQUEUED);
                
                // notify any waiting threads that a task has been added
                if (threadsWaiting > 0)
                {
                    Monitor.Pulse(taskQueue);
                }
            }

            return true;
        }

        /// <summary>
        /// A helper function to assign a function to the thread pool for execution.
        /// </summary>
        public bool Enqueue(Action action)
        {
            return Enqueue(new Task(action));
        }

        private static bool IsCriticalException(Exception exception)
        {
            return (((exception is ExecutionEngineException) || (exception is OutOfMemoryException)) || (exception is SEHException));
        }

        private static int GetNumberOfProcessors()
        {
            int processorCount = 1;
            
            try
            {
                processorCount = Environment.ProcessorCount;
            }
            catch (Exception exception)
            {
                Trace.WriteLine("Exception reading processor count:" + exception);
            }
            
            return processorCount;
        }

        /// <summary>
        /// Shutdown the worker threads gracefully
        /// </summary>
        /// <param name="wait">Set this to true if you want to wait for threads to shutdown.</param>
        public void Shutdown(bool wait)
        {
            InternalLogger.LogInformation(String.Format("BoundedThreadPool \"{0}\" Shutting Down", this.name));

            if (this.status != ThreadPoolStatus.Active) return;

            Thread[] threads;

            lock(taskQueue)
            {
                // Signal the threads to exit
                this.status = ThreadPoolStatus.ShuttingDown;
                Monitor.PulseAll(taskQueue);

                // Make a copy of the threads' references in the pool
                threads = new Thread[numWorkerThreads];
                workerThreads.CopyTo(threads, 0);
            }

            if (wait)
            {
                for (int i = 0; i < this.numWorkerThreads; i++)
                {
                    threads[i].Join();
                }
            }

            // Abort the threads in the pool
            foreach (Thread thread in threads)
            {
                if ((thread != null) && thread.IsAlive)
                {
                    try
                    {
                        thread.Abort(); // Shutdown
                    }
                    catch (SecurityException ex)
                    {
                        ex.GetHashCode();
                    }
                    catch (ThreadStateException ex)
                    {
                        ex.GetHashCode();
                        // In case the thread has been terminated 
                        // after the check if it is alive.
                    }
                }
            }

            // clear threads refs
            Array.Clear(workerThreads, 0, workerThreads.Length);

            this.status = ThreadPoolStatus.Closed;

            InternalLogger.LogInformation(String.Format("BoundedThreadPool \"{0}\" Shut Down", this.name));
        }

        /// <summary>
        /// Shutdown the worker threads gracefully
        /// </summary>
        public void Dispose()
        {
            this.Shutdown(false);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            // GC.SuppressFinalize(this);
        }

        ~BoundedThreadPool()
        {
            Dispose();
        }

        public ThreadPoolStats GetStats()
        {
            if (statsRecorder != null)
            {
                return statsRecorder.GetStats();
            }
            else
            {
                return null;
            }
        }

    }

}
