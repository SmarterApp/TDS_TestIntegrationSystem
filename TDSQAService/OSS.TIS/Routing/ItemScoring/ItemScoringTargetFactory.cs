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
using System.Xml.Xsl;
using System.Configuration;
using TDSQASystemAPI.Routing;
using TDSQASystemAPI.Routing.ItemScoring;
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.Extensions;

namespace OSS.TIS.Routing.ItemScoring
{
    internal class ItemScoringTargetFactory : IItemScoringTargetFactory
    {
        #region IItemScoringTargetFactory Members

        public ItemScoringTarget Create(ItemScoringTarget itemScoringTarget, TestResult testResult, out object[] sendInputArgs)
        {
            // initialize to null; can be set to whatever should be passed to Send on a case-by-case basis
            sendInputArgs = null;

            switch (itemScoringTarget.Type)
            {
                case Target.TargetType.WebService:
                    return new TSSTarget(itemScoringTarget.Name, itemScoringTarget.Class, itemScoringTarget.Type, itemScoringTarget.XmlVersion, itemScoringTarget.TransformSpec,
                        new TSSFileTransformArgs(ItemScoringConfigManager.Instance.GetItemScoringConfig(itemScoringTarget.Name).CallbackURL));
                case Target.TargetType.Custom:
                    return new ItemScoringDaemonTarget(itemScoringTarget.Name);
                default:
                    throw new ApplicationException(String.Format("Item scoring target: {0} has an unsupported type: {1}", itemScoringTarget.Name, itemScoringTarget.Type));
            }
        }

        #endregion
    }
}
