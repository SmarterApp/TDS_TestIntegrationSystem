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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace TIS.ScoringDaemon.Abstractions
{
    [DataContract]
    public class ItemScoreRequestContextToken
    {
        [DataMember(EmitDefaultValue = false)]
        public string clientName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string environment { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Guid oppKey { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public long? oppID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int itemBank { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int itemID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? reportingVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string TISIP { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string TISDbName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string itemType { get; set; }

        public string ItemKey { get { return String.Format("{0}-{1}", itemBank, itemID); } }

        public override string ToString()
        {
            return String.Format("client: {0}, opp: {1}, item: {2}", clientName ?? "Unknown", 
                oppID == null ? oppKey == null ? "Unknown" : oppKey.ToString() : oppID.Value.ToString(), // prefer oppId at this point to debug issues
                ItemKey ?? "Unknown");
        }
    }
}
