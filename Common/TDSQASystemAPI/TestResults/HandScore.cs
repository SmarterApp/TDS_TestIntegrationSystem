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
	/// This class represents a single score type from Hand-scoring.
	/// XML - Score Name="UCMX_OE_FIRSTCC_4" Value="D" CompNameAttribute="CONDITION CODES" Dimension="" Sequence="1" Type="INITIAL" UserID="744" UserLastName="SCORER" UserFirstName="SIVA"
	/// </summary>
	public class HandScore
	{
		/// <summary>
		/// INITIAL = 0, RELIABILITY = 1, BACKREAD = 2, RESOLUTION = 3, MANAGEROVERRIDE = 6, RESCORE = 7, MACHINEINITIAL = 10, MACHINERELIABILITY = 11
		/// </summary>
		public enum ReadType { Initial = 0, Reliability = 1, Backread = 2, Resolution = 3, ManageOverride = 6, Rescore = 7, MachineInitial = 10, MachineReliability = 11, TargetedBackread = 21, Analytic, Final = 99 };
		/// <summary>
		/// A comp name is a Handscoring term, specific to scoring. 
		/// <value>ScorePoints</value>
		/// <value>ConditionCodes</value>
		/// <value>Dimension</value>
		/// </summary>
		public enum CompNameAttribute { ScorePoints = 0, ConditionCodes = 1, Dimension = 2 };

		#region Properties

		private string name = string.Empty;
		public string Name
		{
			get
			{
				return name;
			}
		}

		private string value = string.Empty;
		public string Value
		{
			get
			{
				return value;
			}
		}

        private string conditioncode = string.Empty;
        public string ConditionCode
        {
            get
            {
                return conditioncode;
            }
        }

		private CompNameAttribute compName;
		public CompNameAttribute CompName
		{
			get
			{
				return compName;
			}
		}

		private string dimension = string.Empty;
		public string Dimension
		{
			get
			{
				return dimension;
			}
		}

		private int sequence;
		public int Sequence
		{
			get
			{
				return sequence;
			}
		}

		private ReadType type;
		public ReadType Type
		{
			get
			{
				return type;
			}
		}
		
		private string userID = string.Empty;
		public string UserID
		{
			get
			{
				return userID;
			}
		}

		private string userLastName = string.Empty;
		public string UserLastName
		{
			get
			{
				return userLastName;
			}
		}

		private string userFirstName = string.Empty;
		public string UserFirstName
		{
			get
			{
				return userFirstName;
			}
		}

		#endregion

		/// <summary>
		/// XML - Score Name="UCMX_OE_FIRSTCC_4" Value="1" CompNameAttribute="DIMENSION" Dimension="Illocution" Sequence="1" Type="FINAL" UserID="744" UserLastName="SCORER" UserFirstName="SIVA"
		/// </summary>
        public HandScore(string name, string value, string compName, string dimension, int sequence, string type, string userID, string userLastName, string userFirstName)
        {
            InitializeHandScore(name, value, "", compName, dimension, sequence, type, userID, userLastName, userFirstName);
        }

        public HandScore(string name, string value, string conditioncode, string compName, string dimension, int sequence, string type, string userID, string userLastName, string userFirstName)
        {
            InitializeHandScore(name, value, conditioncode, compName, dimension, sequence, type, userID, userLastName, userFirstName);
        }

        /// <summary>
        /// Copy the handscore object
        /// </summary>
        /// <returns></returns>
        public HandScore DeepCopy()
        {
            HandScore other = (HandScore)this.MemberwiseClone();
            return other;
        }
 
        public void InitializeHandScore(string name, string value, string conditioncode, string compName, string dimension, int sequence, string type, string userID, string userLastName, string userFirstName)
		{
            this.name = name;
			this.value = value;
            this.conditioncode = conditioncode;
			switch (compName.ToLower())
			{
                case "score points":
                    this.compName = CompNameAttribute.ScorePoints;
                    break;
                case "scorepoints":
                    this.compName = CompNameAttribute.ScorePoints;
                    break;
                case "overall":
                    this.compName = CompNameAttribute.ScorePoints;
                    break;
                case "condition codes":
                    this.compName = CompNameAttribute.ConditionCodes;
                    break;
                case "conditioncodes":
                    this.compName = CompNameAttribute.ConditionCodes;
                    break;
                case "dimension":
					this.compName = CompNameAttribute.Dimension;
					break;
				default:
					this.compName = CompNameAttribute.Dimension;
					break;
			}
			this.dimension = dimension;
			this.sequence = sequence;
			switch (type.ToLower())
			{
				case "initial":
					this.type = ReadType.Initial;
					break;
				case "reliability":
					this.type = ReadType.Reliability;
					break;
				case "additional":
					this.type = ReadType.Reliability;
					break;
				case "backread":
					this.type = ReadType.Backread;
					break;
				case "resolution":
					this.type = ReadType.Resolution;
					break;
                case "manager read":
                    this.type = ReadType.ManageOverride;
                    break;
                case "manageoverride":
                    this.type = ReadType.ManageOverride;
                    break;
                case "manageroverride":
                    this.type = ReadType.ManageOverride;
                    break;
                case "machineinitial":
                    this.type = ReadType.Initial;
                    break;
                case "machine initial read":
                    this.type = ReadType.Initial;
                    break;
                case "targeted backread":
                    this.type = ReadType.TargetedBackread;
                    break;
                case "targetedbackread":
                    this.type = ReadType.TargetedBackread;
                    break;
                case "rescore":
					this.type = ReadType.Rescore;
					break;
                case "dimension rater":
                    this.type = ReadType.Analytic;
                    break;
                case "analytic":
                    this.type = ReadType.Analytic;
                    break;
                case "dimension rescore":
                    this.type = ReadType.Analytic;
                    break;
				case "final":
					this.type = ReadType.Final;
					break;
				default:
                    throw new Exception("Unknown ReadType: " + type);
					break;
			}
			this.userID = userID;
			this.userLastName = userLastName;
			this.userFirstName = userFirstName;
		}

        public HandScore(string name, string value, CompNameAttribute compName, string dimension, int sequence, ReadType type, string userID, string userLastName, string userFirstName)
        {
            this.name = name;
            this.value = value;
            this.compName = compName;
            this.dimension = dimension;
            this.sequence = sequence;
            this.type = type;
            this.userID = userID;
            this.userLastName = userLastName;
            this.userFirstName = userFirstName;
        }

        public HandScore(string name, string value, string conditioncode, CompNameAttribute compName, string dimension, int sequence, ReadType type, string userID, string userLastName, string userFirstName)
        {
            this.name = name;
            this.value = value;
            this.conditioncode = conditioncode;
            this.compName = compName;
            this.dimension = dimension;
            this.sequence = sequence;
            this.type = type;
            this.userID = userID;
            this.userLastName = userLastName;
            this.userFirstName = userFirstName;
        }
	}
}
