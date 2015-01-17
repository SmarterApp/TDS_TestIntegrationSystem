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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.TestResults;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Configuration;
using AIR.Configuration;
using TDSQASystemAPI.ExceptionHandling;
using TDSQASystemAPI.Utilities;
using TDSQASystemAPI.Routing.Authorization;

namespace TDSQASystemAPI.Routing
{
    public class RESTMultipartTarget : RESTTarget
    {
        public RESTMultipartTarget(string targetName, TargetClass targetClass, TargetType type, XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec, IFileTransformArgs transformArgs)
            : base(targetName, targetClass, type, xmlVersion, transformSpec, transformArgs) { }

        public RESTMultipartTarget(string targetName, TargetClass targetClass, TargetType type, XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec)
            : base(targetName, targetClass, type, xmlVersion, transformSpec)
        {
        }

        protected override HttpResponseMessage Post(WebService wsSettings, TestResult tr, string xml, OAuthResponse accessToken, params object[] inputArgs)
        {
            HttpResponseMessage response = null;
            using (HttpClient client = new HttpClient())
            {
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                    {
                        using (StreamContent fileContent = new StreamContent(ms))
                        {
                            ContentDispositionHeaderValue contentDispHeaderVal = new ContentDispositionHeaderValue("attachment");
                            contentDispHeaderVal.FileName = String.Format("{0}_{1}.xml", tr.Opportunity.Key, tr.Opportunity.ReportingVersion);
                            fileContent.Headers.ContentDisposition = contentDispHeaderVal;

                            content.Add(fileContent);

                            //string c = content.ReadAsStringAsync().Result; // testing

                            // if there's an oauth config associated with this service, add the key to the header.
                            if (wsSettings.Authorization != null)
                            {
                                if (accessToken == null)
                                    throw new NullReferenceException(String.Format("A valid oauth token is required because Authorization is configured for this service.  Service config name: {0},  Auth config name: {1}",
                                        wsSettings.Name, wsSettings.Authorization.Name));
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(accessToken.token_type, accessToken.access_token.ToString());
                            }

                            response = client.PostAsync(GetRequestUri(wsSettings, tr, inputArgs), content).Result;
                        }
                    }
                }
            }
            return response;
        }
    }
}
