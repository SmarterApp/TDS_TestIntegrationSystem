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
    /*
     This is part of CacheManager.cs
     */

    public abstract partial class CacheManager<T>
    {
        /// <summary>
        /// This is used to store a value in the lookup.
        /// </summary>
        protected class CacheEntry
        {
            private readonly T _value;
            private readonly DateTime _expiresAt;

            public CacheEntry(DateTime expiresAt)
            {
                _expiresAt = expiresAt;
            }

            /// <summary>
            /// Create new instance of CacheEntry.
            /// </summary>
            internal CacheEntry(T value, DateTime expiresAt) : this(expiresAt)
            {
                _value = value;
            }

            /// <summary>
            /// If this is true then the value is loaded.
            /// </summary>
            internal virtual bool IsLoaded
            {
                get
                {
                    return true;
                }
            }

            internal bool IsExpired
            {
                get
                {
                    return IsLoaded && _expiresAt < DateTime.UtcNow;
                }
            }

            /// <summary>
            /// UTC time at which CacheEntry expires.
            /// </summary>
            internal DateTime ExpiresAt
            {
                get
                {
                    return _expiresAt;
                }
            }

            internal virtual T Value
            {
                get
                {
                    return _value;
                }
            }
        }
    }
}
