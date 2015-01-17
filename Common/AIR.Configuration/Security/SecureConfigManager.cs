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
using AIR.Configuration;

namespace AIR.Configuration.Security
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

        private static IEncryptionManager KeyUtil { get; set; }

        /// <summary>
        /// To be added to start of application. This will set ConfigurationManager's s_configSystem to ours.
        /// </summary>
        public static bool Initialize(IEncryptionManager encryptionManager)
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
                        if (encryptionManager == null || !encryptionManager.HasKey)
                        {
                            return false;
                        }

                        SecureConfigManager.KeyUtil = encryptionManager;

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
                            if (Settings.SecureSetting != null)
                            {
                                foreach (SecureSetting setting in Settings.SecureSetting)
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
            b.Password = KeyUtil.Decrypt(cipherText);
            return b.ConnectionString;

            /*     return Regex.Replace(connectionString, @"password=[^;\s]*",
                     String.Format("password={0}", (string.IsNullOrEmpty(_key)? KeyUtil.ToPlainText(cipherText) : 
                     KeyUtil.ToPlainText(cipherText, _key, _initializationVector))));
            
            
             return connectionString;*/
        }

        public static string ToPlainText(string cipherText)
        {
            if (KeyUtil == null)
                throw new CryptographicException("No IEncryptionManager was provided");
            return KeyUtil.Decrypt(cipherText);
        }

        public static string ToPlainText(byte[] data)
        {
            if (KeyUtil == null)
                throw new CryptographicException("No IEncryptionManager was provided");
            return KeyUtil.Decrypt(data);
        }
    }
}
