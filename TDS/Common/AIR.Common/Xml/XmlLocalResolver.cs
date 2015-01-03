/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace AIR.Common.Xml
{
    /// <summary>
    /// Resolve url's using a local root file path.
    /// </summary>
    public class XmlLocalResolver : XmlResolver
    {
        private readonly XmlResolver _resolver;
        private readonly string _rootPath;

        public XmlLocalResolver(XmlResolver resolver, string rootPath)
        {
            if (resolver == null) throw new ArgumentNullException("resolver");
            _resolver = resolver;
            _rootPath = rootPath;
        }

        public static XmlLocalResolver Create(string rootPath)
        {
            return new XmlLocalResolver(new XmlUrlResolver(), rootPath);
        }

        /// <summary>
        /// Sets the credentials to use for resolving
        /// </summary>
        public override System.Net.ICredentials Credentials
        {
            set { _resolver.Credentials = value; }
        }

        /// <summary>
        /// Gets the <see cref="Stream" /> referenced by the uri
        /// </summary>
        /// <returns>a <see cref="Stream"/></returns>
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            // check if uri is already a file
            if (absoluteUri.IsFile)
            {
                // if the file exists return it
                if (File.Exists(absoluteUri.OriginalString))
                {
                    return _resolver.GetEntity(absoluteUri, role, ofObjectToReturn);
                }
                
                return null;
            }

            return GetFileStream(absoluteUri, role, ofObjectToReturn);
        }

        public Stream GetFileStream(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            string domainPath = absoluteUri.Host.Replace('.', '_');
            string localPath = absoluteUri.LocalPath.Replace('/', '\\');
            string resourcePath = _rootPath + domainPath + localPath;
            Debug.WriteLine("XmlLocalResolver: " + resourcePath);

            // if the file exists return file stream
            if (File.Exists(resourcePath))
            {
                return File.OpenRead(resourcePath);
            }
            
            // return null which tells the calling application to ignore the resource
            return null;
        }

    }

}
