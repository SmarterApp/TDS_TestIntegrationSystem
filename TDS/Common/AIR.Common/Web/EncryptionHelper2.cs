/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Text;
using System.Web;
using System.Web.Security;
using AIR.Common.Security;

namespace AIR.Common.Web
{
    /// <summary>
    /// Helpers methods for encryption and encoding
    /// </summary>
    /// <remarks>
    /// If you are going to use the encryption then you must have a pregenerated <machinekey> set in the <system.web> section of your web.config.
    /// </remarks>
    public static class EncryptionHelper2
    {
        /// <summary>
        /// Encrypt a string to base64.
        /// </summary>
        public static string Encrypt(string text, bool urlSafe = false)
        {
            return MachineKeyData.Protect(text, urlSafe);
        }

        public static string Decrypt(string text, bool urlSafe = false)
        {
            return MachineKeyData.Unprotect(text, urlSafe);
        }

    }
}
