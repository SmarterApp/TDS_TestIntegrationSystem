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
using System.ComponentModel;
using System.Text;

namespace AIR.Common.Utilities
{
    public abstract class BaseComparer<T> : IComparer<T>
    {
        protected static int CompareValues(object xValue, object yValue, ListSortDirection direction)
        {
            int num = 0;

            if ((xValue != null) && (yValue == null))
            {
                num = 1;
            }
            else if ((xValue == null) && (yValue != null))
            {
                num = -1;
            }
            else if ((xValue == null) && (yValue == null))
            {
                num = 0;
            }
            else if (xValue is IComparable)
            {
                num = ((IComparable)xValue).CompareTo(yValue);
            }
            else if (yValue is IComparable)
            {
                num = ((IComparable)yValue).CompareTo(xValue);
            }
            else if (!xValue.Equals(yValue))
            {
                num = xValue.ToString().CompareTo(yValue.ToString());
            }

            if (direction == ListSortDirection.Ascending)
            {
                return num;
            }
            else
            {
                return (num * -1);
            }

        }

        public abstract int CompareObjects(T x, T y);

        public int Compare(T x, T y)
        {
            if (x == null & y == null) return 0;
            else if (x == null) return -1;
            else if (y == null) return 1;

            return CompareObjects(x, y);
        }
    }
}
