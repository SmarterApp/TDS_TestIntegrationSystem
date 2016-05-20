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
using System.Xml.Serialization;

namespace TDS.ItemScoringEngine
{
    public enum PropositionState {NotApplicable = -1, NotAsserted = 0, Asserted=1}
    public class Proposition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("description")]
        public string Desc { get; set; }
        [XmlAttribute("state")]
        public PropositionState State { get; set; }
    }
}
