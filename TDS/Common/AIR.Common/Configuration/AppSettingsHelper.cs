/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Configuration;
using AIR.Common.Security;

namespace AIR.Common.Configuration
{
    /// <summary>
    /// A helper class for reading values from appsettings section.
    /// </summary>
    public static class AppSettingsHelper
    {
        /// <summary>
        /// Check if app setting exists.
        /// </summary>
        public static bool Exists(string key)
        {
            return (ConfigurationManager.AppSettings[key] != null);
        }

        public static string Get(string key)
        {
            return Get(key, null);
        }

        public static string Get(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (value == null && defaultValue != null) return defaultValue;
            return value;
        }

        public static T GetValue<T>(string key)
        {
            string value = Get(key);
            if (value != null) return value.ConvertTo<T>();
            return default(T);
        }

        public static int GetInt32(string key)
        {
            return GetInt32(key, null);
        }

        public static int GetInt32(string key, int defaultValue)
        {
            return GetInt32(key, (int?)defaultValue);
        }

        private static int GetInt32(string key, int? defaultValue)
        {
            string rawValue = Get(key);

            if (!string.IsNullOrEmpty(rawValue))
            {
                int value;
                if (int.TryParse(rawValue, out value)) return value;
            }

            return defaultValue.HasValue ? defaultValue.Value : default(int);
        }
        
        public static long GetInt64(string key)
        {
            return GetInt64(key, null);
        }

        public static long GetInt64(string key, long defaultValue)
        {
            return GetInt64(key, (int?)defaultValue);
        }

        private static long GetInt64(string key, long? defaultValue)
        {
            string rawValue = Get(key);

            if (!string.IsNullOrEmpty(rawValue))
            {
                long value;
                if (long.TryParse(rawValue, out value)) return value;
            }

            return defaultValue.HasValue ? defaultValue.Value : default(long);
        }

        public static bool GetBoolean(string key)
        {
            return GetBoolean(key, null);
        }

        public static bool GetBoolean(string key, bool defaultValue)
        {
            return GetBoolean(key, (bool?)defaultValue);
        }

        private static bool GetBoolean(string key, bool? defaultValue)
        {
            string rawValue = Get(key);

            if (!string.IsNullOrEmpty(rawValue))
            {
                bool value;
                if (bool.TryParse(rawValue, out value)) return value;
            }

            return defaultValue.HasValue ? defaultValue.Value : default(bool);
        }

        public static double GetDouble(string key)
        {
            return GetDouble(key, null);
        }

        public static double GetDouble(string key, double defaultValue)
        {
            return GetDouble(key, (double?)defaultValue);
        }

        private static double GetDouble(string key, double? defaultValue)
        {
            string rawValue = Get(key);

            if (!string.IsNullOrEmpty(rawValue))
            {
                double value;
                if (double.TryParse(rawValue, out value)) return value;
            }

            return defaultValue.HasValue ? defaultValue.Value : default(double);
        }

        public static string GetSecure(string key)
        {
            string secureStr = Get(key);
            return secureStr;
        }

    }
}
