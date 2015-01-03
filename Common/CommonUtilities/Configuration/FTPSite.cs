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

namespace CommonUtilities.Configuration
{
    public class FTPSite : ConfigurationElement
    {
        public FTPSite()
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
            IsKey = false)]
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

        [ConfigurationProperty("sslRequired",
            IsRequired = false,
            IsKey = false, 
            DefaultValue = false)]
        public bool SSLRequired
        {
            get
            {
                return (bool)this["sslRequired"];
            }
            set
            {
                this["sslRequired"] = value;
            }
        }

        [ConfigurationProperty("winSCPExecutablePath",
            IsRequired = false,
            IsKey = false)]
        public string WinSCPExecutablePath
        {
            get
            {
                return (string)this["winSCPExecutablePath"];
            }
            set
            {
                this["winSCPExecutablePath"] = value;
            }
        }

        [ConfigurationProperty("username",
            IsRequired = false,
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
                if (decryptedPassword == null)
                    decryptedPassword = (string)this["password"];
                return decryptedPassword;
            }
            set
            {
                this["password"] = value;
            }
        }

        [ConfigurationProperty("hostKey",
            IsRequired = false,
            IsKey = false)]
        public string HostKey
        {
            get
            {
                return (string)this["hostKey"];
            }
            set
            {
                this["hostKey"] = value;
            }
        }

        [ConfigurationProperty("transferMode",
            IsRequired = false,
            IsKey = false,
            DefaultValue = FTPConst.TransferMode.Binary)]
        public FTPConst.TransferMode TransferMode
        {
            get
            {
                return (FTPConst.TransferMode)this["transferMode"];
            }
            set
            {
                this["transferMode"] = value;
            }
        }

        [ConfigurationProperty("timeoutInSeconds",
            IsRequired = false,
            IsKey = false,
            DefaultValue = -1)]
        public int TimeoutInSeconds
        {
            get
            {
                return (int)this["timeoutInSeconds"];
            }
            set
            {
                this["timeoutInSeconds"] = value;
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

        /// <summary>
        /// If FTPing a file that's in memory, can put a path here to a temp dir where the
        /// file can be written prior to FTP.
        /// </summary>
        [ConfigurationProperty("tempDirectoryPath",
            IsRequired = false,
            IsKey = false)]
        public string TempDirectoryPath
        {
            get
            {
                return (string)this["tempDirectoryPath"];
            }
            set
            {
                this["tempDirectoryPath"] = value;
            }
        }
    }
}
