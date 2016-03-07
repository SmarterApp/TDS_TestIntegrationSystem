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
using System.Web;

namespace AIR.Common.Caching
{
    /// <summary>
    /// This is used in ASP.NET to hold cache data for a http request. 
    /// </summary>
    /// <remarks>
    /// This prevents us from having to lookup items in the cache 
    /// and can be a performance benefit.
    /// </remarks>
    internal class PerRequestCache
    {
        private bool InWebContext()
        {
            return (HttpContext.Current != null && HttpContext.Current.Items != null);
        }

        public void Add(string cacheKey, object dataToAdd)
        {
            // If not in a web context, do nothing
            if (InWebContext())
            {
                cacheKey = "CACHEMANAGER-" + cacheKey;

                if (HttpContext.Current.Items.Contains(cacheKey))
                {
                    HttpContext.Current.Items.Remove(cacheKey);
                }
                
                HttpContext.Current.Items.Add(cacheKey, dataToAdd);
            }
        }

        public T Get<T>(string cacheKey) where T: class
        {
            // try per request cache first, but only if in a web context
            if (InWebContext())
            {
                cacheKey = "CACHEMANAGER-" + cacheKey;

                if (HttpContext.Current.Items.Contains(cacheKey))
                {
                    object data = HttpContext.Current.Items[cacheKey];
                    T realData = data as T;
                    if (realData != null)
                    {
                        return realData;
                    }
                }
            }

            return null;
        }
    }
}
