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
using TDSQASystemAPI.Routing;
using TDSQASystemAPI.Routing.ItemScoring;

namespace OSS.TIS.Routing
{
    internal class TISTargetFactory : ITargetFactory
    {
        #region ITargetFactory Members

        public Target Create(string targetName, Target.TargetClass targetClass, Target.TargetType type, TDSQASystemAPI.TestResults.XMLAdapter.AdapterType xmlVersion, FileTransformSpec transformSpec)
        {
            if (targetClass == Target.TargetClass.Handscoring)
                return new ItemScoringTarget(targetName, targetClass, type, xmlVersion, transformSpec);
            else
                return new RESTTarget(targetName, targetClass, type, xmlVersion, transformSpec);
        }

        #endregion
    }
}
