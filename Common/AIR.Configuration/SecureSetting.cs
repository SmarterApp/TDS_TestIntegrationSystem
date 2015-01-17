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
using System.Configuration;
using System.Collections;
using AIR.Configuration.Security;

namespace AIR.Configuration
{
    public class SecureSetting : ConfigurationElement
    {
        public SecureSetting()
        {
        }

        [ConfigurationProperty("key",
            IsRequired = true,
            IsKey = true)]
        public string Key
        {
            get
            {
                return (string)this["key"];
            }
            set
            {
                this["key"] = value;
            }
        }

        private string decryptedValue = null;

        [ConfigurationProperty("value",
            IsRequired = false,
            IsKey = false)]
        public string Value
        {
            get
            {
                if (decryptedValue == null)
                {
                    try
                    {
                        decryptedValue = (Encrypted && !String.IsNullOrEmpty((string)this["value"])) ? SecureConfigManager.ToPlainText((string)this["value"]) : (string)this["value"];
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to decrypt " + (string)this["value"], e);
                    }
                }
                return decryptedValue;
            }
            set
            {
                this["value"] = value;
            }
        }

        // for testing purposes only.  Set to false to use decrypted values.
        [ConfigurationProperty("encrypted",
            IsRequired = false,
            IsKey = false,
            DefaultValue = true)]
        public bool Encrypted
        {
            get
            {
                if (this["encrypted"] == null)
                    return true;
                return (bool)this["encrypted"];
            }
            set
            {
                this["encrypted"] = value;
            }
        }
    }
}
