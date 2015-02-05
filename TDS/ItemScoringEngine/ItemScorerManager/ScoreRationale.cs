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
using System.Xml;
using System.Xml.Serialization;
using AIR.Common.Xml;

namespace TDS.ItemScoringEngine
{
    public class ScoreRationale
    {
        private Exception _exception;
        private string _exceptionStr;

        /// <summary>
        /// Propositions and their assert states
        /// </summary>
        public List<Proposition> Propositions { get; set; }

        /// <summary>
        /// Variable bindings and their values (if requested)
        /// </summary>
        public List<VarBinding> Bindings { get; set; }

        /// <summary>
        /// Free form descriptive message for why the score is what it is
        /// </summary>
        [XmlIgnore]
        public string Msg { get; set; }

        /// <summary>
        /// Exception holder in case any occur during scoring
        /// </summary>
        [XmlIgnore]
        public Exception Exception
        {
            get
            {
                return _exception;
            }
            set
            {
                _exception = value;
                _exceptionStr = _exception.StackTrace;
            }
        }

        // stuff for the Xml Serializer/Deserializer of the specific fields into CDATA elements

        [XmlElement("Message")]
        public XmlCDataText MsgCDATA
        {
            get
            {
                return new XmlCDataText(Msg);
            }
            set
            {
                Msg = value.Value;
            }
        }

        [XmlElement("StackTrace")]
        public XmlCDataText ExpCDATA
        {
            get
            {
                return new XmlCDataText(_exceptionStr);  // keeping a string representation of the exception around is really to help with the serialization/deserialization between ISE and calling host.
            }
            set
            {
                _exceptionStr = value.Value;
            }
        }
    }
}