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
    public class ScorableResponse
    {
        public Guid OppKey { get; set; }
        public String TestKey { get; set; }
        public String TestId { get; set; }
        public int Position { get; set; }
        public int ResponseSequence { get; set; }
        public String Response { get; set; }
        public Guid ScoreMark { get; set; }
        public String ItemKey { get; set; }
        public String Format { get; set; }
        public String Language { get; set; }
        public int Attempts { get; set; }
        public String SegmentId { get; set; }
        public String ItemFile { get; set; }
    }
}
