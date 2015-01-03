/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;

namespace AIR.Common.Configuration
{
    public abstract class AppSetting
    {
        private readonly string _name;

        public string Name
        {
            get { return _name; }
        }

        protected AppSetting(string name)
        {
            _name = name;
        }

        public abstract object GetObject();
    }

    /// <summary>
    /// Represents an app setting value.
    /// </summary>
    public class AppSetting<T> : AppSetting
    {
        private readonly T _value;

        public T Value
        {
            get { return _value; }
        }

        public AppSetting(string name, T value) : base(name)
        {
            _value = value;
        }

        public static implicit operator T(AppSetting<T> value)
        {
            return value.Value;
        }

        public override object GetObject()
        {
            return _value;
        }

        public override string ToString()
        {
            if (_value == null) return "";
            return _value.ToString();
        }
    }

}