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
using System.Security.Principal;
using System.Text;
using System.Threading;
using AIR.Common;
using AIR.Common.Threading;
using AIR.Common.Web;
using TDS.ItemScoringEngine;
using TDS.ScoringDaemon.Abstractions;
using TDS.Shared.Logging;

namespace TDS.ScoringDeamon.Web
{
    public class ReponseRepoMonitor : IDisposable
    {

        private Timer _itemScoringTimer;
        public DateTime LastRun { get; set; }
        public String ClientName { get; private set; }
        public String Environment { get; private set; }
        public String DBIP { get; private set; }
        public String DBName { get; private set; }
        private IResponseRepository ResponseRepository { get; set; }
        private ItemScoringConfig ItemScoringConf { get; set; }
        private ItemScoringClient ScoreRequestSender { get; set; }
        
        public ThreadPoolStats Stats
        {
            get { return ScoreRequestSender != null ? ScoreRequestSender.WorkerPool.GetStats() : null; }
        }

        public ReponseRepoMonitor(string clientName, string environment, String dbIP, String dbName)
        {
            ClientName = clientName;
            Environment = environment;
            DBIP = dbIP;
            DBName = dbName;
            ResponseRepository = ServiceLocator.Resolve<IResponseRespositoryFactory>().Create(ScoringDaemonSettings.HubConnectionString(DBIP, DBName), clientName, environment);
            ScoreRequestSender = new ItemScoringClient(new BoundedThreadPool(
                ScoringDaemonSettings.ScoreRequestThreadPoolCount, String.Format("Score Request Thread Pool{0}:{1}", ClientName, Environment),
                ScoringDaemonSettings.ScoreRequestThreadPoolHighWaterMark, ScoringDaemonSettings.ScoreRequestThreadPoolLowWaterMark));
        }

        public void Activate()
        {
            // the timer interval is in seconds so convert to milliseconds
            long interval = (ScoringDaemonSettings.HubTimerIntervalSeconds * 1000);
            const long startup = 5 * 1000; // Delaying first invocation by 30 seconds to allow the app to fully load

            _itemScoringTimer = new Timer(delegate
                {
                    LastRun = DateTime.Now;
                    try
                    {
                        // check if the item scoring configs need to be reloaded
                        if (ItemScoringConf == null ||
                            DateTime.Now.Subtract(ItemScoringConf.LoadTime).TotalSeconds >
                            ScoringDaemonSettings.ItemScoringConfigCacheSeconds)
                        {
                            // Reload the item scoring configs
                            ItemScoringConf = ResponseRepository.GetItemScoringConfigs();
                        }

                        //Get the items that can be rescored 
                        List<ScorableResponse> pendingResponses = ResponseRepository.GetScoreItems(ScoringDaemonSettings.PendingMins, ScoringDaemonSettings.MinAttempts, ScoringDaemonSettings.MaxAttempts, ScoringDaemonSettings.SessionType, ScoringDaemonSettings.MaxItemsReturned);

                        int failures = ProcessPendingResponses(pendingResponses);
                        if (failures > 0)
                        {
                            TDSLogger.Application.Error(
                                new TraceLog(String.Format("Item Scoring: There were {0} out of {1} pending responses that could not be sent to rescore for {2}:{3}",
                                    failures, pendingResponses.Count, ClientName, Environment)));
                        } 

                        //Get the items that have expired and need to marked as ScoringError and sent on their way
                        List<ScorableResponse> expiredResponses = ResponseRepository.GetScoreItems(ScoringDaemonSettings.PendingMins, ScoringDaemonSettings.MaxAttempts + 1, 999, ScoringDaemonSettings.SessionType, ScoringDaemonSettings.MaxItemsReturned);
                        failures = ProcessExpiredResponses(expiredResponses);
                        if (failures > 0)
                        {
                            TDSLogger.Application.Error(
                                new TraceLog(String.Format("Item Scoring: There were {0} out of {1} expired responses that failed to update in the DB for {2}:{3}",
                                    failures, expiredResponses.Count, ClientName, Environment)));
                        }

                    }
                    catch (Exception ex)
                    {
                        string errorMessage = String.Format("Fatal exception occured on timer thread for {0}:{1}", ClientName, Environment);
                        TDSLogger.Application.Fatal(new TraceLog("Application: " + errorMessage, ex));
                    }


                }, null, startup, interval);
        }

        public void Dispose()
        {
            try
            {
                if (_itemScoringTimer != null)
                {
                    _itemScoringTimer.Dispose();
                }

                if (ScoreRequestSender != null)
                {
                    ScoreRequestSender.Dispose();
                }
            }
            catch (Exception ex)
            {
                // Nothing we can do here
            }
        }

        /// <summary>
        /// Submit responses that need scores to the remote item scoring engine
        /// </summary>
        /// <param name="pendingResponses"></param>
        /// <returns></returns>
        private int ProcessPendingResponses(IEnumerable<ScorableResponse> pendingResponses)
        {
            int failures = 0;

            if (ItemScoringConf == null) return pendingResponses.Count();

            foreach (ScorableResponse scorableResponse in pendingResponses)
            {
                ItemScoringRule rule = ItemScoringConf.GetItemScoringRule(scorableResponse.Format, scorableResponse.TestId);

                if (rule == null || (rule.Enabled && (rule.ServerUrl == null || rule.StudentAppUrl == null)))
                {
                    failures++;
                    continue;
                }

                if (!rule.Enabled || scorableResponse.Response == null)
                {
                    // This item is NOT supposed to be machine scored. 
                    ScoredResponse scoredResponse = new ScoredResponse
                        {
                            ItemKey = scorableResponse.ItemKey,
                            OppKey = scorableResponse.OppKey,
                            Position = scorableResponse.Position,
                            Score = -1,
                            ScoreDimensions = null,
                            ScoreMark = scorableResponse.ScoreMark,
                            ScoreRationale = !rule.Enabled ? "Item configured not to be scored" : "Item Response is NULL",
                            ScoreStatus = "NotScored",
                            Sequence = scorableResponse.ResponseSequence
                        };
                    try
                    {
                        UpdateItemScore(scoredResponse);
                    }
                    catch (Exception e)
                    {
                        failures++;
                    }
                }
                else if (String.IsNullOrEmpty(scorableResponse.ItemFile))   // Sometimes, we get responses that have no item file specified. We cannot do anything with these. 
                {
                    // This item is NOT supposed to be machine scored. 
                    ScoredResponse scoredResponse = new ScoredResponse
                    {
                        ItemKey = scorableResponse.ItemKey,
                        OppKey = scorableResponse.OppKey,
                        Position = scorableResponse.Position,
                        Score = -1,
                        ScoreDimensions = null,
                        ScoreMark = scorableResponse.ScoreMark,
                        ScoreRationale = "Item file path is null",
                        ScoreStatus = "ScoringError",
                        Sequence = scorableResponse.ResponseSequence
                    };
                    try
                    {
                        UpdateItemScore(scoredResponse);
                    }
                    catch (Exception e)
                    {
                        failures++;
                    }
                }
                else
                {
                    // Send this item off to scoring
                    string itemScoringServerUrl = ScoringDaemonSettings.ItemScoringServerUrlOverride ?? rule.ServerUrl;
                    ItemScoreRequest itemScoreRequest = ServiceLocator.Resolve<ItemScoreRequestFactory>().MakeItemScoreRequest(this, scorableResponse, rule, itemScoringServerUrl);
                       
                    if (itemScoreRequest == null)
                    {
                        failures++;
                        continue;
                    }

                    ScoreRequestSender.SendRequestAsync(itemScoringServerUrl, itemScoreRequest, null);
                }
            }

            return failures;
        }

        /// <summary>
        ///  Items that have 
        /// </summary>
        /// <param name="expiredResponses"></param>
        /// <returns></returns>
        private int ProcessExpiredResponses(IEnumerable<ScorableResponse> expiredResponses)
        {
            int failures = 0;

            foreach (ScorableResponse scorableResponse in expiredResponses)
            {
                ScoredResponse scoredResponse = new ScoredResponse
                {
                    ItemKey = scorableResponse.ItemKey,
                    OppKey = scorableResponse.OppKey,
                    Position = scorableResponse.Position,
                    Score = -1,
                    ScoreDimensions = null,
                    ScoreMark = scorableResponse.ScoreMark,
                    ScoreRationale = "Max Score Attempts Exceeded",
                    ScoreStatus = "ScoringError",
                    Sequence = scorableResponse.ResponseSequence
                };
                try
                {
                    UpdateItemScore(scoredResponse);
                }
                catch (Exception e)
                {
                    failures++;
                }
            }
            return failures;
        }

        public void UpdateItemScore(ScoredResponse scoredResponse)
        {
            if (scoredResponse == null) return;   // Should probably log an error here if this were to happen.

            ScorableTest scorableTest = ResponseRepository.UpdateItemScore(scoredResponse);
        }
    }
}
