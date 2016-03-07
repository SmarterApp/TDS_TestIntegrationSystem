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
using System.Runtime.Serialization.Json;
using System.Text;

namespace AIR.Common.Json
{
    /// <summary>
    /// A helper class for serializing/deserializing JSON objects using WCF's build in serializer (reflection).
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// Get an instance of JSON serializer
        /// </summary>
        private static DataContractJsonSerializer CreateSerializer<T>()
        {
            return new DataContractJsonSerializer(typeof(T));
        }

        public static string Serialize<T>(T obj) where T : class
        {
            if (obj == null) return null;

            DataContractJsonSerializer jsonSerializer = CreateSerializer<T>();

            string json;

            using (MemoryStream stream = new MemoryStream())
            {
                jsonSerializer.WriteObject(stream, obj);
                byte[] jsonBytes = stream.ToArray();
                json = Encoding.UTF8.GetString(jsonBytes);
            }

            return json;
        }

        public static T Deserialize<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json)) return default(T);

            DataContractJsonSerializer jsonSerializer = CreateSerializer<T>();
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            
            using (MemoryStream stream = new MemoryStream(jsonBytes))
            {
                return jsonSerializer.ReadObject(stream) as T;
            }
        }

    }
}
