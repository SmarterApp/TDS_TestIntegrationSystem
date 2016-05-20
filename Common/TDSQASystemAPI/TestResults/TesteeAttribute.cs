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
    public class TesteeAttribute : TesteeProperty, IComparable<TesteeAttribute>, IEquatable<TesteeAttribute>, IXmlSerializable
    {
        public static class AttributeName
        {
            //public const string SSID = "SSID";
            public const string FIRST_NAME = "FirstName";
            public const string LAST_NAME = "LastName";
            public const string BIRTH_DATE = "DOB";
            public const string ENROLLED_GRADE = "Grade";
            public const string GENDER = "Gender";
            public const string ETHNICITY = "Ethnicity";

        }

        public TesteeAttribute() { }
        public TesteeAttribute(string context, DateTime contextDate, string name, string value) 
            : base(context, contextDate, name, value)
        {
        }

        #region IComparable<TesteeAttribute> Members

        public int CompareTo(TesteeAttribute other)
        {
            return base.CompareTo(other);
        }

        #endregion

        #region IEquatable<TesteeAttribute> Members

        public bool Equals(TesteeAttribute other)
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
                    writerDec.WriteStartElement(null, "ExamineeAttribute", "", true);
            }

            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("value", Value);
            writer.WriteAttributeString("context", Context);
            writer.WriteAttributeString("contextDate", System.Xml.XmlConvert.ToString(ContextDate, System.Xml.XmlDateTimeSerializationMode.Unspecified));
            
            if (writerDec != null)
                writer.WriteEndElement();
        }

        #endregion

    }
}
