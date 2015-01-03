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
using System.Configuration.Internal;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace CommonUtilities.Security
{
    /// <summary>
    /// Override ConfigurationManager.ConnectionStrings to return modified connectionstrings.
    /// </summary>
    public sealed class SecureConfigManager : IInternalConfigSystem
    {
        private readonly IInternalConfigSystem _baseconf;
        private readonly object lockme = new object(); // instance lock
        private static readonly object sLockme = new object(); // class lock
        private static bool initialized = false;
        private static bool secureConnectionStrings;

        static SecureConfigManager()
        {
            bool b;
            if (ConfigurationManager.AppSettings["secureConnectionStrings"] == null
                || !Boolean.TryParse(ConfigurationManager.AppSettings["secureConnectionStrings"].ToString(), out b))
                b = true;
            secureConnectionStrings = b;
        }

        /// <summary>
        /// To be added to start of application. This will set ConfigurationManager's s_configSystem to ours.
        /// </summary>
        public static bool Initialize()
        {
            // replace config manager with our own version
            if (!initialized)
            {
                lock (sLockme)
                {
                    // check again in case another thread beat us to the punch after the previous check but before the lock was taken
                    if (!initialized)
                    {
                        // check if a key was defined, otherwise assume encryption is disabled
                        if (string.IsNullOrEmpty(_key))
                        {
                            return false;
                        }

                        // initialize manager and load from disk if not done already
                        ConfigurationManager.ConnectionStrings.IsReadOnly();
                        
                        // get s_configSystem field (private static volatile) from ConfigurationManager
                        FieldInfo s_configSystem = typeof(ConfigurationManager).GetField("s_configSystem",
                            BindingFlags.Static | BindingFlags.NonPublic);
                        // set its value to our modified implementation of IInternalConfigSystem
                        s_configSystem.SetValue(null, new SecureConfigManager((IInternalConfigSystem)s_configSystem.GetValue(null)));

                        initialized = true;
                    }
                }
            }

            return true;
        }

        private static string _key;
        private static string _initializationVector;
        /// <summary>
        /// custom initialize to use custom keys if needed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="initializationVector"></param>
        public static bool InitializeWithKeys(string key, string initializationVector)
        {
            lock (sLockme)
            {
                if ((_key ?? "").Equals(key) && (_initializationVector ?? "").Equals(initializationVector))
                    return false;

                _key = key;
                _initializationVector = initializationVector;
                initialized = false;
            }
            return Initialize();
        }

        /// <param name="baseconf">IInternalConfigSystem to modify</param>
        private SecureConfigManager(IInternalConfigSystem baseconf)
        {
            _baseconf = baseconf;
        }

        // the modified ConnectionStringsSection object we want to use vs the one in the config file.
        private object _connectionStrings;
        // the modified appSettings collection that will include any SecureSettings with decrypted values.
        private object _appSettings;

        /// <summary>
        /// Retrieve config object based on key. If connectionstrings, we want to return our modified 
        /// config object. Otherwise, return baseconf's implementation.
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        public object GetSection(string configKey)
        {
            if (configKey != "connectionStrings" && configKey != "appSettings")
                return _baseconf.GetSection(configKey);

            // we'll keep a collection that includes both appSettings and SecureSettings so that they can 
            //  both be accessed via appSettings
            if (configKey == "appSettings")
            {
                if (_appSettings == null)
                {
                    lock (lockme)
                    {
                        if (_appSettings == null)
                        {
                            NameValueCollection appSettingsSection = (NameValueCollection)_baseconf.GetSection(configKey);
                             // first add appSettings to the a new collection
                            NameValueCollection newAppSettings = new NameValueCollection(appSettingsSection); 
                            // then add SecureSettings with decrypted values.
                            if (CommonUtilities.Configuration.Settings.SecureSetting != null)
                            {
                                foreach (CommonUtilities.Configuration.SecureSetting setting in CommonUtilities.Configuration.Settings.SecureSetting)
                                {
                                    if (appSettingsSection[setting.Key] != null)
                                        throw new ApplicationException(String.Format("There's already an appSetting with key: {0}.  Can't have SecureSetting with the same key.", setting.Key));
                                    newAppSettings.Add(setting.Key, setting.Value);
                                }
                            }
                            _appSettings = newAppSettings;
                        }
                    }
                }
                return _appSettings;
            }

            // the ConnectionStringsSection is being requested.  If we don't yet have it cached, cache it.
            if (_connectionStrings == null)
            {
                lock (lockme)
                {
                    if (_connectionStrings == null)
                    {
                        ConnectionStringsSection csSection = _baseconf.GetSection(configKey) as ConnectionStringsSection;
                        if (csSection == null) // not sure how this could happen, but just in case
                            throw new ApplicationException(String.Format("ConfigKey: {0} does not cast to ConnectionStringsSection?!", configKey));

                        ConnectionStringSettingsCollection baseCsCollection = csSection.ConnectionStrings;

                        // create a new ConnectionStringsSection because we're unable to set base ConnectionStringSettingsCollection.
                        ConnectionStringsSection newCsSection = new ConnectionStringsSection();
                        // copy each ConnectionStringSettings from 
                        foreach (ConnectionStringSettings baseCss in baseCsCollection)
                        {
                            var newCs = DecryptConnectionStringPassword(baseCss.ConnectionString);
                            ConnectionStringSettings newCss = new ConnectionStringSettings(baseCss.Name, newCs, baseCss.ProviderName);
                            newCsSection.ConnectionStrings.Add(newCss);
                        }
                        _connectionStrings = newCsSection;
                    }
                }
            }
            return _connectionStrings;
        }

        /// <summary>
        /// Modified to retrieve our connection string section
        /// </summary>
        public void RefreshConfig(string sectionName)
        {
            if (sectionName == "connectionStrings")
            {
                lock (lockme)
                {
                    _connectionStrings = null;
                    _appSettings = null;
                }
            }
            _baseconf.RefreshConfig(sectionName);
        }

        public bool SupportsUserConfig
        {
            get { return _baseconf.SupportsUserConfig; }
        }

        private string DecryptConnectionStringPassword(string connectionString)
        {
            // if we're not securing connection strings, just return the original
            if (!secureConnectionStrings)
                return connectionString;

            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder(connectionString);
            //AM: added this to deal with connection strings using integrated security
            if (String.IsNullOrEmpty(b.Password))
                return connectionString;

            byte[] cipherText = Convert.FromBase64String(b.Password);
            b.Password = ToPlainText(cipherText, _key, _initializationVector);
            return b.ConnectionString;

        }

        public static string ToPlainText(byte[] data, string keyBase64, string ivBase64)
        {
            // check for key/iv parameters
            if (String.IsNullOrEmpty(keyBase64) || String.IsNullOrEmpty(ivBase64))
            {
                throw new Exception("There is no key/iv provided.");
            }

            byte[] key = Convert.FromBase64String(keyBase64);
            byte[] iv = Convert.FromBase64String(ivBase64);
            byte[] decryptedData = Decrypt(data, key, iv);
            return Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        /// Utility function to Decrypt a cipher given a key and iv
        /// </summary>
        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            // Declare the string used to hold 
            // the decrypted text. 
            byte[] decrypted;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                        cs.Close();
                    }
                    decrypted = ms.ToArray();
                    ms.Close();
                }

            }
            return decrypted;
        }
    }
}
