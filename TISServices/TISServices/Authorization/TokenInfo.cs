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
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace TISServices.Authorization
{
    [DataContract]
    public class TokenInfo
    {
        private object lockme;

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            lockme = new object();
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string sbacUUID { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string mail { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string sn { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string cn { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string sbacTenancyChain { get; set; }
        [DataMember(IsRequired=false, EmitDefaultValue=false)]
        public string givenName { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<string> scope { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string grant_type { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string realm { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string token_type { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double expires_in { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Guid access_token { get; set; }

        // if there's an error, these will be populated
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string error { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string error_description { get; set; }

        // lazy load the tenancy chain if we need it
        private List<Tenancy> tenancyChain;
        public List<Tenancy> TenancyChain
        {
            get
            {
                if (tenancyChain == null)
                {
                    lock (lockme)
                    {
                        if (tenancyChain == null)
                        {
                            tenancyChain = Tenancy.CreateTenancyChain(sbacTenancyChain);
                        }
                    }
                }
                return tenancyChain; 
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                // no error and the tenancy chain has at least one TIS Admin role
                return String.IsNullOrEmpty(error) && TenancyChain.FirstOrDefault(t => t.GetTenancyValue(Tenancy.TenancyAttribute.RoleName).Equals("TIS Admin")) != null;
            }
        }
    }
}