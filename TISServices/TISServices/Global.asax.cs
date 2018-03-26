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
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using AIR.Configuration.Security;
using System.Web.Http;
using OSS.TIS.Security;

namespace TISServices
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // handle encrypted configuration data
            SecureConfigManager.Initialize(new NoEncryptionManager());

            //record start time by initializing the statistics recorder
            Utilities.Statistics.Initialize();

            // configure webapi routing
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "AssessmentApi",
                routeTemplate: "api/{controller}/{testPackageKey}",
                defaults: new { id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "TestPackageApi",
                routeTemplate: "api/{controller}/{testPackageKey}",
                defaults: new { id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //TISServices.Utilities.TISServicesLogger.Log("Application started");
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            TISServices.Utilities.TISServicesLogger.Log("Application stopped");
        }
    }
}