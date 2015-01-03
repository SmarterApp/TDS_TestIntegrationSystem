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
using AIR.Common.Configuration;

namespace TDS.ScoringDaemon.Abstractions
{
    public static class ScoringDaemonSettings
    {
        public static DateTime AppStartTime { get; set; }

        public static DateTime CloudLastPollTime { get; set; }

        public static bool DebugEnabled{ get; set; }        

        public static int CloudTimerIntervalSeconds
        {
            get { return AppSettings.Get<int>("ScoringDaemon.CloudTimerIntervalSeconds", 14400); }
        }
        
        public static int HubTimerIntervalSeconds
        {
            get { return AppSettings.Get<int>("ScoringDaemon.HubTimerIntervalSeconds", 300); }
        }

        public static int CallbackThreadPoolCount
        {
            get { return AppSettings.Get<int>("ScoringDaemon.CallbackThreadPoolCount", 20); }
        }

        public static int CallbackThreadPoolHighWaterMark
        {
            get { return AppSettings.Get<int>("ScoringDaemon.CallbackThreadPoolHighWaterMark", 900); }
        }
        
        public static int CallbackThreadPoolLowWaterMark
        {
            get { return AppSettings.Get<int>("ScoringDaemon.CallbackThreadPoolLowWaterMark", 750); }
        }

        public static int ScoreRequestThreadPoolCount
        {
            get { return AppSettings.Get<int>("ScoringDaemon.ScoreRequestThreadPoolCount", 5); }
        }

        public static int ScoreRequestThreadPoolHighWaterMark
        {
            get { return AppSettings.Get<int>("ScoringDaemon.ScoreRequestThreadPoolHighWaterMark", 900); }
        }

        public static int ScoreRequestThreadPoolLowWaterMark
        {
            get { return AppSettings.Get<int>("ScoringDaemon.ScoreRequestThreadPoolLowWaterMark", 750); }
        }

        public static String MachineName
        {
            get { return AppSettings.Get<String>("ScoringDaemon.MachineName", Environment.MachineName); }
        }

        public static int PendingMins
        {
            get { return AppSettings.Get<int>("ScoringDaemon.PendingMins", 15); }
        }

        public static int MinAttempts
        {
            get { return AppSettings.Get<int>("ScoringDaemon.MinAttempts", 0); }
        }

        public static int MaxAttempts
        {
            get { return AppSettings.Get<int>("ScoringDaemon.MaxAttempts", 10); }
        }

        public static int SessionType
        {
            get { return AppSettings.Get<int>("ScoringDaemon.SessionType", 0); }
        }

        public static int MaxItemsReturned
        {
            get { return AppSettings.Get<int>("ScoringDaemon.MaxItemsReturned", 500); }
        }

        public static long ItemScoringConfigCacheSeconds
        {
            get { return AppSettings.Get<long>("ScoringDaemon.ItemScoringConfigCacheSeconds", 14400); }
        }

        [SecureAttribute]
        public static String CloudConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["GEO:Cloud"].ToString(); }
        }

        public static String HubConnectionString(String dbIP , String dbName )
        {
            return (new SqlConnectionStringBuilder(CloudConnectionString)
            {
                InitialCatalog = dbName,
                DataSource = dbIP
            }).ToString();
        }

        public static String ItemBankConnectionString(string dbIP, string dbName)
        {
            return (new SqlConnectionStringBuilder(CloudConnectionString)
            {
                InitialCatalog = dbName,
                DataSource = dbIP
            }).ToString();
        }

        private static String _itemScoringCallBackHostUrl = null;
        public static String ItemScoringCallBackHostUrl
        {
            get { return AppSettings.Get<String>("ScoringDaemon.ItemScoringCallBackHostUrl", _itemScoringCallBackHostUrl); }
            set { _itemScoringCallBackHostUrl = value; }
        }

        public static String StudentAppUrlOverride
        {
            get { return AppSettings.Get<String>("ScoringDaemon.StudentAppUrlOverride", null); }
        }

        // This pref allows us to use localhost if the studentapp and item scoring are colocated.
        // HOWEVER, make sure that localhost resolves correctly on the server farm used. Often times, 
        // we have multiple websites on a single IIS instance and localhost only works for the default website.
        public static bool EnableLocalHostUsageForColocatedApps
        {
            get { return AppSettings.Get<bool>("ScoringDaemon.EnableLocalHostUsageForColocatedApps", false); }
        }

        public static String ItemScoringServerUrlOverride
        {
            get { return AppSettings.Get<String>("ScoringDaemon.ItemScoringServerUrlOverride", null); }
        }        

        public static bool PreferHubPrivateIP
        {
            get { return AppSettings.Get<bool>("ScoringDaemon.PreferHubPrivateIP", true); }
        }

        public static String HubIP(DataStoreInfo serviceInfo)
        {            
            // private IP can sometimes be null so dont rely on it being available all the time
            return PreferHubPrivateIP && !String.IsNullOrEmpty(serviceInfo.PrivateIP) ? serviceInfo.PrivateIP : serviceInfo.PublicIP;
        }

        /// <summary>
        ///  List of item formats for which we dont send the rubric file path but instead the item key
        /// </summary>        
        public static String[] ItemFormatsRequiringItemKeysForRubric
        {
            get
            {
                string csv = AppSettings.Get("ScoringDaemon.ItemFormatsRequiringItemKeysForRubric", "ER");
                return csv.Split(',');
            }
        }
    }
}
