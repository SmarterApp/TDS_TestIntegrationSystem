/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicWrapperExtensions.cs" company="WebOS - http://www.coolwebos.com">
//   Copyright © Dixin 2010 http://weblogs.asp.net/dixin
// </copyright>
// <summary>
//   Defines the DynamicWrapperExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AIR.Common.Dynamic
{
    public static class DynamicWrapperExtensions
    {
        #region Public Methods

        public static dynamic ToDynamic<T>(this T value)
        {
            return new DynamicWrapper<T>(ref value);
        }

        #endregion
    }
}