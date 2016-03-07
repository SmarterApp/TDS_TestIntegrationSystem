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
using System.Web.Caching;

namespace AIR.Common.Caching
{
    /// <summary>
    /// Implementation of CacheManager using ASP.NET Cache object.
    /// </summary>
    /// <remarks>
    /// This class is not intended for use outside of ASP.NET applications. 
    /// It was designed and tested for use in ASP.NET to provide caching for Web applications.
    /// 
    /// The internal Cache object is shared across all instances of this class. So you cannot 
    /// repeat the same cache key. If you need this functionality then please use MemoryCacheManager.
    ///  
    /// When a cache entry is expired it will load in a background thread.
    /// </remarks>
    public class WebCacheManager<T> : CacheManager<T> where T : class
    {
        private readonly Cache _cache = HttpRuntime.Cache;
        private readonly CacheItemUpdateCallback _updateCallback;

        public WebCacheManager(Func<string, T> loader, CacheSettings settings) : base(loader, settings)
        {
            _updateCallback = new CacheItemUpdateCallback(UpdateCallback);
        }

        private void UpdateCallback(string key, CacheItemUpdateReason reason,
            out object value, out CacheDependency dependency,
            out DateTime absoluteexpiration, out TimeSpan slidingexpiration)
        {
            CacheEntry cacheEntry = null;

            // check if we should reload the data
            if (reason == CacheItemUpdateReason.Expired)
            {
                NotifyExpired(key);
                cacheEntry = LoadCacheEntry(key);
            }

            // check if we have a new cache entry
            if (cacheEntry != null)
            {
                value = cacheEntry;
                dependency = null;
                absoluteexpiration = CreateExpiresDate();
                slidingexpiration = Cache.NoSlidingExpiration;
            }
            else
            {
                value = null;
                dependency = null;
                absoluteexpiration = Cache.NoAbsoluteExpiration;
                slidingexpiration = Cache.NoSlidingExpiration;
            }
        }

        protected override CacheEntry GetCachedEntry(string key)
        {
            return _cache.Get(key) as CacheEntry;
        }

        protected override void SetCachedEntry(string key, CacheEntry cacheEntry)
        {
            DateTime expiresAt = CreateExpiresDate();

            // the cache priority for this Insert() function is "NotRemovable"
            _cache.Insert(key, cacheEntry, null, expiresAt, Cache.NoSlidingExpiration, _updateCallback);
        }

        protected override bool RemoveCachedEntry(string key)
        {
            return (_cache.Remove(key) != null);
        }

        protected override bool ContainsCachedEntry(string key)
        {
            return (GetCachedEntry(key) != null);
        }
    }
}
