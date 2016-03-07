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
using AIR.Common.Json;

namespace AIR.Common.Web
{
    /// <summary>
    /// A simple cookie helper
    /// </summary>
    public static class CookieHelper
    {
        /// <summary>
        /// Set the value in a cookie. Use NULL to delete cookie.
        /// </summary>
        public static bool SetValue(string name, string value)
        {
            HttpRequest request = WebHelper.GetCurrentRequest();
            if (request == null) return false;

            HttpResponse response = WebHelper.GetCurrentResponse();
            if (response == null) return false;

            HttpCookie cookie;

            if (value != null)
            {
                // set value name in cookie
                cookie = new HttpCookie(name);
                cookie.Value = value;
            }
            else
            {
                // delete the cookie
                cookie = request.Cookies[name];

                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddYears(-30);
                }
            }

            if (cookie == null) return false;

            // set request cookie
            request.Cookies.Set(cookie);

            // set response cookie
            response.Cookies.Set(cookie);

            return true;
        }

        public static void Clear(string name)
        {
            HttpCookie cookie = new HttpCookie(name);          
            cookie.Expires = DateTime.Now.AddDays(-1);
            HttpResponse response = WebHelper.GetCurrentResponse();
            response.Cookies.Set(cookie);
        }

        /// <summary>
        /// Get the value from the cookie.
        /// </summary>
        public static string GetString(string name)
        {
            HttpRequest request = WebHelper.GetCurrentRequest();

            if (request != null)
            {
                HttpCookie clientCookie = request.Cookies.Get(name);

                if (clientCookie != null && !String.IsNullOrEmpty(clientCookie.Value))
                {
                    return clientCookie.Value;
                }
            }

            return null;
        }

        public static T GetValue<T>(string name, T defaultValue)
        {
            string cookieValue = GetString(name);
            if (cookieValue != null)
            {
                return cookieValue.ConvertTo<T>();
            }
            return defaultValue;
        }

        public static T GetValue<T>(string name)
        {
            return GetValue(name, default(T));
        }

        public static T GetObject<T>(string cookieName) where T : class
        {
            HttpRequest request = WebHelper.GetCurrentRequest();
            if (request == null) return null;

            // read from cooke
            HttpCookie cookie = request.Cookies[cookieName];
            if (cookie == null || String.IsNullOrWhiteSpace(cookie.Value))
            {
                return default(T);
            }

            // decode string
            string data = HttpUtility.UrlDecode(cookie.Value);

            return JsonHelper.Deserialize<T>(data);
        }

        public static bool SetObject<T>(string cookieName, T value) where T : class
        {
            HttpResponse response = WebHelper.GetCurrentResponse();
            if (response == null) return false;

            // serialize into string
            string data = JsonHelper.Serialize(value);

            // encode string
            data = HttpUtility.UrlEncode(data);

            // save to cookie
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Value = data;
            response.Cookies.Set(cookie);
            return true;
        }
        
    }

}
