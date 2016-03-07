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

namespace TDS.ItemScoringEngine
{
    /// <summary>
    /// Class that is used to carry info about the item and student response 
    /// </summary>
    public class ResponseInfo
    {
        /// <summary>
        /// Item format
        /// </summary>
        public string ItemFormat { get; private set; }
        
        /// <summary>
        /// Unique ID for the item (likely the ITS id)
        /// </summary>
        public string ItemIdentifier { get; private set; }
        
        /// <summary>
        /// Student response
        /// </summary>
        public string StudentResponse { get; set; }

        /// <summary>
        /// Specifies whether the student's response is encrypted or not
        /// </summary>
        public bool IsStudentResponseEncrypted { get; set; }
        
        /// <summary>
        /// Rubric information for the scorer
        /// </summary>
        public Object Rubric { get; set; }
        
        /// <summary>
        /// Placeholder to associate add'l info related to this student response (such as testeeid, position etc)
        /// </summary>
        public string ContextToken { get; set;}

        /// <summary>
        /// Specify if the rubric associated with this responseInfo can be cached by the item scoring server
        /// </summary>
        public bool CanCacheRubric { get; set; }

        /// <summary>
        /// Specifies if the rubric is encrypted or not
        /// </summary>
        public bool IsRubricEncrypted { get; set; }

        /// <summary>
        /// Specifies whether the rubric is specified as a Uri or is the rubric content inline
        /// </summary>
        public RubricContentType ContentType { get; set; }

        public List<VarBinding> IncomingBindings { get; set; }

        public List<VarBinding> OutgoingBindings { get; set; }

        /// <summary>
        /// Check if the ResponseInfo is complete (ie all mandatory fields are specified)
        /// </summary>
        public bool isComplete
        {
            get
            {
                // all fields EXCEPT context token are mandatory
                return (ItemFormat != null && ItemIdentifier != null && StudentResponse != null && Rubric != null);
            }
        }

        public ResponseInfo(string itemFormat, string itemID, string studentResponse, Object rubric, RubricContentType contentType, string contextToken, bool allowRubricCaching)
        {
            this.ItemFormat = itemFormat;
            this.ItemIdentifier = itemID;
            this.StudentResponse = studentResponse;
            this.Rubric = rubric;
            this.ContentType = contentType;
            this.ContextToken = contextToken;
            this.CanCacheRubric = allowRubricCaching;
        }

    }
}