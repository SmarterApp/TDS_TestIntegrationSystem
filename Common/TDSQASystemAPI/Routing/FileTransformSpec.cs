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
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Xsl;
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.Extensions;

namespace TDSQASystemAPI.Routing
{
    /// <summary>
    /// Specification as to how to transform the XML file and how to validate the transformed file.
    /// A Singleton-esque pattern that will cache compiled xslt transforms and schema sets so that
    /// they don't have to be loaded every time.
    /// 
    /// The name that's provided must be associated with an XSLT that is an embedded resource in the 
    /// Resources directory.  If validate = true, an XSD of the same name is also expected; that will
    /// be used to validate the transformed file.  We should probably always validate the transformed
    /// file against an XSD, so consider enforcing that.
    /// </summary>
    public class FileTransformSpec
    {
        private static readonly object lockme;
        /// <summary>
        /// cache file transform specs by name
        /// </summary>
        private static readonly Dictionary<string, FileTransformSpec> fileTransformSpecs;

        public XslCompiledTransform XsltTransform { get; private set; }
        public XmlSchemaSet SchemaSet { get; private set; }

        static FileTransformSpec()
        {
            lockme = new object();
            fileTransformSpecs = new Dictionary<string, FileTransformSpec>();
        }

        private FileTransformSpec() { }
            
        /// <summary>
        /// Returns the cached FileTransformSpec corresponding to the name if it exists.
        /// Otherwise, creates it, caches it, then returns it.
        /// </summary>
        /// <param name="qualifiedName">Must correspond to the wualified name of the XSLT file, which must be an embedded resource.
        /// Example: TDSQASystemAPI.Resources.ItemScoreRequest.xslt</param>
        /// <param name="validate">If true, there must be an embedded resource in name namespace called [qualifiedName].xsd.</param>
        /// <returns></returns>
        public static FileTransformSpec Create(string qualifiedName, bool validate)
        {
            if (String.IsNullOrEmpty(qualifiedName))
                return null;

            if (!fileTransformSpecs.ContainsKey(qualifiedName))
            {
                lock (lockme)
                {
                    if (!fileTransformSpecs.ContainsKey(qualifiedName))
                    {
                        FileTransformSpec spec = Create(qualifiedName);
                        DAL.EmbeddedResource embeddedResourceDAL = new DAL.EmbeddedResource();
                        // load the compiled xslt
                        XslCompiledTransform xform = new XslCompiledTransform(true);
                        using (Stream xsltStream = embeddedResourceDAL.GetEmbeddedResourceAsStream(qualifiedName))
                        {
                            XmlReader rdr = new XmlTextReader(xsltStream);
                            xform.Load(rdr);
                        }
                        spec.XsltTransform = xform;

                        if (validate)
                        {
                            // load the XSD schema
                            XmlSchemaSet schemaSet = new XmlSchemaSet();
                            using (XmlReader r = embeddedResourceDAL.GetEmbeddedResourceAsXmlReader(qualifiedName.Replace(".xslt", ".xsd").Replace(".XSLT", ".XSD")))
                            {
                                schemaSet.Add(null, r);
                            }
                            spec.SchemaSet = schemaSet;
                        }

                        fileTransformSpecs[qualifiedName] = spec;
                    }
                }
            }
            return fileTransformSpecs[qualifiedName];
        }

        /// <summary>
        /// To allow for subclassing.  Currently only 1 type, so always return that
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static FileTransformSpec Create(string name)
        {
            return new FileTransformSpec();
        }

        /// <summary>
        /// public transform method will return the transformed XML file.  Other params
        /// are passed in to be used in generating the filename and/or adding parameters to the
        /// XSLT to be used in the transformation.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="testResult"></param>
        /// <param name="dateProcessed"></param>
        /// <param name="districtType"></param>
        /// <returns></returns>
        public string Transform(string xml, TestResult testResult, XsltArgumentList args)
        {
            // transform the xml file
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return Transform(doc, testResult, args);
        }

        /// <summary>
        /// public transform method will return the transformed XML file.  Other params
        /// are passed in to be used in generating the filename and/or adding parameters to the
        /// XSLT to be used in the transformation.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="testResult"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string Transform(XmlDocument doc, TestResult testResult, XsltArgumentList args)
        {
            string xml = null;
            using (MemoryStream resultMs = new MemoryStream())
            {
                using (XmlWriter xw = XmlWriter.Create(resultMs, this.XsltTransform.OutputSettings))
                {
                    this.XsltTransform.Transform(doc, args ?? new XsltArgumentList(), xw);
                    // if we want to strip whitespace, need to use xmlreader.  I've got that
                    //  commented out in the xslt though.  Don't think we want to do this.
                    //xsltTransform.Transform(doc.CreateNavigator().ReadSubtree(), args, xw);

                    if (this.SchemaSet != null)
                    {
                        //validate the transformed file against the XSD 
                        //  will throw exception at first validation error.
                        resultMs.Position = 0;
                        XmlReaderSettings xs = new XmlReaderSettings();
                        xs.ValidationType = ValidationType.Schema;
                        xs.Schemas = this.SchemaSet;
                        XmlReader xr = XmlReader.Create(resultMs, xs);
                        while (xr.Read()) ;
                    }

                    // transformed file is valid. Get the contents as a string
                    resultMs.Position = 0;
                    StreamReader r = new StreamReader(resultMs);
                    xml = r.ReadToEnd();
                }
            }
            return xml;
        }
    }

}
