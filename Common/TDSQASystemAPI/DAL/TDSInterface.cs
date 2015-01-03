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
    public class TDSInterface
    {
        static TDSInterface() { }

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
       
        public static DataTable GetTestDataByRTSKey(long studentRTSKey, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetTestDataByRTSKey", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentRTSKey", GetValue(studentRTSKey));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }
        
        public static Boolean ValidateOppID(string oppid, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("ValidateOppID", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OppID", GetValue(oppid));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            if (table.Rows.Count > 0)
                return Convert.ToBoolean(table.Rows[0][0]);
            return false;
        }
        public static DataTable GetTestDataByOppID(string oppid, bool gethistory, String clientid, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetTestDataByOppID", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OppID", GetValue(oppid));
                    command.Parameters.AddWithValue("@GetHistory", GetValue(gethistory));
                    command.Parameters.AddWithValue("@ClientID", GetValue(clientid));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
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
        public static DataTable GetQASystemServices(int? serviceid, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetQASystemServices", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ServiceID", GetValue(serviceid));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }
        public static void InsertTestNameLookup(string instanceName, string commaSepTestNames, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertTestNameLookup", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InstanceName", GetValue(instanceName));
                    command.Parameters.AddWithValue("@CommaSepTestNames", GetValue(commaSepTestNames));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public static DataTable GetTestNameLookup(string instanceName, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetTestNameLookup", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InstanceName", GetValue(instanceName));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }
        /// <summary>
        /// This function returns the RuleIDs and parameters for the specified project ID
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public static DataTable GetQCRulesTable(int projectID, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetQCRules", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", GetValue(projectID));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }

        /// <summary>
        /// This function returns the table of all possible rules, updated with the parameter values for the rules that the specified project ID has.
        /// This is used in QCRules.aspx.cs in the DoRWebsite project.
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public static DataTable GetAllQCRulesWithParams(int projectID, string connectionString)
        {
            DataTable AllRulesTable = new DataTable();
            DataTable projectRulesTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("GetQCRules", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", -1); // -1 is the projectID that has all possible rules
                    //get a table that has all the possible rules in it
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        AllRulesTable.Load(reader);
                    }
                }
                using (SqlCommand command = new SqlCommand("GetQCRules", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", GetValue(projectID));
                    //get a table that has the rules for this project in it, with the parameter values for that project
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        projectRulesTable.Load(reader);
                    }
                }
            }

            //go through the table with ALL POSSIBLE rules, and fill in the parameters from the project specified
            foreach (DataRow row in projectRulesTable.Rows)
            {
                int rowindex = FindRowIndexByRuleID(row["RuleID"].ToString(), AllRulesTable);
                AllRulesTable.Rows[rowindex]["Parm1"] = row["Parm1"];
                AllRulesTable.Rows[rowindex]["Parm2"] = row["Parm2"];
                AllRulesTable.Rows[rowindex]["Parm3"] = row["Parm3"];
                AllRulesTable.Rows[rowindex]["Parm4"] = row["Parm4"];
                AllRulesTable.Rows[rowindex]["Parm5"] = row["Parm5"];
            }
            return AllRulesTable;
        }

        public static int UpdateQCRule(int projectID, string ruleID, string parm1, string parm2, string parm3, string parm4, string parm5,
                                       string connectionString)
        {
            int returnval = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("UpdateQCRule", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", GetValue(projectID));
                    command.Parameters.AddWithValue("@RuleID", GetValue(ruleID));
                    command.Parameters.AddWithValue("@Parm1", GetValue(parm1));
                    command.Parameters.AddWithValue("@Parm2", GetValue(parm2));
                    command.Parameters.AddWithValue("@Parm3", GetValue(parm3));
                    command.Parameters.AddWithValue("@Parm4", GetValue(parm4));
                    command.Parameters.AddWithValue("@Parm5", GetValue(parm5));
                    conn.Open();
                    returnval = command.ExecuteNonQuery();
                }
            }
            return returnval;
        }
        public static int InsertQCRule(int projectID, string ruleID, string ruleDescription, string parm1, string parm2, string parm3,
                                       string parm4, string parm5, string connectionString)
        {
            int returnval = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertQCRule", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", GetValue(projectID));
                    command.Parameters.AddWithValue("@RuleID", GetValue(ruleID));
                    command.Parameters.AddWithValue("@FcnDesc", GetValue(ruleDescription));
                    command.Parameters.AddWithValue("@Parm1", GetValue(parm1));
                    command.Parameters.AddWithValue("@Parm2", GetValue(parm2));
                    command.Parameters.AddWithValue("@Parm3", GetValue(parm3));
                    command.Parameters.AddWithValue("@Parm4", GetValue(parm4));
                    command.Parameters.AddWithValue("@Parm5", GetValue(parm5));
                    conn.Open();
                    returnval = command.ExecuteNonQuery();
                }
            }
            return returnval;
        }
        public static int DeleteQCRule(int? projectID, string ruleID, string connectionString)
        {
            int returnval = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("DeleteQCRule", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", GetValue(projectID));
                    command.Parameters.AddWithValue("@RuleID", GetValue(ruleID));
                    conn.Open();
                    returnval = command.ExecuteNonQuery();
                }
            }
            return returnval;
        }
        public static int InsertPAIRSLog(DateTime dateReceived, DateTime dateCompleted, string requestType, string message, string details,
                                         DateTime? dateResolved, bool fromDFG, string connectionString)
        {
            int requestID = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertPAIRSLog", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DateReceived", GetValue(dateReceived));
                    command.Parameters.AddWithValue("@DateCompleted", GetValue(dateCompleted));
                    command.Parameters.AddWithValue("@RequestType", GetValue(requestType));
                    command.Parameters.AddWithValue("@Message", GetValue(message));
                    command.Parameters.AddWithValue("@Details", GetValue(details));
                    if(dateResolved != null)
                        command.Parameters.AddWithValue("@DateResolved", dateResolved);
                    command.Parameters.AddWithValue("@FromDFG", GetValue(fromDFG));
                    conn.Open();
                    requestID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return requestID;
        }
        // DFG sets dateReceived and dateCompleted to the same date, so I'm using this overload.
        //TODO: revisit completed date.  It's not nullable, so it's always set when dateReceived is set.
        //  are we getting any value from this?
        public static int InsertPAIRSLog(DateTime logDate, string requestType, string message, string details,
                                         DateTime? dateResolved, bool fromDFG, string connectionString)
        {
            return InsertPAIRSLog(logDate, logDate, requestType, message, details, dateResolved, fromDFG, connectionString);
        }
        public static int InsertPAIRSParameter(int requestID, int sequence, string value, string connectionString)
        {
            int numrows = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertPAIRSParameter", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RequestID", GetValue(requestID));
                    command.Parameters.AddWithValue("@Sequence", sequence);
                    command.Parameters.AddWithValue("@Value", GetValue(value));
                    conn.Open();
                    numrows = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return numrows;
        }
        public static DataTable GetPAIRSParameters(int requestID, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetPAIRSParameters", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RequestID", GetValue(requestID));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }
        public static DataTable GetOutstandingPdfFiles(string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetOutstandingPdfFiles", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }
        public static void ResolveOutstandingPdfFile(int requestID, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("ResolveOutstandingPdfFile", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RequestID", GetValue(requestID));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
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

        public static void InsertHandscoringExtractLog(long extractID, string message, string description, bool isError, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertHandscoringExtractLog", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExtractID", GetValue(extractID));
                    command.Parameters.AddWithValue("@Message", GetValue(message));
                    command.Parameters.AddWithValue("@Description", GetValue(description));
                    command.Parameters.AddWithValue("@IsError", GetValue(isError));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static int InsertTISStatusLog(long TISRequestID, string message, string description, bool isError, string connectionString)
        {
            int logID = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertTISStatusLog", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TISRequestID", GetValue(TISRequestID));
                    command.Parameters.AddWithValue("@Message", GetValue(message));
                    command.Parameters.AddWithValue("@Description", GetValue(description));
                    command.Parameters.AddWithValue("@IsError", GetValue(isError));
                    conn.Open();
                    logID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return logID;
        }
        public static DataTable GetTISStatusLogs(long extractID, bool isError, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetTISStatusLogs", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@HandscoringExtractID", GetValue(extractID));
                    command.Parameters.AddWithValue("@IsError", GetValue(isError));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }

        public static DataTable GetInitialHandscoringExtractByExternalID(int externalExtractID, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetInitialHandscoringExtractByExternalID", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExternalExtractID", GetValue(externalExtractID));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }
        public static DataTable GetMostRecentPaperTestResolutionStatus(string barcode, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetMostRecentPaperTestResolutionStatus", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Barcode", GetValue(barcode));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }

        public static long InsertHandscoringExtract(int extractID, int projectID, bool passedValidation, string path, string connectionString)
        {
            long id = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertHandscoringExtract", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExternalExtractID", GetValue(extractID));
                    command.Parameters.AddWithValue("@ExternalProjectID", GetValue(projectID));
                    command.Parameters.AddWithValue("@PassedValidation", GetValue(passedValidation));
                    command.Parameters.AddWithValue("@Path", GetValue(path));
                    conn.Open();
                    id = Convert.ToInt64(command.ExecuteScalar());
                }
            }
            return id;
        }

        public static void UpdateHandscoringExtractExamCount(long extractID, int examCount, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("UpdateHandscoringExtractExamCount", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExtractID", GetValue(extractID));
                    command.Parameters.AddWithValue("@ExamCount", GetValue(examCount));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static int InsertTISRequest(string oppID, long entityKey, string testname, DateTime dateRecorded, long extractID, string handscoringExamID,
                                            string connectionString)
        {
            int id = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertTISRequest", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OppID", GetValue(oppID));
                    command.Parameters.AddWithValue("@TesteeEntityKey", GetValue(entityKey));
                    command.Parameters.AddWithValue("@TestName", GetValue(testname));
                    command.Parameters.AddWithValue("@DateRecorded", GetValue(dateRecorded));
                    command.Parameters.AddWithValue("@ExtractID", GetValue(extractID));
                    command.Parameters.AddWithValue("@HandscoringExamID", GetValue(handscoringExamID));
                    conn.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return id;
        }
        public static void UpdateHandscoringExtract(long extractID, bool passedValidation, int examCount, int? testCount, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("UpdateHandscoringExtract", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExtractID", GetValue(extractID));
                    command.Parameters.AddWithValue("@PassedValidation", GetValue(passedValidation));
                    command.Parameters.AddWithValue("@ProcessedExamCount", GetValue(examCount));
                    command.Parameters.AddWithValue("@BlankPaperTestCount", GetValue(testCount));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public static DataTable GetTestOpportunitiesInHandscoring(long? entityKey, string testName, int? opportunity, string oppID, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetTestOpportunitiesInHandscoring", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TesteeEntityKey", GetValue(entityKey));
                    command.Parameters.AddWithValue("@TestName", GetValue(testName));
                    command.Parameters.AddWithValue("@Opportunity", GetValue(opportunity));
                    command.Parameters.AddWithValue("@OppID", GetValue(oppID));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }
        public static DataTable GetHandscoringExtracts(long extractID, bool? passedValidation, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetHandscoringExtracts", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExtractID", GetValue(extractID));
                    command.Parameters.AddWithValue("@PassedValidation", GetValue(passedValidation));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }

        public static void InsertProjectID(int handScoringProjectID,string projectDescription, string commaSepTestNames, string commaSepStatuses, string qaLevel,
                                           bool updateRB, bool updateDoR, int dorAdminID, bool sendToHandScoring, bool sendToTIS, bool scoreInvalidations,
                                           bool? updateAppealStatus, bool mergeItemScores, bool accommodationsRB, bool accommodationsDoR, 
                                           bool accommodationsHS, bool accommodationsNone, string connectionString)
        {
            string qalevel = null;
            if (!string.IsNullOrEmpty(qaLevel)) // consider empty string null for QALevel
                qalevel = qaLevel;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertProjectID", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@HandScoringProjectID", GetValue(handScoringProjectID));
                    command.Parameters.AddWithValue("@ProjectDescription", GetValue(projectDescription));
                    command.Parameters.AddWithValue("@CommaSeparatedTestNames", GetValue(commaSepTestNames));
                    command.Parameters.AddWithValue("@CommaDelimitedStatuses", GetValue(commaSepStatuses));
                    command.Parameters.AddWithValue("@QALevel", GetValue(qalevel));
                    command.Parameters.AddWithValue("@UpdateRB", GetValue(updateRB));
                    command.Parameters.AddWithValue("@UpdateDoR", GetValue(updateDoR));
                    command.Parameters.AddWithValue("@DoRAdmin", GetValue(dorAdminID));
                    command.Parameters.AddWithValue("@SendToHandScoring", GetValue(sendToHandScoring));
                    command.Parameters.AddWithValue("@SendToTIS", GetValue(sendToTIS));
                    command.Parameters.AddWithValue("@ScoreInvalidations", GetValue(scoreInvalidations));
                    if (updateAppealStatus != null) // this is a newly added optional parameter, so don't send it unless we have to
                        command.Parameters.AddWithValue("@UpdateAppealStatus", GetValue(updateAppealStatus));
                    command.Parameters.AddWithValue("@MergeItemScores", GetValue(mergeItemScores));
                    command.Parameters.AddWithValue("@AccommodationsRB", GetValue(accommodationsRB));
                    command.Parameters.AddWithValue("@AccommodationsDoR", GetValue(accommodationsDoR));
                    command.Parameters.AddWithValue("@AccommodationsHS", GetValue(accommodationsHS));
                    command.Parameters.AddWithValue("@AccommodationsNone", GetValue(accommodationsNone));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void DeleteProjectID(int projectID, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("DeleteProjectID", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", GetValue(projectID));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void MapToProjectAllStatuses(int handscoringProjectID, string testID, string projectDescription, string commaDelimStatuses,
                                                    string qaLevel, bool useTestFormat, bool useDiscrep, bool deleteOnly, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("MapToProjectAllStatuses", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@HandscoringProjectId", GetValue(handscoringProjectID));
                    command.Parameters.AddWithValue("@TestId", GetValue(testID));
                    command.Parameters.AddWithValue("@ProjectDescription", GetValue(projectDescription));
                    command.Parameters.AddWithValue("@CommaDelimitedStatuses", GetValue(commaDelimStatuses));
                    command.Parameters.AddWithValue("@QALevel", GetValue(qaLevel,true));
                    command.Parameters.AddWithValue("@UseTestFormat", GetValue(useTestFormat));
                    command.Parameters.AddWithValue("@UseDiscrep", GetValue(useDiscrep));
                    command.Parameters.AddWithValue("@DeleteOnly", GetValue(deleteOnly));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void ProjectToDo(string projectDescription,  bool updateRB, bool updateDoR,int dorAdminID, bool sendToHandScoring, 
                                        bool sendToTIS, bool scoreInvalidations, bool? updateAppealStatus, bool mergeItemScores,
                                        bool accommodationsRB, bool accommodationsDoR, bool accommodationsHS, bool accommodationsNone, 
                                        string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("ProjectToDo", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectDescription", GetValue(projectDescription));
                    command.Parameters.AddWithValue("@UpdateRB", GetValue(updateRB));
                    command.Parameters.AddWithValue("@UpdateDoR", GetValue(updateDoR));
                    command.Parameters.AddWithValue("@DoRAdmin", GetValue(dorAdminID));
                    command.Parameters.AddWithValue("@SendToHS", GetValue(sendToHandScoring));
                    command.Parameters.AddWithValue("@SendToTIS", GetValue(sendToTIS));
                    command.Parameters.AddWithValue("@ScoreInvalidations", GetValue(scoreInvalidations));
                    if (updateAppealStatus != null) // this is a newly added optional parameter, so don't send it unless we have to
                        command.Parameters.AddWithValue("@UpdateAppealStatus", GetValue(updateAppealStatus));
                    command.Parameters.AddWithValue("@MergeItemScores", GetValue(mergeItemScores));
                    command.Parameters.AddWithValue("@AccommodationsRB", GetValue(accommodationsRB));
                    command.Parameters.AddWithValue("@AccommodationsDoR", GetValue(accommodationsDoR));
                    command.Parameters.AddWithValue("@AccommodationsHS", GetValue(accommodationsHS));
                    command.Parameters.AddWithValue("@AccommodationsNone", GetValue(accommodationsNone));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void ProjectToDoHelper(int projectID, string groupName, string varName, int? value, string valueString, string description, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("ProjectToDoHelper", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", GetValue(projectID));
                    command.Parameters.AddWithValue("@GroupName", GetValue(groupName));
                    command.Parameters.AddWithValue("@VarName", GetValue(varName));
                    command.Parameters.AddWithValue("@Value", GetValue(value));
                    command.Parameters.AddWithValue("@ValueString", GetValue(valueString));
                    command.Parameters.AddWithValue("@Desc", GetValue(description));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteProjectMetadataEntry(int projectID, string groupName, string varName, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("DeleteProjectMetadataEntry", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", GetValue(projectID));
                    command.Parameters.AddWithValue("@GroupName", GetValue(groupName));
                    command.Parameters.AddWithValue("@VarName", GetValue(varName));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static DataTable GetQCProjectMetaDataFlags(int projectID, string connectionString)
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetQCProjectMetaDataFlags", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", GetValue(projectID));
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }

        public static void InsertRTSAttribute(int projectID, string groupName, string context, DateTime? contextDate, bool decrypt, string xmlName,
                                                   string entityType, string relationship, string fieldName, bool fetchIfNotInXml, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertRTSAttribute", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", GetValue(projectID));
                    command.Parameters.AddWithValue("@GroupName", GetValue(groupName));
                    command.Parameters.AddWithValue("@Context", GetValue(context));
                    command.Parameters.AddWithValue("@ContextDate", GetValue(contextDate, true));
                    command.Parameters.AddWithValue("@Decrypt", GetValue(decrypt));
                    command.Parameters.AddWithValue("@XMLName", GetValue(xmlName));
                    command.Parameters.AddWithValue("@EntityType", GetValue(entityType));
                    command.Parameters.AddWithValue("@Relationship", GetValue(relationship));
                    command.Parameters.AddWithValue("@FieldName", GetValue(fieldName));
                    command.Parameters.AddWithValue("@FetchIfNotInXml", GetValue(fetchIfNotInXml));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteRTSAttribute(int projectID, string groupName, string context, string entityType, string relationship, string fieldName, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("DeleteRTSAttribute", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", GetValue(projectID));
                    command.Parameters.AddWithValue("@GroupName", GetValue(groupName));
                    command.Parameters.AddWithValue("@Context", GetValue(context));
                    command.Parameters.AddWithValue("@EntityType", GetValue(entityType));
                    command.Parameters.AddWithValue("@Relationship", GetValue(relationship));
                    command.Parameters.AddWithValue("@FieldName", GetValue(fieldName));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void CopyQCRules(int projectIDTo, int projectIDFrom, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("CopyQCRules", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectIDTo", GetValue(projectIDTo));
                    command.Parameters.AddWithValue("@ProjectIDFrom", GetValue(projectIDFrom));
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Parse the VarName strings in the QC_ProjectMetadata table and return the distinct list
        /// of qaLevels for ProjectID provided
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<String> GetQALevels(int projectID, string connectionString)
        {
            List<String> qaLevels = new List<string>();

            //Complicated regex for parsing the QC_ProjectMetadata Varname string.
            //The string is of the form {HSProjectID}-{TestName}-{Optional TestMode}-{Optional QALevel}-{Status}
            //Obviously the brackets {} are not in the actual string. This regex will pull out the "{Optional QALevel}" part of the string.
            // "?<=" is a positive lookbehind
            // "-\\d{4}-\\d{4}" matches the years in a test name, i.e. "-2013-2014"
            // "?!" is a negative lookahead
            // "?=" is a positive lookahead
            //reference for lookbehind/lookahead: http://www.regular-expressions.info/lookaround.html
            Regex pattern = new Regex("(?<=-\\d{4}-\\d{4}(-(scanned|paper|online|SCANNED|PAPER|ONLINE))?-)((?!scanned|paper|online|SCANNED|PAPER|ONLINE)[^-]+)+(?=-{1})");

            DataTable table = GetQCProjectMetaDataFlags(projectID, connectionString);
            foreach (DataRow row in table.Rows)
            {
                Match match = pattern.Match(row["VarName"].ToString());
                if (match.Success)
                {
                    if (!qaLevels.Contains(match.Value))
                        qaLevels.Add(match.Value);
                }
            }
            return qaLevels;
        }

        #region helper methods
        private static int FindRowIndexByRuleID(string ruleID, DataTable tableToSearch)
        {
            for (int i = 0; i < tableToSearch.Rows.Count; i++)
            {
                if (tableToSearch.Rows[i]["RuleID"].ToString() == ruleID)
                    return i;
            }
            //rule ID not found
            return -1;
        }
        #endregion
    }
}
