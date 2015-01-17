/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System.Net;

namespace TDS.Shared.Exceptions
{
    public class TDSSecurityException : TDSHttpException
    {
        private const string ErrorMessage = "Your browser session has timed out. Please sign in again.";

        public TDSSecurityException() : base(HttpStatusCode.Forbidden, ErrorMessage)
        {
            // "Your session has timed out. Please sign in again."
            // You either do not have sufficient privileges to view the application or your browser session has timed out.
        }
    }

    public class TDSValidationException : TDSHttpException
    {
        private const string ErrorMessage = "Your browser session is invalid. Please sign in again.";

        public TDSValidationException() : base(HttpStatusCode.Forbidden, ErrorMessage)
        {
        }
    }
}