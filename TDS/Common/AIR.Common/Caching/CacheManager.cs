/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;

namespace AIR.Common.Caching
{
    /// <summary>
    /// Manages a cache for a specific collection of types.
    /// </summary>
    public abstract partial class CacheManager<T> : ICacheManager<T> where T : class
    {
        // public static readonly TimeSpan OneYear = new TimeSpan(360, 0, 0);

        private readonly PerRequestCache _perRequestCache = new PerRequestCache();
        private readonly object _syncRoot = new Object();
        private readonly Func<string, T> _loader;
        private readonly CacheSettings _settings;

        /// <summary>
        /// This fires right before the data is going to be loaded.
        /// </summary>
        public event Action<string> OnLoadStart;

        /// <summary>
        /// This fires after the data has been loaded.
        /// </summary>
        public event Action<string, T> OnLoadSuccess;
        
        /// <summary>
        /// This fires if there was an exception loading data.
        /// </summary>
        public event Action<string, Exception> OnLoadException;

        /// <summary>
        /// This fires when the data has expired.
        /// </summary>
        /// <remarks>
        /// This is mostly used for unit testing.
        /// </remarks>
        public event Action<string> OnExpired;

        protected CacheManager(Func<string, T> loader, CacheSettings settings)
        {
            if (loader == null)
            {
                throw new ArgumentNullException("loader");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (settings.ExpiresIn == TimeSpan.Zero)
            {
                throw new ArgumentException("Must provide valid expires time", "settings");
            }
            
            _loader = loader;
            _settings = settings;
        }

        protected abstract CacheEntry GetCachedEntry(string key);
        protected abstract void SetCachedEntry(string key, CacheEntry entry);
        protected abstract bool RemoveCachedEntry(string key);
        protected abstract bool ContainsCachedEntry(string key);

        protected DateTime CreateExpiresDate()
        {
            return DateTime.UtcNow.Add(_settings.ExpiresIn);
        }

        /// <summary>
        /// Loads the value in the provided loader.
        /// </summary>
        private T LoadValue(string key)
        {
            T value = null;

            // fire event that we are about to begin loading
            NotifyLoadStart(key);

            try
            {
                // start loading value
                value = _loader(key);

                // check if we got any data
                if (_settings.AllowNullValues || value != null)
                {
                    // fire event that we are done loading
                    NotifyLoadSuccess(key, value);
                }
                else
                {
                    // remove the cache entry since there is no data
                    RemoveCachedEntry(key);
                }
            }
            catch (Exception ex)
            {
                // try and get existing cache entry
                CacheEntry existingCacheEntry = GetCachedEntry(key);

                bool reuseValue = false;

                if (existingCacheEntry != null)
                {
                    // check if existing cache entry loaded successfully
                    if (existingCacheEntry.IsLoaded)
                    {
                        // reuse the previously cached value
                        reuseValue = true;
                        value = existingCacheEntry.Value;
                    }
                    // if we don't allow null values remove lazy loaded cache entry since it didn't load
                    else if (!_settings.AllowNullValues)
                    {
                        RemoveCachedEntry(key);
                    }
                }

                // if we aren't reusing existing value and then throw exception
                if (!_settings.IgnoreExceptions && !reuseValue && value == null)
                {
                    // fire event that an exception was thrown during loading 
                    NotifyLoadException(key, ex);

                    // rethrow original exception for caller
                    throw;
                }
            }

            return value;
        }

        /// <summary>
        /// Create a cache entry with a preloaded value.
        /// </summary>
        private CacheEntry CreateCacheEntry(T value)
        {
            DateTime expiresAt = CreateExpiresDate();
            return new CacheEntry(value, expiresAt);
        }

        /// <summary>
        /// Create a cache entry and begin loading the value.
        /// </summary>
        protected CacheEntry LoadCacheEntry(string key)
        {
            T value = LoadValue(key); // <-- this loads value
            return (_settings.AllowNullValues || value != null) ? 
                CreateCacheEntry(value) : null;
        }

        /// <summary>
        /// Create a cache entry with a lazy loaded value.
        /// </summary>
        /// <remarks>
        /// The value will not be loaded until someone requests it.
        /// </remarks>
        protected LazyCacheEntry CreateLazyCacheEntry(string key)
        {
            DateTime expiresAt = CreateExpiresDate();
            return new LazyCacheEntry(() => LoadValue(key), expiresAt);
        }

        public T Get(string key)
        {
            // check if valid key
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            // check web request
            CacheEntry cachedEntry = _perRequestCache.Get<CacheEntry>(key);

            // if it wasn't found in web request then check main cache
            if (cachedEntry == null)
            {
                // get cached value
                cachedEntry = GetCachedEntry(key);

                // if it wasn't found in main cache we need to create Lazy loader placeholder
                if (cachedEntry == null)
                {
                    // lock dictionary
                    lock (_syncRoot)
                    {
                        // check again in case value was loaded while we were locking (double lock check)
                        cachedEntry = GetCachedEntry(key);

                        if (cachedEntry == null)
                        {
                            cachedEntry = CreateLazyCacheEntry(key);
                            SetCachedEntry(key, cachedEntry);
                        }
                    }
                }

                // save to web request for quick access 
                if (cachedEntry != null)
                {
                    _perRequestCache.Add(key, cachedEntry);
                }
            }

            if (cachedEntry != null)
            {
                return cachedEntry.Value;
            }

            return null;
        }

        public void Set(string key, T value)
        {
            CacheEntry cachedEntry = CreateCacheEntry(value);
            SetCachedEntry(key, cachedEntry);
        }

        public bool Contains(string key)
        {
            return ContainsCachedEntry(key);
        }

        public bool Remove(string key)
        {
            return RemoveCachedEntry(key);
        }

        protected void NotifyLoadStart(string key)
        {
            if (OnLoadStart != null)
            {
                try
                {
                    OnLoadStart(key);
                }
                catch (Exception ex)
                {
                    // log here...
                }
            }
        }

        protected void NotifyLoadSuccess(string key, T value)
        {
            if (OnLoadSuccess != null)
            {
                try
                {
                    OnLoadSuccess(key, value);
                }
                catch (Exception ex)
                {
                    // log here...
                }
            }
        }

        protected void NotifyLoadException(string key, Exception ex)
        {
            if (OnLoadException != null)
            {
                try
                {
                    OnLoadException(key, ex);
                }
                catch (Exception ex2)
                {
                    // log here...
                }
            }
        }

        protected void NotifyExpired(string key)
        {
            if (OnExpired != null)
            {
                try
                {
                    OnExpired(key);
                }
                catch (Exception ex)
                {
                    // log here...
                }
            }
        }

    }
}
