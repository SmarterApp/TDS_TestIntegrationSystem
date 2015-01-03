/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AIR.Common.Xml
{
    /// <summary>
    /// This class is used to serialize a string as CData.
    /// </summary>
    /// <remarks>
    /// http://blogs.microsoft.co.il/blogs/dorony/archive/2011/01/29/serializing-a-string-as-cdata-in-wcf.aspx
    /// </remarks>
    [XmlSchemaProvider("GetSchema")]
    public sealed class XmlCDataText : IXmlSerializable
    {
        /// <summary>
        /// In order to make a CDataWrapper property appear as a string in the WSDL, we use the XmlSchemaProvider attribute.
        /// </summary>
        public static XmlQualifiedName GetSchema(XmlSchemaSet xs)
        {           
            return XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).QualifiedName;
        }

        // implicit to/from string
        public static implicit operator string(XmlCDataText obj)
        {
            return (obj == null) ? null : obj.Value;
        }

        public static implicit operator XmlCDataText(string value)
        {
            return (value == null) ? null : new XmlCDataText(value);
        }

        // underlying value
        public string Value { get; set; }

        public XmlCDataText()
        {
        }

        public XmlCDataText(string value)
        {
            Value = value;
        }

        // "" => <Node/>
        // "Foo" => <Node><![CDATA[Foo]]></Node>
        public void WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(Value))
            {
                writer.WriteCData(Value);
            }
        }

        // <Node/> => ""
        // <Node></Node> => ""
        // <Node>Foo</Node> => "Foo"
        // <Node><![CDATA[Foo]]></Node> => "Foo"
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                Value = ""; // <Node/>
            }
            else
            {
                reader.Read();
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        Value = ""; // <Node></Node>
                        break;
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        Value = reader.ReadContentAsString();
                        reader.Read();
                        break;
                    default:
                        throw new InvalidOperationException("Expected text or CData but was: " + reader.NodeType);
                }
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}