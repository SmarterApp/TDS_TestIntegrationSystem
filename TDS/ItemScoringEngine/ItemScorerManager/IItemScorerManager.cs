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

namespace TDS.ItemScoringEngine
{
    public interface IItemScorerManager : IItemScorer
    {
        /// <summary>
        /// Register the specific scoring engine to be used
        /// to score responses of the specified format
        /// </summary>
        /// <param name="itemFormat">Item format (MC, WB, GI, RW etc)</param>
        /// <param name="itemScorer">Scoring engine reference</param>
        void RegisterItemScorer(string itemFormat, IItemScorer itemScorer);

   /*     /// <summary>
        /// Invoke the specific scoring engine based on the response's 
        /// item format and return the result. If callbackReference is
        /// null, then this happens synchronously. Otherwise, it happens 
        /// asynchronously
        /// </summary>
        /// <param name="studentResponse">Student response to be scored</param>
        /// <param name="callbackReference">Call back reference (for async mode) or null (for sync mode)</param>
        /// <returns>Score (final or intermin depending on sync vs async) for the response</returns>
        Score GetScore(ResponseInfo studentResponse, IItemScorerCallback callbackReference); */

    /*    /// <summary>
        /// Retrieve information about the scoring engine that has been 
        /// registered to be used for the specified item format
        /// </summary>
        /// <param name="itemFormat">Item format (MC, WB, GI, RW etc)</param>
        /// <returns>Meta information about the corresponding scoring engine</returns>
        ScorerInfo? GetScorerInfo(string itemFormat); */
    }
}
