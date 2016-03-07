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
using System.Runtime.Serialization;

namespace TDSQASystemAPI.Routing.Authorization
{
    [DataContract]
    public class OAuthResponse
    {
        [DataMember]
        public string scope { get; set; }
        [DataMember]
        public int expires_in { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember(IsRequired=false)]
        public Guid refresh_token { get; set; }
        [DataMember]
        public Guid access_token { get; set; }

        public string AsString()
        {
            return string.Format("Scope={0}. Expires_In={1}, Token_Type={2}", scope ?? "null", expires_in, token_type ?? "null");
        }
    }
}
