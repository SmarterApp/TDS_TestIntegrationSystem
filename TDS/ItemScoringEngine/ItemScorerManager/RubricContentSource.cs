/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
namespace TDS.ItemScoringEngine
{
    /// <summary>
    /// Enum for the various places that an items rubric can be retrieved from.            
    /// </summary>
    public enum RubricContentSource
    {
        /// <summary>
        /// This indicates the item scorer does not require a rubric.
        /// </summary>
        None,

        /// <summary>
        /// This is mainly to support legacy items where the rubric is in the item bank (AnswerKey).  
        /// </summary>
        AnswerKey,

        /// <summary>
        /// Newer machine-scored items have the rubric either embedded in the item's XML as a string (ItemXML) or path to an external rubric file     
        /// </summary>
        ItemXML        
    };
}