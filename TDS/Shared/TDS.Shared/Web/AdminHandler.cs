/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Net;
using System.Web;
using AIR.Common.Configuration;
using AIR.Common.Web;

namespace TDS.Shared.Web
{
    /// <summary>
    /// This is used by the health check monitor.
    /// </summary>
    public class AdminHandler : HttpHandlerBase
    {
        public AdminHandler()
        {
            MapMethod("Shutdown", Shutdown);
            MapMethod("HealthCheck", HealthCheck);
        }

        /// <summary>
        /// Tell health check monitor everything is ok.
        /// </summary>
        protected void SendPassed()
        {
            SetStatus(HttpStatusCode.OK);
            WriteString("passed");
        }

        /// <summary>
        /// Tell health check monitor that request is unauthorized.
        /// </summary>
        protected void SendUnauthorized()
        {
            SetStatus(HttpStatusCode.Forbidden, "Invalid Password");
            WriteString("Invalid Password");
        }

        /// <summary>
        /// Tell health check monitor that there was a problem.
        /// </summary>
        protected void SendFailed(string message)
        {
            SetStatus(HttpStatusCode.InternalServerError);
            WriteString(message);
        }

        protected bool IsValidPassword()
        {
            string userPassword = GetQueryString("password");
            string adminPassword = AppSettingsHelper.Get("AdminPassword", "UnloadAppDomain1000tj");

            // make sure password was set
            if (userPassword == null || adminPassword == null) return false;

            // check if password is correct
            return Utility.IsMatch(userPassword, adminPassword);
        }

        private void Shutdown()
        {
            if (IsValidPassword())
            {
                HttpContext.Current.Response.Redirect("Default.aspx", false);
                HttpRuntime.UnloadAppDomain();
            }
            else
            {
                SetStatus(HttpStatusCode.Forbidden, "Invalid Password");
                WriteString("Invalid Password");
            }
        }

        protected virtual void HealthCheck()
        {
            if (IsValidPassword())
            {
                //healthChecks here
                SetStatus(HttpStatusCode.OK);
                WriteString("passed");
            }
            else
            {
                SetStatus(HttpStatusCode.Forbidden, "Invalid Password");
                WriteString("Invalid Password");
            }
        }
    }
}
