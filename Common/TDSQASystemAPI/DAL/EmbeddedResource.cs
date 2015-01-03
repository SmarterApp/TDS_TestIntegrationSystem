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
using System.Reflection;
using System.IO;
using System.Xml;

namespace TDSQASystemAPI.DAL
{
    internal class EmbeddedResource
    {
        public string GetEmbeddedResourceAsString(string qualifiedResourceName)
        {
            String resourceContents = String.Empty;

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = GetEmbeddedResourceAsStream(qualifiedResourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    resourceContents = reader.ReadToEnd();
                }
            }

            return resourceContents;
        }

        public XmlReader GetEmbeddedResourceAsXmlReader(string qualifiedResourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return XmlReader.Create(GetEmbeddedResourceAsStream(qualifiedResourceName));
        }

        public Stream GetEmbeddedResourceAsStream(string qualifiedResourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(qualifiedResourceName);
        }
    }
}
