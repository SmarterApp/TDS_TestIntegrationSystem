/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Net.Mail;
using AIR.Common.Configuration;

namespace TDS.Shared.Utilities
{
    public static class Email
    {
        public static bool SendEmail(string subject, string body)
        {
            string emailFrom = AppSettingsHelper.Get("EmailFrom");
            string emailTo = AppSettingsHelper.Get("EmailTo");
            string smtpServer = AppSettingsHelper.Get("SMTPServer");

            return SendEmail(subject, body, emailFrom, emailTo, smtpServer);
        }       

        public static bool SendEmail(string subject, string body, string from, string to, string smtpServer)
        {            
            bool emailOff = AppSettingsHelper.GetBoolean("EmailOff", true);
            if (emailOff) //if turn off email, then dont send anything else
                return false;

            if (string.IsNullOrEmpty(smtpServer))
            {
                throw new Exception("Invalid configured smtp server");
            }

            MailMessage mailMessage = new MailMessage(from, to, subject, body);
            SmtpClient smtpClient = new SmtpClient(smtpServer);
            smtpClient.Send(mailMessage);
            return true;          
        }
    }
}
