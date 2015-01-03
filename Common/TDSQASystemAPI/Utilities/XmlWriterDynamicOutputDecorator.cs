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

using TDSQASystemAPI.Config;

namespace TDSQASystemAPI.Utilities
{
    /// <summary>
    /// Decorator that allows for dynamic configuration of serialized output.
    /// Assumes that serializable classes corresponding to configured nodes implement IXmlSerializable
    /// and call their own WriteStartElement and WriteEndElement
    /// </summary>
    class XmlWriterDynamicOutputDecorator : XmlWriter
    {
        /// <summary>
        /// internal writer
        /// </summary>
        private XmlWriter writer;

        /// <summary>
        /// contains all node names that configurable to be included/excluded
        /// </summary>
        private HashSet<string> configurableNodeNames;

        /// <summary>
        /// Used to control the writing out of the container node
        /// </summary>
        private Stack<bool> emptyNodeController;

        /// <summary>
        /// Configuration to control conditional output of variables
        /// </summary>
        public SerializationConfig Config { get; private set; }

        public XmlWriterDynamicOutputDecorator(XmlWriter writer, SerializationConfig config)
            : this(writer, config, new List<string> { "ExamineeAttribute", "ExamineeRelationship" })
        {
        }

        public XmlWriterDynamicOutputDecorator(XmlWriter writer, SerializationConfig config, List<string> configurableNodeNames)
        {
            this.writer = writer;
            this.Config = config;
            if (configurableNodeNames == null)
                this.configurableNodeNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            else this.configurableNodeNames = new HashSet<string>(configurableNodeNames, StringComparer.InvariantCultureIgnoreCase);
            emptyNodeController = new Stack<bool>();
        }

        public override void Flush()
        {
            writer.Flush();
        }

        public override string LookupPrefix(string ns)
        {
            return writer.LookupPrefix(ns);
        }

        public new void WriteAttributeString(string localName, string value)
        {
            //todo: add configurable attribute names to SerializationConfig and pull the configured name
            writer.WriteAttributeString(localName, value);
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            writer.WriteBase64(buffer, index, count);
        }

        public override void WriteCData(string text)
        {
            writer.WriteCData(text);
        }

        public override void WriteCharEntity(char ch)
        {
            writer.WriteCharEntity(ch);
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            writer.WriteChars(buffer, index, count);
        }

        public override void WriteComment(string text)
        {
            writer.WriteComment(text);
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            writer.WriteDocType(name, pubid, sysid, subset);
        }

        public override void WriteEndAttribute()
        {
            writer.WriteEndAttribute();
        }

        public override void WriteEndDocument()
        {
            writer.WriteEndDocument();
        }

        public override void WriteEndElement()
        {
            if (emptyNodeController.Pop())
                writer.WriteEndElement();
        }

        public override void WriteEntityRef(string name)
        {
            writer.WriteEntityRef(name);
        }

        public override void WriteFullEndElement()
        {
            if (emptyNodeController.Pop())
                writer.WriteFullEndElement();
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            writer.WriteProcessingInstruction(name, text);
        }

        public override void WriteRaw(string data)
        {
            writer.WriteRaw(data);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            writer.WriteRaw(buffer, index, count);
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            writer.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteStartDocument(bool standalone)
        {
            writer.WriteStartDocument(standalone);
        }

        public override void WriteStartDocument()
        {
            writer.WriteStartDocument();
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            WriteStartElement(prefix, localName, ns, false);
        }

        public void WriteStartElement(string prefix, string localName, string ns, bool bypassController)
        {
            if (!bypassController && this.configurableNodeNames.Contains(localName ?? ""))
            {
                emptyNodeController.Push(false);
                return;
            }

            emptyNodeController.Push(true);
            writer.WriteStartElement(prefix, localName, ns);
        }

        public override WriteState WriteState
        {
            get { return writer.WriteState; }
        }

        public override void WriteString(string text)
        {
            writer.WriteString(text);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            writer.WriteSurrogateCharEntity(lowChar, highChar);
        }

        public override void WriteWhitespace(string ws)
        {
            writer.WriteWhitespace(ws);
        }
    }
}
