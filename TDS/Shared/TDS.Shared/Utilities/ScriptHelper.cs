/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using AIR.Common.Web;

namespace TDS.Shared.Utilities
{
    public static class ScriptHelper
    {
        public static string CreateRedirect(string url, bool includeBase)
        {
            if (includeBase)
            {
                url = UrlHelper.Base + url;
            }

            // add redirect JS to the page
            string js = @"
                <script type=""text/javascript""> 
                    var urlResults = '" + url + @"';
                    if (top != self) { top.location.href=urlResults } 
                    else {location.href=urlResults } 
                </script>
            ";

            return js;
        }

        public static string CreateRedirect(string url)
        {
            return CreateRedirect(url, true);
        }
    }
}
