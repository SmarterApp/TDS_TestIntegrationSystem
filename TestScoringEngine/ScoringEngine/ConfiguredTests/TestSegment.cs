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

namespace ScoringEngine.ConfiguredTests
{
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class TestSegment
    {
        [XmlAttribute("id")]
        public string ID { get; set; }
        [XmlAttribute("position")]
        public int Position { get; set; }
        [XmlAttribute("formId")]
        public string FormID { get; set; }
        [XmlAttribute("formKey")]
        public string FormKey { get; set; }
        [XmlAttribute("algorithm")]
        public string AlgorithmString { get; set; }

        [XmlIgnore]
        private TestBlueprint.SelectionAlgorithmType? algorithm;
        [XmlIgnore]
        public TestBlueprint.SelectionAlgorithmType Algorithm
        {
            get
            {
                if (!algorithm.HasValue)
                {
                    TestBlueprint.SelectionAlgorithmType a;
                    if (!Enum.TryParse(AlgorithmString, out a))
                        algorithm = TestBlueprint.SelectionAlgorithmType.Adaptive;
                    else algorithm = a;
                }
                return algorithm.Value;
            }
            private set
            {
                algorithm = value;
            }
        }

        //Zach 10/31/2014: Below property Added for OSS XML. only. We will serialize/deserialize, but it is ignored in the code
        //<xs:attribute name="algorithmVersion" use="required" />
        [XmlAttribute("algorithmVersion")]
        public string AlgorithmVersion { get; set; }

        public TestSegment() { }
        public TestSegment(string id, int position, string formId, string formKey, string algorithmString, TestBlueprint.SelectionAlgorithmType algorithm)
        {
            this.ID = id;
            this.Position = position;
            this.FormID = formId;
            this.FormKey = formKey;
            this.AlgorithmString = algorithmString;
            this.Algorithm = algorithm;
        }

        public TestSegment(string id, int position, string formId, string formKey, string algorithm)
            : this(id, position, formId, formKey, algorithm, Util.Value((object)algorithm, TestBlueprint.SelectionAlgorithmType.Adaptive))
        {
        }

        public TestSegment(TestSegment copyFrom)
            : this(copyFrom.ID, copyFrom.Position, copyFrom.FormID, copyFrom.FormKey, copyFrom.AlgorithmString, copyFrom.Algorithm)
        {
        }
    }
}
