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
using TDS.Shared.Data;

namespace TDS.ScoringDaemon.Abstractions
{
    public interface IResponseRepository
    {
        ItemScoringConfig GetItemScoringConfigs();
        List<ScorableResponse> GetScoreItems(int pendingMinutes, int minAttempts, int maxAttempts, int sessionType, int maxItemsReturned);
        ScorableTest UpdateItemScore(ScoredResponse scoredResponse);
        ReturnStatus UpdateTestScore(ScoredTest scoredTest);
    }
}
