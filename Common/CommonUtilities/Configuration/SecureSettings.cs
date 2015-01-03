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
    public class SecureSettings : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SecureSetting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SecureSetting)element).Key;
        }

        new public string this[string key]
        {
            get
            {
                return ((SecureSetting)BaseGet(key)).Value;
            }
        }

        public SecureSetting this[int idx]
        {
            get
            {
                return (SecureSetting)BaseGet(idx);
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }
    }
}
