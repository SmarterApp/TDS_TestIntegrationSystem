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
using System.Xml.Schema;
using System.Xml.Serialization;

using TDSQASystemAPI.Utilities;

namespace TDSQASystemAPI.TestResults
{
    public class TesteeRelationship : TesteeProperty, IComparable<TesteeRelationship>, IEquatable<TesteeRelationship>, IXmlSerializable
    {
        public static class RelationshipName
        {
            public const string SCHOOL_ID = "SchoolID";
            public const string SCHOOL_NAME = "SchoolName";
            public const string DISTRICT_ID = "DistrictID";
            public const string DISTRICT_NAME = "DistrictName";
            public const string DISTRICT_TYPE = "DistrictType";
        }

        //Zach 11/21/2014: Since we extend IXmlSerializable we probably dont need the extra specified flag
        [XmlAttribute("entityKey")]
        public long entityKey { get { return EntityKey.Value; } set { EntityKey = value; } }
        [XmlIgnore]
        public long? EntityKey { get; private set; }
        public bool entityKeySpecified { get { return EntityKey.HasValue; } }

        public TesteeRelationship() { }
        public TesteeRelationship(string context, DateTime contextDate, string name, string value, long? entityKey) 
            : base(context, contextDate, name, value)
        {
            this.EntityKey = entityKey;
        }

        #region IComparable<TesteeRelationship> Members

        public int CompareTo(TesteeRelationship other)
        {
            int val = base.CompareTo(other);
            if (val == 0)
                val = this.EntityKey.Value.CompareTo(other.EntityKey.Value);
            return val;
        }

        #endregion


        #region IEquatable<TesteeRelationship> Members

        public bool Equals(TesteeRelationship other)
        {
            return this.CompareTo(other) == 0;
        }

        #endregion

        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            Name = reader.GetAttribute("name");
            Value = reader.GetAttribute("value");
            Context = reader.GetAttribute("context");

            string contextDateString = reader.GetAttribute("contextDate");
            DateTime d;
            if (!DateTime.TryParse(reader.GetAttribute("contextDate"), out d))
                throw new InvalidCastException(string.Format("contextDate was invalid for TesteeAttribute. Name={0}, Value={1}, Context={2}, ContextDate={3}", Name ?? "null", Value ?? "null", Context ?? "null", reader.GetAttribute("contextDate") ?? "null"));
            ContextDate = d;

            string entityKeyString = reader.GetAttribute("entityKey");
            if (!string.IsNullOrEmpty(entityKeyString))
                entityKey = Int64.Parse(entityKeyString);     

            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlWriterDynamicOutputDecorator writerDec = writer as XmlWriterDynamicOutputDecorator;
            if (writerDec != null)
            {
                if (!writerDec.Config.IncludeAttribute(this))
                    return;
                else
                    writerDec.WriteStartElement(null, "ExamineeRelationship", "", true);
            }

            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("value", Value);
            writer.WriteAttributeString("context", Context);
            writer.WriteAttributeString("contextDate", System.Xml.XmlConvert.ToString(ContextDate, System.Xml.XmlDateTimeSerializationMode.Unspecified));

            if (EntityKey.HasValue)
                writer.WriteAttributeString("entityKey", entityKey.ToString());

            if (writerDec != null)
                writer.WriteEndElement();
        }

        #endregion
    }
}
