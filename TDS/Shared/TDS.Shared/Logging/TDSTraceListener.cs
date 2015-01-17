/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using AIR.Common.Diagnostics;
using AIR.Common.Threading;
using AIR.Common.Web;
using TDS.Shared.Configuration;
using TDS.Shared.Data; using TDS.Shared.Configuration;
using TDS.Shared.Security;

namespace TDS.Shared.Logging
{
    /*
        SELECT serverID, [application], daterecorded, ipaddress, procname, errormessage, _efk_Testee, _efk_TestID, Opportunity 
        FROM systemerrors
        ORDER BY daterecorded desc
    */

    /// <summary>
    /// A .NET TraceListener that supports Larry's SystemErrors table format
    /// </summary>
    public class TDSTraceListener : CustomTraceListener
    {
        private static readonly BoundedThreadPool _threadPool = new BoundedThreadPool(0, "Logging Pool", 50, 10, false);
        private static string _connectionString;

        static TDSTraceListener()
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["SESSION_DB"];
            
            if (connectionStringSettings != null)
            {
                _connectionString = connectionStringSettings.ToString();
            }
        }

        public static string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public TDSTraceListener() : base("TDSTraceListener")
        {
        }

        protected override void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            string webDetails = null;

            if (eventType != TraceEventType.Start && eventType != TraceEventType.Stop &&
                eventType != TraceEventType.Verbose && eventType != TraceEventType.Information)
            {
                HttpRequest currentRequest = WebHelper.GetCurrentRequest();
                webDetails = CreateWebDetails(currentRequest);
            }
            
            InternalTrace(source, eventType, message, webDetails);
        }

        protected override void TraceDataCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            // make sure we have some data and it is an exception
            if (data.Length == 0) return;

            // get/create tracelog
            TraceLog traceLog = data[0] as TraceLog;

            if (traceLog == null)
            {
                // get exception data
                Exception exception = data[0] as Exception;
                
                // if no trace log or exception was passed in then nothing left to do
                if (exception == null) return;
                
                // create tracelog with exception
                traceLog = new TraceLog(exception);
            }

            // try and figure out message
            if (traceLog.Message == null)
            {
                // no message found to log
                if (traceLog.Exception == null) return;

                // get exception message
                traceLog.Message = GetExceptionMessage(traceLog.Exception);
            }

            // only use first 1000 characters of error
            const int maxMessageLength = 1000;
            if (traceLog.Message.Length > maxMessageLength)
            {
                int originalMessageLength = traceLog.Message.Length;
                traceLog.Message = traceLog.Message.Substring(0, maxMessageLength);
                traceLog.Message += String.Format(" [TRIMMED {0}]", originalMessageLength - maxMessageLength);
            } 

            // get error details
            if (traceLog.Exception != null)
            {
                // build formatted error
                StringBuilder formattedError = new StringBuilder(traceLog.Details);

                // exception message
                formattedError.AppendLine("@EXCEPTION:");
                
                // write out exception
                using(StringWriter stringWriter = new StringWriter(formattedError))
                {
                    TextExceptionFormatter exceptionFormatter = new TextExceptionFormatter(stringWriter, traceLog.Exception);
                    exceptionFormatter.Format();
                }

                // web request
                HttpRequest currentRequest = WebHelper.GetCurrentRequest();
                formattedError.AppendLine();
                formattedError.AppendLine("@WEB REQUEST:");
                formattedError.AppendLine(CreateWebDetails(currentRequest));
                formattedError.AppendLine();

                traceLog.Details = formattedError.ToString();
            }

            // write message to DB
            InternalTrace(source, eventType, traceLog.Message, traceLog.Details);
        }

        private static string GetExceptionMessage(Exception exception)
        {
            // create a unique ID for the exception
            ExceptionSignatureBuilder esb = new ExceptionSignatureBuilder();
            esb.AddException(exception);

            // create message
            string type = exception is ReturnStatusException ? "RETURNSTATUS" : "EXCEPTION";
            return string.Format("{0}: {1} [{2}]", type, exception.Message, esb.ToSignatureString());
        }

        private static string CreateWebDetails(HttpRequest request)
        {
            if (request == null) return string.Empty;

            StringBuilder webInfo = new StringBuilder();

            webInfo.AppendFormat("Url : {0}", request.RawUrl);
            webInfo.AppendLine();
            webInfo.AppendFormat("Method : {0}", request.HttpMethod);
            webInfo.AppendLine();
            webInfo.AppendFormat("Physical Path : {0}", request.PhysicalPath);
            webInfo.AppendLine();
            webInfo.AppendFormat("Client IP : {0} ({1})", request.UserHostAddress, request.UserHostName);
            webInfo.AppendLine();

            // write out headers
            webInfo.Append("Headers :");

            if (request.Headers.Count > 0)
            {
                webInfo.AppendLine();

                foreach (string headerKey in request.Headers.AllKeys)
                {
                    // skip cookies
                    if (headerKey == "Cookie") continue;
                    string headerValue = request.Headers[headerKey];

                    webInfo.AppendFormat("   {0}={1}", headerKey, headerValue);
                    webInfo.AppendLine();
                }
            }
            else
            {
                webInfo.AppendLine(" None");
            }

            // write out cookies
            webInfo.Append("Cookies :");

            if (request.Cookies.Count > 0)
            {
                webInfo.AppendLine();

                foreach (string cookieKey in request.Cookies)
                {
                    HttpCookie cookie = request.Cookies[cookieKey];
                    if (cookie == null) continue;

                    webInfo.AppendFormat("   {0}={1}", cookieKey, cookie.Value);
                    webInfo.AppendLine();
                }
            }
            else
            {
                webInfo.AppendLine(" None");
            }

            // write out form
            webInfo.Append("Form :");

            if (request.Form.Count > 0)
            {
                webInfo.AppendLine();

                foreach (string formKey in request.Form)
                {
                    string formValue = request.Form[formKey];
                    
                    // check if large
                    if (formValue != null && formValue.Length > 4096)
                    {
                        formValue = "<LARGE>"; // 2048?
                    }

                    webInfo.AppendFormat("   {0}={1}", formKey, formValue);
                    webInfo.AppendLine();
                }
            }
            else
            {
                webInfo.AppendLine(" None");
            }

            return webInfo.ToString();
        }

        public static void InternalWebTrace(string source, TraceEventType eventType, string message, string details)
        {
            if (eventType != TraceEventType.Start && eventType != TraceEventType.Stop &&
                eventType != TraceEventType.Verbose) // && eventType != TraceEventType.Information
            {
                string webDetails = null;
                
                try
                {
                    HttpRequest currentRequest = WebHelper.GetCurrentRequest();
                    webDetails = CreateWebDetails(currentRequest);
                }
                catch (Exception ex)
                {
                    details = "Error getting web info : " + ex.Message;
                }

                if (!string.IsNullOrEmpty(webDetails))
                {
                    if (details == null) details = webDetails;
                    else details += Environment.NewLine + webDetails;
                }
            }

            InternalTrace(source, eventType, message, details);
        }

        private static void InternalTrace(string source, TraceEventType eventType, string message, string details)
        {
            // check if a connection string was provided
            if (_connectionString == null) return;

            SqlCommand cmd;

            try
            {
                cmd = GetTraceSqlCommand(source, eventType, message, details);
            }
            catch
            {
                return;
            }

            _threadPool.Enqueue(delegate
            {
                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    InternalLogger.LogException(ex);
                }
                finally
                {
                    if (cmd.Connection.State == ConnectionState.Open)
                    {
                        // try and close connection
                        try 
                        {
                            cmd.Connection.Close();
                        }
                        catch(Exception ex)
                        {
                            InternalLogger.LogException(ex);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// This functions creates the SqlCommand object that will be used to write to the DB
        /// </summary>
        private static SqlCommand GetTraceSqlCommand(string source, TraceEventType eventType, string message, string stackTrace)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("_RecordSystemError", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 20;

            // set error
            cmd.AddVarChar("application", HttpRuntime.AppDomainAppVirtualPath ?? "Unknown", 150);
            cmd.AddVarChar("clientname", TDSSettings.GetClientName() ?? "Unknown", 100);

            // set message
            cmd.AddVarCharMax("msg", message);
            
            // set stacktrace
            cmd.AddVarCharMax("stackTrace", stackTrace);

            // get unique ID for the context of this request
            Guid id = (Trace.CorrelationManager.ActivityId == Guid.Empty) ? Guid.NewGuid() : Trace.CorrelationManager.ActivityId;
            cmd.AddUniqueIdentifier("ApplicationContextID", id);

            // create the process name
            string proc = eventType + ":" + source;

            // add HTTP information
            HttpRequest currentRequest = WebHelper.GetCurrentRequest();

            if (currentRequest != null)
            {
                // add IP
                if (currentRequest.UserHostAddress != null)
                {
                    // string clientIP = request.ServerVariables["REMOTE_ADDR"];
                    string clientIP = currentRequest.UserHostAddress;

                    if (currentRequest.UserHostName != null && currentRequest.UserHostAddress != currentRequest.UserHostName)
                    {
                        clientIP += String.Format(" ({0})", currentRequest.UserHostName);
                    }

                    cmd.AddVarChar("clientIP", clientIP, 50);
                }

                // add http page to the proc name
                Uri url = currentRequest.Url;

                if (url != null & url.Segments.Length > 0)
                {
                    proc += ":" + url.Segments[url.Segments.Length - 1];
                }

                // add opportunity information if available
                if (TDSIdentity.Current != null)
                {
                    long? testeeKey = TDSIdentity.Current.Values.Get<long?>("T_KEY");
                    string testKey = TDSIdentity.Current.Values.Get<string>("O_TKEY");
                    Guid? oppKey = TDSIdentity.Current.Values.Get<Guid?>("O_KEY");

                    cmd.AddBigInt("testee", testeeKey);
                    cmd.AddVarChar("test", testKey, 150);
                    cmd.AddUniqueIdentifier("testoppkey", oppKey);
                    // cmd.AddInt("opportunity", tdsCookie.Get<int?>("O_NUM"));
                }
            }

            cmd.AddVarChar("proc", proc, 50);

            // add user information
            /*if (TDSIdentity.Current != null)
            {
                long testeeKey = TDSIdentity.Current.Values.Get<long>("k");
                string testID = TDSIdentity.Current.Values.Get("t");
                int opportunity = TDSIdentity.Current.Values.Get<int>("o");

                cmd.AddBigInt("testee", testeeKey);
                cmd.AddVarChar("test", testID, 150);
                cmd.AddInt("opportunity", opportunity);
            }*/

            return cmd;
        }

        /*
        private static string InferApplicationName()
        {
            HttpContext context = HttpContext.Current;

            string appName = null;

            if (context != null && context.Request != null)
            {
                //
                // ASP.NET 2.0 returns a different and more cryptic value
                // for HttpRuntime.AppDomainAppId compared to previous 
                // versions. Also HttpRuntime.AppDomainAppId is not available
                // in partial trust environments. However, the APPL_MD_PATH
                // server variable yields the same value as 
                // HttpRuntime.AppDomainAppId did previously so we try to
                // get to it over here for compatibility reasons.

                appName = context.Request.ServerVariables["APPL_MD_PATH"];
            }

            if (string.IsNullOrEmpty(appName))
            {
                //
                // Still no luck? Try HttpRuntime.AppDomainAppVirtualPath,
                // which is available even under partial trust.
                //

                appName = HttpRuntime.AppDomainAppVirtualPath;
            }

            return Mask.EmptyString(appName, "/");
        }
        */

    }
}