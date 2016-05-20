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
using TDS.ScoringDaemon.Abstractions;
using TDS.ItemScoringEngine;
using AIR.Common.Web;

namespace TDS.ScoringDeamon.Web
{
    public class ItemScoreRequestFactory
    {
        public ItemScoreRequestFactory() { }

        public virtual ItemScoreRequest MakeItemScoreRequest(ReponseRepoMonitor repoMon, ScorableResponse scorableResponse, ItemScoringRule scoringRule, string itemScoringServerUrl)
        {
            // Check if we have a call back url. Without that, ISE can't send us back a score
            if (ScoringDaemonSettings.ItemScoringCallBackHostUrl == null) return null;

            //The actual rubric is retrieved from the satellites student app. We send the item file path and let the student app give us the rubric
            string itemFile = EncryptionHelper.EncodeToBase64(scorableResponse.ItemFile);
            string studentAppUrl = ScoringDaemonSettings.StudentAppUrlOverride ?? scoringRule.StudentAppUrl;
            Uri rubricUri;
            if (!Uri.TryCreate(studentAppUrl + "ItemScoringRubric.axd?item=" + itemFile, UriKind.Absolute, out rubricUri)) return null;

            // If the item scoring server and the student app are colocated, check if we can use localhost to talk between the 2 instead of thier public URLs
            if (ScoringDaemonSettings.EnableLocalHostUsageForColocatedApps)
            {
                Uri itemScoringServerUri;
                if (Uri.TryCreate(itemScoringServerUrl, UriKind.Absolute, out itemScoringServerUri) &&
                    itemScoringServerUri.Host == rubricUri.Host)
                {
                    rubricUri = (new UriBuilder(rubricUri) { Host = "localhost" }).Uri;
                }
            }

            // If the item format is one that is scored using our Java ISE, we need to send the item bank and key in the rubric URI as opposed to the itemFile
            // Rewrite the rubricUri accordingly
            if (ScoringDaemonSettings.ItemFormatsRequiringItemKeysForRubric.Contains(scorableResponse.Format))
            {
                string[] tokens = scorableResponse.ItemKey.Split('-');  // Item key is something like 195-3456
                if (tokens.Length != 2) return null;   // the item key is not parseable
                if (!Uri.TryCreate(studentAppUrl + String.Format("ItemScoringRubric.axd?itembank={0}&itemid={1}", tokens[0], tokens[1]), UriKind.Absolute, out rubricUri)) return null;
            }

            // Create a context token with enough info in it for the item scoring callback handler to persist the score
            // when it receives it
            string contextToken = GetContextToken(repoMon, scorableResponse);         

            ResponseInfo responseInfo = new ResponseInfo(scorableResponse.Format, scorableResponse.ItemKey,
                                                         scorableResponse.Response, rubricUri, RubricContentType.Uri,
                                                         contextToken, true);

            return new ItemScoreRequest
            {
                ResponseInfo = responseInfo,
                CallbackUrl = ScoringDaemonSettings.ItemScoringCallBackHostUrl + "ItemScoringCallback.axd"
            };
        }

        protected virtual string GetContextToken(ReponseRepoMonitor repoMon, ScorableResponse scorableResponse)
        {
            WebValueCollection tokenData = new WebValueCollection();
            tokenData.Set("oppKey", scorableResponse.OppKey);
            tokenData.Set("itemKey", scorableResponse.ItemKey);
            tokenData.Set("position", scorableResponse.Position);
            tokenData.Set("sequence", scorableResponse.ResponseSequence);
            tokenData.Set("scoremark", scorableResponse.ScoreMark);
            tokenData.Set("hubDBIP", repoMon.DBIP);
            tokenData.Set("hubDBName", repoMon.DBName);
            tokenData.Set("clientName", repoMon.ClientName);
            tokenData.Set("environment", repoMon.Environment);
            return EncryptionHelper.EncryptToBase64(tokenData.ToString(false)); // encrypt token (do not url encode)   
        }
    }
}
