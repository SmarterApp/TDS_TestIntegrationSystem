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
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.Extensions;
using TDSQASystemAPI.ExceptionHandling;
using TDSQASystemAPI.Routing.ItemScoring;
using AIR.Common;

namespace TDSQASystemAPI.Routing.ItemScoring
{
    public class ItemScoringTarget : Target
    {
        public ItemScoringTarget(string targetName, TargetClass targetClass, TargetType type, XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec, IFileTransformArgs transformArgs)
            : base(targetName, targetClass, type, xmlVersion, transformSpec, transformArgs) { }

        public ItemScoringTarget(string targetName, TargetClass targetClass, TargetType type, XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec)
            : base(targetName, targetClass, type, xmlVersion, transformSpec)
        {
        }

        public ItemScoringTarget(string targetName, TargetClass targetClass, TargetType type) 
            : base(targetName, targetClass, type)
        {
        }

        public override ITargetResult Send(TestResult testResult, Action<object> outputProcessor, params object[] inputArgs)
        {
            IItemScoringTargetFactory f = ServiceLocator.Resolve<IItemScoringTargetFactory>();
            if(f == null)
                throw new QAException("There is no IItemScoringTargetFactory in the service repository.", QAException.ExceptionType.ConfigurationError);
            ItemScoringTarget t = f.Create(this, testResult, out inputArgs);
            if (t == null)
                throw new QAException(String.Format("Could not create Target: {0}", base.Name), QAException.ExceptionType.ConfigurationError);
            return t.Send(testResult, outputProcessor, inputArgs);
        }

        protected virtual List<ItemResponse> GetItemsToScore(TestResult testResult)
        {
            List<ItemResponse> itemsToScore = new List<ItemResponse>();

            // get all selected items on the test that are configured for item scoring
            //  and are not already scored
            foreach (ItemResponse ir in testResult.ItemResponses)
                if (ir.IsSelected && !ir.ScoreStatus.Equals("Scored", StringComparison.InvariantCultureIgnoreCase) && ItemScoringConfigManager.Instance.ScoreItem(base.Name, ir))
                    itemsToScore.Add(ir);

            return itemsToScore;
        }
    }
}
