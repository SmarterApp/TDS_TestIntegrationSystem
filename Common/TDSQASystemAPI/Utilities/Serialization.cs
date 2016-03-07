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
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

using TDSQASystemAPI.Config;

namespace TDSQASystemAPI.Utilities
{
    public static class Serialization
    {
        public static T DeserializeJson<T>(string json)
        {
            T result = default(T);

            if (String.IsNullOrEmpty(json))
                return result;

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));

            // deserialize to object
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                result = (T)jsonSerializer.ReadObject(ms);
            }

            return result;
        }

        public static string SerializeJson<T>(T obj)
        {
            string resultString = null;
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            // serialize object back to json
            using (MemoryStream ms = new MemoryStream())
            {
                jsonSerializer.WriteObject(ms, obj);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    resultString = sr.ReadToEnd();
                }
            }
            return resultString;
        }

        /// <summary>
        /// Deserialize an object from an xml reader.
        /// </summary>
        /// <typeparam name="T">The objects type.</typeparam>
        /// <param name="reader">The reader containing the object xml.</param>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeXml<T>(string xmlAsString) where T : class
        {
            T result = default(T);

            if (String.IsNullOrEmpty(xmlAsString))
                return result;

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlAsString)))
            {
                using (XmlReader reader = XmlReader.Create(ms))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    result = xs.Deserialize(reader) as T;
                }
            }
            return result;
        }

        /// <summary>
        /// Serialize an object to an xml writer.
        /// </summary>
        /// <typeparam name="T">The objects type.</typeparam>
        /// <param name="writer">The writer to serialize the xml to.</param>
        /// <param name="obj">The object to serialized.</param>
        public static string SerializeXml<T>(T obj)
        {
            string resultString = null;

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(textWriter, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true, Encoding = Encoding.UTF8 }))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", ""); // this removes all namespaces from serialization
                    xs.Serialize(writer, obj, ns);
                    writer.Flush();
                    resultString = textWriter.ToString();
                }
            }

            return resultString;
        }

        public static string SerializeXml<T>(T obj, SerializationConfig config)
        {
            string resultString = null;

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter writer = new XmlWriterDynamicOutputDecorator(XmlWriter.Create(textWriter, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true, Encoding = Encoding.UTF8 }), config))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", ""); // this removes all namespaces from serialization
                    xs.Serialize(writer, obj, ns);
                    writer.Flush();
                    resultString = textWriter.ToString();
                }
            }

            return resultString;
        }

        /// <summary>
        /// Get an indented string version of the XmlDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string XmlDocumentToString(XmlDocument doc)
        {
            string resultString = null;
            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(textWriter, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true }))
                {
                    doc.WriteTo(writer);
                    writer.Flush();
                    resultString = textWriter.ToString();
                }
            }

            return resultString;
        }
    }
}
