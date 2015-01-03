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
using System.Xml;

namespace TDSQASystemAPI.TestResults
{
	/// <summary>
	/// The score for a single item on a test, for a handscored item. Each score can contain multiple HSScores based on
	/// different scoring dimensions. Sample XML:
	/// 
	/// <item position="20" airkey="14026" operational="1" IsSelected="1" Format="CR" scorepoints="1" score="0" response="B" admindate="10/6/2009 5:37:53 PM" responsedate="10/6/2009 5:41:14 PM" answerkey="C" numberVisits="1" clientID="R0721080" strand="Read-6-DU" contentLevel="Read-6-DU|CCG3|CS1">
	///		<comment>
	///		<![CDATA[]]> 
	///		</comment>
	///		<HSscores airkey="14026" hsSequence="1">
	///			<Score Name="UCMX_OE_FIRSTCC_4" Value="D" CompName="CONDITION CODES" Dimension="" Sequence="1" Type="INITIAL" UserID="744" UserLastName="SCORER" UserFirstName="SIVA" /> 
	///			<Score Name="UCMX_OE_SECONDCC_4" Value="C" CompName="CONDITION CODES" Dimension="" Sequence="1" Type="RELIABILITY" UserID="745" UserLastName="SCORER2" UserFirstName="SIVA" /> 
	///			<Score Name="UCMX_OE_THIRDCC_4" Value="D" CompName="CONDITION CODES" Dimension="" Sequence="1" Type="RESOLUTION" UserID="746" UserLastName="SUPERVISOR 2" UserFirstName="SIVA" /> 
	///		</HSscores>
	///	</item>
	/// 
	/// </summary>
	public class ItemScore
	{
		#region Properties

		private long itemAIRKey;
		public long ItemAIRKey
		{
			get
			{
				return itemAIRKey;
			}
		}

        private string itemLanguage;
        public string ItemLanguage
        {
            get
            {
                return itemLanguage;
            }
        }

		private int hsSequence;
		public int HsSequence
		{
			get
			{
				return hsSequence;
			}
			set
			{
				hsSequence = value;
			}
		}

		private List<HandScore> hsScores = new List<HandScore>();
		public List<HandScore> HSScores
		{
			get
			{
				return hsScores;
			}
		}
		#endregion

		/// <summary>
		/// <HSscores airkey="14026" hsSequence="1">
		///			<Score Name="UCMX_OE_FIRSTCC_4" Value="D" CompNameAttribute="CONDITION CODES" Sequence="1" Type="INITIAL" UserID="744" UserLastName="SCORER" UserFirstName="SIVA" /> 
		///			<Score Name="UCMX_OE_SECONDCC_4" Value="C" CompNameAttribute="CONDITION CODES" Sequence="1" Type="RELIABILITY" UserID="745" UserLastName="SCORER2" UserFirstName="SIVA" /> 
		///			<Score Name="UCMX_OE_THIRDCC_4" Value="D" CompNameAttribute="CONDITION CODES" Sequence="1" Type="RESOLUTION" UserID="746" UserLastName="SUPERVISOR 2" UserFirstName="SIVA" /> 
		///	</HSscores>
		/// </summary>
		public ItemScore(long itemAIRKey, string itemLanguage, int hsSequence, List<HandScore> hsScores)
		{
			this.itemAIRKey = itemAIRKey;
            this.itemLanguage = itemLanguage;
			this.hsSequence = hsSequence;
			this.hsScores = hsScores;
		}

        /// <summary>
        /// Deep copy item score
        /// </summary>
        /// <returns></returns>
        public ItemScore DeepCopy()
        {
            ItemScore copy = (ItemScore)this.MemberwiseClone();
            if (hsScores != null)
                copy.hsScores = hsScores.ConvertAll(x => x.DeepCopy());
            return copy;
        }
	}
}
