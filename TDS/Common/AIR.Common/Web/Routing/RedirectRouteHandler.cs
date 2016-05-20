/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System.Web;
using System.Web.Routing;

namespace AIR.Common.Web.Routing
{
    /// <summary>
    /// Redirect Route Handler
    /// </summary>
    /// <example>
    /// routes.Add(new Route(String.Empty, new RedirectRouteHandler("Pages/LoginShell_EMPTY.aspx")));
    /// routes.Add(new Route("default.aspx", new RedirectRouteHandler("Pages/LoginShell_DEFAULT.aspx")));
    /// </example>
    public class RedirectRouteHandler : IRouteHandler
    {
        private readonly string _newUrl;

        public RedirectRouteHandler(string newUrl)
        {
            this._newUrl = newUrl;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new RedirectHandler(_newUrl);
        }
    }
}