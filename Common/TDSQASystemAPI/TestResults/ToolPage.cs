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
using System.Xml;
using System.Xml.Serialization;

namespace TDSQASystemAPI.TestResults
{
    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class ToolPage
    {
        [XmlAttribute("page")]
        public int PageNumber { get; set; }
        [XmlAttribute("groupId")]
        public string GroupID { get; set; }
        [XmlAttribute("count")]
        public int Count { get; set; }

        public ToolPage() { }
        public ToolPage(int nPageNumber, string sGroupId, int nCount)
        {
            PageNumber = nPageNumber;
            GroupID = sGroupId;
            Count = nCount;
        }
    }
}
