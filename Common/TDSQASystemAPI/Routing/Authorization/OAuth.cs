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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.IO;
using AIR.Configuration;

namespace TDSQASystemAPI.Routing.Authorization
{
    public static class OAuth
    {
        private static readonly object lockme;
        public static readonly int GET_FROM_CACHE_MAX_RETRIES;
        public static readonly int GET_FROM_CACHE_MS_SLEEP_BEFORE_RETRY;

        private static Dictionary<String, OAuthResponse> _cache; // see GetCacheKey for key

        static OAuth()
        {
            lockme = new object();
            _cache = new Dictionary<string, OAuthResponse>();
            GET_FROM_CACHE_MAX_RETRIES = ConfigurationManager.AppSettings["OAuth:GetFromCacheMaxRetries"] == null ? 10 : Convert.ToInt32(ConfigurationManager.AppSettings["OAuth:GetFromCacheMaxRetries"]);
            GET_FROM_CACHE_MS_SLEEP_BEFORE_RETRY = ConfigurationManager.AppSettings["OAuth:GetFromCacheMSSleepBeforeRetry"] == null ? 100 : Convert.ToInt32(ConfigurationManager.AppSettings["OAuth:GetFromCacheMSSleepBeforeRetry"]);
        }

        /// <summary>
        /// Returns the oauth token and a bool to indicate whether the token was in the cache.  If false, it was fetched from the oauth server.
        /// This can be useful in knowing whether or not to try to get a fresh token if the one that's returned fails to authenticate.  If it was
        /// in the cache, then it may have expired.  Otherwise, there's some other problem.
        /// 
        /// Note that we're taking the approach of using a token until it fails to authenticate, rather than dealing with the expiration of the token ourselves.
        /// We'll let the server worry about expiration.  A little bit of a pain to have to retry, but I suspect we'd need to do that anyway under some
        /// circumstances.  Ex: we pull the token from cache just before it expires and use it to call a server that receives our call just as it expires,
        /// in which case it would reject the login and we'd need a fresh token.
        /// </summary>
        /// <param name="authSetting"></param>
        /// <param name="foundInCache"></param>
        /// <returns></returns>
        public static OAuthResponse GetResponse(AIR.Configuration.Authorization authSetting, out bool foundInCache)
        {
            foundInCache = true; // will be flipped if we have to contact the oauth server to fetch the token
            string cacheKey = GetCacheKey(authSetting);
            OAuthResponse value = null;

            // try to get the value from cache
            _cache.TryGetValue(cacheKey, out value);

            // check before locking for perf purposes
            if (value == null && !_cache.ContainsKey(cacheKey))
            {
                lock (lockme)
                {
                    // check again now that we're locked
                    if (!_cache.ContainsKey(cacheKey))
                    {  
                        if (string.IsNullOrEmpty(authSetting.URL) || string.IsNullOrEmpty(authSetting.Realm) || string.IsNullOrEmpty(authSetting.GrantType)
                            || string.IsNullOrEmpty(authSetting.Username) || string.IsNullOrEmpty(authSetting.Password))
                            throw new ApplicationException(String.Format("Improperly configured Authorization setting: {0}", authSetting.Name));

                        // call out to the oauth server to get a token
                        using (HttpClient httpclient = new HttpClient())
                        {
                            string path = string.Format("auth/oauth2/access_token?realm={0}", authSetting.Realm);
                            httpclient.BaseAddress = new Uri(new Uri(Uri.EscapeUriString(authSetting.URL)), path);

                            //add our header to the message
                            HttpRequestMessage m = new HttpRequestMessage(HttpMethod.Post, httpclient.BaseAddress);
                            string content = string.Format("&grant_type={0}&username={1}&password={2}&client_id={3}&client_secret={4}"
                                                            , Uri.EscapeDataString(authSetting.GrantType)
                                                            , Uri.EscapeDataString(authSetting.Username)
                                                            , Uri.EscapeDataString(authSetting.Password)
                                                            , Uri.EscapeDataString(authSetting.ClientId)
                                                            , Uri.EscapeDataString(authSetting.ClientSecret));
                            m.Content = new StringContent(content, Encoding.ASCII, "application/x-www-form-urlencoded");

                            // Add an Accept header for JSON format.
                            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            HttpResponseMessage response = httpclient.SendAsync(m).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                // Parse the response body. Blocking!
                                //string resp = response.Content.ReadAsStringAsync().Result;
                                value = response.Content.ReadAsAsync<OAuthResponse>().Result;
                            }
                            else
                            {
                                throw new ApplicationException(string.Format("Unsuccessful call to OAuth Webservice. Url: {0}, Realm: {1}, GrantType: {2}, Username: {3}, ClientID: {4}, StatusCode: {5}, ReasonPhrase: {6}",
                                    authSetting.URL, authSetting.Realm, authSetting.GrantType, authSetting.Username, authSetting.ClientId, response.StatusCode.ToString(), response.ReasonPhrase));
                            }
                        }
                        _cache[cacheKey] = value;
                        foundInCache = false;
                    }
                }
            }

            // if the value is still null, check the cache again; try a few times to improve our chance of success, sleeping a bit in between
            // Note: may be able to lock and then recursively call this method again to guarantee a value, but I didn't want to 
            //  risk an infinite loop.  If this doesn't do it, refactor to keep retrying until we get a value
            int i;
            for (i = 0; value == null && i < GET_FROM_CACHE_MAX_RETRIES; i++, System.Threading.Thread.Sleep(GET_FROM_CACHE_MS_SLEEP_BEFORE_RETRY))
                _cache.TryGetValue(cacheKey, out value);

            if (value == null)
                throw new ApplicationException(String.Format("Was not able to retreive an oauth token for config: {0} after {1} retries", authSetting.Name, i));

            if (i > 0)
                Utilities.Logger.Log(true, String.Format("Auth setting: {0} required {1} retries to get from cache.", authSetting.Name, i), System.Diagnostics.EventLogEntryType.Information, false, true);

            return value;
        }

        /// <summary>
        /// Removes an entry from the cache if the access token is the same.
        /// </summary>
        /// <param name="authSetting"></param>
        /// <returns></returns>
        public static bool RemoveFromCache(AIR.Configuration.Authorization authSetting, OAuthResponse accessTokenToRemove)
        {
            if (authSetting == null || accessTokenToRemove == null)
                return false;

            string key = GetCacheKey(authSetting);
            bool removed = false;
            if (_cache.ContainsKey(key))
            {
                lock (lockme)
                {
                    if (_cache.ContainsKey(key) && _cache[key].access_token.Equals(accessTokenToRemove.access_token))
                        removed = _cache.Remove(key);
                }
            }
            return removed;
        }

        /// <summary>
        /// Only the auth name at this point.  May expand to include realm in the future.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static string GetCacheKey(AIR.Configuration.Authorization authSetting)
        {
            return authSetting.Name;
        }
    }
}
