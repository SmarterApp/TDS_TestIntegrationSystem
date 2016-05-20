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
using System.Xml.Serialization;

namespace TDSQASystemAPI.TestResults
{
    [System.SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public class Test
    {
        #region properties

        [XmlIgnore]
        private string grade;
        [XmlAttribute("grade")]
        public string Grade
        {
            get
            {
                return grade;
            }
            set
            {
                grade = value;
            }
        }

        [XmlIgnore]
        private string mode;
        [XmlAttribute("mode")]
        public string Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
            }
        }

        [XmlIgnore]
        private string contract;
        [XmlAttribute("contract")]
        public string Contract
        {
            get
            {
                return contract;
            }
            set
            {
                contract = value;
            }
        }

        [XmlIgnore]
        private int airbank;
        [XmlAttribute("bankKey")]
        public int AirBank
        {
            get
            {
                return airbank;
            }
            set
            {
                airbank = value;
            }
        }

        [XmlIgnore]
        private string testid;
        [XmlAttribute("testId")]
        public string TestID
        {
            get
            {
                return testid;
            }
            set
            {
                testid = value;
            }
        }

        [XmlIgnore]
        private string subject;
        [XmlAttribute("subject")]
        public string Subject
        {
            get
            {
                return subject;
            }
            set
            {
                subject = value;
            }
        }

        [XmlIgnore]
        private string testname;
        [XmlAttribute("name")]
        public string TestName
        {
            get
            {
                return testname;
            }
            set
            {
                testname = value;
            }
        }

        [XmlIgnore]
        private int handscoringProjectID;
        [XmlAttribute("handScoreProject")]
        public int HandscoringProjectID
        {
            get
            {
                return handscoringProjectID;
            }
            set
            {
                handscoringProjectID = value;
            }
        }

        //Zach 10/31/2014: the below properties are for the OSS XML only. 
        //We will serialize/deserialize them, but they are ignored in the code
/*        <xs:attribute name="assessmentType" use="required" />
            <xs:attribute name="academicYear" use="required" type="xs:unsignedInt" />
            <xs:attribute name="assessmentVersion" use="required" />*/
        [XmlAttribute("assessmentType")]
        public string AssessmentType { get; set; }
        [XmlAttribute("academicYear")]
        public string AcademicYear { get; set; }
        [XmlAttribute("assessmentVersion")]
        public string AssessmentVersion { get; set; }

        #endregion

        public Test() { }
        public Test(string grade, string mode, string contract, int airbank, string testid, string subject, string testname, int handscoringProject)
        {
            this.grade = grade;
            this.mode = mode;
            this.contract = contract;
            this.airbank = airbank;
            this.testid = testid;
            this.subject = subject;
            this.testname = testname;
            this.handscoringProjectID = handscoringProject;
        }
    }
}
