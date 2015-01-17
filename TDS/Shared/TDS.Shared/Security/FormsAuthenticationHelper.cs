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
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;

namespace TDS.Shared.Security
{
    public class FormsAuthenticationHelper
    {
        public static FormsAuthenticationTicket ExtractTicketFromCookie()
        {
            return ExtractTicketFromCookie(HttpContext.Current, FormsAuthentication.FormsCookieName);
        }

        public static FormsAuthenticationTicket ExtractTicketFromCookie(HttpContext context, string name)
        {
            FormsAuthenticationTicket ticket = null;

            // get cookie
            HttpCookie cookie = context.Request.Cookies[name];
            if (cookie == null) return null;
            
            // get encrypted cookie value
            string encryptedTicket = cookie.Value;

            // decrypt cookie value
            try
            {
                ticket = FormsAuthentication.Decrypt(encryptedTicket);
            }
            catch
            {
                // remove this cookie it caused a serious error it is not valid
                context.Request.Cookies.Remove(name);
            }

            return ticket;
        }

    }
}
