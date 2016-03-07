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
using System.Xml;
using System.Xml.Serialization;

namespace AIR.Common.Xml
{
    /// <summary>
    /// Adds some extensions to IXmlSerializable interface.
    /// </summary>
    public interface IXmlSerializableExt : IXmlSerializable
    {
        /// <summary>
        /// Write the outer element, then call WriteXml().
        /// </summary>
        /// <param name="writer"></param>
        void WriteOuterXml(XmlWriter writer);

        /// <summary>
        /// Don't read the outer element, only its content.
        /// </summary>
        /// <param name="reader"></param>
        void ReadContentXml(XmlReader reader);
    }
}
