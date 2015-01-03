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

namespace TDSQASystemAPI
{
    [DataContract]
    public class IResultMessage : IDisposable
    {
        public TestResults.TestResult tr { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Boolean success { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Guid oppKey { get; set; }

        public Guid brokerGUID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string error { get; set; }

        public IResultMessage BuildOSSMessage(TestResult tr, Boolean accepted, string error)
        {
            this.oppKey = tr.Opportunity.Key;
            this.success = accepted;
            this.error = error;
            return this;
        }
        public void Dispose()
        {
            this.Dispose();
        }



        
    }

       
    
}
