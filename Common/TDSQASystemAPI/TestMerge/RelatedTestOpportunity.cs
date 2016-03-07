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

namespace TDSQASystemAPI.TestMerge
{
    /// <summary>
    /// TestMerge related data class
    /// </summary>
    public class RelatedTestOpportunity
    {
        /// <summary>
        /// Test Id of this opportunity
        /// </summary>
        public string TestId = "";

        /// <summary>
        /// Opportunity number
        /// </summary>
        public int OpportunityNumber = -1;

        /// <summary>
        /// XML File Id of this opportunity
        /// </summary>
        public long LatestFileId = -1;

        /// <summary>
        /// Latest Status in the TOS table
        /// </summary>
        public string LatestStatus = "";

        /// <summary>
        /// Date this was recorded
        /// </summary>
        public DateTime DateRecorded = DateTime.Now;

        /// <summary>
        /// File ID of the recently handscored XML for this test 
        /// </summary>
        public long HandScoredFileId = -1;

        /// <summary>
        /// File ID of the recent XML which appeared before the test was 'appealed'
        /// </summary>
        public long PreAppealFileId = -1;

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="testId"></param>
        /// <param name="nOpportunityNumber"></param>
        /// <param name="latestFileId"></param>
        /// <param name="latestStatus"></param>
        /// <param name="dateRecorded"></param>
        /// <param name="handScoredFileId"></param>
        /// <param name="preAppealFileId"></param>
        public RelatedTestOpportunity(string testId, int nOpportunityNumber, long latestFileId, string latestStatus, DateTime dateRecorded, long handScoredFileId, long preAppealFileId)
        {
            TestId = testId;
            OpportunityNumber = nOpportunityNumber;
            LatestFileId = latestFileId;
            LatestStatus = latestStatus;
            DateRecorded = dateRecorded;
            HandScoredFileId = handScoredFileId;
            PreAppealFileId = preAppealFileId;
        }
    }
}
