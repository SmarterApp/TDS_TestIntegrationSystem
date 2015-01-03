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
using TDSQASystemAPI.MeasurementModels;

namespace TDSQASystemAPI.Config
{
    internal class TestItem
    {
        private bool parametersOnTestScale = true;
		#region Properties

		private IRTModel irtModel;
		internal IRTModel IRTModel
		{
			get
			{
				return irtModel;
			}
		}

		private int itemBank;
		internal int ItemBank
		{
			get
			{
				return itemBank;
			}
		}

		private long itemName;
		internal long ItemName
		{
			get
			{
				return itemName;
			}
		}

		private Stimulus stimulusInfo = null;
		internal Stimulus StimulusInfo
		{
			get
			{
				return stimulusInfo;
			}
			set
			{
				stimulusInfo = value;
			}
		}

		private bool isActive;
		internal bool IsActive
		{
			get
			{
				return isActive;
			}
		}

		private bool isScored;
		internal bool IsScored
		{
			get
			{
				return isScored;
			}
		}

		private bool isFieldTest;
		internal bool IsFieldTest
		{
			get
			{
				return isFieldTest;
			}
		}

		internal int ParameterCount
		{
			get
			{
				return irtModel.ParameterCount;
			}
		}

		internal IRTModelFactory.Model MeasurementModel
		{
			get
			{
				return irtModel.MeasurementModel;
			}
		}

		private int scorePoint;
		internal int ScorePoint
		{
			get
			{
				return scorePoint;
			}
		}

		private string answer;
		internal string Answer
		{
			get
			{
				return answer;
			}
		}

		private int position;
		internal int Position
		{
			get
			{
				return position;
			}
		}

		#endregion

        private Dictionary<string,List<string>> _features = new Dictionary<string,List<string>>(); //key is type of features followed by a list of values

        internal TestItem(int itemBank, long itemName, IRTModelFactory.Model irtType, int parameterCount, 
            bool isActive, bool isScored, bool isFieldtest, bool parametersOnTestScale,string answer, int scorePoint, int position)
        {
            this.itemBank = itemBank;
			this.itemName = itemName;
			this.irtModel = IRTModelFactory.CreateModel(irtType);
			this.irtModel.SetParameterCount(parameterCount);

			this.isActive = isActive;
			this.isScored = isScored;
			this.isFieldTest = isFieldtest;
			this.parametersOnTestScale = parametersOnTestScale;

			this.answer = answer;
			this.scorePoint = scorePoint;
			this.position = position;
        }

        
        internal void SetParameter(int position, double value)
        {
            irtModel.SetParameter(position, value);
        }

        /// <summary>
        /// Identifies this item as having a specific feature/value pair.
        /// </summary>
        /// <param name="featureType"></param>
        /// <param name="featureValue"></param>
        internal void SetFeature(string featureType, string featureValue)
        {

            if (!_features.ContainsKey(featureType))
            {
                _features.Add(featureType,new List<string>());
            }

            List<string> lstValues = _features[featureType];

            if (lstValues.Find(delegate(string exists) {return (exists == featureValue);}) != null)
                lstValues.RemoveAll(delegate(string exists) {return (exists == featureValue);});

            lstValues.Add(featureValue);
        }

        internal bool HasFeature(string featureName, string featureValue)
        {
            if (_features.ContainsKey(featureName))
                if (_features[featureName].Find(delegate(string fv) { return (fv == featureValue); }) != null)
                    return true;
            return false;
        }


        internal void RescaleParameters(double slope, double intercept)
        {
            if (parametersOnTestScale)
                irtModel.RescaleParameters(slope, intercept);
        }
    }//end class
}//end namespace
