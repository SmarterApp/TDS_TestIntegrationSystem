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
    public class RESTTarget : Target
    {
        public RESTTarget(string targetName, TargetClass targetClass, TargetType type, XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec, IFileTransformArgs transformArgs)
            : base(targetName, targetClass, type, xmlVersion, transformSpec, transformArgs) { }

        public RESTTarget(string targetName, TargetClass targetClass, TargetType type, XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec)
            : base(targetName, targetClass, type, xmlVersion, transformSpec)
        {
        }

        public override ITargetResult Send(TestResult tr, Action<object> outputProcessor, params object[] inputArgs)
        {
            // the target's name will be the WebSite's name
            WebService wsSettings = Settings.WebService[base.Name];

            if (wsSettings == null)
                throw new QAException(String.Format("There is no WebService configured with name: {0}", base.Name), QAException.ExceptionType.ConfigurationError);

            string xml = null;
            try
            {
                xml = base.GetPayloadAsString(tr);
            }
            catch (Exception ex)
            {
                throw new QAException(String.Format("Error preparing file for Target: {0}. OppID: {1}, Message: {2}", base.Name, tr.Opportunity.OpportunityID, ex.Message), QAException.ExceptionType.General, ex);
            }

            try
            {
                //TODO: should probably move this into the QASystemMainThread, since it will apply globally.
                //  We've been setting this before each call, so I'm leaving it as-is for now.
                if (!ConfigurationManager.AppSettings["Environment"].Equals("Production", StringComparison.InvariantCultureIgnoreCase))
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(SSLResult);

                bool authTokenFoundInCache = false;
                OAuthResponse oauthToken = null;

                if(wsSettings.Authorization != null)
                    oauthToken = OAuth.GetResponse(wsSettings.Authorization, out authTokenFoundInCache);

                HttpResponseMessage response = Post(wsSettings, tr, xml, oauthToken, inputArgs);

                if ((response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden) && authTokenFoundInCache)
                {
                    // try again with a fresh token; may have expired
                    OAuth.RemoveFromCache(wsSettings.Authorization, oauthToken);
                    oauthToken = OAuth.GetResponse(wsSettings.Authorization, out authTokenFoundInCache);
                    response = Post(wsSettings, tr, xml, oauthToken, inputArgs);
                }

                string result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new QAException(String.Format("Error posting file for oppID: {0} to Target: {1}.  Status Code: {2}, Result: {3}", tr.Opportunity.OpportunityID, base.Name, response.StatusCode, result), QAException.ExceptionType.General);
                }

                if (outputProcessor != null)
                    outputProcessor(result);
            }
            catch (Exception ex)
            {
                throw new QAException(String.Format("Exception posting file for oppId: {0} to Target: {1}.  Message: {3}", tr.Opportunity.OpportunityID, base.Name, ex.Message), QAException.ExceptionType.General, ex);
            }

            return new TargetResult() { Sent = true };
        }

        protected virtual HttpResponseMessage Post(WebService wsSettings, TestResult tr, string xml, OAuthResponse accessToken, params object[] inputArgs)
        {
            HttpResponseMessage response = null;
            using (HttpClient client = new HttpClient())
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    using (StreamContent fileContent = new StreamContent(ms))
                    {
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

                        // if there's an oauth config associated with this service, add the auth key to the header.
                        if (wsSettings.Authorization != null)
                        {
                            if (accessToken == null)
                                throw new NullReferenceException(String.Format("A valid oauth token is required because Authorization is configured for this service.  Service config name: {0},  Auth config name: {1}", 
                                    wsSettings.Name, wsSettings.Authorization.Name));
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(accessToken.token_type, accessToken.access_token.ToString());
                        }

                        response = client.PostAsync(GetRequestUri(wsSettings, tr, inputArgs), fileContent).Result;
                    }
                }
            }
            return response;
        }

        protected virtual string GetRequestUri(WebService wsSettings, TestResult tr, params object[] inputArgs)
        {
            if (inputArgs == null || inputArgs.Length == 0)
                return wsSettings.URL;
            else
                return String.Format(wsSettings.URL, inputArgs.Select(a => Uri.EscapeDataString(a.ToString())));
        }

        private bool SSLResult(object sender, X509Certificate c, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
