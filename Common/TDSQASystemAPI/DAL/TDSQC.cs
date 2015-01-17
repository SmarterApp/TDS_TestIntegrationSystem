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
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace TDSQASystemAPI.DAL
{
    public class TDSQC
    {
        static TDSQC() { }

        /// <summary>
        /// Get a DB safe value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isEmptyStringNull">If this is a string variable and we consider empty strings null this should be true</param>
        /// <returns></returns>
        private static object GetValue(object value, bool isEmptyStringNull = false)
        {
            // if the value is null we return DBNull.Value
            if (value == null || (isEmptyStringNull && String.IsNullOrEmpty(value.ToString())))
            {
                return DBNull.Value;
            }
            return value;
        }
       
        public static DataTable GetTestOpportunityStatus(DateTime? dateRecorded, long? testeeEntityKey, string testName, int? opportunity, string oppID,
                                                         string status, bool? passedValidation, bool? sentToRB, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetTestOpportunityStatus", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DateRecorded", GetValue(dateRecorded));
                    command.Parameters.AddWithValue("@TesteeEntityKey", GetValue(testeeEntityKey));
                    command.Parameters.AddWithValue("@TestName", GetValue(testName));
                    command.Parameters.AddWithValue("@Opportunity", GetValue(opportunity));
                    command.Parameters.AddWithValue("@OppID", GetValue(oppID));
                    command.Parameters.AddWithValue("@Status", GetValue(status));
                    command.Parameters.AddWithValue("@PassedQAValidation", GetValue(passedValidation));
                    command.Parameters.AddWithValue("@SentToRB", GetValue(sentToRB));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }

        public static DataTable GetQC_ValidationExceptions(long fileId, DateTime? startDateTime, int severity, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetQC_ValidationExceptions", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@fileID", GetValue(fileId));
                    command.Parameters.AddWithValue("@startDateTime", GetValue(startDateTime));
                    command.Parameters.AddWithValue("@minimumSeverity", GetValue(severity));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }

        public static DataSet GetWarningsSummary(DateTime startDateTime, int? severity, bool getDetails, string serviceName, string connectionString)
        {
            if (severity == null)
                severity = 100; // default to 100 - warnings

            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetWarningsSummary", conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@StartDate", GetValue(startDateTime));
                        command.Parameters.AddWithValue("@Severity", GetValue(severity));
                        command.Parameters.AddWithValue("@GetDetails", GetValue(getDetails));
                        command.Parameters.AddWithValue("@InstanceName", GetValue(serviceName));
                        conn.Open();
                        adapter.Fill(ds);
                        ds.Tables[0].TableName = "Summary";
                        if (getDetails)
                            ds.Tables[1].TableName = "Details";
                    }
                }
            }
            return ds;
        }

        public static int InsertTestOpportunityStatus(long entityKey, string testname, int opportunity, string oppID, string status, bool passedQAValidation,
                                                      DateTime startDate, DateTime statusDate, DateTime? completedDate, DateTime datePassedValidation,
                                                      string message, string testID, long fileID, bool isDemo, string testWindowID, int? windowOpportunity,
                                                      long? dorRecordID, string mode, long? archiveFileId, bool sentToRB, string connectionString)
        {
            int result = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertTestOpportunityStatus", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TesteeEntityKey", GetValue(entityKey));
                    command.Parameters.AddWithValue("@TestName", GetValue(testname));
                    command.Parameters.AddWithValue("@Opportunity", GetValue(opportunity));
                    command.Parameters.AddWithValue("@OppID", GetValue(oppID));
                    command.Parameters.AddWithValue("@Status", GetValue(status));
                    command.Parameters.AddWithValue("@PassedQAValidation", GetValue(passedQAValidation));
                    command.Parameters.AddWithValue("@OpportunityStartDate", GetValue(startDate));
                    command.Parameters.AddWithValue("@OpportunityStatusDate", GetValue(statusDate));
                    command.Parameters.AddWithValue("@OpportunityDateCompleted", GetValue(completedDate));
                    SqlParameter p = command.Parameters.AddWithValue("@DatePassedValidation", GetValue(datePassedValidation));
                    p.SqlDbType = SqlDbType.DateTime2;
                    command.Parameters.AddWithValue("@Message", GetValue(message));
                    command.Parameters.AddWithValue("@TestID", GetValue(testID));
                    command.Parameters.AddWithValue("@FileID", GetValue(fileID));
                    command.Parameters.AddWithValue("@isDemo", GetValue(isDemo));
                    command.Parameters.AddWithValue("@TestWindowID", GetValue(testWindowID));
                    command.Parameters.AddWithValue("@WindowOpportunity", GetValue(windowOpportunity));
                    command.Parameters.AddWithValue("@DoRRecordID", GetValue(dorRecordID));
                    command.Parameters.AddWithValue("@Mode", GetValue(mode));
                    if(archiveFileId != null)
                        command.Parameters.AddWithValue("@ArchiveFileID", archiveFileId);
                    command.Parameters.AddWithValue("@SentToRB", GetValue(sentToRB));
                    conn.Open();
                    result = command.ExecuteNonQuery();
                }
            }
            return result;
        }

        public static int QC_RECORDEXCEPTION(long? entityKey, string testname, int? opportunity, string opportunityID, string validationType, string xpath,
                                             string message, string ruleID, DateTime dateEntered, long fileID, int severity, string connectionString)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("QC_RECORDEXCEPTION", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TesteeEntityKey", GetValue(entityKey));
                    command.Parameters.AddWithValue("@TestName", GetValue(testname));
                    command.Parameters.AddWithValue("@Opportunity", GetValue(opportunity));
                    command.Parameters.AddWithValue("@OpportunityID", GetValue(opportunityID));
                    command.Parameters.AddWithValue("@ValidationType", GetValue(validationType));
                    command.Parameters.AddWithValue("@XPath", GetValue(xpath));
                    command.Parameters.AddWithValue("@Message", GetValue(message));
                    command.Parameters.AddWithValue("@RuleID", GetValue(ruleID));
                    command.Parameters.AddWithValue("@DateEntered", GetValue(dateEntered));
                    command.Parameters.AddWithValue("@FileID", GetValue(fileID));
                    command.Parameters.AddWithValue("@Severity", GetValue(severity));
                    conn.Open();
                    count = command.ExecuteNonQuery();
                }
            }
            return count;
        }
    }
}
