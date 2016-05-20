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
using System.IO;
using System.Linq;
using System.Text;

namespace AIR.Common.Json
{
    /// <summary>
    /// This is a helper class used to build up a string of JSON.
    /// </summary>
    public class JsonBuilder : IDisposable
    {
        private readonly MemoryStream _stream;
        private readonly JsonWriter _writer;

        public JsonBuilder()
        {
            _stream = new MemoryStream();
            _writer = JsonWriter.Create(_stream, Encoding.UTF8, true);
        }

        public JsonWriter GetJsonWriter()
        {
            return _writer;
        }

        public override string ToString()
        {
            _writer.XmlWriter.Flush();
            
            byte[] jsonBytes = _stream.ToArray();
            return Encoding.UTF8.GetString(jsonBytes);
        }

        public void Dispose()
        {
            _writer.Dispose();
            _stream.Dispose();
        }
    }
}
