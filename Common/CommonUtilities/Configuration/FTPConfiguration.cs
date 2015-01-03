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

namespace CommonUtilities.Configuration
{
    public static class FTPConfiguration
    {
        private static object ftpConfig = ConfigurationManager.GetSection("FTPSettings");
        [Obsolete("Use Settings", false)]
        public static FTPSettings FTP
        {
            get
            {
                if (ftpConfig == null)
                    throw new ApplicationException("FTPSettings are not configured for this application.");
                return ((FTPConfig)ftpConfig).FTPSettings;
            }
        }
    }
}
