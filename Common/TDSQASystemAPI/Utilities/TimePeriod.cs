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
    public class TimePeriod
    {
        private DateTime _start;
        private DateTime _end;

        public TimePeriod(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
        }

        #region properties

        public DateTime Start
        {
            get
            {
                return _start;
            }
        }

        public DateTime End
        {
            get
            {
                return _end;
            }
        }
        #endregion properties

        #region operations

        public bool Encompasses(TimePeriod tp)
        {
            return ((tp.Start >= this.Start) && (tp.End <= this.End));
        }

        #endregion operations
    }
}
