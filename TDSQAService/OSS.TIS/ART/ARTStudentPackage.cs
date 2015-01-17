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

namespace OSS.TIS.ART
{
    internal class ARTAccommodation
    {
        /// <summary>
        /// Element name
        /// </summary>
        public string AccomTDSType;
        /// <summary>
        /// Element inner text
        /// </summary>
        public string AccomCode;
    }

    internal class ARTStudentPackage
    {
        private XmlDocument _doc;
        /// <summary>
        /// Key1 = subject code, key2 = element name, value = accom object
        /// </summary>
        private Dictionary<string, Dictionary<string, ARTAccommodation>> _accoms;

        #region XML paths

        private static readonly string AccomPath = @"StudentPackage/Student/Accommodations/Accommodation";
        private static readonly string SubjectPath = @"SubjectCode";

        #endregion

        internal ARTStudentPackage(XmlDocument doc)
        {
            this._doc = doc;
            Load();
        }

        private void Load()
        {
            this._accoms = new Dictionary<string, Dictionary<string, ARTAccommodation>>(StringComparer.InvariantCultureIgnoreCase);

            //populate accommodations
            XmlNodeList accomNodes = _doc.SelectNodes(AccomPath);
            foreach (XmlNode accomNode in accomNodes)
            {
                //get subject 
                XmlNode subjectElem = accomNode.SelectSingleNode(SubjectPath);
                if (subjectElem == null)
                    throw MakeCouldNotFindNodeError(SubjectPath, "Accommodation", accomNode.OuterXml);
                string subject = subjectElem.InnerText;

                if (!_accoms.ContainsKey(subject))
                    _accoms.Add(subject, new Dictionary<string, ARTAccommodation>(StringComparer.InvariantCultureIgnoreCase));

                //get all accommodation nodes for this subject (all other child nodes) and add them to the dict
                foreach (XmlNode accom in accomNode.ChildNodes)
                {
                    if (accom.Name.Equals(SubjectPath)) continue; // don't read the subject node again
                    if (!_accoms[subject].ContainsKey(accom.Name))
                        _accoms[subject].Add(accom.Name, new ARTAccommodation() { AccomTDSType = accom.Name, AccomCode = accom.InnerText });
                }
            }
        }

        internal Dictionary<string, ARTAccommodation> GetAccommodations(string subject)
        {
            if (subject == null || !_accoms.ContainsKey(subject)) return null;
            return _accoms[subject];
        }

        internal ARTAccommodation GetAccommodation(string subject, string name)
        {
            Dictionary<string, ARTAccommodation> subjAccoms = GetAccommodations(subject);
            if (subjAccoms == null || !subjAccoms.ContainsKey(name)) return null;
            return subjAccoms[name];
        }

        private NullReferenceException MakeCouldNotFindNodeError(string nodeName, string parentNodeName, string outerXml)
        {
            return new NullReferenceException(string.Format("Could not find {0} node in ART {1} node. Node XML = {2}", nodeName, parentNodeName, outerXml));
        }
    }
}
