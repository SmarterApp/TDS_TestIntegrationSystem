/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Runtime.Caching;
using System.Threading;

namespace AIR.Common.Caching
{
    /// <summary>
    /// Implementation of CacheManager using the MemoryCache object (introduced in .NET 4.0).
    /// </summary>
    /// <remarks>
    /// The default constructor creates a MemoryCache that is shared across all instances of this class. 
    /// So you cannot repeat the same cache key. If you need this functionality then please use the other constructors.
    /// 
    /// When a cache entry is expired it will load in a background thread.
    /// 
    /// The MemoryCache has bugs that are fixed in .NET 4.5. You should try and use that framework. 
    /// </remarks>
    public class MemoryCacheManager<T> : CacheManager<T> where T : class
    {
        private readonly MemoryCache _cache;

        public MemoryCacheManager(Func<string, T> loader, CacheSettings settings) : base(loader, settings)
        {
            // BUG: http://stackoverflow.com/questions/7422859/memorycache-disposed-in-web-application
            using (ExecutionContext.SuppressFlow())
            {
                // Create memory cache instance under disabled execution context flow
                _cache = MemoryCache.Default;
            }
        }

        public MemoryCacheManager(string name, Func<string, T> loader, CacheSettings settings) : base(loader, settings)
        {
            // BUG: http://stackoverflow.com/questions/7422859/memorycache-disposed-in-web-application
            using (ExecutionContext.SuppressFlow())
            {
                // Create memory cache instance under disabled execution context flow
                _cache = new MemoryCache(name);
            }
        }
        
        public MemoryCacheManager(MemoryCache cache, Func<string, T> loader, CacheSettings settings) : base(loader, settings)
        {
            _cache = cache;
        }

        /// <summary>
        /// Create a MemoryCache object.
        /// </summary>
        private CacheItem CreateCacheItem(string key)
        {
            CacheEntry cacheEntry = LoadCacheEntry(key);

            if (cacheEntry != null)
            {
                return new CacheItem(key, cacheEntry);
            }

            return null;
        }

        /// <summary>
        /// Create a MemoryCache policy.
        /// </summary>
        private CacheItemPolicy CreateCachePolicy()
        {
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = CreateExpiresDate();
            cacheItemPolicy.UpdateCallback = UpdateCallback;
            return cacheItemPolicy;
        }

        private void UpdateCallback(CacheEntryUpdateArguments args)
        {
            if (args.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                NotifyExpired(args.Key);
                CacheItem updatedCacheItem = CreateCacheItem(args.Key);

                if (updatedCacheItem != null)
                {
                    args.UpdatedCacheItem = updatedCacheItem;
                    args.UpdatedCacheItemPolicy = CreateCachePolicy();
                }
            }
        }

        protected override CacheEntry GetCachedEntry(string key)
        {
            return _cache.Get(key) as CacheEntry;
        }

        protected override void SetCachedEntry(string key, CacheEntry cacheEntry)
        {
            CacheItem cacheItem = new CacheItem(key, cacheEntry);
            CacheItemPolicy cacheItemPolicy = CreateCachePolicy();
            _cache.Set(cacheItem, cacheItemPolicy);
        }

        protected override bool RemoveCachedEntry(string key)
        {
            return (_cache.Remove(key) != null);
        }

        protected override bool ContainsCachedEntry(string key)
        {
            return _cache.Contains(key);
        }
    }
}