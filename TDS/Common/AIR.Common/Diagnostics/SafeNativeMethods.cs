/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AIR.Common.Diagnostics
{
    internal static class SafeNativeMethods
    {
        [SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "0"),
         SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api")]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern void OutputDebugString(string message);
    }
}