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
using System.Configuration;

namespace TISServices.Authorization
{
    public class AuthTokenCache
    {
        private static readonly object cacheLock = new object(); // will be used for all locking, static and instance
        private const string AUTH_CACHE = "AUTH_CACHE";

        /// <summary>
        /// If the cache is not accessed in this number of minutes, it will be dumped from memory
        /// </summary>
        public double SlidingExpirationMinutes { get; private set; }

        /// <summary>
        /// The max number of tokens to store in the cache.  If we receive more than max, we'll start purging with each new token received.
        /// </summary>
        public int MaxSize { get; private set; }

        /// <summary>
        /// The number of entries to purge if the count is >= MaxSize.  In general the token cache shoudl not need to be purged.  But just in case,
        /// if it gets full, we'll remove this many before inserting the next one.
        /// </summary>
        public int PurgeCount { get; private set; }

        /// <summary>
        /// The DateTime of the last purge due to a full cache
        /// </summary>
        public DateTime? LastPurge { get; private set; }
        
        private AuthTokenCache()
        {
            SlidingExpirationMinutes = String.IsNullOrEmpty(ConfigurationManager.AppSettings["AuthTokenCache:SlidingExpirationMinutes"]) ? 240 : Convert.ToInt32(ConfigurationManager.AppSettings["AuthTokenCache:SlidingExpirationMinutes"]);
            MaxSize = String.IsNullOrEmpty(ConfigurationManager.AppSettings["AuthTokenCache:MaxSize"]) ? 100 : Convert.ToInt32(ConfigurationManager.AppSettings["AuthTokenCache:MaxSize"]);
            PurgeCount = String.IsNullOrEmpty(ConfigurationManager.AppSettings["AuthTokenCache:PurgeCount"]) ? 9 : Convert.ToInt32(ConfigurationManager.AppSettings["AuthTokenCache:PurgeCount"]);
        }

        private static AuthTokenCache instance;
        /// <summary>
        /// The singleton instance
        /// </summary>
        public static AuthTokenCache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (cacheLock)
                    {
                        if (instance == null)
                        {
                            instance = new AuthTokenCache();
                            // access the cache so that it's loaded into the HttpRuntime cache, but just throw it away for now.
                            Dictionary<Guid, CacheEntry> c = instance.Cache;
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// The actual cache, currently stored in the HttpRuntime.Cache
        /// </summary>
        private Dictionary<Guid, CacheEntry> Cache
        {
            get
            {
                Dictionary<Guid, CacheEntry> cache = HttpRuntime.Cache.Get(AUTH_CACHE) as Dictionary<Guid, CacheEntry>;
                if (cache == null)
                {
                    lock (cacheLock)
                    {
                        cache = HttpRuntime.Cache.Get(AUTH_CACHE) as Dictionary<Guid, CacheEntry>;
                        if (cache == null)
                        {
                            cache = new Dictionary<Guid, CacheEntry>();
                            HttpRuntime.Cache.Add(AUTH_CACHE, cache, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(SlidingExpirationMinutes), System.Web.Caching.CacheItemPriority.Default, null);
                        }
                    }
                }
                return cache;
            }
        }

        /// <summary>
        /// Get a CacheEntry by the token.  If the entry is expired, it'll be automatically removed and null will be returned.
        /// If the CacheEntry is not found, null will be returned
        /// </summary>
        /// <param name="key">the auth token key</param>
        /// <returns></returns>
        public CacheEntry Get(Guid key)
        {
            return Get(key, true);
        }

        /// <summary>
        /// Get a CacheEntry by the token.  If the entry is expired and removeExpired is true, it'll be automatically removed and null will be returned.
        /// Otherwise the expired entry will be returned.
        /// If the CacheEntry is not found, null will be returned
        /// </summary>
        /// <param name="key"></param>
        /// <param name="removeExpired"></param>
        /// <returns></returns>
        private CacheEntry Get(Guid key, bool removeExpired)
        {
            CacheEntry entry = null;
            // if we got an entry and it's expired, remove it from the cache and return null;
            if (Cache.TryGetValue(key, out entry) && entry.IsExpired && removeExpired)
            {
                Cache.Remove(key);
                entry = null;
            }
            return entry;
        }

        /// <summary>
        /// Insert an authenticated token into the cache.  If the cache is full (or over full), the oldest CacheEntry(ies) will first be
        /// purged so that there is room to insert this token or until PurgeCount has been reached.
        /// The token will always be inserted, even if the cache is still full or overfull after purging.  If this happens often though, there is probably a bug.
        /// </summary>
        /// <param name="tokenInfo"></param>
        public void Put(TokenInfo tokenInfo)
        {
            if (Get(tokenInfo.access_token, true) == null)
            {
                lock (cacheLock)
                {
                    if (Get(tokenInfo.access_token, true) == null)
                    {
                        if (Cache.Count >= MaxSize)
                            RemoveOldest(PurgeCount); // just trying to keep the size manageable.  TDS should be caching their keys, so we don't expect many (or any) purges.
                        Cache.Add(tokenInfo.access_token, new CacheEntry(tokenInfo));
                    }
                }
            }
        }

        /// <summary>
        /// Removes a CacheEntry from the cache
        /// </summary>
        /// <param name="entry"></param>
        public void Remove(CacheEntry entry)
        {
            if (Get(entry.Info.access_token, false) != null)
            {
                lock (cacheLock)
                {
                    if (Get(entry.Info.access_token, false) != null)
                    {
                        Cache.Remove(entry.Info.access_token);
                    }
                }
            }
        }

        /// <summary>
        /// Purges the "num" oldest CacheEntries from the cache
        /// called prior to inserting a new entry if the cache is full or overfull.
        /// Should never be needed.  Just want to make sure there's not a memory leak.
        /// If purging happens, something is probably wrong.
        /// </summary>
        /// <param name="num">The number of entries to purge if the cache is full or overfull.</param>
        private void RemoveOldest(int num)
        {
            if (Cache.Count >= MaxSize) // only purge if the cache is full
            {
                lock (cacheLock)
                {
                    if (Cache.Count >= MaxSize)
                    {
                        List<CacheEntry> entries = new List<CacheEntry>(Cache.Values);
                        if (entries.Count > 1) // order by CreateDate asc
                            entries.Sort(delegate(CacheEntry e1, CacheEntry e2) { return e1.CreateDate.CompareTo(e2.CreateDate); });

                        int stop = Math.Min(entries.Count, num);
                        for (int i = 0; i < stop && Cache.Count > 0; i++)
                        {
                            Remove(entries[i]);
                        }
                        LastPurge = DateTime.UtcNow;
                    }
                }
            }
        }

        /// <summary>
        /// The number of entries currently in the cache
        /// </summary>
        public int Count
        {
            get
            {
                return Cache.Count;
            }
        }

        public class CacheEntry
        {
            public DateTime CreateDate { get; private set; }
            public TokenInfo Info { get; private set; }

            public CacheEntry(TokenInfo info)
            {
                this.Info = info;
                this.CreateDate = DateTime.UtcNow;
            }

            public bool IsExpired
            {
                get
                {
                    return DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(Info.expires_in)) >= CreateDate;
                }
            }
        }
    }
}