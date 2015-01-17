/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;

namespace TDS.Shared.Data
{
    public class ReturnStatusException : Exception
    {
        private readonly ReturnStatus _returnStatus;

        public ReturnStatus ReturnStatus
        {
            get { return _returnStatus; }
        }

        public ReturnStatusException(ReturnStatus returnStatus) : base(returnStatus.Reason)
        {
            this._returnStatus = returnStatus;
        }
    }
}
