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
    /// Interface that is used for the scoring engine to call back once asynchronous scoring is complete
    /// </summary>
    public interface IItemScorerCallback
    {
        /// <summary>
        /// Main method of this interface
        /// </summary>        
        /// <param name="itemScore">Final Score for an invocation to the scoring engine</param>
        /// <param name="responseInfo">Response/Item information passed in when the scorer was invoked</param>
        void ScoreAvailable(ItemScore itemScore, ResponseInfo responseInfo);
    }
}