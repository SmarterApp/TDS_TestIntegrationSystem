/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Reflection;
using System.Web;

namespace AIR.Common.Web
{
    /// <summary>
    /// Wraps HttpRuntime and allows access to private fields.
    /// </summary>
    public class HttpRuntimeWrapper
    {
        private readonly HttpRuntime _runtime;

        private T GetValue<T>(string name)
        {
            if (_runtime != null)
            {
                try
                {
                    return (T)_runtime.GetType().InvokeMember(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, _runtime, null);
                }
                catch {}
            }

            return default(T);
        }

        public HttpRuntimeWrapper()
        {
            try
            {
                _runtime = (HttpRuntime)typeof(HttpRuntime).InvokeMember("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
            }
            catch {}
        }

        public HttpRuntime Instance
        {
            get { return _runtime; }
        }

        public int ActiveRequestCount
        {
            get { return GetValue<int>("_activeRequestCount"); }
        }

        public DateTime FirstRequestStartTime
        {
            get { return GetValue<DateTime>("_firstRequestStartTime"); }
        }

        public bool UserForcedShutdown
        {
            get { return GetValue<bool>("_userForcedShutdown"); }
        }

        public ApplicationShutdownReason ShutDownReason
        {
            get { return GetValue<ApplicationShutdownReason>("_shutdownReason"); }
        }

        public string ShutDownMessage
        {
            get { return GetValue<string>("_shutDownMessage"); }
        }

        public string ShutDownStack
        {
            get { return GetValue<string>("_shutDownStack"); }
        }

        public DateTime LastShutdownAttemptTime
        {
            get { return GetValue<DateTime>("_lastShutdownAttemptTime"); }
        }

        public string WPUserId
        {
            get { return GetValue<string>("_wpUserId"); }
        }

    }
}