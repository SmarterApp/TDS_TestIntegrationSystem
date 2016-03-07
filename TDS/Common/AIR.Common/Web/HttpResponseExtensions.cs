/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using AIR.Common.Json;

namespace AIR.Common.Web
{
    public static class HttpResponseExtensions
    {
        public static void SetContentType(this HttpResponseBase response, ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.Text: response.ContentType = "text/plain"; break;
                case ContentType.Xml: response.ContentType = "text/xml"; break;
                case ContentType.Html: response.ContentType = "text/html"; break;
                case ContentType.Json: response.ContentType = "application/json"; break;
                case ContentType.Javascript: response.ContentType = "text/javascript"; break;
            }
        }

        /// <summary>
        /// Write out a raw string to HTTP output stream.
        /// </summary>
        public static void WriteString(this HttpResponseBase response, string value)
        {
            response.Write(value);
        }

        /// <summary>
        /// Write out a raw string to HTTP output stream.
        /// </summary>
        public static void WriteString(this HttpResponseBase response, string value, params object[] values)
        {
            WriteString(response, string.Format(value, values));
        }
        
        /// <summary>
        /// Writes out a C# object to the HTTP output stream.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="data">The object we are sending.</param>
        public static void WriteJsonObject<T>(this HttpResponseBase response, T data)
        {
            SetContentType(response, ContentType.Json);
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            jsonSerializer.WriteObject(response.OutputStream, data);
        }

        public static void SetStatus(this HttpResponseBase response, HttpStatusCode statusCode, string statusDescription, params object[] values)
        {
            response.StatusCode = (int)statusCode;

            if (!string.IsNullOrEmpty(statusDescription))
            {
                // NOTE: You can't have a linebreak in the middle of an HTTP header.
                if (statusDescription.Contains(Environment.NewLine))
                {
                    statusDescription = statusDescription.Replace(Environment.NewLine, " ");
                }

                response.StatusDescription = string.Format(statusDescription, values);
            }
        }

        public static void SetStatus(this HttpResponseBase response, HttpStatusCode statusCode)
        {
            SetStatus(response, statusCode, null);
        }
    }

}
