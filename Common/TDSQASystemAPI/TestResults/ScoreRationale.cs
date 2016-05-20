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
using System.Xml.Serialization;

namespace TDSQASystemAPI.TestResults
{
    [Serializable]
    public class ScoreRationale
    {
        /// <summary>
        /// Free form descriptive message for why the score is what it is
        /// </summary>
        [XmlElement("Message")]
        public string Msg { get; set; }

        [XmlIgnore]
        HandScoreInfo handscoreInfo = null;
        [XmlIgnore]
        bool handScoreInfoChecked = false;
        /// <summary>
        /// Returns the HandscoringInfo for a ScoreInfo with a scoreDimension that corresponds
        /// to a hand-scoring read.  Returns null if there's no handscore info or if the
        /// ScoreRationale/Message is not in JSON format.
        /// </summary>
        [XmlIgnore]
        public HandScoreInfo HandScoreDetails
        {
            get
            {
                if (!handScoreInfoChecked)
                {
                    //  If it's an empty string, the deserialize method will handle that and return null.
                    //  If it's not json, then return null.  Shouldn't be consulting this property unless
                    //  handscore data is expected (i.e. for scoreDimensions that correspond to handscoring reads),
                    //  in which case Msg is required to be either empty or contain HandScoreInfo in json format.
                    try
                    {
                        handscoreInfo = Utilities.Serialization.DeserializeJson<HandScoreInfo>(Msg);
                    }
                    catch
                    {
                        handscoreInfo = null;
                    }

                    handScoreInfoChecked = true;
                }
                return handscoreInfo;
            }
        }

        public ScoreRationale() { }

        public ScoreRationale(ScoreRationale other)
        {
            this.Msg = other.Msg;
        }

        // stuff for the Xml Serializer/Deserializer of the specific fields into CDATA elements

/*        [XmlElement("Message")]
        public XmlCDataSection MsgCDATA
        {
            get
            {
                return String.IsNullOrEmpty(Msg) ? null : new XmlDocument().CreateCDataSection(Msg);
            }
            set
            {
                Msg = value.Value;
            }
        }*/
    }
}
