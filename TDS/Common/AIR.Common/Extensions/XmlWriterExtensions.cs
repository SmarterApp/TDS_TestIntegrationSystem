/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;

namespace System.Xml
{
    public static class XmlWriterExtensions
    {
        public static void WriteElementValue(this XmlWriter writer, string elementName, object value)
        {
            if (value != null)
            {
                writer.WriteStartElement(elementName);
                writer.WriteValue(value);
                writer.WriteEndElement();
            }
        }

        public static void WriteElementValue(this XmlWriter writer, string elementName, Enum value)
        {
            if (value != null)
            {
                string name = Enum.GetName(value.GetType(), value);
                writer.WriteStartElement(elementName);
                writer.WriteValue(name);
                writer.WriteEndElement();
            }
        }

        public static void WriteElementCData(this XmlWriter writer, string elementName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.WriteStartElement(elementName);
                writer.WriteCData(value.Trim());
                writer.WriteEndElement();
            }
        }

        public static void WriteAttribute<T>(this XmlWriter writer, string attribName, T value)
        {
            if (value is Boolean)
            {
                writer.WriteAttributeString(attribName, value.ToString().ToLower());
            }
            else
            {
                writer.WriteAttributeString(attribName, value.ToString());
            }
        }
    }
}