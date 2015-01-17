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
using System.Diagnostics;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.ExceptionHandling;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.Config;
using AIR.Common;

namespace TDSQASystemAPI.Utilities
{
	/// <summary>
	/// Log issues with the windows service, tester application, or while processing generalFiles. This class provides different
	/// methods for message or information logging.
	/// </summary>
	public class Logger
	{
        private static string environment
        {
            get
            {
                return ConfigurationManager.AppSettings["Environment"];
            }
        }
        private static string clientName
        {
            get
            {
                return ConfigurationManager.AppSettings["ClientName"];
            }
        }
        private static string serviceName
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceName"];
            }
        }
        
		/// <summary>
		/// A list of recipients to receive fatal errors.
		/// </summary>
        private static string fatalErrorTos;
		//private static string fatalErrorTos = ConfigurationManager.AppSettings["FatalErrorsTo"];
		/// <summary>
		/// A list of recipients in the fatalErrorCc list to receive fatal errors.
		/// </summary>
        private static string fatalErrorCc;
        //private static string fatalErrorCc = ConfigurationManager.AppSettings["FatalErrorsCc"];
		/// <summary>
		/// A list of recipients to receive warnings.
		/// </summary>
        private static string warningTos;
        //private static string warningTos = ConfigurationManager.AppSettings["WarningsTo"];
		/// <summary>
		/// A list of recipients in the warningsCc list to receive warnings.
		/// </summary>
        private static string warningCc;
        //private static string warningCc = ConfigurationManager.AppSettings["WarningsCc"];

        private static string errorFile;
        private static StreamWriter _logFile;
		/// <summary>
		/// This is the system event log where all events that are associated with the service,
		/// and are not database or xml file issues will be logged.
		/// </summary>
		private static EventLog qaSystemEventLog;

        private static Dictionary<EventLogEntryType, Queue<DateTime>> messageTimeQueues = new Dictionary<EventLogEntryType, Queue<DateTime>>();
        
        private static int maxErrors = 10;
        private static int errorTimeInterval = 30; // minutes

        static Logger()
        {
            qaSystemEventLog = new EventLog(ConfigurationManager.AppSettings["EventLogName"]);
            if (ConfigurationManager.AppSettings["ErrorLog"] != null)
                errorFile = ConfigurationManager.AppSettings["ErrorLog"];
            else
                errorFile = "ResultLog.txt"; //Default Log location.

            fatalErrorTos = ServiceLocator.Resolve<ISystemConfigurationManager>().GetConfigSettingsValueOrEmptyString(clientName, "FatalErrorsTo");
            fatalErrorCc = ServiceLocator.Resolve<ISystemConfigurationManager>().GetConfigSettingsValueOrEmptyString(clientName, "FatalErrorsCc");
            warningTos = ServiceLocator.Resolve<ISystemConfigurationManager>().GetConfigSettingsValueOrEmptyString(clientName, "WarningsTo");
            warningCc = ServiceLocator.Resolve<ISystemConfigurationManager>().GetConfigSettingsValueOrEmptyString(clientName, "WarningsCc");

            _logFile = new StreamWriter(errorFile, true);
			qaSystemEventLog.Source = ConfigurationManager.AppSettings["EventLogSource"];

            if (ConfigurationManager.AppSettings["MaxErrorEMails"] != null)
                maxErrors = Convert.ToInt32(ConfigurationManager.AppSettings["MaxErrorEMails"]);
            if (ConfigurationManager.AppSettings["MaxErrorEMailInterval"] != null)
                errorTimeInterval = Convert.ToInt32(ConfigurationManager.AppSettings["MaxErrorEMailInterval"]);

            // create message queues for different severities
            messageTimeQueues.Add(EventLogEntryType.Error, new Queue<DateTime>());
            messageTimeQueues.Add(EventLogEntryType.Warning, new Queue<DateTime>());
            messageTimeQueues.Add(EventLogEntryType.Information, new Queue<DateTime>());

            foreach (Queue<DateTime> messageQueue in messageTimeQueues.Values)
            {
                for (int i = 0; i < maxErrors; i++)
                {
                    messageQueue.Enqueue(DateTime.MinValue);
                }
            }
        }

        //AM 8/13/2010: wasn't being used
        ///// <summary>
        ///// Email message message.
        ///// </summary>
        ///// <param name="message"></param>
        //private static void SendErrorEmail(string message, bool isHtml)
        //{
        //    SendErrorEmail("(" + environment + ", " + serviceName + ") QA System", message, fatalErrorTos, fatalErrorCc, isHtml);
        //}

		/// <summary>
		/// Send an email to a predefined receiver with a description of the message.
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="message"></param>
        private static void SendErrorEmail(string subject, string message, string to, string cc, bool isHtml, EventLogEntryType logEntryType)
        {
            try
            {
				if (to.Length > 0)
				{
                    // don't send (by default) more than 10 messages in 30 minutes

                    lock (messageTimeQueues[logEntryType]) // one thread at a time please!
                    {
                        DateTime now = DateTime.Now;
                        if (messageTimeQueues[logEntryType].Peek().AddMinutes(errorTimeInterval).CompareTo(now) < 0)
                        {
                            messageTimeQueues[logEntryType].Dequeue();
                            string messageTypeAsString = Enum.GetName(typeof(EventLogEntryType), logEntryType).ToLower();
                            if (messageTimeQueues[logEntryType].Peek().AddMinutes(errorTimeInterval).CompareTo(now) > 0)
                            {
                                
                                string lastMsg = "NOTE: This is " + messageTypeAsString + " message " + maxErrors.ToString() + " in the last " + errorTimeInterval.ToString() + " minutes, if another " + messageTypeAsString + " occurs no e-mail will be sent. Please check the DB logs!";
                                if (isHtml)
                                {
                                    message += @"<div style=""font-family:verdana;font-size:12px;font-weight:bolder;background-color:yellow;"">" + lastMsg + @"</div>";
                                }
                                else
                                {
                                    message += Environment.NewLine + Environment.NewLine + lastMsg;
                                }
                            }
                            messageTimeQueues[logEntryType].Enqueue(now);

                            using (MailMessage mailMessage = new MailMessage())
                            {
                                mailMessage.To.Add(to);
                                if (cc.Length > 0)
                                {
                                    mailMessage.CC.Add(cc);
                                }
                                //Subject = (<Environment>) <Message Type> - <Client> <System> - <Instance Name, if applicable> <- configurable info, if any>
                                mailMessage.Subject = "(" + environment + ") " + messageTypeAsString + " - " + clientName + " QA System";
                                if (serviceName.Length > 0)
                                    mailMessage.Subject += " - " + serviceName;
                                if (subject.Length > 0)
                                    mailMessage.Subject += " - " + subject;
                                mailMessage.Body = GetSafeMessage(message,0); //TODO: still need this?  pwds are encrypted now.
                                mailMessage.BodyEncoding = Encoding.ASCII;
                                mailMessage.IsBodyHtml = isHtml;

                                try
                                {
                                    using (SmtpClient smtpClient = new SmtpClient())
                                    {
                                        smtpClient.Send(mailMessage);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new QAException("Unable to send email.", QAException.ExceptionType.General, ex);
                                }
                            }
                        }
                    }
				}
            }
            catch (Exception ex)
            {
                LogToFile("Error sending an email:" + ex.ToString() + "\n StackTrace:" + ex.StackTrace);
            }
        }

        /// <summary>
        /// This method recursively calls itself replacing any string of the form password=xxx with
        /// password=CENSORED. There can be any number of these that it will replace.
        /// </summary>
        /// <param name="message">message to remove passwords from</param>
        /// <param name="index">set to 0</param>
        /// <returns></returns>
        private static string GetSafeMessage(string message, int index)
        {
            string pw = "password=";
            string censored = "CENSORED";

            // handle null message
            message = message ?? "";

            if (index >= message.Length)
                return message;

            int idx = message.IndexOf(pw, index, StringComparison.InvariantCultureIgnoreCase);
            //first check if the string has a password in it
            if (idx >= 0)
            {
                int pwStartIdx = idx + pw.Length;
                //check that the password isn't already CENSORED. If it is then skip replacing it
                if (((pwStartIdx + censored.Length) >= message.Length) || message.Substring(pwStartIdx, censored.Length) != censored)
                {
                    int semicolonIdx = message.IndexOf(";", pwStartIdx);
                    //check if there is a semicolon after the password
                    if (semicolonIdx < 0)
                    {
                        //if there is not a semicolon after the password then go to the next word
                        semicolonIdx = message.IndexOf(" ", pwStartIdx);
                        //if there is no word after the password then we go to the end of the string
                        if (semicolonIdx < 0)
                            semicolonIdx = message.Length;
                    }
                    return GetSafeMessage(message.Substring(0, pwStartIdx) + censored + message.Substring(semicolonIdx), pwStartIdx + censored.Length + 2); // +2 to account for the semicolon or space as well
                }
                else return GetSafeMessage(message, pwStartIdx + censored.Length + 1);
            }
            else return message;
        }

        private static void LogToFile(string error)
        {
            try
            {
                string str = string.Empty;
                _logFile.WriteLine(DateTime.Now.ToString() + "==> " + error);
                _logFile.Flush();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

		/// <summary>
		/// This method is temporary and it logs message to a log file. It is only used for testing purposes.
		/// In productions, errors will only be logged using the event logger.
		/// </summary>
		/// <param name="message"></param>
		private static void LogToFile(string error, bool sendEmail)
		{
			try
			{
				if (sendEmail)
				{
					//string subject = "(" + environment + ", " + serviceName + ") QA System Service Info";
					SendErrorEmail("", error, fatalErrorTos, fatalErrorCc, false, EventLogEntryType.Error);
				}
				LogToFile(error);
			}
			catch (Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message">The message to record.</param>
		/// <param name="logSeverity">The message severity for this event log entry.</param>
		/// <param name="sendEmail">Send an email alert.</param>
		/// <param name="logToFile">Log this message into the result log file.</param>
		public static void Log(bool logToEventLog, string message, EventLogEntryType logSeverity, bool sendEmail, bool logToFile)
		{
            Log(logToEventLog, message, logSeverity, sendEmail, logToFile, false);
		}
        
        // Overloaded to allow passing of isHtml w/o impacting current clients
        public static void Log(bool logToEventLog, string message, EventLogEntryType logSeverity, bool sendEmail, bool logToFile, bool isHtml)
        {
            //should we add handling for warning messages here too? If it is a warning message and we only send a summary of warnings
            //should we not send the email?
            if (sendEmail)
            {
                string to = fatalErrorTos;
                string cc = fatalErrorCc;
                //string subject = "(" + environment + ", " + serviceName + ") ";
                if (logSeverity == EventLogEntryType.Error)
                {
                    //subject += "QA System Service Error";
                    to = fatalErrorTos;
                    cc = fatalErrorCc;
                }
                else if (logSeverity == EventLogEntryType.Warning)
                {
                    //subject += "QA System Service Warning";
                    to = warningTos;
                    cc = warningCc;
                }
                else if (logSeverity == EventLogEntryType.Information)
                {
                    //subject += "QA System Service Info";
                }
                else
                {
                    //subject += "QA System Service";
                }
                SendErrorEmail("", message, to, cc, isHtml, logSeverity);
            }
            Log(logToEventLog, message, logSeverity, logToFile);
        }

        /// <summary>
        /// Use this to send an html-formatted email of validation errors, warnings, etc, including
        /// QA processing exceptions.
        /// </summary>
        /// <param name="message">General message will function as the header of the email</param>
        /// <param name="tr">A TestResult instance if one exists.  Otherwise, null</param>
        /// <param name="fileName">The name of the file that's being processed by QA</param>
        /// <param name="processingStartDateTime">When this file was picked up by QA</param>
        /// <param name="connectionString">The connection string that will be used to connect to the TDS QC database</param>
        /// <param name="minimumSeverity">min Severity to include in the email.  Warning, Information, 
        /// and Fatal are supported.  Unknown/Null Severity records are always included.</param>
        public static void SendHtmlValidationEmail(string message, TestResult tr, long fileId,
            DateTime processingStartDateTime, string connectionString, Severity minimumSeverity, EventLogEntryType logEntryType)
        {
            string exceptionsMessage = GetValidationEmailContent(message, tr, fileId, processingStartDateTime, connectionString, minimumSeverity);
            if (!String.IsNullOrEmpty(exceptionsMessage))
            {
                Log(false, exceptionsMessage, logEntryType, true, false, true);
            }
        }

        /// <summary>
        /// Generates an email message for validation exceptions that displays all
        /// exceptions that occurred during processing in a formatted HTML table.
        /// The fileName allows us to uniquely identify a file that was processed by QA;
        /// however, we may process a file more than once.  So we'll also use the datetime
        /// that QA processing started on this file in order to exclude errors that may
        /// have occurred in a previous run.
        /// </summary>
        /// <param name="headerMessage">This will be the header of the email; should be a short description</param>
        /// <param name="tr">The TestResult instance that's being processed, if it exists.  
        /// It's possible that errors may have occurred before it could be created.</param>
        /// <param name="fileName">The name of the file being processed by QA.</param>
        /// <param name="startDateTime">Should be the datetime that processing started for this file.</param>
        /// <param name="connectionString">the TDS connection string</param>
        /// <param name="minimumSeverity">min Severity to include in the email.  Warning, Information, 
        /// and Fatal are supported.  Unknown/Null Severity records are always included.</param>
        /// <returns>A string containing the formatted HTML email, or NULL if there are no records to send.</returns>
        private static string GetValidationEmailContent(string headerMessage, TestResult tr, long fileId,
            DateTime startDateTime, String connectionString, Severity minimumSeverity)
        {
            // Get all exceptions that were logged to the QC_ValidationExceptions table.  This includes
            //  validation biz rule failures as well as general exceptions.  It may also include warnings and possibly
            //  informational records depending on the minimumSeverity.
            DataTable valExceptions = TDSQC.GetQC_ValidationExceptions(fileId, startDateTime, (int)minimumSeverity, connectionString);
            
            // if there are no records, do not generate the email.  Just return null.
            if (valExceptions.Rows.Count == 0)
            {
                return null;
            }

            // there are records to email.  Build the email and return the formatted message.

            StringBuilder formattedExceptions = new StringBuilder();
            formattedExceptions.Append(@"<html><body style=""font-family:verdana;font-size:12px;"">");

            // if there was a message passed in, put that at the top.
            if (!String.IsNullOrEmpty(headerMessage))
            {
                formattedExceptions.Append(@"<div style=""font-weight:bolder;font-size:14px;color:darkRed;"">");
                formattedExceptions.Append(headerMessage);
                formattedExceptions.Append("</div><br>");
            }

            // start the table
            formattedExceptions.Append(@"<table border=""0"" cellpadding=""4"" cellspacing=""1"">");

            if (valExceptions.Rows.Count == 0)
            {
                formattedExceptions.Append(@"<tr><td style=""font-family:verdana;font-size:12px;"">No validation exceptions logged for fileId '");
                formattedExceptions.Append(fileId);
                formattedExceptions.Append("' starting at ");
                formattedExceptions.Append(startDateTime.ToString("MM/dd/yyyy hh:mm:ss:fff tt"));
                formattedExceptions.Append(".");
                formattedExceptions.Append("</td></tr>");
            }
            else
            {
                valExceptions.Columns["_efk_ruleID"].ColumnName = "Rule";
                valExceptions.Columns["_efk_Testee"].ColumnName = "StudentID";
                valExceptions.Columns["_efk_TestID"].ColumnName = "Test";

                // header row
                formattedExceptions.Append("<tr>");
                foreach (DataColumn col in valExceptions.Columns)
                {
                    formattedExceptions.Append(@"<td style=""font-family:verdana;font-size:12px;font-weight:bolder;background-color:gainsboro;"">");
                    formattedExceptions.Append(col.ColumnName);
                    formattedExceptions.Append("</td>");
                }
                formattedExceptions.Append("</tr>");

                // data
                foreach (DataRow valException in valExceptions.Rows)
                {
                    formattedExceptions.Append("<tr>");
                    int i = 0;
                    foreach (DataColumn col in valExceptions.Columns)
                    {
                        if (i % 2 == 0)
                        {
                            formattedExceptions.Append(@"<td style=""font-family:verdana;font-size:12px;"">");
                        }
                        else
                        {
                            formattedExceptions.Append(@"<td style=""font-family:verdana;font-size:12px;background-color:lightYellow;"">");
                        }
                        formattedExceptions.Append(valException[col]);
                        formattedExceptions.Append("</td>");
                        i++;
                    }
                    formattedExceptions.Append("</tr>");
                }
            }

            formattedExceptions.Append(@"<tr><td style=""font-family:verdana;font-size:10px;padding-top:10px;"" colspan=""");
            formattedExceptions.Append(valExceptions.Columns.Count);
            formattedExceptions.Append(@"""><i>Severity: Unknown = 0/(blank), Information = 10, Warning = 100, Fatal = 200</i></td></tr>");

            // close out the message
            formattedExceptions.Append("</table></body></html>");

            return formattedExceptions.ToString();
        }


		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="logSeverity"></param>
		/// <param name="logToFile"></param>
		private static void Log(bool logToEventLog, string error, EventLogEntryType logSeverity, bool logToFile)
		{
		    try
		    {
				if (logToEventLog)
				{
					qaSystemEventLog.WriteEntry(error, logSeverity);
				}
				if (logToFile)
				{
					LogToFile(error);
				}
		    }
		    catch (Exception e)
		    {
		        throw new ApplicationException(e.Message);
		    }
		}
        /// <summary>
        /// send an email message with a summary of all warnings in the TestOpportunityStatus table since the last time the email was sent
        /// NOTE: Currently have it set not to get the Details table. To include the details table in the emails change the parameter to "true" in GetWarningsSummary,
        ///       and uncomment the lines regarding the `detailsTable` variable.
        /// </summary>
        public static void SendWarningSummaryEmail(DateTime startDate)
        {
            string tdsQCConnectionString = ServiceLocator.Resolve<ISystemConfigurationManager>().GetConfigSettingsValue(clientName, "TDSQCConnectionString");
            //getting the tables from QC_ValidationExceptions table. The first table is a summary, the second is a detailed report of all warnings.
            DataSet ds = TDSQC.GetWarningsSummary(startDate, 100, false, serviceName, tdsQCConnectionString);
            DataTable summaryTable = ds.Tables["Summary"].Copy();
            //get the list of all test IDs that have warnings, so we can separate them out and report them each in their own table in the email
            List<String> testIDs = new List<string>();
            foreach (DataRow row in summaryTable.Rows)
            {
                if (!testIDs.Contains(row["TestID"].ToString()))
                    testIDs.Add(row["TestID"].ToString());
            }
            //DataTable detailsTable = ds.Tables["Details"].Copy();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<html><body style=""font-family:verdana;font-size:12px;"">");
            sb.Append(@"<div style=""font-weight:bolder;font-size:14px;color:darkRed;"">");
            sb.Append("The following test opportunities were successfully processed with warnings on " + DateTime.Now.ToShortDateString());
            sb.Append("</div><br>");
            if (testIDs.Count == 0)
            {
                sb.Append(@"<div style=""font-weight:bolder;font-size:14px;color:darkRed;"">");
                sb.Append("No warnings found since " + startDate.ToShortDateString());
                sb.Append("</div><br>");
            }
            else // else isn't really needed...
            {
                //now add a table for each test ID
                foreach (string id in testIDs)
                {
                    DataTable currentSummaryTable = summaryTable.Clone();
                    foreach (DataRow row in summaryTable.Select("TestID = '" + id + "'"))
                        currentSummaryTable.ImportRow(row);
                    currentSummaryTable.Columns.Remove("TestID");
                    sb.Append(CreateHTMLTable("Summary for Test ID '" + id + "':", startDate, false, currentSummaryTable));
                    //DataTable currentDetailsTable = detailsTable.Clone();
                    //foreach (DataRow row in detailsTable.Select("TestID = '" + id + "'"))
                    //    currentDetailsTable.ImportRow(row);
                    //currentDetailsTable.Columns.Remove("TestID");
                    //sb.Append(CreateHTMLTable("Details for Test ID '" + id + "':", startDate, true, currentDetailsTable));
                }
            }
            sb.Append("</body></html>");
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.To.Add(warningTos);

                if (!String.IsNullOrEmpty(warningCc))
                {
                    mailMessage.CC.Add(warningCc);
                }

                //Subject = (<Environment>) <Message Type> - <Client> <System> - <Instance Name, if applicable> <- configurable info, if any>
                mailMessage.Subject = "(" + environment + ") Information - " + clientName;
                mailMessage.Subject += " QA System";
                if (serviceName.Length > 0)
                    mailMessage.Subject += " - " + serviceName;
                mailMessage.Subject += " - QA System Warnings Summary For " + DateTime.Now.ToShortDateString();

                mailMessage.Body = sb.ToString();
                mailMessage.BodyEncoding = Encoding.ASCII;
                mailMessage.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);
            }
        }
        /// <summary>
        /// Create an HTML table for use in an HTML email message body
        /// </summary>
        /// <param name="headerText">header to put above the table</param>
        /// <param name="startDate">For use if table contains no data</param>
        /// <param name="addSeveritiesFooter">True to put the list of severities as the last row of table</param>
        /// <param name="table">a DataTable containing the data to be printed in the HTML table</param>
        /// <returns></returns>
        private static string CreateHTMLTable(string headerText, DateTime startDate, bool addSeveritiesFooter, DataTable table)
        {
            StringBuilder sb = new StringBuilder();
            // start the summary table, and add a header for it
            sb.Append(@"<div style=""font-weight:bolder;font-size:14px;color:darkRed;"">");
            sb.Append(headerText);
            sb.Append("</div><br>");
            sb.Append(@"<table border=""0"" cellpadding=""4"" cellspacing=""1"">");

            if (table.Rows.Count == 0)
            {
                sb.Append(@"<tr><td style=""font-family:verdana;font-size:12px;"">No warnings between ");
                sb.Append(startDate.ToString("MM/dd/yyyy hh:mm:ss:fff tt"));
                sb.Append(" and ");
                sb.Append(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss:fff tt"));
                sb.Append(".");
                sb.Append("</td></tr>");
            }
            else
            {
                // header row
                sb.Append("<tr>");
                foreach (DataColumn col in table.Columns)
                {
                    sb.Append(@"<td style=""font-family:verdana;font-size:12px;font-weight:bolder;background-color:gainsboro;"">");
                    sb.Append(col.ColumnName);
                    sb.Append("</td>");
                }
                sb.Append("</tr>");

                // data
                foreach (DataRow row in table.Rows)
                {
                    sb.Append("<tr>");
                    int i = 0;
                    foreach (DataColumn col in table.Columns)
                    {
                        if (i % 2 == 0)
                        {
                            sb.Append(@"<td style=""font-family:verdana;font-size:12px;"">");
                        }
                        else
                        {
                            sb.Append(@"<td style=""font-family:verdana;font-size:12px;background-color:lightYellow;"">");
                        }
                        sb.Append(row[col]);
                        sb.Append("</td>");
                        i++;
                    }
                    sb.Append("</tr>");
                }
            }

            sb.Append(@"<tr><td style=""font-family:verdana;font-size:10px;padding-top:10px;"" colspan=""");
            sb.Append(table.Columns.Count);
            sb.Append(@""">");
            if (addSeveritiesFooter)
                sb.Append(@"<i>Severity: Unknown = 0/(blank), Information = 10, Warning = 100, Fatal = 200</i>");
            sb.Append(@"</td></tr>");

            // close out the message
            sb.Append("</table><br>");

            return sb.ToString();
        }
	}
}
