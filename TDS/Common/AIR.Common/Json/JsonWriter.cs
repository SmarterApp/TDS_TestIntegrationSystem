/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using AIR.Common.Configuration;
using AIR.Common.Utilities;

namespace AIR.Common.Json
{
    /// <summary>
    /// A manual JSON Writer that uses WCF manual XmlWriter underneath the covers (no reflection).
    /// </summary>
    /// <remarks>
    /// MONO: Got some good ideas from MONO's serialization code (but is different code so you cannot diff).
    /// https://github.com/mono/mono/tree/master/mcs/class
    /// https://github.com/mono/mono/tree/master/mcs/class/System.ServiceModel.Web/System.Runtime.Serialization.Json
    /// https://github.com/mono/mono/tree/master/mcs/class/System.ServiceModel.Web/Test/System.Runtime.Serialization.Json
    /// Version of JsonSerializer at the time: https://github.com/mono/mono/commit/382a385a8f29133c05f538ecf2e7569e668fcfca#mcs/class/System.ServiceModel.Web/System.Runtime.Serialization.Json/JsonSerializationWriter.cs
    /// </remarks>
    public class JsonWriter : IDisposable
    {
        private readonly XmlWriter _writer;

        public JsonWriter(XmlWriter xmlWriter)
        {
            this._writer = xmlWriter;
        }

        public static JsonWriter Create(Stream stream, Encoding encoding, bool ownsStream)
        {
            XmlDictionaryWriter xmlDictionaryWriter = JsonReaderWriterFactory.CreateJsonWriter(stream, encoding, ownsStream);
            JsonWriter jsonWriter = new JsonWriter(xmlDictionaryWriter);
            jsonWriter.WriteRoot(); // NOTE: you have to this to start
            return jsonWriter;
        }

        public static JsonWriter Create(Stream stream)
        {
            return Create(stream, Encoding.UTF8, false);
        }

        public XmlWriter XmlWriter
        {
            get { return _writer; }
        }

        public void WriteRoot()
        {
            _writer.WriteStartElement("root");
        }

        public void WriteStartObjectLiteral() // {
        {
            _writer.WriteAttributeString("type", "object");
        }

        public void WriteStartObjectLiteral(string name) // "name": {
        {
            WriteStartObject(name);
            WriteStartObjectLiteral();
        }

        public void WriteStartObject(string name) // "name"
        {
            _writer.WriteStartElement(name);
        }

        public void WriteEndObject() // } or ]
        {
            _writer.WriteEndElement();
        }

        public void WriteObject(string name, object value) // "name": 1
        {
            WriteStartObject(name);
            WriteObjectContent(value);
            WriteEndObject();
        }

        /// <summary>
        /// Call this when starting to create an array.
        /// </summary>
        public void WriteStartArray()
        {
            _writer.WriteAttributeString("type", "array");
        }

        /// <summary>
        /// Call this before adding a value to an array.
        /// </summary>
        private void WriteStartArrayObject()
        {
            WriteStartObject("item");
        }

        /// <summary>
        /// Call this to add a value to an array
        /// </summary>
        public void WriteArrayObject(object graph)
        {
            WriteObject("item", graph);
        }

        public void WriteArray<T>(IEnumerable<T> list)
        {
            WriteCollection(list);
            WriteEndObject(); // ]
        }

        /// <summary>
        /// Manually create an array of values or objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<T> WriteEnumerable<T>(IEnumerable<T> list)
        {
            WriteStartArray(); // [

            foreach (T value in list)
            {
                // start array object
                WriteStartArrayObject();

                // 
                yield return value;
            }

            WriteEndObject(); // ]
        }

        private void WriteNull()
        {
            _writer.WriteAttributeString("type", "null");
            //_writer.WriteString(null);
        }

        private void WriteObjectContent(object graph)
        {
            if (graph == null)
            {
                WriteNull();
                return;
            }

            if (graph is AppSetting)
            {
                graph = ((AppSetting)graph).GetObject();
            }

            var type = graph.GetType();
            
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.String:
                    _writer.WriteString(graph.ToString());
                    break;
                case TypeCode.Single:
                case TypeCode.Double:
                    WriteFloating(graph);
                    break;
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                    WriteNumber(type, graph);    
                    break;
                case TypeCode.Boolean:
                    WriteBoolean(graph);
                    break;
                case TypeCode.DateTime:
                    WriteDateTime(graph);
                    break;
                default:
                    if (graph is Guid)
                    {
                        goto case TypeCode.String;
                    }
                    else if (graph is Uri)
                    {
                        goto case TypeCode.String;
                    }
                    else if (graph is IDictionary)
                    {
                        WriteDictionary(graph as IDictionary);
                    }
                    else if (graph is IEnumerable)
                    {
                        WriteCollection(graph as IEnumerable);
                    }
                    else
                    {
                        if (graph is IJsonSerializable)
                        {
                            WriteStartObjectLiteral();
                            ((IJsonSerializable)graph).ToJson(this);
                        }
                        else
                        {
                            bool serialized = Serialize(graph);

                            if (!serialized)
                            {
                                throw new InvalidDataContractException(String.Format("Type {0} cannot be serialized by this JSON serializer", type));
                            }
                        }
                    }
                    
                    break;
            }
        }

        private void WriteFloating(object graph)
        {
            _writer.WriteAttributeString("type", "number");
            _writer.WriteString(((IFormattable)graph).ToString("R", CultureInfo.InvariantCulture));
        }

        private void WriteNumber(Type type, object graph)
        {
            _writer.WriteAttributeString("type", "number");

            if (type.IsEnum)
            {
                graph = ((IConvertible)graph).ToType(Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture);
            }

            _writer.WriteString(((IFormattable)graph).ToString("G", CultureInfo.InvariantCulture));
        }

        private void WriteBoolean(object graph)
        {
            _writer.WriteAttributeString("type", "boolean");

            if ((bool)graph)
            {
                _writer.WriteString("true");
            }
            else
            {
                _writer.WriteString("false");
            }
        }

        private void WriteDateTime(object graph)
        {
            _writer.WriteString(String.Format(CultureInfo.InvariantCulture, "/Date({0})/", (long)((DateTime)graph).Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds));
        }
        
        private void WriteDictionary(IDictionary dictionary)
        {
            WriteStartObjectLiteral();

            foreach (object keyObj in dictionary.Keys)
            {
                string key = keyObj.ToString();
                object value = dictionary[keyObj];
                WriteObject(key, value);
            }
        }

        private void WriteCollection(IEnumerable list)
        {
            WriteStartArray();

            foreach (object obj in list)
            {
                WriteArrayObject(obj);
            }
        }

        public void Close()
        {
            this._writer.Close();
        }

        public void Dispose()
        {
            this.Close();
        }

        private readonly Dictionary<Type, JsonSerializer> _objectSerializers = new Dictionary<Type, JsonSerializer>();

        public void RegisterSerializer<T>(JsonSerializerDelegate<T> handler) where T : class
        {
            JsonSerializer<T> serializer = new JsonSerializer<T>(handler);
            _objectSerializers[typeof(T)] = serializer;
        }

        private bool Serialize(object value)
        {
            Type type = value.GetType();
            JsonSerializer serializer;
            
            if (_objectSerializers.TryGetValue(type, out serializer))
            {
                WriteStartObjectLiteral();
                serializer.Serialize(value, this);
                return true;
            }

            return false;
        }
    }
}