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
using System.Runtime.Serialization.Json;
using TDSQASystemAPI.TestResults;

namespace TDSQASystemAPI.Acknowledgement
{
    [DataContract]
    public class Message
    {
        [DataMember(Order = 1)]
        public Guid oppKey { get; set; }

        [DataMember(Order = 2)]
        public Boolean success { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string error { get; set; }

        [DataMember(Order = 4)]
        public string trt { get; set; }

        public Message(Guid oppKey, bool success, string error, string trt = null)
        {
            this.oppKey = oppKey;
            this.success = success;
            this.error = error;
            this.trt = trt;
        }
    }

}
