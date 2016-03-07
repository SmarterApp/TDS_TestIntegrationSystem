/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System.Xml.Serialization;

namespace TDS.ItemScoringEngine
{
    /// <summary>
    /// Enum to represent the status of the scoring operation
    /// </summary>
    public enum ScoringStatus
    {
        /// <summary>
        /// Item has not been scored. The item has an indeterminate score
        /// </summary>
        [XmlEnum("NotScored")]
        NotScored, 
        
        /// <summary>
        /// Item has been scored properly. The item's score is valid
        /// </summary>
        [XmlEnum("Scored")]
        Scored, 

        /// <summary>
        /// Item is in the process of being scored. The item has an indeterminate score
        /// </summary>
        [XmlEnum("WaitingForMachineScore")]
        WaitingForMachineScore, 
        
        /// <summary>
        /// Item cannot be scored as no suitable scorer exists for this item type. The item has an indeterminate score
        /// </summary>
        [XmlEnum("NoScoringEngine")]
        NoScoringEngine,

        /// <summary>
        /// Item cannot be scored as an error occurred during the scoring process. The item has an indeterminate score
        /// </summary>
        [XmlEnum("ScoringError")]
        ScoringError
    };
}