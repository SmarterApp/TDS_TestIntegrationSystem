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
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;
using TDS.Shared.Data; using TDS.Shared.Configuration;
using TDS.Shared.Exceptions;
using TDS.Shared.Logging;
using TDS.Shared.Security;

namespace TDS.Shared.Web
{
    /// <summary>
    /// Base class for any HTTP handlers
    /// </summary>
    public abstract class RESTHandler: IHttpHandler
    {
        private Dictionary<string, Action> actions = new Dictionary<string, Action>(StringComparer.CurrentCultureIgnoreCase);

        private HttpContext _currentContext;

        public HttpContext CurrentContext
        {
            get { return _currentContext; }
        }

        private bool _debug = false;

        public RESTHandler()
        {
        }

        public void ProcessRequest(HttpContext context)
        {
            this._currentContext = context;

            if (_currentContext.Request.PathInfo.Length == 0)
            {
                ThrowError(HttpStatusCode.NotFound, "No method name was provided.");
            }

            string methodName = _currentContext.Request.PathInfo.Remove(0, 1);

            if (string.IsNullOrEmpty(methodName))
            {
                ThrowError(HttpStatusCode.NotFound, "Empty method name was provided.");
            }

            Action methodAction;

            // check if action matches a mapping
            if (actions.TryGetValue(methodName, out methodAction))
            {
                // run method
                try
                {
                    methodAction();
                }
                catch (TDSHttpException te)
                {
                    SetStatus((HttpStatusCode)te.GetHttpCode(), te.Message); // intentional exception thrown with HTTP error codes provided
                }
                catch(ReturnStatusException rse) // explicit error condition returned from SP
                {
                    SetStatus(HttpStatusCode.Forbidden, rse.ReturnStatus.Reason); // 403
                }
                catch (ThreadAbortException tae)
                {
                }
                catch(Exception e)
                {
                    TDSLogger.Application.Fatal(e);
                    
                    if (_debug)
                    {
                        SetStatus(HttpStatusCode.InternalServerError, e.Message);
                    }
                    else
                    {
                        SetStatus(HttpStatusCode.InternalServerError);
                    }
                }
            }
            else
            {
                // method not found!
                TDSLogger.Application.Error(string.Format("The method '{0}' was not found.", methodName));
                SetStatus(HttpStatusCode.NotFound); // 500
            }
        }

        /// <summary>
        /// Maps path to a method
        /// </summary>
        /// <param name="name">PathInfo of the url 
        /// (e.x., you would register "methodName" if the url looked like: http://example.com/handler.ashx/methodName?param=test)</param>
        /// <param name="action">the function to call</param>
        protected void MapMethod(string name, Action action)
        {
            actions.Add(name, action);
        }

        protected void SetContentType(ContentType contentType)
        {
            switch(contentType)
            {
                case ContentType.Text: _currentContext.Response.ContentType = "text/plain"; break; 
                case ContentType.Xml: _currentContext.Response.ContentType = "text/xml"; break; 
                case ContentType.Html: _currentContext.Response.ContentType = "text/html"; break; 
                case ContentType.Json: _currentContext.Response.ContentType = "application/json"; break; 
            }
        }

        /// <summary>
        /// Sends a json object to the response stream
        /// </summary>
        /// <param name="value"></param>
        protected void SendJsonString(string value)
        {
            SetContentType(ContentType.Json);
            _currentContext.Response.Write(value);
        }

        

        protected string GetQueryString(string name)
        {
            return _currentContext.Request.QueryString[name];
        }

        protected T GetQueryObject<T>(string name)
        {
            T value = default(T);
            string queryValue = _currentContext.Request.QueryString[name];
            
            if (!string.IsNullOrEmpty(queryValue))
            {
                value = ChangeType<T>(queryValue);
            }

            return value;
        }

        protected List<T> GetQueryObjects<T>(string name)
        {
            List<T> values = new List<T>();

            string[] queryValues = _currentContext.Request.QueryString.GetValues(name);

            foreach (string queryValue in queryValues)
            {
                T value = ChangeType<T>(queryValue);
                values.Add(value);
            }

            return values;
        }

        protected string GetFormString(string name)
        {
            string queryValue = _currentContext.Request.Form[name];

            if (string.IsNullOrEmpty(queryValue))
            {
                return string.Empty;
            }
            else
            {
                return queryValue;
            }
        }

        protected T GetFormObject<T>(string name)
        {
            T value = default(T);
            string queryValue = _currentContext.Request.Form[name];
            
            if (!string.IsNullOrEmpty(queryValue))
            {
                value = ChangeType<T>(queryValue);
            }

            return value;
        }

        protected List<T> GetFormObjects<T>(string name)
        {
            List<T> values = new List<T>();

            string[] queryValues = _currentContext.Request.Form.GetValues(name);

            foreach (string queryValue in queryValues)
            {
                T value = ChangeType<T>(queryValue);
                values.Add(value);
            }

            return values;
        }

        private static T ChangeType<T>(object value)
        {
            Type conversionType = typeof(T);

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null) return default(T);

                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            return (T)Convert.ChangeType(value, conversionType);
        }

        protected void SetStatus(HttpStatusCode statusCode, string statusDescription, params object[] values)
        {
            _currentContext.Response.StatusCode = (int) statusCode;

            if (!string.IsNullOrEmpty(statusDescription))
            {
                _currentContext.Response.StatusDescription = string.Format(statusDescription, values);
            }
        }

        protected void SetStatus(HttpStatusCode statusCode)
        {
            SetStatus(statusCode, null);
        }

        protected static void ThrowError(HttpStatusCode statusCode, string statusDescription, params object[] values)
        {
            throw new TDSHttpException(statusCode, string.Format(statusDescription, values));
        }

        protected static void CheckAuthenticated()
        {
            if (TDSIdentity.Current == null || !TDSIdentity.Current.IsAuthenticated)
            {
                throw new TDSHttpException(HttpStatusCode.Forbidden, "The request requires user authentication");
            }
        }

        /// <summary>
        /// Determines if this httphandler will be kept around for use by all requests (meaning you shouldn't keep state in internal fields)
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        // write out to http stream
        public void Write(string s, params object[] values)
        {
            this.CurrentContext.Response.Write(string.Format(s, values));
        }

        // below is WCF serializers
        public void WriteDataContractJson<T>(T o)
        {
            SetContentType(ContentType.Json);
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            jsonSerializer.WriteObject(CurrentContext.Response.OutputStream, o);
        }

        public static string ReadDataContractJsonString<T>(T o)
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

        public T ReadDataContractJson<T>()
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            return (T)jsonSerializer.ReadObject(CurrentContext.Request.InputStream);
        }

        static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        /// <summary>
        /// Returns java style milliseconds based on epoch time
        /// </summary>
        /// <returns></returns>
        public long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Epoch).TotalMilliseconds;
        }

        public enum ContentType
        {
            Text, // text/plain
            Xml, // text/xml
            Html, // text/html
            Json // application/json
        }
    }
}