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

namespace AIR.Common.Web
{
    public class HttpWorkerHelper
    {
        
        static public HttpWorkerRequest GetWorkerRequest()
        {
            IServiceProvider provider = (IServiceProvider)HttpContext.Current;
            HttpWorkerRequest wr = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
            return wr;
        }

        static public string GetNIC()
        {
            HttpWorkerRequest wr = GetWorkerRequest();
            return wr.GetLocalAddress();
        }

        // e.x., pass in HttpWorkerRequest.HeaderAcceptEncoding
        static public string GetHeader(int requestHeader)
        {
            HttpWorkerRequest wr = GetWorkerRequest();
            return wr.GetKnownRequestHeader(requestHeader); 
        }

        static public HttpVerb HttpVerb
        {
            get
            {
                HttpVerb _httpVerb = HttpVerb.Unparsed;

                HttpContext context = HttpContext.Current;

                if (context == null || context.Request == null || string.IsNullOrEmpty(context.Request.HttpMethod))
                {
                    return _httpVerb;
                }

                _httpVerb = HttpVerb.Unknown;
                string httpMethod = context.Request.HttpMethod;

                switch (httpMethod.Length)
                {
                    case 3:
                        if (httpMethod != "GET")
                        {
                            if (httpMethod == "PUT")
                            {
                                _httpVerb = HttpVerb.PUT;
                            }
                            break;
                        }
                        _httpVerb = HttpVerb.GET;
                        break;

                    case 4:
                        if (httpMethod != "POST")
                        {
                            if (httpMethod == "HEAD")
                            {
                                _httpVerb = HttpVerb.HEAD;
                            }
                            break;
                        }
                        _httpVerb = HttpVerb.POST;
                        break;

                    case 5:
                        if (httpMethod == "DEBUG")
                        {
                            _httpVerb = HttpVerb.DEBUG;
                        }
                        break;

                    case 6:
                        if (httpMethod == "DELETE")
                        {
                            _httpVerb = HttpVerb.DELETE;
                        }
                        break;
                }
                return _httpVerb;
            }                
        }
    }
}
