/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Threading;
using System.Web;
using System.Xml;
using System.IO;
using AIR.Common;
using AIR.Common.Threading;
using AIR.Common.Web;
using TDS.ItemScoringEngine;
using TDS.ItemScoringEngine.Web;
using TDS.ScoringDaemon.Abstractions;
using TDS.Shared.Logging;

namespace TDS.ScoringDeamon.Web
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
            catch(Exception ex)
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

            // decrypt token
            string decryptedToken = EncryptionHelper.DecryptFromBase64(itemScoreResponse.Score.ContextToken);

            // get test data
            WebValueCollection tokenData = new WebValueCollection();
            tokenData.FillFromString(decryptedToken);

            // parse test data and collect up the information needed to persist this score
            ScoredResponse scoredResponse = new ScoredResponse
                {
                    OppKey = tokenData.Get<Guid>("oppKey"),
                    ItemKey = tokenData.Get("itemKey"),
                    Position = tokenData.Get<int>("position"),
                    Sequence = tokenData.Get<int>("sequence"),
                    ScoreMark = tokenData.Get<Guid>("scoremark"),
                    Score = itemScoreResponse.Score.ScoreInfo.Points,
                    ScoreStatus = itemScoreResponse.Score.ScoreInfo.Status.ToString(),
                    ScoreRationale = HttpWebHelper.SerializeXml(itemScoreResponse.Score.ScoreInfo),
                    ScoreDimensions = GetDimensionsXmlForSP(itemScoreResponse.Score.ScoreInfo)
                };

            string hubDBIP = tokenData.Get<String>("hubDBIP");
            string hubDBName = tokenData.Get<String>("hubDBName");
            string clientName = tokenData.Get<String>("clientName");
            string environment = tokenData.Get<String>("environment");

            // create function for submitting scores
            Action submitScore = delegate
            {
                Interlocked.Increment(ref ProgressCount);

                try
                {
                    // Save score to DB
                    IResponseRepository responseRepo = ServiceLocator.Resolve<IResponseRespositoryFactory>()
                                  .Create(ScoringDaemonSettings.HubConnectionString(hubDBIP, hubDBName), clientName, environment);

                    if (responseRepo != null)
                    {
                        responseRepo.UpdateItemScore(scoredResponse);
                    }
                    else
                    {
                        // this is really unusual. We got a item score response for an unknown hub.
                        string errorMessage =
                            String.Format(
                                "Got a score response for a hub that we dont monitor. oppKey:{0}, itemKey: {1}, position: {2}, sequence:{3}, scoreMark: {4}, clientName: {5}, environment: {6}",
                                scoredResponse.OppKey, scoredResponse.ItemKey, scoredResponse.Position,
                                scoredResponse.Sequence, scoredResponse.ScoreMark, clientName, environment);
                        throw new InvalidDataException(errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    Interlocked.Decrement(ref ProgressCount);
                    Interlocked.Increment(ref ErrorCount);

                    LastError = String.Format("{0} ({1})", ex.Message, DateTime.Now);
                    TDSLogger.Application.Error(new TraceLog("Item Scoring: Error submitting callback score.", ex));

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

        protected virtual string GetDimensionsXmlForSP(ItemScoreInfo scoreInfo)
        {           
            scoreInfo.Rationale = null;

            if (scoreInfo.SubScores != null)
            {
                foreach (ItemScoreInfo subScore in scoreInfo.SubScores)
                {
                    subScore.Rationale = null;
                }
            }

            return HttpWebHelper.SerializeXml(scoreInfo);
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
