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

namespace TDSQASystemAPI.TestResults
{
    public class TestResultSerializerFactory : ITestResultSerializerFactory
    {
        #region ITestResultSerializerFactory Members

        public virtual XmlSerializer CreateSerializer(string type, TestResult tr)
        {
            return new XmlSerializerOSS(tr);
        }

        public virtual XMLAdapter CreateDeserializer(string type, XmlDocument xml)
        {
            if (type.Equals("OSS", StringComparison.InvariantCultureIgnoreCase))
                return new XMLAdapterOSS(xml);
            else
                throw new ApplicationException(String.Format("Unknown serializer type: {0}", type));
        }

        public virtual XMLAdapter CreateDeserializer(XmlDocument xml)
        {
            if (xml.DocumentElement.Name == "TDSReport")
                return new XMLAdapterOSS(xml);
            else
                throw new ApplicationException(String.Format("Unknown document format.  Document node name: {0}", xml.DocumentElement.Name));
        }

        #endregion
    }
}
