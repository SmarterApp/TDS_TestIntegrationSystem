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
    public class CacheSettings
    {
        /// <summary>
        /// How long until the cache expires.
        /// </summary>
        public TimeSpan ExpiresIn { get; set; }
        
        /// <summary>
        /// If this is true then we cache null values that get returned from the loader.
        /// </summary>
        public bool AllowNullValues { get; set; }
        
        /// <summary>
        /// If this is true then we ignore exceptions thrown from the loader and return a null.
        /// </summary>
        /// <remarks>
        /// If you have AllowNullValues as true then the null returned from the exception gets cached.
        /// </remarks>
        public bool IgnoreExceptions { get; set; }
    }
}