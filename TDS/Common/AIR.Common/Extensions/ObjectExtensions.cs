/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.ComponentModel;
using System.Globalization;

public static class ObjectExtensions 
{

    /// <summary>
    /// Converts an object to the specified target type or returns the default value.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "value">The value.</param>
    /// <returns>The target type</returns>
    public static T ConvertTo<T>(this object value)
    {
        return value.ConvertTo(default(T));
    }

    /// <summary>
    /// Converts an object to the specified target type or returns the default value.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "value">The value.</param>
    /// <param name = "defaultValue">The default value.</param>
    /// <returns>The target type</returns>
    public static T ConvertTo<T>(this object value, T defaultValue)
    {
        if (value != null)
        {
            Type targetType = typeof(T);

            if (value.GetType() == targetType) return (T)value;

            // try and convert to the value type
            TypeConverter converter = TypeDescriptor.GetConverter(value);
            
            if (converter.CanConvertTo(targetType))
            {
                return (T)converter.ConvertTo(value, targetType);
            }

            // try and convert from the targets type
            converter = TypeDescriptor.GetConverter(targetType);
            
            if (converter.CanConvertFrom(value.GetType()))
            {
                return (T)converter.ConvertFrom(value);
            }
        }

        return defaultValue;
    }
}
