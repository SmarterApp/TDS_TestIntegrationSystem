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
    public abstract class TesteeProperty
    {
        public enum PropertyContext : int
        {
            INITIAL = 0,
            FINAL = 1
        };
        [XmlAttribute("context")]
        public string Context { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("value")]
        public string Value { get; set; }
        [XmlAttribute("contextDate")]
        public DateTime ContextDate { get; set; }


        protected TesteeProperty() { }
        protected TesteeProperty(string context, DateTime contextDate, string name, string value)
        {
            this.Context = context;
            this.Name = name;
            this.Value = value ?? String.Empty;
            this.ContextDate = contextDate;
        }

        protected int CompareTo(TesteeProperty other)
        {
            int compare = this.Name.CompareTo(other.Name);
            if (compare == 0)
                compare = ((int)Enum.Parse(typeof(TesteeProperty.PropertyContext), this.Context.ToUpper())).CompareTo((int)Enum.Parse(typeof(TesteeProperty.PropertyContext), other.Context.ToUpper()));
            return compare;
        }
    }
}
