/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace AIR.Common.Web
{
    public enum ContentType
    {
        Text, // text/plain
        Xml, // text/xml
        Html, // text/html
        Json, // application/json
        Javascript // text/javascript
    }

    /// <summary>
    /// Helper functions to work with the current ASP.NET context.
    /// </summary>
    public static class WebHelper
    {
        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1);

        /// <summary>
        /// Get the current ASP.NET HTTP request.
        /// </summary>
        [DebuggerHidden]
        public static HttpRequest GetCurrentRequest()
        {
            HttpContext context = HttpContext.Current;

            if (context != null)
            {
                try
                {
                    return context.Request;
                }
                catch
                {
                    // When using IIS integrated mode this can throw error if application is not started
                }
            }

            return null;
        }

        /// <summary>
        /// Get the current ASP.NET HTTP response.
        /// </summary>
        public static HttpResponse GetCurrentResponse()
        {
            HttpContext context = HttpContext.Current;

            if (context != null)
            {
                try
                {
                    return context.Response;
                }
                catch
                {
                    // When using IIS integrated mode this can throw error if application is not started
                }
            }

            return null;
        }

        /// <summary>
        /// Returns java style milliseconds based on epoch time
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - EPOCH).TotalMilliseconds;
        }

        public static void SetContentType(string contentType)
        {
            HttpContext.Current.Response.ContentType = contentType;
        }

        public static void SetContentType(ContentType contentType)
        {
            switch(contentType)
            {
                case ContentType.Text: SetContentType("text/plain"); break;
                case ContentType.Xml: SetContentType("text/xml"); break;
                case ContentType.Html: SetContentType("text/html"); break;
                case ContentType.Json: SetContentType("application/json"); break;
                case ContentType.Javascript: SetContentType("text/javascript"); break;
            }
        }

        /// <summary>
        /// Gets a string value from the query string, form, server variables or cookies.
        /// </summary>
        public static string GetString(string name)
        {
            return HttpContext.Current.Request.Params[name];
        }

        public static string GetQueryString(string name)
        {
            return HttpContext.Current.Request.QueryString[name];
        }

        public static T GetQueryValue<T>(string name, T defaultValue)
        {
            string queryValue = GetQueryString(name);
            if (queryValue != null) return queryValue.ConvertTo<T>();
            return defaultValue;
        }

        public static T GetQueryValue<T>(string name)
        {
            return GetQueryValue(name, default(T));
        }

        public static List<T> GetQueryValues<T>(string name)
        {
            List<T> values = new List<T>();

            string[] queryValues = HttpContext.Current.Request.QueryString.GetValues(name);

            if (queryValues != null)
            {
                foreach (string queryValue in queryValues)
                {
                    T value = queryValue.ConvertTo<T>();
                    values.Add(value);
                }
            }

            return values;
        }

        public static string GetFormString(string name)
        {
            return HttpContext.Current.Request.Form[name];
        }

        public static T GetFormValue<T>(string name, T defaultValue)
        {
            string formValue = GetFormString(name);
            if (formValue != null) return formValue.ConvertTo<T>();
            return defaultValue;
        }

        public static T GetFormValue<T>(string name)
        {
            return GetFormValue(name, default(T));
        }

        public static List<T> GetFormValues<T>(string name)
        {
            List<T> values = new List<T>();

            string[] formValues = HttpContext.Current.Request.Form.GetValues(name);

            if (formValues != null)
            {
                foreach (string queryValue in formValues)
                {
                    T value = queryValue.ConvertTo<T>();
                    values.Add(value);
                }
            }

            return values;
        }

        /// <summary>
        /// Write out a raw string to HTTP output stream.
        /// </summary>
        public static void WriteString(string value)
        {
            HttpContext.Current.Response.Write(value);
        }

        /// <summary>
        /// Write out a raw string to HTTP output stream.
        /// </summary>
        public static void WriteString(string value, params object[] values)
        {
            WriteString(string.Format(value, values));
        }

        /// <summary>
        /// Write out a string to HTTP output stream and set content type as JSON.
        /// </summary>
        public static void WriteJsonString(string value)
        {
            SetContentType(ContentType.Json);
            WriteString(value);
        }

        /// <summary>
        /// Writes out a C# object to the HTTP output stream.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="data">The object we are sending.</param>
        public static void WriteJsonObject<T>(T data)
        {
            SetContentType(ContentType.Json);

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            jsonSerializer.WriteObject(HttpContext.Current.Response.OutputStream, data);
        }

        /// <summary>
        /// Gets a C# object from HTTP input stream.
        /// </summary>
        public static T GetJsonObject<T>()
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            return (T)jsonSerializer.ReadObject(HttpContext.Current.Request.InputStream);
        }

        /// <summary>
        /// Parse a C# object and get back JSON string representation.
        /// </summary>
        public static string ParseJsonString<T>(T o)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));

            string json;

            using (MemoryStream stream = new MemoryStream())
            {
                jsonSerializer.WriteObject(stream, o);
                json = Encoding.UTF8.GetString(stream.ToArray());
            }

            return json;
        }


        /// <summary>
        /// Set caching headers to prevent http client caching.
        /// </summary>
        public static void PreventClientCache()
        {
            HttpContext httpContext = HttpContext.Current;
            if (httpContext == null) return;

            HttpResponse httpResponse = httpContext.Response;
            httpResponse.Cache.SetExpires(DateTime.UtcNow.AddDays(-30));
            httpResponse.Cache.SetValidUntilExpires(false);
            httpResponse.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            httpResponse.Cache.SetCacheability(HttpCacheability.NoCache);
            httpResponse.Cache.SetNoServerCaching();
            httpResponse.Cache.SetNoStore();
        }

        public static void SetContextValue(string key, object value)
        {
            if (value != null)
            {
                HttpContext context = HttpContext.Current;

                if (context != null)
                {
                    context.Items[key] = value;
                }
            }
        }

        public static T GetContextValue<T>(string key)
        {
            HttpContext context = HttpContext.Current;

            if (context != null)
            {
                object value = context.Items[key];

                if (value != null)
                {
                    return (T)value;
                }
            }

            return default(T);
        }


    }
}
