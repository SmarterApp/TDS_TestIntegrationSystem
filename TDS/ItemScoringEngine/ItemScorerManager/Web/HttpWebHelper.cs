/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TDS.ItemScoringEngine.Web
{
    /// <summary>
    /// Helper functions to make web requests/responses.
    /// </summary>
    public class HttpWebHelper
    {
        /// <summary>
        /// Deserialize an object from an xml reader.
        /// </summary>
        /// <typeparam name="T">The objects type.</typeparam>
        /// <param name="reader">The reader containing the object xml.</param>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeXml<T>(XmlReader reader) where T : class
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            return xs.Deserialize(reader) as T;
        }

        /// <summary>
        /// Serialize an object to an xml writer.
        /// </summary>
        /// <typeparam name="T">The objects type.</typeparam>
        /// <param name="writer">The writer to serialize the xml to.</param>
        /// <param name="obj">The object to serialized.</param>
        public static void SerializeXml<T>(XmlWriter writer, T obj)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", ""); // this removes all namespaces from serialization
            xs.Serialize(writer, obj, ns);
        }

        /// <summary>
        /// Serialize an object to a string.
        /// </summary>
        /// <typeparam name="T">The objects type.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The xml of the serialized object with indenting.</returns>
        public static string SerializeXml<T>(T obj, bool indent = true)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true, // Larry doesn't want this
                ConformanceLevel = ConformanceLevel.Document,
                Encoding = new UTF8Encoding(false), // pass false to remove BOM
                Indent = indent
            };

            // serialize into stream
            MemoryStream ms = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(ms, settings);
            SerializeXml(writer, obj);

            // get string from stream
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        /// <summary>
        /// Deserialize an object from a HttpWebRequest.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <param name="webRequest">The HttpWebRequest to read the object from.</param>
        /// <returns>The deserialized object from the HttpWebRequest.</returns>
        private static T DeserializeXml<T>(HttpWebRequest webRequest) where T : class
        {
            T outputData = default(T);

            using (HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse)
            {
                // Parse the stream if it is XML and there was an output data object passed in
                if (webResponse != null && webResponse.ContentType == "text/xml")
                {
                    // Get the response stream  
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (XmlReader reader = XmlReader.Create(responseStream))
                            {
                                outputData = DeserializeXml<T>(reader);
                                reader.Close();
                            }
                        }
                    }
                }
            }
            return outputData;
        }

        /// <summary>
        /// Serialize an object and send it to a url using HTTP POST.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="url">The url to send the object to.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>Return the HttpWebRequest that was used to send the object.</returns>
        public static HttpWebRequest SendXml<T>(string url, T obj) where T : class
        {
            // Create the web request
            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.KeepAlive = false;

            // Set type to POST
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml";

            // Write data  
            using (Stream postStream = webRequest.GetRequestStream())
            {
                using (XmlWriter writer = XmlWriter.Create(postStream))
                {
                    SerializeXml(writer, obj);
                }
            }

            return webRequest;
        }

        public static void SendXml<T>(string url, T obj, StringBuilder sb) where T : class
        {
            HttpWebRequest webRequest = SendXml(url, obj);

            // Get response
            using (HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                sb.Append(reader.ReadToEnd());
                webResponse.Close();
            }
        }

        public static O SendAndReadXml<I, O>(string url, I inputData) where I : class where O : class
        {
            HttpWebRequest webRequest = SendXml(url, inputData);
            return DeserializeXml<O>(webRequest);
        }
       
    }
}
