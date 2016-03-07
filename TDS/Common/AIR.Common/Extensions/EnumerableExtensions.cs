/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Linq;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {

        public static string Join<T>(this IEnumerable<T> collection, string delimiter)
        {
            return string.Join(delimiter, collection.Select(o => o.ToString()).ToArray());
        }

        public static string Join<T>(this IEnumerable<T> collection, string delimiter, Func<T, string> convert)
        {
            return string.Join(delimiter, collection.Select(convert).ToArray());
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source) action(element);
        }

    }
}
