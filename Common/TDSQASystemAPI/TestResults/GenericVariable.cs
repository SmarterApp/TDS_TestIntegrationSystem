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
    public class GenericVariable
    {
        [XmlIgnore]
        public const string APPEAL_REQUEST_ID_VAR_NAME = "REQUEST_ID";
        [XmlIgnore]
        public const string RESCORE_IND_VAR_NAME = "RESCORE_IND";

        public enum VariableContext
        {
            DEMOGRAPHIC,
            CALCULATED,
            MERGE,
            APPEAL,
            OTHER
        }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlIgnore]
        private VariableContext? context;
        [XmlIgnore]
        public VariableContext Context
        {
            get
            {
                if (!context.HasValue)
                {
                    VariableContext c;
                    if (!Enum.TryParse(ContextString, out c))
                        context = VariableContext.OTHER;
                    else context = c;
                }
                return context.Value;
            }
            private set
            {
                context = value;
            }
        }

        [XmlAttribute("context")]
        public string ContextString { get; set; }

        public GenericVariable() { }
        public GenericVariable(string context, string name, string value)             
        {
            VariableContext vc = VariableContext.OTHER;
            this.Context = (Enum.TryParse(context, out vc) && Enum.IsDefined(typeof(VariableContext), vc)) ? vc : VariableContext.OTHER;
            this.ContextString = context;
            this.Name = name;
            this.Value = value;
        }

        public GenericVariable(VariableContext context, string name, string value)
        {
            this.Context = context;
            this.ContextString = context.ToString();
            this.Name = name;
            this.Value = value;
        }
    }
}
