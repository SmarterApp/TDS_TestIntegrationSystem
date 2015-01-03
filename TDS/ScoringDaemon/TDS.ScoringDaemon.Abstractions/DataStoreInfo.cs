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
    public class DataStoreInfo
    {
        public String ServiceType { get; set; }
        public String ServiceRole { get; set; }
        public String ClientName { get; set; }
        public String Environment { get; set; }
        public String PublicIP { get; set; }
        public String PrivateIP { get; set; }
        public String ServerName { get; set; }
        public String DBName { get; set; }              
    }
}
