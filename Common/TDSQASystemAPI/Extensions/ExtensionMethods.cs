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
using System.Xml.Xsl;

namespace TDSQASystemAPI.Extensions
{
    public static class ExtensionMethods
    {
        public static string GetXmlFormattedString(this DateTime dateTime)
        {
            return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Unspecified);
        }

        public static void AddFormattedDateTimeParam(this XsltArgumentList args, string name, string namespaceUri, DateTime parameter)
        {
            args.AddParam(name, namespaceUri, parameter.GetXmlFormattedString());
        }

        public static string GetExceptionMessage(this Exception e, bool includeStackTrace)
        {
            StringBuilder message = new StringBuilder();
            Exception temp = e;
            while (temp != null)
            {
                message.Append(temp.Message);
                message.Append(" ");
                temp = temp.InnerException;
            }
            if (includeStackTrace && !String.IsNullOrEmpty(e.StackTrace))
            {
                message.Append(e.StackTrace);
            }
            return message.ToString();
        }
    }
}
