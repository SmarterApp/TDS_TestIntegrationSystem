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
	public class Stimulus
	{
		private string stimulusID;
		public string StimulusID
		{
			get
			{
				return stimulusID;
			}
		}

		private long itemBank;
		public long ItemBank
		{
			get
			{
				return itemBank;
			}
		}

		private int numItemsRequired;
		public int NumItemsRequired
		{
			get
			{
				return numItemsRequired;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stimulusID"></param>
		/// <param name="itemBank"></param>
		/// <param name="numItemsRequired"></param>
		public Stimulus(string stimulusID, long itemBank, int numItemsRequired)
		{
			this.stimulusID = stimulusID;
			this.itemBank = itemBank;
			this.numItemsRequired = numItemsRequired;
		}

        public Stimulus DeepCopy()
        {
            Stimulus copy = (Stimulus)(this.MemberwiseClone());
            return copy;
        }
	}
}
