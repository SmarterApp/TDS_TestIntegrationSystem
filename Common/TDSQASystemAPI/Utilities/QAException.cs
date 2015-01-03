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

namespace TDSQASystemAPI.Utilities
{
    public class QAException : Exception
    {
        private string _message;
        private ExceptionType _type;

        internal enum ExceptionType { Unknown = 0, General = 1, ConfigurationError = 2, ProgrammingError = 3, Unspecified = 4 };

        internal QAException(string message, ExceptionType type)
        {
            _message = message;
            _type = type ;
        }

        public QAException(string message)
        {
            _message = message;
            _type = ExceptionType.Unspecified;
        }
    }
}
