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
using System.Web;

namespace TISServices.Authorization
{

    /// <summary>
    /// Represents 1 element of the sbacTenancyChain from the Access Token info: 
    /// (multi-valued, not required) this attribute contains the user’s role within a given scope. The scope may consist of some combination of entities that define where
    /// the user is permitted to use this role.  The format of the sbacTenancyChain is as follows:
    /// |RoleID|Name|Level|ClientID|Client|GroupOfStatesID|GroupOfStates|StateID|State|GroupOfDistrictsID|GroupOfDistricts|DistrictID|District|GroupOfInstitutionsID|GroupOfInstitutions|InstitutionID|Institution|
    /// 
    /// Example:
    /// 
    /// "sbacTenancyChain": "|1000|Tib Exporter|CLIENT|1000|QA Client|||||||||||||,|1000|Test Registration Administrator|CLIENT|1000|QA Client|||||||||||||,|1000|Tib Viewer|CLIENT|1000|QA Client|||||||||||||,|MN|Test Registration Administrator|STATE|1000|QA Client|1234|test|MN|Minnesota|||||||||,|1000|Client Coordinator|CLIENT|1000|QA Client|||||||||||||,|MN|State Coordinator|STATE|1000|QA Client|1234|test|MN|Minnesota|||||||||,|WI|State Coordinator|STATE|1000|QA Client|||WI|Wisconsin|||||||||,|1000|Tib Importer|CLIENT|1000|QA Client|||||||||||||,|WI|Test Registration Administrator|STATE|1000|QA Client|||WI|Wisconsin|||||||||"
    /// "sbacTenancyChain": "|1000|Administrator|CLIENT|1000|AIR_TR_CI|||||||||||||"
    /// </summary>
    public class Tenancy
    {
        public enum TenancyAttribute : int
        {
            RoleID = 1,
            RoleName,
            RoleLevel,
            ClientID,
            Client,
            GroupOfStatesID,
            GroupOfStates,
            StateID,
            State,
            GroupOfDistrictsID,
            GroupOfDistricts,
            DistrictID,
            District,
            GroupOfInstitutionsID,
            GroupOfInstitutions,
            InstitutionID,
            Institution
        }

        private string[] tenancyValues;

        private Tenancy(string[] tenancyValues)
        {
            this.tenancyValues = tenancyValues;
        }

        public string GetTenancyValue(TenancyAttribute attribute)
        {
            return tenancyValues[(int)attribute];
        }

        private static Tenancy Create(string tenantString)
        {
            return new Tenancy(tenantString.Split(new char[] { '|' }, StringSplitOptions.None));
        }

        internal static List<Tenancy> CreateTenancyChain(string tenancyChain)
        {
            List<Tenancy> tenants = new List<Tenancy>();
            if (string.IsNullOrEmpty(tenancyChain))
                return tenants;

            string[] tenantStrings = tenancyChain.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string tenantString in tenantStrings)
            {
                Tenancy t = Create(tenantString);
                if (t != null)
                    tenants.Add(t);
            }

            return tenants;
        }
    }
}