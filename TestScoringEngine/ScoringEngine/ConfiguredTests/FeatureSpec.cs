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
    public class FeatureSpec
    {
        private string _featureName = "";
        private string _featureValue = "";
        private int _minNumber = 0;
        private int _maxNumber = 0;
        private double _startAbility = 0;
        private double _startInformation = 0;
        private double _cut = 0;
        private double _lambdaMultiplier = 0;
        private double _scaledCutScore = Double.NaN;
        private string testSegment = String.Empty;

        internal FeatureSpec(string featureName, string featureValue, int minNum, int maxNum, double startAbility, double startInfo, double cut, double lambda, double scaledCutScore, string testSegment)
        {
            _featureName = featureName;
            _featureValue = featureValue;
            _minNumber = minNum;
            _maxNumber = maxNum;
            _startAbility = startAbility;
            _startInformation = startInfo;
            _cut = cut;
            _lambdaMultiplier = lambda;
            _scaledCutScore = scaledCutScore;
            this.testSegment = testSegment;
        }

        #region properties

        public string FeatureValue
        {
            get
            {
                return _featureValue;
            }
        }

        public string FeatureName
        {
            get
            {
                return _featureName;
            }
        }

        public int MinimumNumber
        {
            get
            {
                return _minNumber;
            }
        }

        public int MaximumNumber
        {
            get
            {
                return _maxNumber;
            }
        }

        public double StartAbility
        {
            get
            {
                return _startAbility;
            }
        }

        public double StartInformation
        {
            get
            {
                return _startInformation;
            }
        }

        public double CutScore
        {
            get
            {
                return _cut;
            }
        }

        public double LambdaMultiplier
        {
            get
            {
                return _lambdaMultiplier;
            }
        }

        public double ScaledCutScore
        {
            get
            {
                return _scaledCutScore;
            }
        }

        public string TestSegment
        {
            get
            {
                return testSegment;
            }
        }

        #endregion properties


    }
}
