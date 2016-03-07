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
using System.Data;

namespace TDS.Shared.Data
{
    /// <summary>
    /// Represents a result of a SP along with a return status (optional).
    /// </summary>
    /// <typeparam name="T">The type of object to be returned.</typeparam>
    public class TDSSqlResult<T> 
    {
        /// <summary>
        /// The object returned from the sql request.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// The return status returned from the sql request.
        /// </summary>
        public ReturnStatus ReturnStatus { get; set; }

        public TDSSqlResult()
        {
        }

        public TDSSqlResult(T value)
        {
            Value = value;
        }

        public TDSSqlResult(ReturnStatus returnStatus)
        {
            ReturnStatus = returnStatus;
        }

        public TDSSqlResult(T value, ReturnStatus returnStatus)
        {
            ReturnStatus = returnStatus;
            Value = value;
        }

    }
}




