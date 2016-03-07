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
    public class ToolUsage
    {
        [XmlElement("ToolPage")]
        public List<ToolPage> ToolPages { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("code")]
        public string Code { get; set; }

        public ToolUsage() { }
        public ToolUsage(string sType, string sCode)
        {
            Type = sType;
            Code = sCode;
            ToolPages = new List<ToolPage>();
        }

        public void AddToolPage(ToolPage toolPage)
        {
            if (toolPage != null)
                ToolPages.Add(toolPage);
        }
    }
}
