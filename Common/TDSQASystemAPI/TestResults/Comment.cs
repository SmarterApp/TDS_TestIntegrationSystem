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

namespace TDSQASystemAPI.TestResults
{
    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class Comment
    {
        [XmlAttribute("context")]
        public string Context { get; set; }
        [XmlAttribute("date")]
        public DateTime Date { get; set; }

        [XmlIgnore]
        public int? ItemPosition { get; private set; }
        [XmlAttribute("itemPosition")]
        public string itemPositionAsText
        {
            get { return (ItemPosition.HasValue) ? ItemPosition.ToString() : null; }
            set { ItemPosition = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }


        [XmlText()]
        public string CommentValue { get; set; }

        public Comment() { }
        public Comment(string context, DateTime date, int? itemPosition, string value)
        {
            this.Context = context;
            this.Date = date;
            this.ItemPosition = itemPosition;
            this.CommentValue = value ?? String.Empty;
        }        
    }
}
