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
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using TDSQASystemAPI.Utilities;
using AIR.Configuration;
using TISServices.Utilities;

namespace TISServices.Authorization
{
    public class AuthorizeOpenAMAttribute : AuthorizationFilterAttribute
    {
        private static readonly string OpenAMUrl;

        static AuthorizeOpenAMAttribute()
        {
            OpenAMUrl = (Settings.WebService["OpenAM"] == null) ? null : Settings.WebService["OpenAM"].URL;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

            // if we don't have the info we need to authenticate, then do not authenticate
            Guid authKey;
            if (String.IsNullOrEmpty(OpenAMUrl) || actionContext.Request.Headers.Authorization == null || !Guid.TryParse(actionContext.Request.Headers.Authorization.Parameter, out authKey))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Not authorized to access this endpoint.") };
                Utilities.Statistics.AddToUnauthenticatedRequestCount();
                return;
            }

            // try to get it from cache
            AuthTokenCache.CacheEntry entry = AuthTokenCache.Instance.Get(authKey);
            if (entry != null)
                return; // it's in the cache, so let it through

            // wasn't in the cache.  Authenticate it then add it to the cache if successful
             HttpResponseMessage response = null;
             try
             {
                 using (HttpClient client = new HttpClient())
                 {
                     response = client.GetAsync(String.Format("{0}?access_token={1}&realm=/sbac", OpenAMUrl, authKey)).Result;
                     if (response.IsSuccessStatusCode)
                     {
                         // appears to be a successful authentication. 
                         try
                         {
                             // get the token info; if it's good, cache it
                             TokenInfo tok = response.Content.ReadAsAsync<TokenInfo>().Result;                             
                             if (tok.IsAuthenticated)
                                 AuthTokenCache.Instance.Put(tok);
                             else // token is not valid.  Do not authenticate
                                 response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent(String.Format("Token not authenticated: {0}, {1}", tok.error, tok.error_description)) };
                         }
                         catch (Exception ex)
                         {
                             // there was an error deserializing the token info.  Do not authenticate.
                             response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent(String.Format("Could not read token: {0}", ex.Message)) };
                         }
                     }
                 }
             }
             catch (Exception ex)
             {
                 TISServicesLogger.Log(ex);
                 response = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("An exception occurred while attempting to access the oauth server.") };
             }

            if (!response.IsSuccessStatusCode)
            {
                actionContext.Response = response;
                Utilities.Statistics.AddToUnauthenticatedRequestCount();
            }
        }
    }
}