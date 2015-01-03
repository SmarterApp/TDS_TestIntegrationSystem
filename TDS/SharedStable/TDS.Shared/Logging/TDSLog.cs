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
using System.Data.SqlClient;
using System.Web;
using System.Net.Mail;
using AIR.Common.Sql;

namespace TDS.Shared.Logging
{
    public class TDSLog
    {        
        //static string connectionString = ConfigurationManager.ConnectionStrings["SESSION_DB"].ToString();
        private static string connectionString;

        static TDSLog()
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["SESSION_DB"];
            
            if (connectionStringSettings != null)
            {
                connectionString = connectionStringSettings.ToString();
            }
        }       



        static string smtpServer;
        public static string SMTPServer
        {
            get
            {
                if (string.IsNullOrEmpty(smtpServer))
                    smtpServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();
                return smtpServer;
            }
        }
        static string emailFrom;
        public static string EmailFrom
        {
            get
            {
                if (string.IsNullOrEmpty(emailFrom))
                    emailFrom = ConfigurationManager.AppSettings["EmailFrom"].ToString();
                return emailFrom;
            }
        }
        static string emailTo;
        public static string EmailTo
        {
            get
            {
                if (string.IsNullOrEmpty(emailTo))
                    emailTo = ConfigurationManager.AppSettings["EmailTo"].ToString();
                return emailTo;
            }
        }
        static bool? emailOff;
        public static bool EmailOff
        {
            get
            {
                if (emailOff == null)
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailOff"]))
                        emailOff = Convert.ToBoolean(ConfigurationManager.AppSettings["EmailOff"]);
                    else
                        emailOff = true;
                }
                return (bool)emailOff;
            }
        }
       
        //public static void LogError(Exception ex)
        //{
        //    try
        //    {
        //        LogError("Error: System Exception", ex);
        //    }
        //    catch
        //    {
        //        ;
        //    }
        //}

        ////Main log function
        //public static void LogError(string proc, Exception ex)
        //{
        //    try
        //    {
        //        UserCookie userInfoCookie = new UserCookie(HttpContext.Current, AppConfigSingleton.Instance.CookieName_UserInfo);
        //        string userName = userInfoCookie.GetValue("userEmail");
        //        string strMsg = "";
        //        if(ex != null)
        //            strMsg = string.Format("UserID: {0} -- IPAddress: {5} -- URL: {1} -- Message: {2} -- Source: {3} -- Stack trace: {4}",
        //                userName, HttpContext.Current.Request.Url, ex.Message, ex.Source, ex.StackTrace, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
        //        else
        //            strMsg = "Exception object is null";

        //        LogError(proc, strMsg, null, null, null, AppConfigSingleton.Instance.AppName, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
        //        //clear exception 
        //        HttpContext.Current.Server.ClearError();

        //        //send email here ...

        //    }
        //    catch
        //    {
        //        ;
        //    }
        //}
        //public static void LogError(string proc, string msg)
        //{
        //    try
        //    {              
        //        if(HttpContext.Current !=null)
        //            LogError(proc, msg, null, null, null, AppConfigSingleton.Instance.AppName, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
        //        else
        //            LogError(proc, msg, null, null, null, AppConfigSingleton.Instance.AppName, null);
        //        //send email here ...
        //    }
        //    catch
        //    {
        //        ;
        //    }
        //} 

        //private static void LogError(string proc, string msg, long? testee, string test, int? opportunity, string application)
        //{
        //    RecordSystemError(proc, msg, testee, test, opportunity, application, null);
        //}
        //private static void LogError(string proc, string msg, long? testee, string test, int? opportunity, string application, string clientIP)
        //{
        //    RecordSystemError(proc, msg, testee, test, opportunity, application, clientIP);
        //}
        
        //private static void RecordSystemError(string proc, string msg, long? testee, string test, int? opportunity, string application, string clientIP)
        //{           
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        using (SqlCommand cmd = new SqlCommand("_RecordSystemError", connection))
        //        {
        //            cmd.CommandTimeout = 5;
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("proc", DbHelper.Nullify(proc));
        //            cmd.Parameters.AddWithValue("msg", DbHelper.Nullify(msg));
        //            cmd.Parameters.AddWithValue("testee", DbHelper.Nullify(testee));
        //            cmd.Parameters.AddWithValue("test", DbHelper.Nullify(test));
        //            cmd.Parameters.AddWithValue("opportunity", DbHelper.Nullify(opportunity));
        //            cmd.Parameters.AddWithValue("application", DbHelper.Nullify(application));
        //            cmd.Parameters.AddWithValue("clientIP", DbHelper.Nullify(clientIP));
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        public static void LogSystemClient(string UserID, string clientname, string appName, HttpContext thisContext)
        {
            try
            {
                string recordSystemClient = ConfigurationManager.AppSettings["RecordSystemClient"].ToString();
                if (recordSystemClient != "true")
                    return;
                RecordSystemClient(clientname, appName, UserID,
                    thisContext.Request.ServerVariables["REMOTE_ADDR"], thisContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"], 
                    thisContext.Request.UserAgent);
            }
            catch { }
        }
        private static void RecordSystemClient(string clientname, string application, string UserID, string ClientIP, string ProxyIP, string UserAgent)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("_RecordSystemClient", connection))
                {
                    cmd.CommandTimeout = 5;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("clientname", DbHelper.Nullify(clientname));
                    cmd.Parameters.AddWithValue("application", DbHelper.Nullify(application));
                    cmd.Parameters.AddWithValue("UserID", DbHelper.Nullify(UserID));
                    cmd.Parameters.AddWithValue("ClientIP", DbHelper.Nullify(ClientIP));
                    cmd.Parameters.AddWithValue("ProxyIP", DbHelper.Nullify(ProxyIP));
                    cmd.Parameters.AddWithValue("UserAgent", DbHelper.Nullify(UserAgent));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool SendEmail(string subject, string body)
        {
            return SendEmail(subject, body, EmailFrom, EmailTo, SMTPServer,null);
        }

        public static bool SendEmail(string subject, string body, string[] attachments)
        {
            return SendEmail(subject, body, EmailFrom, EmailTo, SMTPServer, attachments);
        }

        public static bool SendEmail(string subject, string body, string from, string to, string smtpServer)
        {
            try
            {
                if (EmailOff) //if turn off email, then dont send anything else
                    return false;

                if (string.IsNullOrEmpty(smtpServer))
                {
                    TDSLogger.Application.Warn("SMTPServer string is null or empty.");
                    return false;
                }
                
                MailMessage mailMessage = new MailMessage(from, to, subject, body);
                SmtpClient smtpClient = new SmtpClient(smtpServer);
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                TDSLogger.Application.Error(ex);              
                return false;
            }
        }

        public static bool SendEmail(string subject, string body, string from, string to, string smtpServer, string[] attachments)
        {
            try
            {
                if (EmailOff) //if turn off email, then dont send anything else
                    return false;
                if (string.IsNullOrEmpty(smtpServer))
                {
                    TDSLogger.Application.Warn("SMTPServer string is null or empty.");
                    return false;
                }

                MailMessage mailMessage = new MailMessage(from, to, subject, body);
                if (attachments != null && attachments.Length > 0)
                {
                    foreach (string file in attachments)
                    {
                        Attachment att = new Attachment(file);
                        mailMessage.Attachments.Add(att);
                    }
                }

                SmtpClient smtpClient = new SmtpClient(smtpServer);
                smtpClient.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                TDSLogger.Application.Error(ex);              
                return false;
            }
        }

        public static void LogLastExceptionError()
        {
            // Code that runs when an unhandled error occurs
            try
            {
                Exception ex = HttpContext.Current.Server.GetLastError().GetBaseException();
                TDSLogger.Application.Error(ex);              
            }
            catch {}
        }

    }
}
