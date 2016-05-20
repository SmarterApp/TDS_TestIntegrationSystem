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

namespace TDSQASystemAPI.Config
{
    public class RTSAttribute
    {
        public int ProjectID { get; private set; }
        /// <summary>
        /// One of GroupName from QC_ProjectMetaData, or 'All' or 'None' (in case RTS is only for QA use, but not to send downstream)
        /// </summary>
        public string GroupName { get; private set; } 
        /// <summary>
        /// INITIAL, FINAL or CONFIGURED
        /// </summary>
        public string Context { get; private set; } 
        /// <summary>
        /// CONFIGURED date for fetching RTS data
        /// </summary>
        public DateTime? ContextDate { get; set; } 
        /// <summary>
        /// Whether to decrypt encrypted data or not
        /// </summary>
        public bool Decrypt { get; private set; }
        /// <summary>
        /// name to use in testeeattribute or testeerelationship node
        /// </summary>
        public string XMLName { get; private set; }
        /// <summary>
        /// to match entityType col in tblEntityType
        /// </summary>
        public string EntityType { get; private set; }
        /// <summary>
        /// if EntityType isn't student then this should match some relationshipType from tblRelationshipType
        /// </summary>
        public string Relationship { get; private set; }
        /// <summary>
        /// to match fieldname from tblEntityAttribute
        /// </summary>
        public string FieldName { get; private set; }
        /// <summary>
        /// Whether to fetch the data if it is not in the XML already
        /// </summary>
        public bool FetchIfNotInXml { get; private set; }

        public RTSAttribute(int projectID, string groupName, string context, DateTime? contextDate, bool decrypt, string xmlName, string entityType,
                              string relationship, string fieldName, bool fetchIfNotInXml)
        {
            this.ProjectID = projectID;
            this.GroupName = groupName;
            this.Context = context;
            this.ContextDate = contextDate;
            this.Decrypt = decrypt;
            this.XMLName = xmlName;
            this.EntityType = entityType;
            this.Relationship = relationship;
            this.FieldName = fieldName;
            this.FetchIfNotInXml = fetchIfNotInXml;
        }
    }
}
