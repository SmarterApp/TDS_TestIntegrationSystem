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
using System.Threading;
using System.Linq;
using TDSQASystemAPI.BL;
using TDSQASystemAPI.Config;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.TestResults;
using System.Diagnostics;
using System.IO;
using TDSQASystemAPI.Utilities;
using System.Globalization;
using TDSQASystemAPI.ExceptionHandling;
using System.Net;
using System.Net.Security;
using System.Configuration;
using TDSQASystemAPI.TIS;
using TDSQASystemAPI.Data;
using ScoringEngine;
using ScoringEngine.Scoring;
using ScoringEngine.ConfiguredTests;
using System.Xml;
using System.Data;
using TDSQASystemAPI.TestMerge;
using TDSQASystemAPI.Routing;
using TDSQASystemAPI.Routing.ItemScoring;
using TDSQASystemAPI.Acknowledgement;
using AIR.Common;
using TDSQASystemAPI.Extensions;

namespace TDSQASystemAPI
{
    public class QASystem
    {
        private string _TDSQCConnectionString = null;
        public string TDSQCConnectionString
        {
            get
            {
                return _TDSQCConnectionString;
            }
        }


        public string ItemBankConnectionString { get; private set; }

        //TODO: move into XmlRepositoryItem?
        public enum ArchiveStrategy { None = 0, UpdateOriginalFile = 1, ArchiveAndInsert = 2 }
        private ArchiveStrategy archiveStrategy = ArchiveStrategy.None;
        public bool SetArchiveStrategy(ArchiveStrategy newArchiveStrategy)
        {
            bool strategyChanged = false;
            // if it's not set or if the current value is lower
            //  than the new value, then set it.  Otherwise, leave it alone.
            if ((int)archiveStrategy < (int)newArchiveStrategy)
            {
                archiveStrategy = newArchiveStrategy;
                strategyChanged = true;
            }
            return strategyChanged;
        }

        public enum QAResult { Unknown = 0, Success = 1, FailValidation = 2, FailUpdate = 3 };
        public string dbHandleConfig { get; private set; }
        public string dbHandleResults { get; private set; }
        private TestCollection _tc;
        public TestCollection TC { get { return _tc; } }

        /// <summary>
        /// Instance of xmlRepositoryBL class
        /// </summary>
        private BL.XmlRepository xmlRepositoryBL;

        public QASystem(string dbHandleConfig, string dbHandleResults)
        {
            ItemBankConnectionString = null;
            this.dbHandleConfig = dbHandleConfig;
            this.dbHandleResults = dbHandleResults;
            try
            {
                _tc = ServiceLocator.Resolve<ConfigurationHolder>().TestCollection(dbHandleConfig);
            }
            catch (Exception e)
            {
                Logger.Log(true, "Failed to initialize the TestCollection due to: " + e.Message, EventLogEntryType.Error, false, true);
                throw new Exception("Failed to initialize the TestCollection due to: " + e.Message, e);
            }
            try
            {
                _TDSQCConnectionString = ConfigurationManager.ConnectionStrings[dbHandleConfig].ToString();
            }
            catch (Exception e)
            {
                Logger.Log(true, "Failed to initialize the TDS QC Connection string due to: " + e.Message, EventLogEntryType.Error, false, true);
                throw new Exception("Failed to initialize the TDS QC Connection string due to: " + e.Message, e);
            }

            try
            {
                ItemBankConnectionString = ConfigurationManager.ConnectionStrings["ITEMBANK"].ToString();
            }
            catch (Exception e)
            {
                Logger.Log(true, "Failed to initialize ItemBank Connection due to: " + e.Message, EventLogEntryType.Error, false, true);
                throw new Exception("Failed to initialize ItemBank Connection due to: " + e.Message, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="senderType"></param>
        /// <param name="fileID"></param>
        /// <param name="updateRB"></param>
        /// <param name="updateDoR"></param>
        /// <param name="sendToHandScoring"></param>
        /// <param name="ignoreHandscoringDuplicates"></param>
        /// <param name="emailAlertForWarnings"></param>
        /// <returns></returns>
        public QAResult ReceiveTestResult(XmlDocument xml, XmlRepositoryItem xmlRepoItem)
        {
            DateTime startDateTime = DateTime.Now;
            QAResult result = QAResult.Success;
            ArchiveDB ar = new ArchiveDB();
            TestResult tr = null;
            bool isFatal = false;
            bool isValid = false;
            XMLAdapter adapter;
            SendToModifiers sendToModifiers = null;
            long? archivedFileID = null;
            long? dorRecordId = null;
            // these config settings may be overridden or modified below for this test.
            ProjectMetaData projectMetaData = new ProjectMetaData();
            ITISExtender tisExtender = ServiceLocator.Resolve<ITISExtender>() ?? new NullTISExtender();
            ITISExtenderState tisExtenderState = tisExtender.CreateStateContainer();
            ITestResultSerializerFactory serializerFactory = ServiceLocator.Resolve<ITestResultSerializerFactory>();
            if (serializerFactory == null)
                throw new ApplicationException("Must register an ITestResultSerializerFactory with the ServiceLocator.");

            try
            {
                xmlRepositoryBL = new BL.XmlRepository(QASystemConfigSettings.Instance.LongDbCommandTimeout);
                adapter = serializerFactory.CreateDeserializer(xml);
                tr = adapter.CreateTestResult(_tc, out isValid, true);
                if (tr != null)
                {
                    // set the project metadata based on the current project (set in the parser)
                    projectMetaData.SetProjectMetaData(tr.ProjectID);

                    // if the opp is null, we can't process the file
                    if (tr.Opportunity == null)
                    {
                        result = QAResult.FailValidation;
                        ar.SaveResultExceptions(tr, adapter.ValidationRecords, xmlRepoItem.FileID);
                        return result;
                    }

                    //merge item scores from outside vendor if necessary
                    bool qaProjectChanged = false;
                    if (projectMetaData.mergeItemScores && adapter.MergeScores(tr, out qaProjectChanged))
                        SetArchiveStrategy(ArchiveStrategy.ArchiveAndInsert);

                    // if the project changed, refresh the metadata
                    if (qaProjectChanged)
                        projectMetaData.SetProjectMetaData(tr.ProjectID);

                    //PRESCORE:
                    tisExtender.PreScore(this, tr, xmlRepoItem, projectMetaData, tisExtenderState);
                    
                    //SCORE:
                    if (tisExtender.ShouldScore(this, adapter, tr, projectMetaData, tisExtenderState))
                    {
                        if (tr.AddScores(_tc))
                            SetArchiveStrategy(ArchiveStrategy.ArchiveAndInsert);
                    }

                    //COMPLETENESS STATUS:
                    tr.Opportunity.Completeness = !tr.IsComplete() ? "Partial" : "Complete";

                    // Validation section. This checks the xml file against several business rules.
                    // If validation fails, the file is moved to a failed files directory.

                    List<ValidationRecord> vrs = new List<ValidationRecord>();

                    tr.ValidationRecords = adapter.ValidationRecords;
                    if (tr.ValidationRecords.Count == 0)
                    {
                        //POSTSCORE:
                        tisExtender.PostScore(this, tr, xmlRepoItem, projectMetaData, tisExtenderState);

                        // if the startdate is null in the file, it will be set to the statusDate.
                        //  Make sure we archive it so that we have a record of the unmodified file.
                        if (tr.Opportunity.OriginalStartDate == DateTime.MinValue)
                            SetArchiveStrategy(ArchiveStrategy.ArchiveAndInsert);

                        switch (archiveStrategy)
                        {
                            case ArchiveStrategy.ArchiveAndInsert:
                                //Change the status of old XML file to Arcvhive
                                //insert the new XML file with status as Pprocessing
                                //get the new File id
                                archivedFileID = xmlRepoItem.FileID;
                                xmlRepoItem.FileID = xmlRepositoryBL.InsertAndArchiveXML(xmlRepoItem.FileID, BL.XmlRepository.Location.processing, tr.ToXml(serializerFactory));
                                break;
                            case ArchiveStrategy.UpdateOriginalFile:
                                // just update the contents of the existing file w/o archiving
                                xmlRepositoryBL.UpdateFileContent(xmlRepoItem.FileID, tr.ToXml(serializerFactory));
                                break;
                        }

                        vrs = tisExtender.Validate(this, tr, xmlRepoItem, projectMetaData, tisExtenderState, out isFatal, out sendToModifiers);

                        tr.Acknowledged = true;

                        if (((tr.Testee.EntityKey < 0 || tr.Opportunity.OpportunityNumber < 0) && !tr.Opportunity.IsDiscrepancy)
                            || tr.Opportunity.StatusDate == null
                            || tr.ValidationRecords.Count > 0
                            || isFatal)
                        {
                            // Update db with message messages...
                            ar.SaveResultExceptions(tr, tr.ValidationRecords, xmlRepoItem.FileID);
                            // If fatal then save exception and move to failed folder
                            if (isFatal)
                            {
                                result = QAResult.FailValidation;
                                //AM 8/13/2010: changed this; 1 email will be sent at the end with all
                                //  warnings (and errors too if there were any) if emailAlertForWarnings = true
                                //  and there were warnings to send.
                                ar.SaveResultExceptions(tr, vrs, xmlRepoItem.FileID);
                                //ar.SaveResultExceptions(tr, vrs, destinationFile, fileName, emailAlertForWarnings);
                                return result;
                            }
                        }
                        //AM 8/13/2010: same comment as above
                        ar.SaveResultExceptions(tr, vrs, xmlRepoItem.FileID);
                        //ar.SaveResultExceptions(tr, vrs, destinationFile, fileName, emailAlertForWarnings);
                    }
                    else // If there are some XML validation errors then save exception and move to exception folder
                    {
                        result = QAResult.FailValidation;
                        ar.SaveResultExceptions(tr, adapter.ValidationRecords, xmlRepoItem.FileID);
                        return result;
                    }
                }
                else // tr == null
                {
                    result = QAResult.FailValidation;
                    ar.SaveResultExceptions(tr, adapter.ValidationRecords, xmlRepoItem.FileID);
                    return result;
                }

                // Update the test results after validation has been completed.
                try
                {

                    if (isValid)
                    {
                        //PREROUTE:
                        tisExtender.PreRoute(this, adapter, tr, xmlRepoItem, projectMetaData, sendToModifiers, tisExtenderState);

                        // Update XML destinations in case some business rule modified it
                        if (sendToModifiers != null)
                        {
                            foreach (KeyValuePair<SendTo, bool> sendInfo in (SendToModifiersTyped)sendToModifiers)
                            {
                                switch (sendInfo.Key)
                                {
                                    case SendTo.DoR:
                                        projectMetaData.updateDoR = sendInfo.Value;
                                        break;
                                    case SendTo.Handscoring:
                                        projectMetaData.sendToHandScoring = sendInfo.Value;
                                        break;
                                    case SendTo.RB:
                                        projectMetaData.updateRB = sendInfo.Value;
                                        break;
                                }
                            }
                        }

                        if (projectMetaData.updateDoR)
                        {
                            try
                            {
                                List<Target> dorTarget = Target.GetOrderedTargets(tr.ProjectID, Target.TargetClass.DoR);
                                if (dorTarget != null && dorTarget.Count == 1)
                                    dorTarget[0].Send(tr, delegate(object o) { dorRecordId = (long)o; }, projectMetaData.doRAdminID);
                            }
                            catch (Exception ex)
                            {
                                throw new QAException(String.Format("DoR update failed for fileId: {0}, Message: {1}", xmlRepoItem.FileID, ex.Message), QAException.ExceptionType.General, ex);
                            }
                        }

                        //send to configured Handscoring targets
                        ItemScoringManager.Instance.Send(tr, sendToModifiers);

                        // send to configured targets in order (incl RB)
                        foreach (Target t in Target.GetOrderedTargets(tr.ProjectID, Target.TargetClass.General))
                        {
                            if (!sendToModifiers.ShouldSend(t.Name))
                                continue;
                            ITargetResult targetResult = t.Send(tr);
                            if(targetResult.Sent)
                                Logger.Log(true, String.Format("Sent data for FileId: {0} to Target: {1} ({2}).", xmlRepoItem.FileID, t.Name, targetResult.ID ?? "<unspecified>"), EventLogEntryType.Information, false, true);
                        }

                        //POSTROUTE:
                        tisExtender.PostRoute(this, tr, xmlRepoItem, projectMetaData, tisExtenderState);
                    }

                    // Check if this test need to be merged with something else 
                    try
                    {
                        TDSQASystemAPI.TestMerge.TestMerge testMerge = TestMergeConfiguration.Instance.GetTestMerge(tr.Name);
                        if (testMerge != null)
                            testMerge.CreateCombinedTest(_tc, tr, serializerFactory);
                    }
                    catch (Exception ex)
                    {
                        throw new QAException(String.Format("TestMerge operation failed for fileId: {0}, Message: {1}", xmlRepoItem.FileID, ex.Message), QAException.ExceptionType.General, ex);
                    }
                }
                catch (QAException qae)
                {
                    //TODO: 
                    result = QAResult.FailUpdate;
                    string message = qae.GetExceptionMessage(true);
                    if (message.StartsWith("DoR"))
                    {
                        ar.SaveResultExceptions(tr, "DoR update failed: ", message, xmlRepoItem.FileID);
                        Logger.Log(true, message, EventLogEntryType.Error, false, true);
                    }
                    else if (message.StartsWith("Handscoring"))
                    {
                        ar.SaveResultExceptions(tr, "Handscoring update failed: ", message, xmlRepoItem.FileID);
                        Logger.Log(true, message, EventLogEntryType.Error, false, true);
                    }
                    else if (message.StartsWith("TestMerge"))
                    {
                        ar.SaveResultExceptions(tr, "Test merge operation failed: ", message, xmlRepoItem.FileID);
                        Logger.Log(true, message, EventLogEntryType.Error, false, true);
                    }
                    else if (message.StartsWith("AutoAppeal"))
                    {
                        ar.SaveResultExceptions(tr, "AutoAppeal failed: ", message, xmlRepoItem.FileID);
                        Logger.Log(true, message, EventLogEntryType.Error, false, true);
                    }
                    else
                    {
                        ar.SaveResultExceptions(tr, "Response Bank update failed: ", message, xmlRepoItem.FileID);
                        Logger.Log(true, message, EventLogEntryType.Error, false, true);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    result = QAResult.FailUpdate;
                    string message = ex.GetExceptionMessage(true);
                    ar.SaveResultExceptions(tr, "Response Bank update failed: ", message, xmlRepoItem.FileID);
                    Logger.Log(true, message, EventLogEntryType.Error, false, true);
                    return result;
                }

                // Log to TestOpportunityStatus, Update the TDS_QC database, and possibly send to TIS
                if (tr != null)
                {
                    try
                    {
                        // Log to TestOpportunityStatus, Update the TDS_QC database
                        ar.SaveResultInfo(tr, xmlRepoItem.FileID, tr.Testee.IsDemo, dorRecordId, archivedFileID, projectMetaData.updateRB);
                    }
                    catch (Exception ex)
                    {
                        result = QAResult.FailUpdate;
                        string message = ex.GetExceptionMessage(true);
                        ar.SaveResultExceptions(tr, "Archive ", ex.Message, xmlRepoItem.FileID);
                        Logger.Log(true, message, EventLogEntryType.Error, false, true);
                        return result;
                    }

                    //POSTSAVE:
                    tisExtender.PostSave(this, tr, xmlRepoItem, projectMetaData, tisExtenderState);

                }
                result = QAResult.Success;
            }
            catch (Exception ex)
            {
                result = QAResult.Unknown;
                string message = ex.GetExceptionMessage(true);

                if (tr != null)
                {
                    ar.SaveResultExceptions(tr, "QA System Exception:", message, xmlRepoItem.FileID);
                }
                else
                {
                    ar.SaveResultExceptions("QA System Exception:", message, xmlRepoItem.FileID);
                }
                Logger.Log(true, message, EventLogEntryType.Error, false, true);
            }
            finally
            {
                if (!(tr == null || tr.Opportunity == null))
                {
                    // We don't want to call back to TDS for scanned paper tests, since they did not originate from TDS.  
                    //  Just skip w/o logging.
                    if (!tr.Mode.Equals("scanned"))
                    {
                        Boolean accepted = true;
                        string message = null;
                        if (result == QAResult.FailUpdate || result == QAResult.FailValidation || result == QAResult.Unknown || !(tr.PassedAllValidations()))
                        {
                            accepted = false;
                            if (result == QAResult.FailUpdate)
                                message = "failed validation";
                            else if (result == QAResult.FailValidation)
                                message = "failed while either updating the Response Bank, storing data into the DoR, or invoking the Handscoring webservice";
                            else if (result == QAResult.Unknown)
                                message = "An unknown exception occurred";
                            else if (!(tr.PassedAllValidations()))
                                message = "Failed rules validation";
                        }

                        IAcknowledgementTargetFactory ackTargetFactory = AIR.Common.ServiceLocator.Resolve<IAcknowledgementTargetFactory>();
                        if (ackTargetFactory != null) // ok not to acknowledge I suppose
                        {
                            IAcknowledgementTarget ackTarget = ackTargetFactory.SelectTarget(xmlRepoItem);
                            try
                            {
                                if (ackTarget == null
                                    || !ackTarget.Send(new Message(tr.Opportunity.Key, accepted, message), xmlRepoItem))
                                    Logger.Log(true, String.Format("Acknowledgement not sent for fileID: {0}", xmlRepoItem.FileID), EventLogEntryType.Information, false, true);
                            }
                            catch (Exception ex)
                            {
                                // allow these to be treated as warnings or errors.  If TreatAcknowledgementFailureAsError is set to true,
                                //  a failure to send an ACK will result in the file being dumped into the reject bin.
                                //  Default behavior is to treat these as warnings.  We generally don't want to fail a file just because
                                //  we can't send the ACK.  Note also that a combo may already have been created (if applicable).
                                bool treatAckfailureAsError = false;

                                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["TreatAcknowledgementFailureAsError"])
                                    && Convert.ToBoolean(ConfigurationManager.AppSettings["TreatAcknowledgementFailureAsError"]))
                                    treatAckfailureAsError = true;

                                // if the file would have otherwise succeeded and there was an exception while attempting to send the ACK and
                                //  we're not treating these as warnings, fail the file.
                                if (treatAckfailureAsError && result == QAResult.Success)
                                    result = QAResult.Unknown;

                                Logger.Log(true, String.Format("Could not send acknowledgement for fileID: {0}, Exception: {1}", xmlRepoItem.FileID, ex.GetExceptionMessage(true)),
                                    treatAckfailureAsError ? EventLogEntryType.Error : EventLogEntryType.Warning, false, true);
                            }
                        }
                    }
                }
                
                //Move file to appropriate folder based on the QAResult and log the status accordingly.
                LogAndCleanup(result, xmlRepoItem.FileID, tr, startDateTime);


            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="fileName"></param>
        /// <param name="failedProcessingFolder"></param>
        /// <param name="destinationFile"></param>        
        private void LogAndCleanup(QAResult result, long fileID, TestResult tr, DateTime processingStartDateTime)
        {
            string message = String.Empty;
            Severity minSeverityForErrorEmail = QASystemConfigSettings.Instance.EmailAlertForWarnings ? Severity.Warning : Severity.Fatal;
            ///bool ok = true;
            try
            {
                switch (result)
                {
                    case QAResult.Success:
                        // Move file to a "destination" directory for the test taken date for archiving purposes.
                        xmlRepositoryBL.UpdateFileLocation(fileID, TDSQASystemAPI.BL.XmlRepository.Location.destination);
                        Logger.Log(false, "File ID: " + fileID + "  parsed successfully.", EventLogEntryType.Information, false, true);
                        //AM 8/13/2010: added this to send warnings as an html-formatted email.
                        if (QASystemConfigSettings.Instance.EmailAlertForWarnings)
                        {
                            Logger.SendHtmlValidationEmail("File was successfully processed with the following warnings...", tr, fileID, processingStartDateTime, TDSQCConnectionString, Severity.Warning, EventLogEntryType.Warning);
                        }
                        break;
                    case QAResult.FailValidation:
                        // Move it to a "rejects" directory for later inspection of what caused the message.
                        xmlRepositoryBL.UpdateFileLocation(fileID, TDSQASystemAPI.BL.XmlRepository.Location.reject);
                        message = String.Format("File  ID: " + fileID + " failed validation");
                        //Logger.Log(true, "\"" + Path.GetFileName(fileName) + "\" failed validation.", EventLogEntryType.Error, false, true);
                        Logger.Log(true, message, EventLogEntryType.Error, false, true);
                        Logger.SendHtmlValidationEmail(message, tr, fileID, processingStartDateTime, TDSQCConnectionString, minSeverityForErrorEmail, EventLogEntryType.Error);
                        break;
                    case QAResult.FailUpdate:
                        // The file stays where it is, in the Processing folder
                        // Send an email alert...
                        xmlRepositoryBL.UpdateFileLocation(fileID, TDSQASystemAPI.BL.XmlRepository.Location.reject);
                        message = "File ID: " + fileID + " failed while either updating the Response Bank, storing data into the DoR, or invoking the AutoAppeals/Handscoring webservice.";
                        Logger.Log(true, message, EventLogEntryType.Error, false, true);
                        Logger.SendHtmlValidationEmail(message, tr, fileID, processingStartDateTime, TDSQCConnectionString, minSeverityForErrorEmail, EventLogEntryType.Error);
                        break;
                    case QAResult.Unknown:
                        xmlRepositoryBL.UpdateFileLocation(fileID, TDSQASystemAPI.BL.XmlRepository.Location.reject);
                        message = String.Format("A exception occurred for file ID: " + fileID);
                        //Logger.Log(true, "A exception occurred for file \"" + Path.GetFileName(fileName) + "\", please investigate in the event log.\n", EventLogEntryType.Error, false, true);
                        Logger.Log(true, message, EventLogEntryType.Error, false, true);
                        Logger.SendHtmlValidationEmail(message, tr, fileID, processingStartDateTime, TDSQCConnectionString, minSeverityForErrorEmail, EventLogEntryType.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                //ok = false;
                string emailHeaderMsg = "Failed changing the location of the file id: " + fileID;
                message = String.Format("{0}: {1}", emailHeaderMsg, ex.GetExceptionMessage(true));
                new ArchiveDB().SaveResultExceptions(tr, "Log and Cleanup failed:", message, fileID);
                Logger.Log(true, message, EventLogEntryType.Error, false, true);
                Logger.SendHtmlValidationEmail(emailHeaderMsg, tr, fileID, processingStartDateTime, TDSQCConnectionString, minSeverityForErrorEmail, EventLogEntryType.Error);
            }
            //return ok;
        }
    }//end class
}//end namespace
