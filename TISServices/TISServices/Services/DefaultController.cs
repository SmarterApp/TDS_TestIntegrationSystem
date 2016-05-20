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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TISServices.Services
{
    public class DefaultController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            //TODO: can return some stats here and apply css to format.  Set start page to api/default
            //  Or just use Default.aspx 
            //Zach 12/5/2014: Using Default.aspx. Not sure what to do here for that.
            return null;
        }
    }
}