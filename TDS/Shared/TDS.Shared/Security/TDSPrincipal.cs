/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System.Security.Principal;
using System.Web.Security;

namespace TDS.Shared.Security
{
    public class TDSPrincipal : IPrincipal
    {
        private TDSIdentity tdsIdentity;
        private string[] testeeRoles;

        public TDSPrincipal(TDSIdentity tdsIdentity, FormsAuthenticationTicket ticket)
        {
            this.tdsIdentity = tdsIdentity;
        }

        public TDSPrincipal(TDSIdentity identity, string[] testeeRoles)
        {
            this.tdsIdentity = identity;
            this.testeeRoles = testeeRoles;
        }

        public bool IsInRole(string role)
        {

/*
            if (!string.IsNullOrEmpty(role) && testeeRoles != null)
            {
                int l = testeeRoles.Length;

                for (int i = 0; i < l; i++)
                {
                    if (Utility.IsMatch(role, testeeRoles[i])) return true;
                }
            }
*/

            return false;
        }

        public string[] GetRoles()
        {
            return testeeRoles;
        }

        public IIdentity Identity
        {
            get { return tdsIdentity; }
        }
    }
}
