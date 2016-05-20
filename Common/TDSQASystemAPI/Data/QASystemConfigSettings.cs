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
using System.Globalization;

namespace TDSQASystemAPI.Data
{
    //TODO: make pluggable through service locator
    //TODO: stop passing this around now that it's globally accessible
    public class QASystemConfigSettings
    {
        const int DEFAULT_HOUR_TO_SEND_WARNING_SUMMARY = 18;

        public bool UpdateRB { get; private set; }
        public bool UpdateDoR { get; private set; }
        public bool SendToHandscoring { get; private set; }
        public bool IgnoreHandscoringDuplicates { get; private set; }
        public bool IgnoreHandscoringResetMissingOrig { get; private set; }
        public bool EmailAlertForWarnings { get; private set; }
        public double HourToSendWarningSummary { get; private set; }
        public int? LongDbCommandTimeout { get; private set; }
        public string Client { get; private set; }
        public string Environment { get; private set; }

        public static readonly QASystemConfigSettings Instance = new QASystemConfigSettings();

        private QASystemConfigSettings()
        {
            UpdateRB = String.IsNullOrEmpty(ConfigurationManager.AppSettings["UPDATE_RB"]) ? false : 
                Convert.ToBoolean(ConfigurationManager.AppSettings["UPDATE_RB"], CultureInfo.InvariantCulture);

            UpdateDoR = String.IsNullOrEmpty(ConfigurationManager.AppSettings["UpdateDoR"]) ? false : 
                Convert.ToBoolean(ConfigurationManager.AppSettings["UpdateDoR"], CultureInfo.InvariantCulture);

            SendToHandscoring = String.IsNullOrEmpty(ConfigurationManager.AppSettings["SendToHandscoring"]) ? false : 
                (ConfigurationManager.AppSettings["SendToHandscoring"].ToString().Equals("pretend", StringComparison.CurrentCultureIgnoreCase) 
                    || Convert.ToBoolean(ConfigurationManager.AppSettings["SendToHandscoring"], CultureInfo.InvariantCulture));

            IgnoreHandscoringDuplicates = String.IsNullOrEmpty(ConfigurationManager.AppSettings["IgnoreHandscoringDuplicates"]) ? false :
                Convert.ToBoolean(ConfigurationManager.AppSettings["IgnoreHandscoringDuplicates"], CultureInfo.InvariantCulture);

            // ignore the case where a reset is sent to HS w/o any preceding non-reset; this can happen if an incomplete test is reset
            //  instead of expired.  We generally don't care about this, so the default is ignore=true.
            IgnoreHandscoringResetMissingOrig = String.IsNullOrEmpty(ConfigurationManager.AppSettings["IgnoreHandscoringResetMissingOrig"]) ? true :
                Convert.ToBoolean(ConfigurationManager.AppSettings["IgnoreHandscoringResetMissingOrig"], CultureInfo.InvariantCulture);
            
            Client = ConfigurationManager.AppSettings["ClientName"];
            Environment = ConfigurationManager.AppSettings["Environment"];
            
            //if SendWarningsAsSummary is true, then we do not want to send email alerts for every warning. If it is false this should be true. So
            // take the opposite of what we find.
            bool sendWarningsAsSummary = String.IsNullOrEmpty(ConfigurationManager.AppSettings["SendWarningsAsSummary"]) ? true :
                Convert.ToBoolean(ConfigurationManager.AppSettings["SendWarningsAsSummary"], CultureInfo.InvariantCulture);
            EmailAlertForWarnings = !sendWarningsAsSummary;

            double hourToSendWarnings;
            if (!double.TryParse((ConfigurationManager.AppSettings["HourToSendWarningSummary"] ?? "").ToString(), out hourToSendWarnings))
                hourToSendWarnings = DEFAULT_HOUR_TO_SEND_WARNING_SUMMARY;
            HourToSendWarningSummary = hourToSendWarnings;

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["LongDbCommandTimeout"]))
                LongDbCommandTimeout = int.Parse(ConfigurationManager.AppSettings["LongDbCommandTimeout"]);

        }
    
    }
}
