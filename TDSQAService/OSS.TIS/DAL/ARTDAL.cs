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
using System.Data;
using System.Configuration;
using System.Xml;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TDSQASystemAPI.Routing.Authorization;

namespace OSS.TIS.DAL
{
    public class ARTDAL
    {
        AIR.Configuration.WebService artService;

        public ARTDAL(AIR.Configuration.WebService artService)
        {
            if (artService == null || String.IsNullOrEmpty(artService.URL))
                throw new ApplicationException("Must provide web service settings");
            this.artService = artService;
        }

        /// <summary>
        /// Get student package from ART. This returns the entire student package.
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="stateAbbrviation"></param>
        /// <returns></returns>
        public XmlDocument GetStudentPackageXML(string ssid, string stateAbbrviation, bool useAlternateStudentId)
        {
            if (string.IsNullOrEmpty(ssid) || string.IsNullOrEmpty(stateAbbrviation))
                throw new ApplicationException(String.Format("Illegal parameters... ssid: {0}, stateAbbrviation: {1}", ssid ?? "<null>", stateAbbrviation ?? "<null>"));

            XmlDocument doc = null;

            OAuthResponse oauthToken = null;
            bool authTokenFoundInCache = false;

            if (artService.Authorization != null)
                oauthToken = OAuth.GetResponse(artService.Authorization, out authTokenFoundInCache);

            HttpResponseMessage response = Get(ssid, stateAbbrviation, oauthToken, useAlternateStudentId);

            if ((response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.BadRequest) && authTokenFoundInCache)
            {
                // try again with a fresh token; may have expired
                OAuth.RemoveFromCache(artService.Authorization, oauthToken);
                oauthToken = OAuth.GetResponse(artService.Authorization, out authTokenFoundInCache);
                response = Get(ssid, stateAbbrviation, oauthToken, useAlternateStudentId);
            }

            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                string resp = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(resp))
                {
                    doc = new XmlDocument();
                    doc.LoadXml(resp);
                }
            }
            else throw new Exception(string.Format("Unsuccessful call to ART Webservice. Url: {0}, StateAbbreviation: {1}, StatusCode: {2}, ReasonPhrase: {3}", artService.URL, stateAbbrviation, response.StatusCode.ToString(), response.ReasonPhrase));

            return doc;
        }

        private HttpResponseMessage Get(string ssid, string stateAbbrviation, OAuthResponse authToken, bool useAlternateStudentId)
        {
            using (HttpClient httpclient = new HttpClient())
            {
                string path = string.Format("rest/studentpackage?{0}={1}&stateabbreviation={2}", useAlternateStudentId ? "externalId" : "ssid", Uri.EscapeDataString(ssid), Uri.EscapeDataString(stateAbbrviation));
                httpclient.BaseAddress = new Uri(artService.URL);

                if (artService.Authorization != null)
                {
                    //add our header to the message
                    if (authToken == null)
                        throw new NullReferenceException(String.Format("A valid oauth token is required because Authorization is configured for this service.  Service config name: {0},  Auth config name: {1}",
                                    artService.Name, artService.Authorization.Name));
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authToken.token_type, authToken.access_token.ToString());
                }

                return httpclient.GetAsync(path).Result;
            }
        }
    }
}
