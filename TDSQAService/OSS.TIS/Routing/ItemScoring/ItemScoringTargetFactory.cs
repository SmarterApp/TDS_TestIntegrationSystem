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

        public Target Create(ItemScoringTarget itemScoringTarget, TestResult testResult)
        {
            return new TSSTarget(itemScoringTarget.Name, itemScoringTarget.Class, itemScoringTarget.Type, itemScoringTarget.XmlVersion, itemScoringTarget.TransformSpec, 
                new TSSFileTransformArgs(ConfigurationManager.AppSettings["ItemScoring:CallbackUrl"]));
        }

        #endregion
    }
}
