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
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Web.Security;

namespace AIR.Common.Web
{
    /// <summary>
    /// A class for storing multiple name/value pairs in a single cookie 
    /// (NOTE: Does not support multiple values for the same name).
    /// </summary>
    public class CookieContainer
    {
        private readonly string _key;

        private bool _isDirty = false;
        private readonly NameValueCollection _values = new NameValueCollection();

        /// <summary>
        /// Cookies key.
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Has a change been made to a value.
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
        }

        public bool IsEmpty
        {
            get { return (_values.Count == 0); }
        }

        public void Clear()
        {
            _values.Clear();
        }

        public CookieContainer(string key)
        {
            _key = key;
        }

        /// <summary>
        /// Load the cookie from the HTTP request.
        /// </summary>
        public bool Load()
        {
            // clear internal collection
            _values.Clear();

            if (HttpContext.Current == null) return false;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[_key];
            if (cookie == null) return false;
            _values.Add(cookie.Values);
            _isDirty = false;
            return true;
        }

        /// <summary>
        /// Save the cookie to the HTTP response.
        /// </summary>
        public bool Save()
        {
            if (HttpContext.Current == null) return false;
            HttpCookie cookie = new HttpCookie(_key);
            cookie.Values.Add(_values);
            HttpContext.Current.Response.Cookies.Set(cookie);
            _isDirty = false;
            return true;
        }
        
        /// <summary>
        /// Set a name/value on the cookie.
        /// </summary>
        public void Set(string name, object value)
        {
            if (value == null) return;
            string strValue = value.ToString();
            strValue = Encode(strValue);
            _values.Set(name, strValue);
            _isDirty = true;
        }

        /// <summary>
        /// Add a name/value to the cookie.
        /// </summary>
        public void Add(string name, object value)
        {
            if (value == null) return;
            string strValue = value.ToString();
            strValue = Encode(strValue);
            _values.Add(name, strValue);
            _isDirty = true;
        }

        /// <summary>
        /// Check if a name/value exists.
        /// </summary>
        public bool Exists(string name)
        {
            return (_values[name] != null);            
        }

        /// <summary>
        /// Get the string value for a name.
        /// </summary>
        public string Get(string name)
        {
            string value = _values.Get(name);
            return Decode(value);
        }

        /// <summary>
        /// Get the string value for a name.
        /// </summary>
        public string[] GetValues(string name)
        {
            string[] values = _values.GetValues(name);

            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = Decode(values[i]);
                }
            }

            return values;
        }

        /// <summary>
        /// Get the strongly typed value for a name.
        /// </summary>
        public T Get<T>(string name)
        {
            string value = _values[name];
            if (value == null) return default(T);
            value = Decode(value);
            return value.ConvertTo<T>();
        }

        private static string Encode(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            return HttpContext.Current.Server.UrlEncode(data);
        }

        private static string Decode(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            return HttpContext.Current.Server.UrlDecode(data);
        }

        public string ComputeHash()
        {
            // get the sorted cookie values
            Dictionary<string, string> sortedData = _values.ToDictionary();
            
            // create a salt
            byte[] data = Encoding.UTF8.GetBytes(sortedData.Values.Join("-"));
            string salt = MachineKey.Encode(data, MachineKeyProtection.Encryption);
            sortedData.Add("salt", salt);

            // compute the hash
            HMACSHA1 hmacsha1 = new HMACSHA1();
            string hash = hmacsha1.ComputeHash(sortedData);

            return hash;
        }

    }
}
