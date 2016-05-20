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
    public class ScorableTest
    {        
        public string TestKey { get; set; }        
        public string ClientName { get; set; }
        public string Environment { get; set; }
        public string ItemString { get; set; }
        public Guid OppKey { get; set; }
        public DateTime DateCompleted { get; set; }
        public char RowDelimiter { get; set; }
        public char ColDelimiter { get; set; }
        public string ItemBankDB { get; set; }
    }
}
