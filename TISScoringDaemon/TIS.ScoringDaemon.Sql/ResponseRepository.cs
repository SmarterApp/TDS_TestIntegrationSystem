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
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using AIR.Common.Sql;
using TDS.ScoringDaemon.Abstractions;
using TDS.Shared.Data;
using TDS.Shared.Sql;
using TDS.Shared.Logging;
using TIS.ScoringDaemon.Abstractions.Extensions;

namespace TIS.ScoringDaemon.Sql
{
    public class ResponseRepository: TDSRepository, IResponseRepository
    {
        public ResponseRepository(string connectionString, String clientName)
            : base(connectionString, clientName)
        {
        }

        public ItemScoringConfig GetItemScoringConfigs()
        {
            SqlCommand cmd = CreateCommand(CommandType.StoredProcedure, "SD_GetItemScoringConfigs");
            AddClientParameter(cmd);

           ItemScoringConfig config = new ItemScoringConfig
               {
                   LoadTime = DateTime.Now,
                   ItemScoringRules = new List<ItemScoringRule>(),
                   Satellites = new List<ScoreHostInfo>()
               };

            ExecuteReader(cmd, delegate(IColumnReader reader)
            {
                while (reader.Read())
                {
                    ItemScoringRule rule = new ItemScoringRule
                        {
                            Context = reader.GetString("Context"),
                            ItemType = reader.GetString("ItemType"),
                            Enabled = reader.GetBoolean("Item_in")                            
                        };
                    if (reader.GetFieldType("Priority") == typeof(Boolean))
                    {
                        rule.Priority = reader.GetBoolean("Priority") ? 1 : 0;
                    }
                    else
                    {
                        rule.Priority = reader.GetInt32("Priority");
                    }
                    if (!reader.IsDBNull("ServerUrl"))
                    {
                        rule.ServerUrl = reader.GetString("ServerUrl");
                    }

                    config.ItemScoringRules.Add(rule);
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        ScoreHostInfo satellite = new ScoreHostInfo
                            {
                                ScoreHost = reader.GetString("scoreHost"),
                                StudentApp = reader.GetString("studentApp")
                            };
                        config.Satellites.Add(satellite);
                    }
                }
            });

            if (config.ItemScoringRules.Count == 0)
                return null;

            return config;
        }

        public List<ScorableResponse> GetScoreItems(int pendingMinutes, int minAttempts, int maxAttempts, int sessionType, int maxItemsReturned)
        {
            SqlCommand cmd = CreateCommand(CommandType.StoredProcedure, "SD_GetScoreItems");
            AddClientParameter(cmd);
            cmd.AddInt("pendingMinutes", pendingMinutes);
            cmd.AddInt("minAttempts", minAttempts);
            cmd.AddInt("maxAttempts", maxAttempts);
            //cmd.AddInt("sessionType", sessionType);
            cmd.AddInt("maxItemsReturned",maxItemsReturned);

            List<ScorableResponse> scorableResponses = new List<ScorableResponse>();
            ExecuteReader(cmd, delegate(IColumnReader reader)
            {
                while (reader.Read())
                {
                    ScorableResponse scorableResponse = new ScorableResponse
                        {
                            OppKey = reader.GetGuid("OppKey"),
                            //TODO: this is probably a good thing to have for item resolution rules.  Can add this to the contextToken
                            //  so that we can key off of it in the callback handler to determine which rule to create.  We'll want to store
                            //  it though in TestOpportunityItemScore to avoid joins, and we don't need it right now since there's only 1 item resolution
                            //  rule that keys off of the item type.
                            //TestKey = reader.GetString("testkey"),
                            TestId = reader.GetString("testID"),
                            Position = reader.GetInt32("position"),
                            ResponseSequence = reader.GetInt32("reportingversion"),
                            Response = !reader.IsDBNull("response") ? reader.GetString("response") : null,  // Response can be NULL so making sure we dont error out because of that.
                            //ScoreMark = reader.GetGuid("scoremark"),
                            ItemKey = reader.GetString("itemKey"),
                            Format = reader.GetString("ItemType"),
                            //Language = reader.GetString("Language"),
                            Attempts = reader.GetInt32("attempts"),
                            //SegmentId = reader.GetString("segmentID"),
                            //note: this is currently being set to a bogus, static value, but it needs to be non-blank/null
                            //  in order for the scoring request to be made.
                            ItemFile = !reader.IsDBNull("itemFile") ? reader.GetString("itemFile") : null
                        };

                    scorableResponses.Add(scorableResponse);
                }
            });

            return scorableResponses;
        }

        /// <summary>
        /// Will attempt to update the item score for this response.  If there are no more
        /// responses waiting for action, it will also flip the test status to 'handscored'
        /// and submit the file back to the source bin in xmlrepo for TIS to pick up and process.
        /// </summary>
        /// <param name="scoredResponse"></param>
        /// <returns></returns>
        public ScorableTest UpdateItemScore(ScoredResponse scoredResponse)
        {
            ReturnStatus returnStatus = null;
            SqlCommand cmd = CreateCommand(CommandType.StoredProcedure, "SD_UpdateItemScore");
            MemoryStream ms = null;
            int[] itemKey = scoredResponse.GetItemKeyTokens();

            try
            {
                cmd.AddUniqueIdentifier("oppkey", scoredResponse.OppKey);
                cmd.AddInt("itemBank", itemKey[0]);
                cmd.AddInt("itemKey", itemKey[1]);
                //cmd.AddInt("position", scoredResponse.Position);
                cmd.AddInt("sequence", scoredResponse.Sequence); // TODO: this will be reportingversion
                cmd.AddInt("score", scoredResponse.Score);
                cmd.AddVarChar("scorestatus", scoredResponse.ScoreStatus, 150);
                if (!String.IsNullOrEmpty(scoredResponse.ScoreRationale))
                    cmd.AddVarCharMax("scoreRationale", scoredResponse.ScoreRationale);
                //cmd.AddUniqueIdentifier("scoremark", scoredResponse.ScoreMark);
                if (!String.IsNullOrEmpty(scoredResponse.ScoreDimensions))
                {
                    ms = new MemoryStream();
                    scoredResponse.ScoreInfoAsXml().Save(ms);
                    cmd.AddXml("scoreDimensions", new SqlXml(ms));
                }

                ExecuteReader(cmd, delegate(IColumnReader reader)
                {
                    if (reader.Read())
                    {
                        returnStatus = ReturnStatus.Parse(reader);
                    }
                });
            }
            finally
            {
                if (ms != null)
                    ms.Dispose();
            }

            // if the sproc didn't return success, it means that the opp was not in a state that allows
            //  item score updates.  Either the reportingVersion didn't match or the opp was not in handscoring or appeal status
            //  Log the score in this case so that we have a record of it.
            if (!returnStatus.Status.Equals("success"))
                TDSLogger.Application.Info(new TraceLog(String.Format("Item Scoring: Did not accept score: {0}", scoredResponse.AsString())));

            // always returning null for ScorableTest.  Will not support test scoring here.  That will be the responsibility of TIS
            return null;
        }

        public ReturnStatus UpdateTestScore(ScoredTest scoredTest)
        {
            //TIS_UpdateOppStatusForItemsScored will now be called by TIS_UpdateItemScore; didn't fit neatly
            //  with the TDS/base workflow, so the test scoring functionality will not be supported in this
            //  instantiation of the ScoringDaemon.
            //
            //SqlCommand cmd = CreateCommand(CommandType.StoredProcedure, "TIS_UpdateOppStatusForItemsScored");
            //cmd.AddUniqueIdentifier("oppKey", scoredTest.OppKey);
            //bool ok = Convert.ToBoolean(cmd.ExecuteScalar());
            //return new ReturnStatus(ok ? "success" : "failure");
            return new ReturnStatus("failure");
        }
    }
}
