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
using System.Configuration;
using TDSQASystemAPI.TestMerge;
using System.Data.SqlClient;
using System.Data;

namespace TDSQASystemAPI.DAL
{
    /// <summary>
    /// Class to provide access to Test Merge related database config information 
    /// </summary>
    public class TestMergeDataAccess
    {
        /// <summary>
        /// Connection string 
        /// </summary>
        private static string TDSQCConnection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["TDSQC"].ConnectionString;
            }
        }

        static TestMergeDataAccess() { }

        /// <summary>
        /// Get all the configured test merge configurations 
        /// </summary>
        /// <returns></returns>
        public static List<MergeConfig> GetConfiguredMergeConfigs()
        {
            Dictionary<string, MergeConfig> mergeConfigs = new Dictionary<string, MergeConfig>();

            var command = "select CTM.ComponentTestName, CTM.CombinationTestName, CTM.ComponentSegmentName, CTM.CombinationSegmentName, CTFM.ComponentFormKey, CTFM.CombinationFormKey "
                          + "from dbo.CombinationTestMap CTM "
                          + "left join dbo.CombinationTestFormMap CTFM on CTM.ComponentSegmentName = CTFM.ComponentSegmentName";
            using (var adapter = new SqlDataAdapter(command, TDSQCConnection))
            {
                var ds = new DataSet();
                adapter.Fill(ds);
                var tbl = ds.Tables[0];
                foreach (DataRow r in tbl.Rows)
                {
                    string sComponentTestName = r.Field<string>("ComponentTestName");
                    string sCombinationTestName = r.Field<string>("CombinationTestName");
                    string sComponentSegmentName = r.Field<string>("ComponentSegmentName");
                    string sCombinationSegmentName = r.Field<string>("CombinationSegmentName");
                    string sComponentFormKey = r.Field<string>("ComponentFormKey");
                    string sCombinationFormKey = r.Field<string>("CombinationFormKey");
                    
                    MergeConfig mergeConfig = null;
                    if (mergeConfigs.ContainsKey(sCombinationTestName))
                        mergeConfig = mergeConfigs[sCombinationTestName];
                    else 
                    {
                        mergeConfig = new MergeConfig(sCombinationTestName);
                        mergeConfigs.Add(sCombinationTestName, mergeConfig);
                    }                    
                    mergeConfig.AddConfig(sComponentTestName, sComponentSegmentName, sCombinationSegmentName, sComponentFormKey, sCombinationFormKey);
                }
            }
            return mergeConfigs.Values.ToList();
        }

        /// <summary>
        /// Get Combined Test OppID and Key for the given testeekey
        /// </summary>
        /// <param name="combinedTestName"></param>
        /// <param name="testeeKey"></param>
        /// <param name="oppNumber"></param>
        /// <param name="OppId"></param>
        /// <param name="OppKey"></param>
        public static void GetCombinedTestOppId(string combinedTestName, long testeeKey, int oppNumber, out long OppId, out Guid OppKey)
        {
            string storedProcName = "dbo.GetCombinedTestOppId";
            using (var conn = new SqlConnection(TDSQCConnection))
            {
                using (var command = new SqlCommand(storedProcName, conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 120; // 2 minutes
                    var param = command.Parameters.Add("@CombinationTestName", SqlDbType.VarChar, 255);
                    param.Value = combinedTestName;
                    param = command.Parameters.Add("@TesteeKey", SqlDbType.BigInt);
                    param.Value = testeeKey;
                    param = command.Parameters.Add("@OpportunityNumber", SqlDbType.Int);
                    param.Value = oppNumber;
                    param = command.Parameters.Add("@OppId", SqlDbType.BigInt);
                    param.Direction = ParameterDirection.Output;
                    param = command.Parameters.Add("@OppKey", SqlDbType.UniqueIdentifier);
                    param.Direction = ParameterDirection.Output;
                    conn.Open();
                    command.ExecuteNonQuery();
                    OppId = DBNull.Value.Equals(command.Parameters["@OppId"].Value) ? -1 : Convert.ToInt64(command.Parameters["@OppId"].Value.ToString());
                    Guid.TryParse(DBNull.Value.Equals(command.Parameters["@OppKey"].Value) ? "" : command.Parameters["@OppKey"].Value.ToString(), out OppKey);
                }
            }
        }

        /// <summary>
        /// Get latest XML file ID, info for the given testee
        /// </summary>
        /// <param name="nTestee"></param>
        /// <returns></returns>
        public static List<RelatedTestOpportunity> GetLatestXMLFileInfo(long nTestee)
        {
            var xmlFileInfo = new List<RelatedTestOpportunity>();
            var procName = "dbo.GetRelatedFilesForMerge";
            using (var conn = new SqlConnection(TDSQCConnection))
            {
                using (var command = new SqlCommand(procName, conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 120; // 2 minutes
                    var param = command.Parameters.Add("@nTestee", SqlDbType.BigInt);
                    param.Value = nTestee;
                    conn.Open();
                    using (SqlDataReader r = command.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string sTestName = r.IsDBNull(r.GetOrdinal("TestId")) ? "" : r.GetString(r.GetOrdinal("TestId"));
                            int opportunity = r.IsDBNull(r.GetOrdinal("Opportunity")) ? -1 : (int)r.GetInt32(r.GetOrdinal("Opportunity"));
                            long latestFileId = r.IsDBNull(r.GetOrdinal("LatestFileId")) ? -1 : (long)r.GetInt64(r.GetOrdinal("LatestFileId"));
                            string latestStatus = r.IsDBNull(r.GetOrdinal("Status")) ? "" : r.GetString(r.GetOrdinal("Status"));
                            DateTime dateRecorded = r.IsDBNull(r.GetOrdinal("DateRecorded")) ? DateTime.Now : r.GetDateTime(r.GetOrdinal("DateRecorded"));
                            long handScoredFileId = r.IsDBNull(r.GetOrdinal("HandScoredFileId")) ? -1 : (long)r.GetInt64(r.GetOrdinal("HandScoredFileId"));
                            long preAppealFileId = r.IsDBNull(r.GetOrdinal("PreAppealFileId")) ? -1 : (long)r.GetInt64(r.GetOrdinal("PreAppealFileId"));
                            xmlFileInfo.Add(new RelatedTestOpportunity(sTestName, opportunity, latestFileId, latestStatus, dateRecorded, handScoredFileId, preAppealFileId));
                        }
                    }
                }
            }
            return xmlFileInfo;
        }
    }    
}
