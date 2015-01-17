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
using System.Web;
using AIR.Common.Configuration;
using AIR.Common.Web;

namespace TDS.Shared.Configuration
{
    /// <summary>
    /// General application settings used by all TDS systems.
    /// </summary>
    public class TDSSettings
    {
        /// <summary>
        /// Get the session DB connection string
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["SESSION_DB"];
            return (connectionStringSettings != null) ? connectionStringSettings.ToString() : null;
        }

        /// <summary>
        /// Get the current application name.
        /// </summary>
        public static string GetAppName()
        {
            string appName = AppSettingsHelper.Get("AppName");
            if (appName != null) appName = appName.Trim();
            return appName;
        }

        /// <summary>
        /// Get the session type name from the web.config.
        /// </summary>
        /// <remarks>
        /// 0 = Normal
        /// 1 = ProctorAsStudent
        /// </remarks>
        public static int GetSessionType()
        {
            return AppSettingsHelper.GetInt32("SessionType");
        }

        public static string GetCookieName(string typeName)
        {
            string appName = GetAppName();
            return String.Format("TDS-{0}-{1}", appName, typeName);
        }

        /// <summary>
        /// Store client name in a cookie. Use NULL to delete client name.
        /// </summary>
        public static void SetClientName(string clientName)
        {
            string cookieName = GetCookieName("Client");
            CookieHelper.SetValue(cookieName, clientName);
        }

        /// <summary>
        /// Get the client name from the querystring.
        /// </summary>
        public static string GetClientNameFromQueryString()
        {
            HttpRequest request = WebHelper.GetCurrentRequest();

            if (request != null)
            {
                string clientName = request.QueryString.Get("c") ?? 
                                    request.QueryString.Get("client");
                if (!String.IsNullOrEmpty(clientName)) return clientName;
            }

            return null;
        }

        /// <summary>
        /// Get the client name from the cookie.
        /// </summary>
        public static string GetClientNameFromCookie()
        {
            string cookieName = GetCookieName("Client"); // e.x., TDS-Student-Client
            return CookieHelper.GetString(cookieName);
        }
        
        /// <summary>
        /// Get the client name from the web.config.
        /// </summary>
        public static string GetClientNameFromConfig()
        {
            return AppSettingsHelper.Get("ClientName");
        }

        /// <summary>
        /// Get the current client name.
        /// </summary>
        public static string GetClientName()
        {
            bool useClientQueryString = AppSettingsHelper.GetBoolean("ClientQueryString", false);
            bool useClientCookie = AppSettingsHelper.GetBoolean("ClientCookie", false);

            // first check the querystring
            if (useClientQueryString)
            {
                // first check the querystring
                string clientName = GetClientNameFromQueryString();
                if (!String.IsNullOrEmpty(clientName)) return clientName;
            }

            // second check the cookie
            if (useClientCookie)
            {
                string clientName = GetClientNameFromCookie();
                if (!String.IsNullOrEmpty(clientName)) return clientName;
            }

            // finally check the web.config
            return GetClientNameFromConfig();
        }
    }
}
