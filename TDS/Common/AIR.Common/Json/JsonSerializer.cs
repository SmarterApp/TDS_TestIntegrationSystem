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

namespace AIR.Common.Json
{
    public interface IJsonSerializable
    {
        void ToJson(JsonWriter writer);
    }

    public delegate void JsonSerializerDelegate<T>(T value, JsonWriter jsonWriter);

    public abstract class JsonSerializer
    {
        public abstract void Serialize(object value, JsonWriter writer);
    }

    public class JsonSerializer<T> : JsonSerializer where T : class
    {
        private readonly JsonSerializerDelegate<T> _handler;

        public JsonSerializer(JsonSerializerDelegate<T> handler)
        {
            _handler = handler;
        }

        public override void Serialize(object value, JsonWriter writer)
        {
            _handler(value as T, writer);
        }
    }
}
