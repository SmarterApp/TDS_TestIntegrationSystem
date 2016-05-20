/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Security;

namespace TDS.Shared.Security
{
    public class AuthorizationModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.PostAuthenticateRequest += PostAuthenticateRequest;
        }

        private static void PostAuthenticateRequest(object sender, EventArgs e)
        {
            // Get a reference to the current user
            IPrincipal principal = HttpContext.Current.User;

            // Check if we are dealing with an authenticated forms authentication request
            if (principal.Identity.IsAuthenticated && 
                principal.Identity.AuthenticationType == "Forms")
            {   
                FormsIdentity formsIdentity = principal.Identity as FormsIdentity;

                if (formsIdentity == null) return; // gaurd

                // Create StudentIdentity based on the FormsAuthenticationTicket           
                TDSIdentity identity = new TDSIdentity(formsIdentity.Ticket);

                // Create StudentPrincipal and get roles from FormsAuthenticationTicket
                TDSPrincipal tdsPrincipal = new TDSPrincipal(identity, formsIdentity.Ticket);

                // Attach the TDSPrincipal to HttpContext.User and Thread.CurrentPrincipal
                HttpContext.Current.User = tdsPrincipal;
                Thread.CurrentPrincipal = tdsPrincipal;
            }
        }

        public void Dispose() { }
    }
}
