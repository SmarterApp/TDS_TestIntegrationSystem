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
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using TDSQASystemAPI.TestResults;
using System.Data.Common;
using System.Data;
using System.Configuration;
using System.IO;
using TDSQASystemAPI.Utilities;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;


namespace TDSQASystemAPI.DAL
{
    public class ArchiveDB
    {
        //private Database _db = null;
        //private Database _db_session = null;

        public ArchiveDB()
        {
            //_db = DatabaseFactory.CreateDatabase("TDSQC");
            //_db_session = DatabaseFactory.CreateDatabase("TDS_Session");
        }

        /// <summary>
        /// Log information about the test opportunity in table TestOpportunityStatus, and
        /// also invoke the QC_AcceptOpportunity stored procedure, which signals TDS_Session that
        /// the opportunity has been succesfully reported.
        /// 
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="fileLocation"></param>
        /// <param name="environment"></param>
        public void SaveResultInfo(TestResult tr, Int64 fileID, bool isDemo, long? dorRecordId, long? archiveFileId, bool sentToRB)
        {
            SaveResultInfo(tr, fileID, isDemo, dorRecordId, archiveFileId, "Test opportunity passed validation.", true, sentToRB);
        }
        /// <summary>
        /// Log information about the test opportunity in table TestOpportunityStatus, and
        /// also invoke the QC_AcceptOpportunity stored procedure, which signals TDS_Session that
        /// the opportunity has been succesfully reported.
        /// 
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="fileLocation"></param>
        /// <param name="environment"></param>
        public void SaveResultInfo(TestResult tr, Int64 fileID, bool isDemo, long? dorRecordId, long? archiveFileId, string message, bool passedValidation, bool sentToRB)
        {
            DataTable testStatusTable = new DataTable();
            int opportunity = -1;
            int? windowOpportunity = null;
            string oppID = string.Empty;
            string testWindowID = string.Empty;

            if (tr.Opportunity != null)
            {
                opportunity = tr.Opportunity.OpportunityNumber;
                oppID = tr.Opportunity.OpportunityID;
                testWindowID = tr.Opportunity.WindowID;
                windowOpportunity = tr.Opportunity.WindowOpportunityNumber;
            }

            int result = TDSInterface.InsertTestOpportunityStatus(tr.Testee.EntityKey,tr.Name,opportunity,oppID, tr.Opportunity.Status,
                                                                    passedValidation, tr.Opportunity.StartDate, tr.Opportunity.StatusDate, tr.Opportunity.CompletedDate,
                                                                    tr.Opportunity.QASystemDateRecorded,message,
                                                                    tr.TestID, fileID, isDemo, testWindowID, windowOpportunity,
                                                                    dorRecordId, tr.Mode, archiveFileId,sentToRB,
                                                                    ConfigurationManager.ConnectionStrings["TDSQC"].ConnectionString);
            
            if (result <= 0)
            {
                Logger.Log(true, "Unable to insert TestOpportunityStatus record for OppID: " + tr.Opportunity.OpportunityID + ", entityKey: " + tr.Testee.EntityKey, EventLogEntryType.Warning, false, true);
            }

            // Paper tests will have an empty opp key.  For these, we don't want to 
            //  call back to TDS.  Just return.
            //commented this section as this is handled in Finally block of QA system.cs
            /*if (tr.Opportunity.Key == Guid.Empty)
                return;

			using (DbCommand cmd = _db_session.GetStoredProcCommand("QC_ACCEPTOPPORTUNITY"))
            {
                _db_session.AddInParameter(cmd, "@opportunityKey", DbType.Guid, tr.Opportunity.Key);
                _db_session.AddInParameter(cmd, "@allRulesPassed", DbType.Boolean, tr.PassedAllValidations());
                _db_session.AddInParameter(cmd, "@fileLocation", DbType.String, fileID);
                try
                {
                    int count = _db_session.ExecuteNonQuery(cmd);
                    if (count == 0)
                    {
                        throw new Exception("No records saved in archive database.");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }*/
        }

        public void SaveResultExceptions(TestResult tr, List<ValidationRecord> vrs, long fileID)
        {
            try
            {
                foreach (ValidationRecord vr in vrs)
                {
                    long entityKey = -1;
                    string testname = "";
                    int opportuntiyNumber = -1;
                    string opportunityID = "";
                    if (tr != null)
                    {
                        
                        if (tr.Testee != null)
                            entityKey = tr.Testee.EntityKey;
                        testname = tr.Name;
                        if (tr.Opportunity != null)
                        {
                            opportuntiyNumber = tr.Opportunity.OpportunityNumber;
                            opportunityID = tr.Opportunity.OpportunityID;
                        }
                    }
                    int count = TDSInterface.QC_RECORDEXCEPTION(entityKey, testname, opportuntiyNumber, opportunityID, vr.Type.ToString(), vr.XPath,
                                                                         vr.Message, vr.RuleID, DateTime.Now, fileID, (int)vr.ResultSeverity, 
                                                                         ConfigurationManager.ConnectionStrings["TDSQC"].ConnectionString);
                    if (count == 0)
                    {
                        throw new Exception("No records saved in archive database.");
                    }
                }
                /*
                using (DbCommand cmd = _db.GetStoredProcCommand("QC_RECORDEXCEPTION"))
                {
                    foreach (ValidationRecord vr in vrs)
                    {
                        cmd.Parameters.Clear();
                        if (tr != null)
                        {
                            if(tr.Testee != null)
							    _db.AddInParameter(cmd, "@TesteeEntityKey", DbType.Int64, tr.Testee.EntityKey);
                            else
                                _db.AddInParameter(cmd, "@TesteeEntityKey", DbType.Int64, -1);

							_db.AddInParameter(cmd, "@TestName", DbType.String, tr.Name);
                            if (tr.Opportunity != null)
                            {
                                _db.AddInParameter(cmd, "@Opportunity", DbType.Int32, tr.Opportunity.OpportunityNumber);
                                _db.AddInParameter(cmd, "@OpportunityID", DbType.String, tr.Opportunity.OpportunityID);
                            }
                            else
                            {
                                _db.AddInParameter(cmd, "@Opportunity", DbType.Int32, -1);
                                _db.AddInParameter(cmd, "@OpportunityID", DbType.String, "");
                            }
                        }
                        else
                        {
							_db.AddInParameter(cmd, "@TesteeEntityKey", DbType.Int64, -1);
							_db.AddInParameter(cmd, "@TestName", DbType.String, "");
                            _db.AddInParameter(cmd, "@Opportunity", DbType.Int32, -1);
							_db.AddInParameter(cmd, "@OpportunityID", DbType.String, "");
                        }
                        _db.AddInParameter(cmd, "@ValidationType", DbType.String, vr.Type.ToString());
                        _db.AddInParameter(cmd, "@XPath", DbType.String, vr.XPath);
                        _db.AddInParameter(cmd, "@Message", DbType.String, vr.Message);
                        _db.AddInParameter(cmd, "@RuleID", DbType.String, vr.RuleID);
                        _db.AddInParameter(cmd, "@DateEntered", DbType.DateTime, DateTime.Now);
                        _db.AddInParameter(cmd, "@FileID", DbType.Int64, fileID);
                        _db.AddInParameter(cmd, "@Severity", DbType.Int32, (int)vr.ResultSeverity);

                        int count = _db.ExecuteNonQuery(cmd);
                        if (count == 0)
						{
                            throw new Exception("No records saved in archive database.");
                        }
                    }
                }*/
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveResultExceptions(TestResult tr, string validationType, string message, long fileID)
        {
            try
            {

                long? entityKey = null;
                string testname = null;
                int? opportunity = null;
                string opportunityid = null;
                if (tr != null)
                {
                    if (tr.Testee != null)
                        entityKey = tr.Testee.EntityKey;
                    else entityKey = -1;
                    testname = tr.Name;
                    if (tr.Opportunity != null)
                    {
                        opportunity = tr.Opportunity.OpportunityNumber;
                        opportunityid = tr.Opportunity.OpportunityID;
                    }
                    else
                    {
                        opportunity = -1;
                        opportunityid = "-1";
                    }
                }
                int count = TDSInterface.QC_RECORDEXCEPTION(entityKey, testname, opportunity, opportunityid, validationType, string.Empty, message, string.Empty,
                                                            DateTime.Now, fileID, (int)Severity.Fatal, ConfigurationManager.ConnectionStrings["TDSQC"].ConnectionString);
                if (count == 0)
                {
                    throw new Exception("No records saved in archive database.");
                }


                /*using (DbCommand cmd = _db.GetStoredProcCommand("QC_RECORDEXCEPTION"))
                {
                    cmd.Parameters.Clear();
                    if (tr != null)
                    {
                        if (tr.Testee != null)
                            _db.AddInParameter(cmd, "@TesteeEntityKey", DbType.Int64, tr.Testee.EntityKey);
                        else
                            _db.AddInParameter(cmd, "@TesteeEntityKey", DbType.Int64, -1);
                        
                        _db.AddInParameter(cmd, "@TestName", DbType.String, tr.Name);
                        if (tr.Opportunity != null)
                        {
                            _db.AddInParameter(cmd, "@Opportunity", DbType.Int32, tr.Opportunity.OpportunityNumber);
                            _db.AddInParameter(cmd, "@OpportunityID", DbType.String, tr.Opportunity.OpportunityID);
                        }
                        else
                        {
                            _db.AddInParameter(cmd, "@Opportunity", DbType.Int32, -1);
                            _db.AddInParameter(cmd, "@OpportunityID", DbType.String, -1);
                        }
                    }
                    _db.AddInParameter(cmd, "@ValidationType", DbType.String, validationType);
                    _db.AddInParameter(cmd, "@XPath", DbType.String, string.Empty);
                    _db.AddInParameter(cmd, "@Message", DbType.String, message);
                    _db.AddInParameter(cmd, "@RuleID", DbType.String, string.Empty);
                    _db.AddInParameter(cmd, "@DateEntered", DbType.DateTime, DateTime.Now);
                    _db.AddInParameter(cmd, "@FileID", DbType.Int64, fileID);
                    _db.AddInParameter(cmd, "@Severity", DbType.Int32, (int)Severity.Fatal);
                    int count = _db.ExecuteNonQuery(cmd);
                    if (count == 0)
                    {
                        throw new Exception("No records saved in archive database.");
                    }
                }*/
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save a validation exception that happened before parsing of the xml was completed.
        /// As there is no Testee information, those values will be set to default values.
        /// </summary>
        /// <param name="validationType"></param>
        /// <param name="message"></param>
        /// <param name="fileLocation"></param>
        public void SaveResultExceptions(string validationType, string message, long fileID)
        {
            try
            {
                int count = TDSInterface.QC_RECORDEXCEPTION(-1, string.Empty, -1, "-1", validationType, string.Empty, message, string.Empty, DateTime.Now,
                                                            fileID, (int)Severity.Fatal, ConfigurationManager.ConnectionStrings["TDSQC"].ConnectionString);
                if (count == 0)
                {
                    throw new Exception("No records saved in archive database.");
                }

                /*using (DbCommand cmd = _db.GetStoredProcCommand("QC_RECORDEXCEPTION"))
                {
                    cmd.Parameters.Clear();
                    _db.AddInParameter(cmd, "@TesteeEntityKey", DbType.Int64, -1);
                    _db.AddInParameter(cmd, "@TestName", DbType.String, string.Empty);
                    _db.AddInParameter(cmd, "@Opportunity", DbType.Int32, -1);
                    _db.AddInParameter(cmd, "@OpportunityID", DbType.String, -1);
                    _db.AddInParameter(cmd, "@ValidationType", DbType.String, validationType);
                    _db.AddInParameter(cmd, "@XPath", DbType.String,string.Empty );
                    _db.AddInParameter(cmd, "@Message", DbType.String, message);
                    _db.AddInParameter(cmd, "@RuleID", DbType.String, string.Empty);
                    _db.AddInParameter(cmd, "@DateEntered", DbType.DateTime, DateTime.Now);
                    _db.AddInParameter(cmd, "@FileName", DbType.Int64, fileID);
                    _db.AddInParameter(cmd, "@Severity", DbType.Int32, (int)Severity.Fatal);
                    int count = _db.ExecuteNonQuery(cmd);
                if (count == 0)
                {
                    throw new Exception("No records saved in archive database.");
                }
            }*/
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime GetPrevAttemptCompletedDate(TestResult tr)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["TDSQC"].ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("QC_RECORDEXCEPTION"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@studentKey", tr.Testee.EntityKey);
                    command.Parameters.AddWithValue("@testname", tr.Name);
                    command.Parameters.AddWithValue("@oppNum", tr.Opportunity.OpportunityNumber);
                    SqlParameter startparam = command.Parameters.Add("@datestarted", SqlDbType.DateTime);
                    startparam.Direction = ParameterDirection.Output;
                    SqlParameter completedparam = command.Parameters.Add("@dateCompleted", SqlDbType.DateTime);
                    completedparam.Direction = ParameterDirection.Output;
                    try
                    {
                        conn.Open();
                        command.ExecuteNonQuery();
                        if (command.Parameters["@dateCompleted"].Value != DBNull.Value)
                        {
                            return Convert.ToDateTime(command.Parameters["@dateCompleted"].Value);
                        }
                        return DateTime.MinValue;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            } 
        }
    }
}