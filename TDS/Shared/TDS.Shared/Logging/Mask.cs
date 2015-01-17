/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
namespace TDS.Shared.Logging
{
    #region Imports

    using System;

    #endregion

    /// <summary>
    /// Collection of utility methods for masking values.
    /// </summary>
    
    public sealed class Mask
    {
        public static string NullString(string s)
        {
            return s == null ? string.Empty : s;
        }

        public static string EmptyString(string s, string filler)
        {
            return Mask.NullString(s).Length == 0 ? filler : s;
        }
        
        private Mask() {}
    }
}
