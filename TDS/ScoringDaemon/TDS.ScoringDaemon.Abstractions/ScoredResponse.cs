/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;

namespace TDS.ScoringDaemon.Abstractions
{
    public class ScoredResponse
    {
        public Guid OppKey { get; set; }
        public String ItemKey { get; set; }
        public int Position { get; set; }
        public int Sequence { get; set; }
        public int Score { get; set; }
        public String ScoreStatus { get; set; }
        public String ScoreRationale { get; set; }
        public Guid ScoreMark { get; set; }
        public String ScoreDimensions { get; set; }
    }
}
