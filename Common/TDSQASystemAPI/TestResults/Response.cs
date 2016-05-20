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
using System.Xml.Serialization;

namespace TDSQASystemAPI.TestResults
{
    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class Response
    {
        [XmlText()]
        public string ResponseString { get; set; }
        [XmlAttribute("type")]
        public string type { get; set; }
        [XmlAttribute("date")]
        public DateTime date { get; set; } // for paper test MC items

        public Response() { }
        public Response(string response, string type, DateTime date)
        {
            this.ResponseString = response;
            this.type = type;
            this.date = date;
        }

        public Response DeepCopy()
        {
            return (Response)this.MemberwiseClone();
        }
    }
}
