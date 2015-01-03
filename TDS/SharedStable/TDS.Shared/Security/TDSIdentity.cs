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
using AIR.Common.Web;

namespace TDS.Shared.Security
{
    /// <summary>
    /// Represents a ASP.NET identity for a testee. This is backed by a secure encrypted cookie.
    /// </summary>
    /// <remarks>
    /// http://stackoverflow.com/questions/549/the-definitive-guide-to-forms-based-website-authentication
    /// </remarks>
    public class TDSIdentity : IIdentity
    {
        private FormsAuthenticationTicket ticket;
        private WebValueCollection values = new WebValueCollection();

        public TDSIdentity(FormsAuthenticationTicket ticket)
        {
            this.ticket = ticket;

            ParseTicketData();
        }

        private void ParseTicketData()
        {
            // try to parse tickets userData
            if (string.IsNullOrEmpty(ticket.UserData)) return;
            values.FillFromString(ticket.UserData);
        }

        /// <summary>
        /// SSID
        /// </summary>
        public string Name
        {
            get { return ticket.Name; }
        }

        public string AuthenticationType
        {
            get { return "TDS"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }


        public WebValueCollection Values
        {
            get { return values; }
        }

        public static TDSPrincipal Principal
        {
            get
            {
                return Thread.CurrentPrincipal as TDSPrincipal;
            }
        }

        public static TDSIdentity Current
        {
            get
            {
                return Thread.CurrentPrincipal.Identity as TDSIdentity;
            }
        }

        /// <summary>
        /// Creates a ASP.NET forms auth ticket.
        /// </summary>
        private static FormsAuthenticationTicket CreateTicket(string name, string userData)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, name, DateTime.Now, DateTime.Now.AddMinutes(150), true, userData);
            return ticket;
        }

        /// <summary>
        /// Creates a ASP.NET cookie with encrypted forms auth ticket.
        /// </summary>
        private static HttpCookie CreateCookie(string name, string userData)
        {
            // create forms ticket
            FormsAuthenticationTicket ticket = CreateTicket(name, userData);
            
            // Create the cookie that contains the forms authentication ticket
            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(name, false);
            // authCookie.HttpOnly = false;
            //authCookie.Expires = ticket.Expiration;

            // Update the authCookie's Value to use the encrypted version of newTicket
            authCookie.Value = FormsAuthentication.Encrypt(ticket);

            return authCookie;
        }

        /// <summary>
        /// Adds a ASP.NET cookie with encrypted forms auth ticket along with user data.
        /// </summary>
        public static void New(string name, WebValueCollection values)
        {
            // create encrypted cookie
            string userData = (values == null) ? string.Empty : values.ToString(false);
            HttpCookie authCookie = CreateCookie(name, userData);

            // Manually add the authCookie to the Cookies collection
            HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        /// <summary>
        /// Adds a ASP.NET cookie with encrypted forms auth ticket along with user data.
        /// </summary>
        public void Save()
        {
            // create encrypted cookie
            string userData = (values == null) ? string.Empty : values.ToString(false);
            HttpCookie authCookie = CreateCookie(this.Name, userData);

            // Manually add the authCookie to the Cookies collection
            HttpContext.Current.Response.Cookies.Add(authCookie);
        }

    }
}
