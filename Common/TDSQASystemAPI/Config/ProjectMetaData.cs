/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using AIR.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.Data;
using TDSQASystemAPI.ExceptionHandling;

namespace TDSQASystemAPI.Config
{
    public class ProjectMetaData
    {
        public int ProjectID { get; private set; }
        public short doRAdminID { get; set; }
        public int autoAppealTriggerID { get; set; }
        public bool mergeItemScores { get; set; }
        public bool scoreInvalidations { get; set; }
        public bool updateAppealStatus { get; set; }
        public bool updateDoR { get; set; }
        public bool updateRB { get; set; }
        public bool sendToHandScoring { get; set; }

        public ProjectMetaData()
        {
            Initialize();
        }

        private void Initialize()
        {
            ProjectID = 0;
            doRAdminID = -1;
            autoAppealTriggerID = int.MinValue;
            mergeItemScores = false;
            scoreInvalidations = false;
            updateAppealStatus = false;
            updateDoR = QASystemConfigSettings.Instance.UpdateDoR;
            updateRB = QASystemConfigSettings.Instance.UpdateRB;
            sendToHandScoring = QASystemConfigSettings.Instance.SendToHandscoring;
        }

        public void SetProjectMetaData(int projectId)
        {
            // re-init back to default values then reset according to config
            Initialize();
            ConfigurationHolder configHolder = ServiceLocator.Resolve<ConfigurationHolder>();
            this.ProjectID = projectId;

            MetaDataEntry metaDataUpdateDor = configHolder.GetFromMetaData(projectId, "DoR", "UpdateDoR");
            //TODO: not used for OSS; refactor; for now allow this to be unconfigured.
            //if (metaDataUpdateDor == null)
            //    throw new QAException("Meta data not found for group = \"DoR\", variable = \"UpdateDoR\"", QAException.ExceptionType.ConfigurationError, new Exception("Meta data not found for group = \"DoR\", variable = \"UpdateDoR\""));
            if (metaDataUpdateDor == null || metaDataUpdateDor.IntVal == 0)
                this.updateDoR = false;
            else if (metaDataUpdateDor.IntVal != 1)
                throw new QAException("Bad Meta data value", QAException.ExceptionType.ConfigurationError, new Exception("Meta data not for group = \"DoR\", variable = \"UpdateDoR\" must have value 0 or 1 not" + metaDataUpdateDor.IntVal));
            MetaDataEntry metaDataDorAdminID = configHolder.GetFromMetaData(projectId, "DoR", "DoRAdmin");
            if (this.updateDoR || metaDataDorAdminID != null)
                this.doRAdminID = (short)metaDataDorAdminID.IntVal;

            MetaDataEntry metaDataUpdateRB = configHolder.GetFromMetaData(projectId, "RB", "UpdateRB");
            //TODO: not used for OSS; refactor; for now allow this to be unconfigured.
            //if (metaDataUpdateRB == null)
            //    throw new QAException("Meta data not found for group = \"RB\", variable = \"UpdateRB\"", QAException.ExceptionType.ConfigurationError, new Exception("Meta data not found for group = \"RB\", variable = \"UpdateRB\""));
            if (metaDataUpdateRB == null || metaDataUpdateRB.IntVal == 0)
                this.updateRB = false;
            else if (metaDataUpdateRB.IntVal != 1)
                throw new QAException("Bad Meta data value", QAException.ExceptionType.ConfigurationError, new Exception("Meta data not for group = \"RB\", variable = \"UpdateRB\" must have value 0 or 1 not" + metaDataUpdateRB.IntVal));

            MetaDataEntry metaDataSendToHandScoring = configHolder.GetFromMetaData(projectId, "HandScoring", "SendToHandScoring");
            //TODO: not used for OSS; refactor; for now allow this to be unconfigured.
            //if (metaDataSendToHandScoring == null)
            //    throw new QAException("Meta data not found for group = \"HandScoring\", variable = \"SendToHandScoring\"", QAException.ExceptionType.ConfigurationError, new Exception("Meta data not found for group = \"HandScoring\", variable = \"SendToHandScoring\""));
            if (metaDataSendToHandScoring == null || metaDataSendToHandScoring.IntVal == 0)
                this.sendToHandScoring = false;
            else if (metaDataSendToHandScoring.IntVal != 1)
                throw new QAException("Bad Meta data value", QAException.ExceptionType.ConfigurationError, new Exception("Meta data not for group = \"HandScoring\", variable = \"SendToHandScoring\" must have value 0 or 1 not" + metaDataSendToHandScoring.IntVal));

            MetaDataEntry metaDataScoreInvalidations = configHolder.GetFromMetaData(projectId, "QA", "ScoreInvalidations");
            if (metaDataScoreInvalidations != null && metaDataScoreInvalidations.IntVal == 1)
                this.scoreInvalidations = true;

            MetaDataEntry metaDataUpdateAppealStatus = configHolder.GetFromMetaData(projectId, "QA", "UpdateAppealStatus");
            if (metaDataUpdateAppealStatus != null && metaDataUpdateAppealStatus.IntVal == 1)
                this.updateAppealStatus = true;

            MetaDataEntry metaDataAutoAppealTrigger = configHolder.GetFromMetaData(projectId, "QA", "AutoAppealTrigger");
            if (metaDataAutoAppealTrigger != null)
                this.autoAppealTriggerID = metaDataAutoAppealTrigger.IntVal;

            MetaDataEntry itemScoring = configHolder.GetFromMetaData(projectId, "QA", "MergeItemScores");
            if (itemScoring != null)
                this.mergeItemScores = Convert.ToBoolean(itemScoring.IntVal);
        }
    }
}
