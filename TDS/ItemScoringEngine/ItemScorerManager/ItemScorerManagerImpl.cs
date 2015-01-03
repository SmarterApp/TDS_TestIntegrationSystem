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
using AIR.Common.Threading;
using TDS.Shared.Logging;

namespace TDS.ItemScoringEngine
{
    public class ItemScorerManagerImpl : IItemScorerManager
    {
        #region Variables

        /// <summary>
        /// Maintains the itemformat to corresponding scoring engine relationship
        /// </summary>
        protected readonly Dictionary<string, IItemScorer> scoringEngines = new Dictionary<string, IItemScorer>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// A thread pool to use for Async scoring
        /// </summary>
        IThreadPool threadPool;
        
        #endregion

        public ItemScorerManagerImpl(int threadCount, int numTasksHighWaterMark, int numTasksLowWaterMark)
        {
            threadPool = new BoundedThreadPool(threadCount, "ItemScorerManagerImpl", numTasksHighWaterMark, numTasksLowWaterMark);
            ((BoundedThreadPool)threadPool).ExceptionHandler = (task, exp) => TDSLogger.Application.Error(exp);
        }

        public ThreadPoolStats GetThreadPoolStats()
        {
            return threadPool.GetStats();
        }

        #region IScoringEngineMaster Implemtation

        public void RegisterItemScorer(string itemFormat, IItemScorer itemScorerImpl)
        {
            scoringEngines.Add(itemFormat, itemScorerImpl);
        }

        public ItemScore ScoreItem(ResponseInfo studentResponse, IItemScorerCallback callbackReference)
        {
            IItemScorer itemScorerImpl;

            if (scoringEngines.TryGetValue(studentResponse.ItemFormat, out itemScorerImpl))
            {
                if (callbackReference == null)
                {
                    //This is synchronous scoring
                    return InvokeSynchronousScoring(itemScorerImpl, studentResponse);
                }
                else
                {
                    // This is asynchronous scoring
                    return InvokeAsynchronousScoring(itemScorerImpl, studentResponse, callbackReference);
                }
            }

            return new ItemScore(-1, -1, ScoringStatus.NoScoringEngine, null, new ScoreRationale{Msg = "No scoring engine found for " + studentResponse.ItemFormat}, null, studentResponse.ContextToken);
        }

        public ScorerInfo GetScorerInfo(string itemFormat)
        {
            IItemScorer itemScorerImpl;

            if (scoringEngines.TryGetValue(itemFormat, out itemScorerImpl))
            {
                return itemScorerImpl.GetScorerInfo(itemFormat);
            }

            return null;
        }

        #endregion

        #region Private Members

        private ItemScore InvokeSynchronousScoring(IItemScorer scorerImpl, ResponseInfo studentResponse)
        {
            return scorerImpl.ScoreItem(studentResponse, null);
        }

        private ItemScore InvokeAsynchronousScoring(IItemScorer scorerImpl, ResponseInfo studentResponse, IItemScorerCallback callbackReference)
        {
            // if the scorer supports async mode then let the item scorer server deal with async
            if (scorerImpl.GetScorerInfo(studentResponse.ItemFormat).SupportsAsyncMode)
            {
                return scorerImpl.ScoreItem(studentResponse, callbackReference);
            }
            
            // if the scorer does not handle async mode then we need to handle it in our own thread queue
            if (threadPool.Enqueue(new AsyncScoringTask(scorerImpl, studentResponse, callbackReference)))
            {
                return new ItemScore(-1, -1, ScoringStatus.WaitingForMachineScore, null, null, null, studentResponse.ContextToken);
            }
            
            // if we get here then the thread queue is filled (probably waiting on a bunch of scores to come back)
            return new ItemScore(-1, -1, ScoringStatus.WaitingForMachineScore, null, new ScoreRationale(){Msg="Cannot enqueue scoring task"}, null, studentResponse.ContextToken);
        }

        #endregion
    }

    class AsyncScoringTask : Task
    {
        IItemScorer scorerImpl;
        ResponseInfo studentResponse;
        IItemScorerCallback callback;

        public AsyncScoringTask(IItemScorer scorer, ResponseInfo studentResponse, IItemScorerCallback callback) : base(null)
        {
            this.scorerImpl = scorer;
            this.studentResponse = studentResponse;
            this.callback = callback;
        }

        // TODO: Add Error(Exception) override function to Task to support better error handling

        public override void Execute()
        {
            ItemScore itemScore = null;

            // Step 1: Score the Item
            try
            {
                // Note: This is sync but it is OK since it is executed by 1 of the thread pool threads so it is async to the original caller
                itemScore = scorerImpl.ScoreItem(studentResponse, null);                
            }
            catch (Exception ex)
            {
                // Exception scoring item. Prepare an error report
                itemScore = new ItemScore(-1, -1, ScoringStatus.ScoringError, null,
                              new ScoreRationale(){Msg="Exception scoring item " + studentResponse.ItemIdentifier + "\n" + ex.Message, Exception = ex}, 
                              null, studentResponse.ContextToken);
            }
            
            // Step 2: Use the callback to report the score
            try
            {
                callback.ScoreAvailable(itemScore, studentResponse);
            }
            catch (Exception ex)
            {
                TDSLogger.Application.Error(new TraceLog("Item Scoring: Exception Reporting Score", ex));                                    
            }
        }
    }
}
