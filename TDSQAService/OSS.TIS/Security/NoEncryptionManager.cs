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
using System.Threading.Tasks;
using AIR.Configuration.Security;

namespace OSS.TIS.Security
{
    /// <summary>
    /// Assumes values are not encrypted, so just returns them as-is (in string representation)
    /// </summary>
    public class NoEncryptionManager : IEncryptionManager
    {
        #region IEncryptionManager Members

        /// <summary>
        /// returning false disables encryption in the SecureConfigManager; could return true and let the methods below handle the unencrypted values if we wanted
        /// </summary>
        public bool HasKey
        {
            get { return false; }
        }

        public string Decrypt(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public string Decrypt(string cipherText)
        {
            return cipherText;
        }

        #endregion
    }
}
