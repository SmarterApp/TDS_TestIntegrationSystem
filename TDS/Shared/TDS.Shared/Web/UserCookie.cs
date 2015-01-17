/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Web;
using AIR.Common.Utilities;

namespace TDS.Shared.Web
{
    public class UserCookie
    {
        private HttpContext thisContext;
        private string cookieName;
        private HttpCookie cookie;
        private long cookieTimeout;
        private bool bEncryption = true;
        private bool bHttpOnly = true;

        public UserCookie(HttpContext thisContext, string cookieName)
        {
            this.thisContext = thisContext;
            this.cookieName = cookieName;
            cookie = thisContext.Request.Cookies[cookieName];
        }
        public UserCookie(HttpContext thisContext, string cookieName, long cookieTimeout)
            : this(thisContext, cookieName)
        {
            this.cookieTimeout = cookieTimeout;
        }
        public UserCookie(HttpContext thisContext, string cookieName, bool bEncryption)
            : this(thisContext, cookieName)
        {
            this.bEncryption = bEncryption;
        }

        public UserCookie(HttpContext thisContext, string cookieName, bool bEncryption, bool bHttpOnly)
            : this(thisContext, cookieName, bEncryption)
        {
            this.bHttpOnly = bHttpOnly;
        }


        public HttpCookie Cookie
        {
            get
            {
                if (cookie == null)
                {
                    cookie = new HttpCookie(cookieName);
                    cookie.HttpOnly = bHttpOnly;
                    //cookie.Expires = DateTime.Now.AddMinutes(cookieTimeout);
                    thisContext.Response.Cookies.Add(cookie);
                }
                return cookie;
            }
            set { cookie = value; }
        }
        public bool HasValues
        {
            get { return Cookie.HasKeys; }
        }
        /// <summary>
        /// Response.Cookies
        /// </summary>
        public void ExpiresCookie()
        {
            try
            {
                if (Cookie != null)
                {
                    Cookie.Expires = DateTime.Now.AddDays(-1);
                    thisContext.Response.Cookies.Set(Cookie);
                    Cookie = null;
                }
            }
            catch { ;}
        }
        public static void ExpiresCookie(HttpContext thisContext, string cookieName)
        {
            try
            {
                HttpCookie cookie = thisContext.Response.Cookies[cookieName];
                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    thisContext.Response.Cookies.Set(cookie);
                }
            }
            catch { ;}
        }
        public void RemoveCookie()
        {
            try
            {
                thisContext.Response.Cookies.Remove(cookieName);
                ExpiresCookie();
            }
            catch { ;}
        }
        public static void RemoveCookie(HttpContext thisContext, string cookieName)
        {
            thisContext.Request.Cookies.Remove(cookieName);
            HttpCookie cookie = thisContext.Response.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                thisContext.Response.Cookies.Set(cookie);
                cookie = null;
            }
        }
        public string GetValue(string key)
        {
            string value = Cookie[key];
            if (value == null)
                return "";
            if (bEncryption)
                return Encryption.UnScrambleText("", value);
            else
                return value;
        }

        
        public void SetValue(string key, string value)
        {
            if (value == null)
                value = "";
            Cookie.HttpOnly = bHttpOnly;
            if (bEncryption)
                Cookie.Values[key] = Encryption.ScrambleText("", value);
            else
                Cookie.Values[key] = value;
            thisContext.Response.Cookies.Set(Cookie); // save to response output
        }
    }
}
