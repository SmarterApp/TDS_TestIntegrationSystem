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
using System.Threading.Tasks;

using TDSQASystemAPI.TestResults;

namespace TDSQASystemAPI.TIS
{
    internal class TestOpportunityItemScore
    {
        public long OppID { get; private set; }
        public string ScoreInfo;
        public string ScoreStatus;
        public int? Score;
        public long ItemKey;
        public long BankKey;
        public string ScoreRationale { get; private set; }
        public string Response { get; private set; }

        public TestOpportunityItemScore(long oppID, string scoreInfo, string scoreStatus, int? score, long itemKey, long bankKey, 
            string scoreRationale, string response)
        {
            this.OppID = oppID;
            this.Score = score;
            this.ScoreInfo = scoreInfo;
            this.ScoreStatus = scoreStatus;
            this.ItemKey = itemKey;
            this.BankKey = bankKey;
            this.ScoreRationale = scoreRationale;
            this.Response = response;
        }

        /// <summary>
        /// Serialize the ScoreInfo property into an ItemScoreInfo object. This does not handle errors. Returns null if ScoreInfo is null or empty
        /// </summary>
        /// <returns></returns>
        public ItemScoreInfo GetItemScoreInfo()
        {
            if (string.IsNullOrEmpty(ScoreInfo))
            {
                ItemScoreInfo si = new ItemScoreInfo();
                if (!String.IsNullOrEmpty(this.ScoreRationale))
                {
                    si.Rationale = new ScoreRationale();
                    si.Rationale.Msg = this.ScoreRationale;
                }
                return si;
            }

            return (ItemScoreInfo)Utilities.Serialization.DeserializeXml<ItemScoreInfo>(ScoreInfo);
        }
    }
}
