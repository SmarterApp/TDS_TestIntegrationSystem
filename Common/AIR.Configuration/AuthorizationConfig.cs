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
    public class AuthorizationConfig : ConfigurationSection
    {
        //Index into the AuthorizationSettings by name.
        //Note: Setting name to "" allows me to skip the outer <AuthorizationSettings><AuthorizationSettings> node
        [ConfigurationProperty("",
            IsDefaultCollection = true, IsRequired = true)]
        public AuthorizationSettings AuthSettings
        {
            get
            {
                AuthorizationSettings authSettingsCollection =
                (AuthorizationSettings)base[""];
                return authSettingsCollection;
            }
        }
    }
}
