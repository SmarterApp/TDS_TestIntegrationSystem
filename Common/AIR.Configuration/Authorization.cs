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

namespace AIR.Configuration
{
    public class Authorization : ConfigurationElement
    {
        public Authorization()
        {
        }

        [ConfigurationProperty("name",
            IsRequired = true,
            IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("url",
            IsRequired = true,
            IsKey = true)]
        public string URL
        {
            get
            {
                return (string)this["url"];
            }
            set
            {
                this["url"] = value;
            }
        }

        [ConfigurationProperty("realm",
            IsRequired = false,
            IsKey = false)]
        public string Realm
        {
            get
            {
                return (string)this["realm"];
            }
            set
            {
                this["realm"] = value;
            }
        }

        [ConfigurationProperty("grantType",
            IsRequired = true,
            IsKey = false)]
        public string GrantType
        {
            get
            {
                return (string)this["grantType"];
            }
            set
            {
                this["grantType"] = value;
            }
        }

        [ConfigurationProperty("username",
            IsRequired = true,
            IsKey = false)]
        public string Username
        {
            get
            {
                return (string)this["username"];
            }
            set
            {
                this["username"] = value;
            }
        }

        private string decryptedPassword = null;

        [ConfigurationProperty("password",
            IsRequired = false,
            IsKey = false)]
        public string Password
        {
            get
            {
                if(decryptedPassword == null)
                    decryptedPassword = (PasswordEncrypted && !String.IsNullOrEmpty((string)this["password"])) ? Security.SecureConfigManager.ToPlainText((string)this["password"]) : (string)this["password"];
                return decryptedPassword;
            }
            set
            {
                this["password"] = value;
            }
        }

        [ConfigurationProperty("clientId",
            IsRequired = true,
            IsKey = false)]
        public string ClientId
        {
            get
            {
                return (string)this["clientId"];
            }
            set
            {
                this["clientId"] = value;
            }
        }

        //TODO: need to encrypt this too?  Only for OSS, and we won't be providing encryption for OSS, so ok as-is for now.
        [ConfigurationProperty("clientSecret",
            IsRequired = false,
            IsKey = false)]
        public string ClientSecret
        {
            get
            {
                return (string)this["clientSecret"];
            }
            set
            {
                this["clientSecret"] = value;
            }
        }

        [ConfigurationProperty("passwordEncrypted",
            IsRequired = false,
            IsKey = false,
            DefaultValue = true)]
        public bool PasswordEncrypted
        {
            get
            {
                if (this["passwordEncrypted"] == null)
                    return true;
                return (bool)this["passwordEncrypted"];
            }
            set
            {
                this["passwordEncrypted"] = value;
            }
        }
    }
}
