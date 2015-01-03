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

namespace TDSQASystemAPI.Config
{
    internal class ProjectMetaDataLoader : IProjectMetaDataLoader
    {
        private int handscoringProjectID;
        private string testName;
        private string qaLevel;
        private string status;

        public ProjectMetaDataLoader(int handscoringProjectID, string testName, string status, string qaLevel)
        {
            this.handscoringProjectID = handscoringProjectID;
            this.testName = testName;
            this.status = status;
            this.qaLevel = qaLevel;
        }

        #region IProjectMetaDataLoader Members

        public MetaDataEntry GetProjectMetaDataEntry(Dictionary<string, MetaDataEntry> projectMap)
        {
            const string MDE_FMT_STR = "{0}-{1}{2}-{4}";

            string mdeKey = String.Empty;
            MetaDataEntry mde = null;

            if (projectMap.ContainsKey(mdeKey = String.Format(MDE_FMT_STR, handscoringProjectID, testName, String.IsNullOrEmpty(qaLevel) ? "" : String.Format("-{0}", qaLevel), status)))
                    mde = projectMap[mdeKey];

            if (mde == null)
            {
                // project map was not found.  throw exception
                throw new ApplicationException(String.Format("There is no project that corresponds the following handscoring ID, test, [qaLevel] and status combination: {0}", mdeKey));
            }

            return mde;
        }

        public void Refresh(TestResult testResult)
        {
            this.handscoringProjectID = testResult.HandscoringProjectID;
            this.testName = testResult.test.TestName;
            this.qaLevel = testResult.Opportunity.QALevel;
            this.status = testResult.Opportunity.Status;
        }

        #endregion
    }
}
