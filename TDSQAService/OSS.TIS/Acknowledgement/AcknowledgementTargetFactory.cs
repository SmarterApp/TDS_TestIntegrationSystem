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
using TDSQASystemAPI.Acknowledgement;

namespace OSS.TIS.Acknowledgement
{
    internal class AcknowledgementTargetFactory : IAcknowledgementTargetFactory
    {
        #region IAcknowledgementTargetFactory Members

        public IAcknowledgementTarget SelectTarget(TDSQASystemAPI.Data.XmlRepositoryItem xmlRepoItem)
        {
            // only REST is supported in OSS TIS
            return new RESTAcknowledgementTarget();
        }

        #endregion
    }
}
