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

using TDSQASystemAPI.TestResults;

namespace TDSQASystemAPI.Config
{
    //todo: Make this class more robust when we implement the IXmlSerializable classes for configurable serialization
    public class SerializationConfig
    {
 //       public bool IncludeAccommodations { get; private set; }
        public List<RTSAttribute> IncludedAttributes { get; private set; }

        public SerializationConfig(/*bool includeAccommodations,*/ List<RTSAttribute> attributes)
        {
  //          this.IncludeAccommodations = includeAccommodations;
            this.IncludedAttributes = attributes;
        }

        /// <summary>
        /// returns true if it finds an attribute with the same name and context
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool IncludeAttribute(TesteeProperty property)
        {
            return IncludedAttributes != null && IncludedAttributes.Exists(x => x.XMLName.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)
                                                                                && x.Context.Equals(property.Context, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
