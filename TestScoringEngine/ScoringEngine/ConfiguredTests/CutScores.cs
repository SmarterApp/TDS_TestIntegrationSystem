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

namespace ScoringEngine.ConfiguredTests
{
    public class CutScoreStrand
    {
        private float _cutScore = -1;
        private string _domain = string.Empty;
        private double _min = -1;
        private double _max = -1;
        private string _pl = string.Empty;
        private string _testName = string.Empty;

        public float CutScore
        {
            get { return _cutScore; }
        }
        public string Domain
        {
            get { return _domain; }
        }
        public double Min
        {
            get { return _min; }
        }
        public double Max
        {
            get { return _max; }
        }
        public string PerformanceLevel
        {
            get { return _pl; }
        }
        public string TestName
        {
            get { return _testName; }
        }

        internal CutScoreStrand(float cutscore, string subjectCode)
        {
            _cutScore = cutscore;
            _domain = subjectCode;
        }

        internal CutScoreStrand(string domain, double min, double max, string testName, string performanceLevel)
        {
            _domain = domain;
            _min = min;
            _max = max;
            _pl = performanceLevel;
            _testName = testName;
        }

    }
}