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
using System.Diagnostics;
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.Utilities;

namespace TDSQASystemAPI.Routing.ItemScoring
{
    public class ItemScoringManager
    {
        public static readonly ItemScoringManager Instance = new ItemScoringManager();

        public bool Pretend { get; private set; }

        private ItemScoringManager()
        {
            Pretend = ConfigurationManager.AppSettings["SendToHandscoring"].ToString().Equals("pretend", StringComparison.CurrentCultureIgnoreCase);
        }

        public int Send(TestResult tr, SendToModifiers sendToModifiers)
        {
            int sent = 0;

            List<Target> hsTargets = Target.GetOrderedTargets(tr.ProjectID, Target.TargetClass.Handscoring);
            
            foreach (Target t in hsTargets)
            {
                if (sendToModifiers != null && !sendToModifiers.ShouldSend(t.Name))
                    continue;

                if (t.Send(tr).Sent)
                {
                    sent++;
                    Logger.Log(true, String.Format("Sent data for OppId: {0} to Handscoring Target: {1}{2})", tr.Opportunity.OpportunityID, t.Name,
                        Pretend ? " (pretend)" : ""), EventLogEntryType.Information, false, true);
                }
            }

            // if we sent the file (or pretended) to one or more handscoring servers and this is not a reset, invalidation, or appeal, 
            //  update the status to handscoring.
            if (sent > 0 && !(tr.Opportunity.Status == "reset" || tr.Opportunity.Status == "invalidated" || tr.Opportunity.Status == "appeal"))
                tr.Opportunity.Status = "handscoring";

            return sent;
        }

        public int Reset(TestResult tr)
        {
            // We don't want to alter the state of tr, but rather than cloning it which would be expensive, 
            //  just preserve the initial status and status date then set them back afterward.
            //  Note that status date probably isn't necessary, but seems like the right thing to do.
            string origStatus = tr.Opportunity.Status;
            DateTime origStatusDate = tr.Opportunity.StatusDate;
            tr.Opportunity.Status = "reset";
            tr.Opportunity.StatusDate = DateTime.Now;
            // send it
            int sent = Send(tr, null);
            // reinstate original status info
            tr.Opportunity.Status = origStatus;
            tr.Opportunity.StatusDate = origStatusDate;

            return sent;
        }
    }
}
