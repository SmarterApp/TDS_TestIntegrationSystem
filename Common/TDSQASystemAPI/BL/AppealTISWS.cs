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
using System.Configuration;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TISWS = TDSQASystemAPI.TISAppealsWS;
using System.Net;
using System.Text;
using TDSQASystemAPI.TISService;

namespace TDSQASystemAPI.BL
{
    public class AppealTISWS : AppealWS
    {

        private static string ClientName;
        private static string TdsQcConnectString;
        private static TISWS.Appeals tisAppeal;

        public AppealTISWS()
        {
            ClientName = ConfigurationManager.AppSettings["ClientName"];
            TdsQcConnectString = ConfigurationManager.ConnectionStrings["TDSQC"].ConnectionString;

            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["TISAppealsWebServiceURL"]))
                return;
            tisAppeal = new TISWS.Appeals();
            tisAppeal.Url = ConfigurationManager.AppSettings["TIDEAppealsWebServiceURL"];
            TISWS.AppealsAuthHeader authHeader = new TISWS.AppealsAuthHeader();
            authHeader.Key = "DoRAppealsAuth$!2";
            tisAppeal.AppealsAuthHeaderValue = authHeader;
        }

        public bool RemoteCertificateValidationCB(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }



    }
}
