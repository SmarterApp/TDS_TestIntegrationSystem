/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Concurrent;

namespace AIR.Common.Caching
{
    /// <summary>
    /// Implementation of CacheManager using the ConcurrentDictionary.
    /// </summary>
    /// <remarks>
    /// When a cache entry is expired it will load on-demand. 
    /// A cache entry will not load data in the background.
    /// </remarks>
    public class DictionaryCacheManager<T> : CacheManager<T> where T : class
    {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new ConcurrentDictionary<string, CacheEntry>();

        public DictionaryCacheManager(Func<string, T> loader, CacheSettings settings) : base(loader, settings)
        {
        }

        protected override CacheEntry GetCachedEntry(string key)
        {
            CacheEntry cacheEntry;
            _cache.TryGetValue(key, out cacheEntry);
            
            // try and get the cached entry
            if (cacheEntry != null)
            {
                // check if the cache entry is loaded and not yet removed before checking if it is expired
                if (cacheEntry.IsExpired)
                {
                    // the cache is expired so we need to remove it
                    RemoveCachedEntry(key);

                    // ignore this entry since it was expired
                    cacheEntry = null;
                }
            }

            return cacheEntry;
        }

        protected override void SetCachedEntry(string key, CacheEntry entry)
        {
            _cache[key] = entry;
        }

        protected override bool RemoveCachedEntry(string key)
        {
            CacheEntry cacheEntry;
            return _cache.TryRemove(key, out cacheEntry);
        }

        protected override bool ContainsCachedEntry(string key)
        {
            return _cache.ContainsKey(key);
        }
    }
}