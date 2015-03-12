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
using AIR.Common.Json;
using AIR.Common.Web;
using TDS.ScoringDaemon.Abstractions;
using TDS.ScoringDeamon.Web;
using TIS.ScoringDaemon.Abstractions;
using TIS.ScoringDaemon.Abstractions.Extensions;

namespace TIS.ScoringDaemon.Web
{
    public class ItemScoreRequestFactory : TDS.ScoringDeamon.Web.ItemScoreRequestFactory
    {
        public ItemScoreRequestFactory() 
            : base()
        {
        }

        protected override string GetContextToken(ReponseRepoMonitor repoMon, ScorableResponse scorableResponse)
        {
            int[] itemKey = scorableResponse.GetItemKeyTokens();

            ItemScoreRequestContextToken contextToken = new ItemScoreRequestContextToken()
            {
                clientName = repoMon.ClientName,
                oppKey = scorableResponse.OppKey,
                reportingVersion = scorableResponse.ResponseSequence,
                itemBank = itemKey[0],
                itemID = itemKey[1],
                TISIP = repoMon.DBIP,
                TISDbName = repoMon.DBName,
                environment = repoMon.Environment,
                itemType = scorableResponse.Format
            };

            return EncryptionHelper.EncryptToBase64(JsonHelper.Serialize<ItemScoreRequestContextToken>(contextToken));  // encrypt token (do not url encode)  
        }

    }
}
