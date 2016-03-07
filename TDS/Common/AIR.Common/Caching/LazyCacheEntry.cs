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

namespace AIR.Common.Caching
{
    /*
     This is part of CacheManager.cs
     */

    public abstract partial class CacheManager<T>
    {
        /// <summary>
        /// This is used to store a lazy load value in the lookup.
        /// </summary>
        protected class LazyCacheEntry : CacheEntry
        {
            private readonly Lazy<T> _lazy;

            /// <summary>
            /// Create new instance of CacheEntry.
            /// </summary>
            internal LazyCacheEntry(Func<T> loader, DateTime expiresAt) : base(expiresAt)
            {
                _lazy = new Lazy<T>(loader);
            }

            /// <summary>
            /// If this is true then the value is loaded.
            /// </summary>
            internal override bool IsLoaded
            {
                get
                {
                    return _lazy.IsValueCreated;
                }
            }

            internal override T Value
            {
                get
                {
                    return _lazy.Value;
                }
            }
        }
    }
}
