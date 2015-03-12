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
using System.Configuration;
using TDSQASystemAPI.Utilities;
using TDSQASystemAPI.Data;
using TDSQASystemAPI.TestResults;

namespace TDSQASystemAPI.Routing.ItemScoring
{
    public class TSSTarget : ItemScoringTarget
    {
        public TSSTarget(string targetName, TargetClass targetClass, TargetType type, XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec, IFileTransformArgs transformArgs)
            : base(targetName, targetClass, type, xmlVersion, transformSpec, transformArgs) { }

        public override ITargetResult Send(TestResults.TestResult testResult, Action<object> outputProcessor, params object[] inputArgs)
        {
            // first save items to the database using the item scoring daemon target
            ItemScoringDaemonTarget isd = new ItemScoringDaemonTarget(this.Name);
            ITargetResult result = isd.Send(testResult, null);

            // if there were no items to send, and it's not a reset or invalidation, then return the !sent result.
            //  If it was a reset or invalidation, we want to send this to TSS even if all items have already been scored.
            //  Resets in particular should cause the opp to be reset in TSS.  TSS can ignore invalidations if they want
            //  or apply special handling for those too.
            if (!result.Sent 
                    && !testResult.Opportunity.Status.Equals("reset", StringComparison.InvariantCultureIgnoreCase)
                    && !testResult.Opportunity.Status.Equals("invalidated", StringComparison.InvariantCultureIgnoreCase))
                return result;

            // check "pretend" setting now that we've written the items to the item scoring table
            if (ItemScoringManager.Instance.Pretend)
                return new TargetResult() { Sent = true };

            // now send to TSS using the multipart REST target
            RESTMultipartTarget rmt = new RESTMultipartTarget(base.Name, base.Class, base.Type, base.XmlVersion, base.TransformSpec, base.TransformArgs);
            return rmt.Send(testResult, delegate(object o)
            {
                TSSResponse response = Serialization.DeserializeJson<TSSResponse>((String)o);
                if (response.Files.Count == 0)
                    throw new ApplicationException(String.Format("Error sending oppID: {0} to Target: {1}. Response contains no Files!", testResult.Opportunity.OpportunityID, base.Name));
                if (!response.Files[0].Success)
                {
                    if (!(QASystemConfigSettings.Instance.IgnoreHandscoringDuplicates && (response.Files[0].ErrorMessage ?? "").Contains("duplicate opportunity")))
                        throw new ApplicationException(String.Format("Error sending oppId: {0} to Target: {1}.  Error message: {2}", testResult.Opportunity.OpportunityID, base.Name, response.Files[0].ErrorMessage));
                }
            });
        }
    }
}
