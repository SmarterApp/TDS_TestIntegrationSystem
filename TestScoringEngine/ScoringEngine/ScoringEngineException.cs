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

namespace ScoringEngine
{
    public class ScoringEngineException : Exception
    {
        public ScoringEngineException()
            : base()
		{
		}

		public ScoringEngineException(String message) : base(message)
		{
		}

        public ScoringEngineException(String message, Exception exception)
            : base(message, exception)
		{
		}
    }
}
