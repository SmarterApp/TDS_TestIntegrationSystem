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
using System.Configuration;
using TDSQASystemAPI.Config;

namespace OSS.TIS.Config
{
    public class SystemConfigurationManager : ISystemConfigurationManager
    {
        public string System
        {
            get
            {
                return "QASystem";
            }
        }

        public string Environment
        {
            get
            {
                return ConfigurationManager.AppSettings["Environment"];
            }
        }

        #region ISystemConfigurationManager Members

        // note: clientName isn't currently used.  Will ne needed when this is switched over to multi-client or 
        //  if we switch to AIRConfiguration
        public string GetConfigSettingsValueOrEmptyString(string clientName, string key)
        {
            string val = ConfigurationManager.AppSettings[key];
            if (val == null) // try connection strings; some of the common code uses the config mgr to get conn strs.
            {
                // there is some common code that expects TDSQCConnectionString, but most expects TDSQC; use TDSQC
                //  same for ITEMBANK
                if (key == "TDSQCConnectionString")
                    key = "TDSQC";
                else if (key == "ItemBankConnectionString")
                    key = "ITEMBANK";
                val = ConfigurationManager.ConnectionStrings[key] == null ? null : ConfigurationManager.ConnectionStrings[key].ConnectionString;
            }
            return val ?? String.Empty;
        }

        public string GetConfigSettingsValue(string clientName, string key)
        {
            string val;
            if (String.IsNullOrEmpty(val = GetConfigSettingsValueOrEmptyString(clientName, key)))
                throw new ApplicationException(String.Format("There is no app setting with key: {0}", key));
            return val;
        }

        #endregion
    }
}
