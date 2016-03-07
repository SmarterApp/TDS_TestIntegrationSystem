/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
namespace System.Xml
{
    public static class XmlReaderExtensions
    {
        public static bool GetAttributeAsBoolean(this XmlReader reader, string name)
        {
            return reader.GetAttributeAsBoolean(name, default(bool));
        }

        public static bool GetAttributeAsBoolean(this XmlReader reader, string name, bool defaultValue)
        {
            string value = reader.GetAttribute(name);
            return value == null ? defaultValue : XmlConvert.ToBoolean(value);
        }

        public static DateTime GetAttributeAsDateTime(this XmlReader reader, string name)
        {
            return reader.GetAttributeAsDateTime(name, default(DateTime));
        }

        public static DateTime GetAttributeAsDateTime(this XmlReader reader, string name, DateTime defaultValue)
        {
            string value = reader.GetAttribute(name);
            return value == null ? defaultValue : XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
        }

        public static float GetAttributeAsFloat(this XmlReader reader, string name)
        {
            return reader.GetAttributeAsFloat(name, default(float));
        }

        public static double GetAttributeAsDouble(this XmlReader reader, string name, double defaultValue)
        {
            string value = reader.GetAttribute(name);
            return value == null ? defaultValue : XmlConvert.ToDouble(value);
        }

        public static double GetAttributeAsDouble(this XmlReader reader, string name)
        {
            return reader.GetAttributeAsDouble(name, default(double));
        }

        public static float GetAttributeAsFloat(this XmlReader reader, string name, float defaultValue)
        {
            string value = reader.GetAttribute(name);
            return value == null ? defaultValue : XmlConvert.ToSingle(value);
        }

        public static decimal GetAttributeAsDecimal(this XmlReader reader, string name)
        {
            return reader.GetAttributeAsDecimal(name, default(decimal));
        }

        public static decimal GetAttributeAsDecimal(this XmlReader reader, string name, decimal defaultValue)
        {
            string value = reader.GetAttribute(name);
            return value == null ? defaultValue : XmlConvert.ToDecimal(value);
        }

        public static int GetAttributeAsInt(this XmlReader reader, string name)
        {
            return reader.GetAttributeAsInt(name, default(int));
        }

        public static int GetAttributeAsInt(this XmlReader reader, string name, int defaultValue)
        {
            string value = reader.GetAttribute(name);
            return String.IsNullOrEmpty(value) ? defaultValue : XmlConvert.ToInt32(value);
        }

        public static long GetAttributeAsLong(this XmlReader reader, string name)
        {
            return reader.GetAttributeAsLong(name, default(long));
        }

        public static long GetAttributeAsLong(this XmlReader reader, string name, long defaultValue)
        {
            string value = reader.GetAttribute(name);
            return value == null ? defaultValue : XmlConvert.ToInt64(value);
        }

        public static Guid GetAttributeAsGuid(this XmlReader reader, string name)
        {
            return reader.GetAttributeAsGuid(name, Guid.Empty);
        }

        public static Guid GetAttributeAsGuid(this XmlReader reader, string name, Guid defaultValue)
        {
            string value = reader.GetAttribute(name);
            return value == null ? defaultValue : XmlConvert.ToGuid(value);
        }

        /// <summary>
        /// Advances the XmlReader to the next descendant element.
        /// </summary>
        /// <returns>true if a descendant element is found; otherwise false. If a child element is not found, 
        /// the XmlReader is positioned on the end tag (NodeType is XmlNodeType.EndElement) of the element. 
        /// If the XmlReader is not positioned on an element when ReadToDescendant was called, this method returns 
        /// false and the position of the XmlReader is not changed.</returns>
        public static bool ReadToDescendant(this XmlReader reader)
        {
            int depth = reader.Depth;

            if (reader.NodeType != XmlNodeType.Element)
            {
                if (reader.ReadState != ReadState.Initial)
                {
                    return false;
                }

                depth--;
            }
            else if (reader.IsEmptyElement)
            {
                return false;
            }

            while (reader.Read() && (reader.Depth > depth))
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Advances the XmlReader to the next sibling element.
        /// </summary>
        /// <returns>true if a sibling element is found; otherwise false. If a sibling element is not found, 
        /// the XmlReader is positioned on the end tag (NodeType is XmlNodeType.EndElement) of the parent element.</returns>
        public static bool ReadToNextSibling(this XmlReader reader)
        {
            XmlNodeType nodeType;

            do
            {
                SkipSubtree(reader);
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    return true;
                }
            }
            while ((nodeType != XmlNodeType.EndElement) && !reader.EOF);

            return false;
        }

        private static void SkipSubtree(XmlReader reader)
        {
            if (reader.ReadState != ReadState.Interactive) return;

            reader.MoveToElement();

            if ((reader.NodeType == XmlNodeType.Element) && !reader.IsEmptyElement)
            {
                int depth = reader.Depth;

                while (reader.Read() && (depth < reader.Depth))
                {
                }

                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    reader.Read();
                }
            }
            else
            {
                reader.Read();
            }
        }

    }

}
