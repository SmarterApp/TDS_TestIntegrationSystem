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
using System.Text;

namespace TDS.Shared.Logging
{
    public static class TDSLogger
    {
        public static ILogger Config = new TraceLogger("TDSConfig"); // web.config or TDS config issues
        public static ILogger Sql = new TraceLogger("TDSSql"); // SP/SQL TDS errors, return status, timeouts
        public static ILogger Application = new TraceLogger("TDSApplication"); // general TDS errors
        
        public static ILogger Renderer = new TraceLogger("TDSRenderer"); // renderer, accommodations or layout errors
        public static ILogger Adaptive = new TraceLogger("TDSAdaptive"); // larry's adaptive error
    }
}
