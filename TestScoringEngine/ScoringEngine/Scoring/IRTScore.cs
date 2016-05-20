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
using System.Text;

namespace ScoringEngine.Scoring
{
    enum IRTScoreType { Converged = 0, FailedConvergence = 1, NoItems = 2, Diverged = 3};

    class IRTScore
    {
        double score;
        double se;
        IRTScoreType type;

        public double Score
        {
            get
            {
                return score;
            }
        }

        public double StandardError
        {
            get
            {
                return se;
            }
        }

        public IRTScoreType Type
        {
            get
            {
                return type;
            }
        }

        public IRTScore(double score, double se, IRTScoreType type)
        {
            this.score = score;
            this.se = se;
            this.type = type;
        }
    }
}
