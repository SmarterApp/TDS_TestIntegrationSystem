/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization.Json;
using System.Web;

namespace AIR.Common.Web
{
    public static class HttpRequestExtensions
    {
        private static string GetQueryString(this HttpRequestBase request, string name, bool validate)
        {
            string value = request.QueryString[name];

            // if validation is enabled verify if they key exists
            if (value == null && validate && !request.QueryString.AllKeys.Contains(name))
            {
                const string error = "A required query parameter was not specified for this request. [{0}]";
                throw new HttpException(400, String.Format(error, name));
            }

            return value;
        }

        public static string GetQueryString(this HttpRequestBase request, string name)
        {
            return GetQueryString(request, name, true); // validate
        }

        public static string GetQueryString(this HttpRequestBase request, string name, string defaultValue)
        {
            return GetQueryString(request, name, false) ?? defaultValue;
        }

        public static T GetQueryString<T>(this HttpRequestBase request, string name, T defaultValue, bool validate)
        {
            string queryValue = GetQueryString(request, name, validate);
            
            if (queryValue != null)
            {
                try
                {
                    return queryValue.ConvertTo<T>();
                }
                catch
                {
                    const string error = "Could not convert the query parameter to the designated type. [{0}]";
                    throw new HttpException(400, String.Format(error, name));
                }
            }

            return defaultValue;
        }

        public static T GetQueryString<T>(this HttpRequestBase request, string name, T defaultValue)
        {
            return GetQueryString(request, name, defaultValue, false);
        }

        public static T GetQueryString<T>(this HttpRequestBase request, string name)
        {
            return GetQueryString(request, name, default(T), true);
        }

        public static List<T> GetQueryList<T>(this HttpRequestBase request, string name)
        {
            List<T> values = new List<T>();

            string[] queryValues = request.QueryString.GetValues(name);

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

        private static string GetForm(this HttpRequestBase request, string name, bool validate)
        {
            string value = request.Form[name];

            // if validation is enabled verify if they key exists
            if (value == null && validate && !request.Form.AllKeys.Contains(name))
            {
                const string error = "A required form parameter was not specified for this request. [{0}]";
                throw new HttpException(400, String.Format(error, name));
            }

            return value;
        }

        public static string GetForm(this HttpRequestBase request, string name)
        {
            return GetForm(request, name, true); // validate
        }

        public static string GetForm(this HttpRequestBase request, string name, string defaultValue)
        {
            return GetForm(request, name, false) ?? defaultValue;
        }

        private static T GetForm<T>(this HttpRequestBase request, string name, T defaultValue, bool validate)
        {
            string formValue = GetForm(request, name, validate);
            
            if (formValue != null)
            {
                try
                {
                    return formValue.ConvertTo<T>();
                }
                catch
                {
                    const string error = "Could not convert the form parameter to the designated type. [{0}]";
                    throw new HttpException(400, String.Format(error, name));
                }
            }

            return defaultValue;
        }

        public static T GetForm<T>(this HttpRequestBase request, string name, T defaultValue)
        {
            return GetForm(request, name, defaultValue, false);
        }

        public static T GetForm<T>(this HttpRequestBase request, string name)
        {
            return GetForm(request, name, default(T), true); // validate
        }

        public static List<T> GetFormList<T>(this HttpRequestBase request, string name)
        {
            List<T> values = new List<T>();

            string[] formValues = request.Form.GetValues(name);

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
        /// Gets a C# object from HTTP input stream.
        /// </summary>
        public static T GetJsonObject<T>(this HttpRequestBase request)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            return (T)jsonSerializer.ReadObject(request.InputStream);
        }
        
    }

}
