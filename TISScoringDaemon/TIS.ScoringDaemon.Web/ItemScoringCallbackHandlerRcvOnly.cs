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
using System.Web;

namespace TIS.ScoringDaemon.Web
{
    /// <summary>
    /// Use this version when receiving requests that the SD did not send.
    /// For instance, when integrating with TSS for handscoring, TIS will send the
    /// request, not the SD.  The SD just receives the scores.  In this case, TSS constructs the 
    /// ContextToken using data from the request, and it's not encrypted.  When the SD sends the requests,
    /// the ContextToken is encrypted and therefore must be decrypted.  In that case, wire up the base class.
    /// </summary>
    public class ItemScoringCallbackHandlerRcvOnly : ItemScoringCallbackHandler
    {
        protected override string GetContextToken(TDS.ItemScoringEngine.ItemScoreResponse itemScoreResponse)
        {
            return itemScoreResponse.Score.ContextToken;
        }
    }
}
