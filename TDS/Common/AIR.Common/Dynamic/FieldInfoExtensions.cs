/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldInfoExtensions.cs" company="WebOS - http://www.coolwebos.com">
//   Copyright © Dixin 2010 http://weblogs.asp.net/dixin
// </copyright>
// <summary>
//   Defines the FieldInfoExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIR.Common.Dynamic
{
    using System.Reflection;

    internal static class FieldInfoExtensions
    {
        #region Methods

        internal static void SetValue<T>(this FieldInfo field, ref T obj, object value)
        {
            if (typeof(T).IsValueType)
            {
                field.SetValueDirect(__makeref(obj), value);
            }
            else
            {
                field.SetValue(obj, value);
            }
        }

        #endregion
    }
}