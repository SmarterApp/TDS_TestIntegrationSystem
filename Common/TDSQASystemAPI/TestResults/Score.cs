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
using System.Xml.Serialization;

namespace TDSQASystemAPI.TestResults
{
    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class Score
	{

        #region Properties
        [XmlIgnore]
		private string measureOf;
        [XmlAttribute("measureOf")]
		public string MeasureOf
		{
			get
			{
				return measureOf;
            }
            set { measureOf = value; }
		}
        [XmlIgnore]
        private string measureLabel;
        [XmlAttribute("measureLabel")]
        public string MeasureLabel
		{
			get
			{
                return measureLabel;
            }
            set { measureLabel = value; }
		}
        [XmlIgnore]
        private string measureValue;
        [XmlAttribute("value")]
        public string MeasureValue
        {
            get
            {
                return measureValue;
            }
            set { measureValue = value; }
        }
        [XmlIgnore]
        private string measureSE;
        [XmlAttribute("standardError")]
        public string MeasureSE
        {
            get
            {
                return measureSE;
            }
            set { measureSE = value; }
        }

		#endregion

        public Score() { }
        public Score(string measureOf, string measureLabel, string measureValue, string measureSE)
        {
            this.measureOf = measureOf;
            this.measureLabel = measureLabel;
            this.measureValue = measureValue;
            this.measureSE = measureSE;
        }
    }
}
