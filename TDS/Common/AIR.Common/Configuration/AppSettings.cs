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

namespace AIR.Common.Configuration
{
    public delegate bool AppSettingsHandler(string name, out object value);

    /// <summary>
    /// This class is used to get app settings out of web.config or a database.
    /// </summary>
    public static class AppSettings
    {
        private static AppSettingsHandler _handler;

        /// <summary>
        /// Set the handler used to lookup settings. In this handler you
        /// should figure out what the current client is and other context
        /// related issues. 
        /// </summary>
        public static void SetHandler(AppSettingsHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Lookup an app settings.
        /// </summary>
        /// <typeparam name="T">The data type of the setting. Right now there is only string, bool and int.</typeparam>
        /// <param name="name">The name of the app setting key.</param>
        /// <param name="defaultValue">A default value in case the app setting is not found.</param>
        /// <returns></returns>
        public static AppSetting<T> Get<T>(string name, T defaultValue) 
        {
            // first check web.config
            if (AppSettingsHelper.Exists(name))
            {
                T value = AppSettingsHelper.GetValue<T>(name);
                return new AppSetting<T>(name, value);
            }

            // if a getter function was assigned then load from that
            if (_handler != null)
            {
                object value;
                
                if (_handler(name, out value))
                {
                    try
                    {
                        return new AppSetting<T>(name, (T)value);
                    }
                    catch(InvalidCastException ice)
                    {
                        throw new InvalidCastException(String.Format("{0} ({1})", ice.Message, name), ice);
                    }
                }
            }

            // return default
            return new AppSetting<T>(name, defaultValue);
        }

        /// <summary>
        /// Lookup an app settings.
        /// </summary>
        /// <typeparam name="T">The data type of the setting. Right now there is only string, bool and int.</typeparam>
        /// <param name="name">The name of the app setting key.</param>
        /// <returns></returns>
        public static AppSetting<T> Get<T>(string name) 
        {
            return Get(name, default(T));
        }

        public static object ParseString(string type, string value)
        {
            // get value
            object obj;

            switch (type.ToLower())
            {
                case "boolean": obj = value.ConvertTo<bool>(); break;
                case "integer": obj = value.ConvertTo<int>(); break;
                default: obj = value; break; // default also covers "string"
            }

            return obj;
        }
    }

}
