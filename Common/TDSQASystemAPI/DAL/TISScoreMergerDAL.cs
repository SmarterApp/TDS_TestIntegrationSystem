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
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

using TDSQASystemAPI.TIS;

namespace TDSQASystemAPI.DAL
{
    internal class TISScoreMergerDAL
    {
        private static string TDSQCConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["TDSQC"].ConnectionString;
            }
        }
        private static string ClientName
        {
            get
            {
                return ConfigurationManager.AppSettings["ClientName"];
            }
        }

        /// <summary>
        /// get all rows from TestOpportunityItemScore table for this oppID
        /// Note that WaitingForHandScore is being converted to WaitingForMachineScore to simplify evaluation.
        /// It's all the same to TIS == waiting.
        /// </summary>
        /// <param name="oppID"></param>
        /// <param name="includeResponse">if false will return null for the response</param>
        /// <returns></returns>
        internal static List<TestOpportunityItemScore> GetItemScores(long oppID, bool includeResponse)
        {
            List<TestOpportunityItemScore> scores = new List<TestOpportunityItemScore>();
            using (SqlConnection conn = new SqlConnection(TDSQCConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("TIS_GetItemScores", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@oppID", oppID);
                    cmd.Parameters.AddWithValue("@clientName", ClientName);
                    cmd.Parameters.AddWithValue("@includeResponse", includeResponse);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            TestOpportunityItemScore score = 
                                new TestOpportunityItemScore(Convert.ToInt64(rdr["OppID"].ToString())
                                    ,rdr["ScoreInfo"] == DBNull.Value ? null : rdr["ScoreInfo"].ToString()
                                    , rdr["ScoreStatus"].ToString().Equals("WaitingForHandScore", StringComparison.InvariantCultureIgnoreCase) ? "WaitingForMachineScore" : rdr["ScoreStatus"].ToString()
                                    ,rdr["Score"] == DBNull.Value ? null : (int?)Convert.ToInt32(rdr["Score"].ToString()) 
                                    ,Convert.ToInt64(rdr["_efk_Item"].ToString())
                                    , Convert.ToInt64(rdr["_efk_ItemBank"].ToString())
                                    , rdr["scoreRationale"] == DBNull.Value ? null : rdr["scoreRationale"].ToString()
                                    ,rdr["Response"] == DBNull.Value ? null : rdr["Response"].ToString());
                            scores.Add(score);
                        }
                    }
                }
            }
            return scores;
        }

        internal static int UpdateItemsToScore(TestResults.TestResult testResult, List<TestResults.ItemResponse> responses, bool scoreInvalidations, bool batchScoring, bool updateSameReportingVersion, bool handscored)
        {
            int responsesUpdated = 0;

            if (responses.Count == 0)
                return responsesUpdated;

            using (SqlConnection conn = new SqlConnection(TDSQCConnectionString))
            {
                conn.Open();

                //actually, I don't think we need the transaction.  If it updates a subset of the items, the test will have to be
                //  resubmitted anyway at which point it'll get the rest.
                //using (SqlTransaction tran = conn.BeginTransaction())
                //{
                    foreach (TestResults.ItemResponse r in responses)
                    {
                        using (SqlCommand cmd = new SqlCommand("TIS_UpdateItemToScore", conn))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@OppID", testResult.Opportunity.OpportunityID);
                            cmd.Parameters.AddWithValue("@status", testResult.Opportunity.Status);
                            cmd.Parameters.AddWithValue("@_efk_Item", r.ItemKey);
                            cmd.Parameters.AddWithValue("@_efk_ItemBank", r.BankKey);
                            cmd.Parameters.AddWithValue("@reportingversion", testResult.Opportunity.ReportingVersion);
                            cmd.Parameters.AddWithValue("@OppKey", testResult.Opportunity.Key);
                            cmd.Parameters.AddWithValue("@_efk_ClientName", testResult.Opportunity.ClientName);
                            cmd.Parameters.AddWithValue("@testID", testResult.TestID);
                            cmd.Parameters.AddWithValue("@ItemType", r.Format);
                            cmd.Parameters.AddWithValue("@position", r.Position);
                            cmd.Parameters.AddWithValue("@response", r.Response);
                            cmd.Parameters.AddWithValue("@scoreInvalidations", scoreInvalidations);
                            cmd.Parameters.AddWithValue("@batchScoring", batchScoring);
                            cmd.Parameters.AddWithValue("@updateSameReportingVersion", updateSameReportingVersion);
                            cmd.Parameters.AddWithValue("@handscored", handscored);

                            responsesUpdated += Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                //    tran.Commit();
                //}
            }

            return responsesUpdated;
        }

    }
}
