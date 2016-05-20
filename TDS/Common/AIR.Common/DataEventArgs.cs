/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;

namespace AIR.Common
{
    public class DataEventArgs<T> : EventArgs
    {
        T _data;

        public DataEventArgs(T data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            _data = data;
        }

        public T Data
        {
            get { return _data; }
        }

        public override string ToString()
        {
            return _data.ToString();
        }
    }
}
