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

namespace TDSQASystemAPI.Executive
{
    public class Alerter
    {
        public enum AlertType { Unknown = 0, FileSaveError = 1, DatabaseAccessError = 2, FatalValidationError=3 };

        internal void RaiseAlert(string p, AlertType alertType)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
