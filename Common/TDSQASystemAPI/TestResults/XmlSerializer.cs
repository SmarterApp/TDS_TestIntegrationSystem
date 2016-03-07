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

using TDSQASystemAPI.ExceptionHandling;
using TDSQASystemAPI.Config;

namespace TDSQASystemAPI.TestResults
{
    public abstract class XmlSerializer
    {
        protected TestResult _testResult;

        public XmlSerializer(TestResult tr)
        {
            this._testResult = tr;
        }

        //public static XmlSerializer Create(string type, TestResult tr)
        //{
        //    switch (type)
        //    {
        //        case "TDS":
        //            return new XmlSerializerTDS(tr);
        //        case "OSS":
        //            return new XmlSerializerOSS(tr);
        //        default:
        //            throw new QAException("Unknown XML type. Only know TDS and OSS right now", QAException.ExceptionType.ProgrammingError);
        //    }
        //}

        //public static XmlSerializer Create(XMLAdapter.AdapterType type, TestResult tr)
        //{
        //    return Create(type.ToString(), tr);
        //}

        abstract public string Serialize(SerializationConfig config);
    }
}
