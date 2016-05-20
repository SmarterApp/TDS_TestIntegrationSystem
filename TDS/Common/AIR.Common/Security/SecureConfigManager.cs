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
using System.Web.Configuration;
using AIR.Common.Configuration;
using AIR.Common.Dynamic;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace AIR.Common.Security
{
    /// <summary>
    /// Override ConfigurationManager.ConnectionStrings to return modified connectionstrings.
    /// </summary>
    public sealed class SecureConfigManager : IInternalConfigSystem
    {
        private readonly IInternalConfigSystem _baseconf;

        /// <summary>
        /// To be added to start of application. This will set ConfigurationManager's s_configSystem to ours.
        /// </summary>
        public static bool Initialize()
        {
            // check if securing connection string is disabled
            if (!AppSettingsHelper.GetBoolean("secureConnectionStrings", true))
            {
                return false;
            }

            // check if a key was defined, otherwise assume encryption is disabled
            if (string.IsNullOrEmpty(_key))
            {
                return false;
            }

            InitializeMachineKey();

            // initialize manager and load from disk if not done already
            ConfigurationManager.ConnectionStrings.IsReadOnly();

            // get s_configSystem field (private static volatile) from ConfigurationManager
            FieldInfo s_configSystem = typeof(ConfigurationManager).GetField("s_configSystem",
                BindingFlags.Static | BindingFlags.NonPublic);
            // set its value to our modified implementation of IInternalConfigSystem
            s_configSystem.SetValue(null, new SecureConfigManager((IInternalConfigSystem)s_configSystem.GetValue(null)));

            return true;
        }

        private static bool InitializeMachineKey()
        {
            string validationKey = AppSettingsHelper.GetSecure("aspnet.mk.vk");
            string decryptionKey = AppSettingsHelper.GetSecure("aspnet.mk.dk");

            // check if keys were provided;
            if (validationKey == null || decryptionKey == null) return false;

            // get <machineKey>
            var machineKeySection = ConfigurationManager.GetSection("system.web/machineKey") as MachineKeySection;
            if (machineKeySection == null) return false;

            // trigger data to load
            machineKeySection.IsReadOnly();

            // dynmically set values
            var configValues = machineKeySection.ToDynamic().Values; // ConfigurationValues
            configValues["compatibilityMode"] = MachineKeyCompatibilityMode.Framework20SP1;
            configValues["validation"] = "SHA1";
            configValues["validationKey"] = validationKey;
            configValues["decryption"] = "AES";
            configValues["decryptionKey"] = decryptionKey;

            return true;
        }

        private static string _key;
        private static string _initializationVector;
        /// <summary>
        /// custom initialize to use custom keys if needed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="initializationVector"></param>
        public static void InitializeWithKeys(string key, string initializationVector)
        {
            _key = key;
            _initializationVector = initializationVector;
            Initialize();
        }

        /// <param name="baseconf">IInternalConfigSystem to modify</param>
        public SecureConfigManager(IInternalConfigSystem baseconf)
        {
            _baseconf = baseconf;
        }

        // the modified ConnectionStringsSection object we want to use vs the one in the config file.
        private object _connectionStrings;

        /// <summary>
        /// Retrieve config object based on key. If connectionstrings, we want to return our modified 
        /// config object. Otherwise, return baseconf's implementation.
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        public object GetSection(string configKey)
        {
            // if our modified ConnectionStringsSection object is set, return.
            if (configKey == "connectionStrings" && _connectionStrings != null)
                return _connectionStrings;

            // get section from config file on disk
            object o = _baseconf.GetSection(configKey);
            // copy all original connection strings into ours
            if (configKey == "connectionStrings" && o is ConnectionStringsSection)
            {
                ConnectionStringSettingsCollection baseCsCollection = ((ConnectionStringsSection) o).ConnectionStrings;

                // create a new ConnectionStringsSection because we're unable to set base ConnectionStringSettingsCollection.
                ConnectionStringsSection newCsSection = new ConnectionStringsSection();
                // copy each ConnectionStringSettings from 
                foreach (ConnectionStringSettings baseCss in baseCsCollection)
                {
                    var newCs = DecryptConnectionStringPassword(baseCss.ConnectionString);
                    ConnectionStringSettings newCss = new ConnectionStringSettings(baseCss.Name, newCs, baseCss.ProviderName);
                    newCsSection.ConnectionStrings.Add(newCss);
                }
                o = _connectionStrings = newCsSection;
            }
            return o;
        }

        /// <summary>
        /// Modified to retrieve our connection string section
        /// </summary>
        public void RefreshConfig(string sectionName)
        {
            if (sectionName == "connectionStrings") _connectionStrings = null;
            _baseconf.RefreshConfig(sectionName);
        }

        public bool SupportsUserConfig
        {
            get { return _baseconf.SupportsUserConfig; }
        }

        private string DecryptConnectionStringPassword(string connectionString)
        {
            Match m = Regex.Match(connectionString, @"password=[^;\s]*");
            if (m.Captures.Count > 0)
            {
                byte[] cipherText = Convert.FromBase64String(m.Captures[0].Value.Remove(0, 9));
                return Regex.Replace(connectionString, @"password=[^;\s]*",
                    String.Format("password={0}", ToPlainText(cipherText, _key, _initializationVector)));
            }

            return connectionString;
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
