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
using System.Threading;
using System.Web;
using System.Xml;
using System.IO;
using AIR.Common;
using AIR.Common.Threading;
using AIR.Common.Web;
using AIR.Common.Json;
using TDS.ScoringDeamon.Web;
using TDS.ItemScoringEngine;
using TDS.ItemScoringEngine.Web;
using TDS.ScoringDaemon.Abstractions;
using TDS.Shared.Logging;
using TIS.ScoringDaemon.Abstractions;
using TISItemResolution;
using TDSQASystemAPI.Utilities;

namespace TIS.ScoringDaemon.Web
{
    /// <summary>
    /// This handler is used on the client for recieving scoring responses.
    /// </summary>
    public class ItemScoringCallbackHandler : IHttpHandler
    {
        public static long RequestCount = 0; // incoming http request
        public static long SuccessCount = 0; // saved to the DB
        public static long ProgressCount = 0; // currently in progress
        public static long RejectCount = 0; // rejected by thread pool
        public static long ErrorCount = 0; // error when saving to DB
        public static string LastError = null; // the last error

        public static BoundedThreadPool WorkerPool { get; set; }

        public static ThreadPoolStats Stats
        {
            get { return WorkerPool != null ? WorkerPool.GetStats() : null; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.ContentLength == 0) return;

            try
            {
                ProcessScore(context);
            }
            catch (Exception ex)
            {
                TDSLogger.Application.Error(ex);
            }
        }

        private void ProcessScore(HttpContext context)
        {
            Interlocked.Increment(ref RequestCount);

            // GET RESPONSE
            ItemScoreResponse itemScoreResponse;

            // Get XML
            using (XmlReader reader = XmlReader.Create(context.Request.InputStream))
            {
                itemScoreResponse = HttpWebHelper.DeserializeXml<ItemScoreResponse>(reader);
                reader.Close();
            }

            // get the context token and deserialize it
            string decryptedToken = GetContextToken(itemScoreResponse);
            ItemScoreRequestContextToken tokenData = null;

            // get test data
            try
            {
                tokenData = JsonHelper.Deserialize<ItemScoreRequestContextToken>(decryptedToken);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format("Could not read contextToken: {0},  Error: {1}.", decryptedToken ?? "(null)", ex.Message));
            }

            // create response
            ScoredResponse scoredResponse = new ScoredResponse()
            {
                ItemKey = tokenData.ItemKey,
                OppKey = tokenData.oppKey,
                Sequence = tokenData.reportingVersion ?? -1,
                Score = itemScoreResponse.Score.ScoreInfo.Points,
                ScoreStatus = itemScoreResponse.Score.ScoreInfo.Status.ToString(),
                ScoreRationale = null,
                ScoreDimensions = HttpWebHelper.SerializeXml(itemScoreResponse.Score.ScoreInfo)
            };

            string hubDBIP = tokenData.TISIP;
            string hubDBName = tokenData.TISDbName;
            string clientName = tokenData.clientName;
            string environment = tokenData.environment;

            // create function for submitting scores
            Action submitScore = delegate
            {
                Interlocked.Increment(ref ProgressCount);

                try
                {
                    if (String.IsNullOrEmpty(hubDBIP))
                    {
                        // must be sending items for scoring in the TDSReport format where we don't send
                        //  this information in the context token.  In this case, we can only support a single configured hub.
                        //  Grab the info from that hub.
                        IAdminRepository cloudRepository = ServiceLocator.Resolve<IAdminRepository>();
                        List<DataStoreInfo> hubs = cloudRepository.GetMonitoredDataStores(ScoringDaemonSettings.MachineName);
                        hubs = hubs.FindAll(h => h.ClientName == clientName).ToList<DataStoreInfo>();
                        if (hubs.Count == 0)
                            throw new ApplicationException(String.Format("No hubs are configured for client: {0} on machine: {1}", clientName, ScoringDaemonSettings.MachineName));
                        if (hubs.Count > 1)
                            throw new ApplicationException(String.Format("TIS item scoring callback handler only supports a single hub per client for this type of request.  {0} are configured for client: {1}.", hubs.Count, clientName));
                        hubDBIP = ScoringDaemonSettings.HubIP(hubs[0]);
                        hubDBName = hubs[0].DBName;
                        clientName = hubs[0].ClientName;
                        environment = hubs[0].Environment;
                    }

                    // Save score to DB
                    IResponseRepository responseRepo = ServiceLocator.Resolve<IResponseRespositoryFactory>()
                                  .Create(ScoringDaemonSettings.HubConnectionString(hubDBIP, hubDBName), clientName, environment);

                    if (responseRepo == null)
                    {
                        // this is really unusual. We got a item score response for an unknown hub.
                        string errorMessage =
                            String.Format(
                                "Got a score response for a hub that we dont monitor. oppKey:{0}, itemKey: {1}, sequence:{2}, clientName: {3}, environment: {4}",
                                scoredResponse.OppKey, scoredResponse.ItemKey, scoredResponse.Sequence, clientName, environment);
                        throw new InvalidDataException(errorMessage);
                    }
                    
                    //run resolution rule if there is one
                    //Zach 12/11/2014 TODO: Add checks or try/catches around each part of running the rule to give more specific error messages?
                    TISItemResolutionRule rule = TISItemResolutionRule.CreateRule(tokenData.itemType, scoredResponse.ItemKey, clientName);
                    if (rule != null)
                    {
                        TDSQASystemAPI.TestResults.ItemScoreInfo resolvedScore = rule.ResolveItemScore(Serialization.DeserializeXml<TDSQASystemAPI.TestResults.ItemScoreInfo>(scoredResponse.ScoreDimensions));
                        scoredResponse.Score = (int)resolvedScore.Points;
                        scoredResponse.ScoreStatus = resolvedScore.Status == TDSQASystemAPI.TestResults.ScoringStatus.Scored ? ScoringStatus.Scored.ToString() : ScoringStatus.ScoringError.ToString();
                        scoredResponse.ScoreDimensions = HttpWebHelper.SerializeXml(resolvedScore);
                    }

                    responseRepo.UpdateItemScore(scoredResponse);
                }
                catch (Exception ex)
                {
                    Interlocked.Decrement(ref ProgressCount);
                    Interlocked.Increment(ref ErrorCount);

                    LastError = String.Format("{0} ({1})", ex.Message, DateTime.Now);

                    // wasn't logging the exception.  See TDS.Shared.Logging.TraceLog.ToString()
                    string id = tokenData == null ? "Unknown" : tokenData.ToString();
                    TDSLogger.Application.Error(new ApplicationException(String.Format("Error submitting callback score for: {0}", id), ex));
                    //TDSLogger.Application.Error(new TraceLog("Error submitting callback score.", ex));

                    return;
                }

                Interlocked.Decrement(ref ProgressCount);
                Interlocked.Increment(ref SuccessCount);
            };

            // check if thread pooling is enabled for callbacks
            if (WorkerPool != null)
            {
                // async
                if (!WorkerPool.Enqueue(submitScore))
                {
                    Interlocked.Increment(ref RejectCount);
                }
            }
            else
            {
                // sync
                submitScore();
            }
        }

        /// <summary>
        /// This handler expects an encrypted context token.  If for some reason it can't be decrypted, try the plain text value.
        /// The ItemScoringCallbackHandlerRcvOnly handler should be used if an encrypted context token is not expected.
        /// </summary>
        /// <param name="itemScoreResponse"></param>
        /// <returns></returns>
        protected virtual string GetContextToken(ItemScoreResponse itemScoreResponse)
        {
            string contextToken = null;
            try
            {
                contextToken = EncryptionHelper.DecryptFromBase64(itemScoreResponse.Score.ContextToken);
            }
            catch (Exception ex)
            {
                TDSLogger.Application.Warn(String.Format("Error decrypting context token.  Trying plain text value.  Message: {0}", ex.Message));
                contextToken = itemScoreResponse.Score.ContextToken;
            }
            return contextToken;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
