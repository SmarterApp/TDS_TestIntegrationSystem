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
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using TDSQASystemAPI.TIDEAppealsWebService;
using TDSQASystemAPI.Data;
using TDSQASystemAPI.TestResults;

namespace TDSQASystemAPI.BL
{
    public class AppealTIDEWS : AppealWS
    {
        private  string STATUS_UPDATE_FMT_STR = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                    <AppealStatus>
                                      <ClientName>{0}</ClientName>
                                      <AppealID>{1}</AppealID>
                                      <Reason>{2}</Reason>
                                      <Status Val=""{3}""/>
                                    </AppealStatus>";
        private  string ClientName;
        private  string TdsQcConnectString;
        private  AppealsWebService AppealsWS;

        public AppealTIDEWS()
        {
            ClientName = ConfigurationManager.AppSettings["ClientName"];
            TdsQcConnectString = ConfigurationManager.ConnectionStrings["TDSQC"].ConnectionString;

            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["TIDEAppealsWebServiceURL"]))
                return;
            AppealsWS = new AppealsWebService();
            AppealsWS.Url = ConfigurationManager.AppSettings["TIDEAppealsWebServiceURL"];
            AuthSoapHd authHeader = new AuthSoapHd();
            authHeader.encryptedKey = ConfigurationManager.AppSettings["TIDEAppealsAuthKey"];
            AppealsWS.AuthSoapHdValue = authHeader;
        }

        public bool RemoteCertificateValidationCB(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //accept any certificate
            return true;
        }


        #region  UpdateAppealStatus


        // factored this out so that it can be called from the TestResult ctor for
        //  tests with appeal status that are not configured in QA.
        public bool UpdateAppealStatus(long appealRequestId, string oppId, bool succeeded, string errorMessage)
        {
            // get any active appeals for this oppId
            AppealDataSet.AppealRequestDataTable appealRequests = this.GetActiveAppeals(oppId);

            // if there are no active appeals, just return w/o doing anything.
            //  we can't update the status in this case.  Maybe an appeal file was resubmitted to 
            //  QA manually.
            if (appealRequests.Count == 0)
                return false;

            // find this particular appeal request
            DataRow[] thisRequest = appealRequests.Select(String.Format("RequestID={0}", appealRequestId));

            // Maybe this particular request was already closed and the file was
            //  manually resubmitted.  If so, nothing to update.
            if (thisRequest.Length == 0)
                return false;

            // update the appeal status for this request
            UpdateAppealStatus((AppealDataSet.AppealRequestRow)thisRequest[0], succeeded, errorMessage);

            //If this is a successful RESET or INV appeal and there is an active RESCORE appeal
            //  that was initiated prior to this appeal, we'll have to update that one too to Failure, 
            //  because we won't be getting scores back for that.  Effectivly, the RESCORE has failed.
            Appeal.AppealType appealType = (Appeal.AppealType)Enum.Parse(typeof(Appeal.AppealType), thisRequest[0]["AppealType"].ToString());
            if (succeeded && (appealType == Appeal.AppealType.RESET || appealType == Appeal.AppealType.INV))
            {
                DataRow[] rescoreAppeal = appealRequests.Select(String.Format("AppealType='{0}' and RequestDate < '{1:MM/dd/yyyy hh:mm:ss tt}'", Data.Appeal.AppealType.RESCORE, thisRequest[0]["RequestDate"]));
                if (rescoreAppeal.Length > 0)
                {
                    // sanity check; should just be 1
                    if (rescoreAppeal.Length > 1)
                        throw new ApplicationException(String.Format("{0} appeal requests found of type: {1} for oppId: {2}.", rescoreAppeal.Length, Data.Appeal.AppealType.RESCORE, oppId));

                    string msg = appealType == Appeal.AppealType.INV ? "invalidated" : "reset";
                    UpdateAppealStatus((AppealDataSet.AppealRequestRow)rescoreAppeal[0], false, String.Format("Test has been {0}.", msg));
                }
            }

            return true;
        }


        public void UpdateAppealStatus(AppealDataSet.AppealRequestRow appealRequest, bool succeeded, string errorMessage)
        {
            // if this appeal was initiated from TIDE, update the status in TIDE
            // Added this so that we can test w/o using TIDE as a front end by using a negative TIDEAppealID
            if (!String.IsNullOrEmpty(appealRequest["TIDEAppealID"].ToString())
                && long.Parse(appealRequest["TIDEAppealID"].ToString()) > 0)
            {
                if (AppealsWS == null)
                    throw new ApplicationException(String.Format("Cannot resolve TIDE Appeal Request: {0}.  TIDEAppealsWebServiceURL is not configured.", appealRequest["TIDEAppealID"]));

                string status = succeeded ? "Success" : "Failure";
                string reason = succeeded ? "Appeal succeeded" : SecurityElement.Escape(errorMessage); // no CDATA, so escape any special chars

                // accept any certificate, then call the web service to update the status in TIDE
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCB;
                    AppealsWS.UpdateAppealStatus(String.Format(STATUS_UPDATE_FMT_STR, ClientName, appealRequest["TIDEAppealID"], reason, status));
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(
                        String.Format("Error calling TIDE appeals web service for AppealID: {0} with status: {1}.  Message: {2}", appealRequest["TIDEAppealID"], status, ex.Message), ex);
                }
            }

            if (succeeded)
                new DAL.Appeal(TdsQcConnectString).UpdateAppealRequestStatus("Completed", true, null, long.Parse(appealRequest["RequestID"].ToString()));
            else
                new DAL.Appeal(TdsQcConnectString).UpdateAppealRequestStatus("Failed", true, errorMessage, long.Parse(appealRequest["RequestID"].ToString()));
        }

        public bool UpdateAppealStatus(TestResult tr, bool succeeded, string errorMessage)
        {
            // get the appeal request id for this test
            GenericVariable appealGenVar =
                        tr.Opportunity.GetGenericVariableByContextName(GenericVariable.VariableContext.APPEAL, Data.Appeal.APPEAL_REQUEST_ID_GENVAR_NAME);

            // sanity check.  should never happen
            if (appealGenVar == null)
                throw new ApplicationException(String.Format("Cannot update appeal status.  Cannot find embedded appeal RequestID for Opp: {0}, status: {1}.", tr.Opportunity.OpportunityID, tr.Opportunity.Status));

            return UpdateAppealStatus(long.Parse(appealGenVar.Value), tr.Opportunity.OpportunityID, succeeded, errorMessage);
        }


        #endregion


        public AppealDataSet.AppealRequestRow GetActiveAppeal(string oppId, Data.Appeal.AppealType appealType)
        {
            // get any active appeals for this oppId
            AppealDataSet.AppealRequestDataTable appealRequests = this.GetActiveAppeals(oppId);
            if (appealRequests.Count == 0)
                return null;
            // see if one of these has the desired appealType; there can be at most 1 per appeal type
            DataRow[] appeal = appealRequests.Select(String.Format("AppealType='{0}'", appealType));
            return appeal.Length == 0 ? null : (AppealDataSet.AppealRequestRow)appeal[0];
        }

        public bool IsAppealResolution(TestResult tr)
        {
            // appeal resolutions will have the appeal request id as a generic variable.
            //  if the status is appeal, we still need to wait for the scores before we can resolve.
            return (tr.Opportunity.Status != "appeal" &&
                tr.Opportunity.GetGenericVariableByContextName(GenericVariable.VariableContext.APPEAL, Data.Appeal.APPEAL_REQUEST_ID_GENVAR_NAME) != null);
        }


        #region private memebers

        private AppealDataSet.AppealRequestDataTable GetActiveAppeals(string oppId)
        {
            return new DAL.Appeal(TdsQcConnectString).GetActiveAppeals(oppId);
        }
        #endregion

    }
}
