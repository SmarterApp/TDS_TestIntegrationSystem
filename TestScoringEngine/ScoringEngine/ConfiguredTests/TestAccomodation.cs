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
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Data;

namespace ScoringEngine.ConfiguredTests
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class TestAccomodation
	{
        //todo: extend IXmlSerializable

        #region Properties
        [XmlIgnore]
		private string code = string.Empty;
        [XmlAttribute("code")]
		public string Code
		{
			get
			{
				return code;
            }
            set { code = value; }
		}

        [XmlIgnore]
        private string type = string.Empty;
        [XmlAttribute("type")]
		public string Type
		{
			get
			{
				return type;
            }
            set { type = value; }
		}

        [XmlIgnore]
        private string description = string.Empty;
        [XmlAttribute("value")]
		public string Description
		{
			get
			{
				return description;
            }
            set { description = value; }
		}

        [XmlIgnore]
        private int segment;
        [XmlAttribute("segment")]
        public int Segment
        {
            get
            {
                return segment;
            }
            set { segment = value; }
        }
        [XmlIgnore]
		private string source = string.Empty;
        [XmlIgnore]
		public string Source
		{
			get
			{
				return source;
			}
			set
			{
				source = value;
			}
		}
        [XmlIgnore]
        private bool multipleSelect = false;
        [XmlIgnore]
		public bool MultipleSelect
		{
			get
			{
				return multipleSelect;
			}
		}

		#endregion

        public TestAccomodation()
        {
        }

		public TestAccomodation(string type, string description, string code, int segment, string source, int multipleSelect)
        {
            this.type = type;
            this.description = description;
			this.code = code;
            this.segment = segment;
			this.source = source;
            if (multipleSelect == 0)
                this.multipleSelect = false;
            else if (multipleSelect == 1)
                this.multipleSelect = true;
            else
                throw new ScoringEngineException("multipleSelect for accomodations must be 0 or 1");
        }

        public string ToTypeCodeString()
        {
            return Type + "|" + Code;
        }
    }
}
