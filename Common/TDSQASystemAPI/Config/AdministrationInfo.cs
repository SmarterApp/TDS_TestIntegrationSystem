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

namespace TDSQASystemAPI.Config
{
    public class AdministrationInfo
    {
        private long _key;
        private DateTime _startDate;
        private DateTime _endDate;

        public long Key
        {
            get { return _key; }
        }
        public DateTime StartDate
        {
            get { return _startDate; }
        }
        public DateTime EndDate
        {
            get { return _endDate; }
        }

        public AdministrationInfo(long key, DateTime startDate, DateTime endDate)
        {
            _key = key;
            _startDate = startDate;
            _endDate = endDate;
        }
    }
}
