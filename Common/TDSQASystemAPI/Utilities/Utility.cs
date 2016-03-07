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
using System.Data;
using System.Data.Common;
using System.IO;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Net.Mail;
using System.Diagnostics;
using TDSQASystemAPI.Config;
using ScoringEngine.ConfiguredTests;
using TDSQASystemAPI.TestResults;


namespace TDSQASystemAPI.Utilities
{
    public class Utility
    {
		private Utility()
		{}

        #region value from object

        public static long Value(object obj, long defaultValue)
        {
            try
            {
                if (Convert.IsDBNull(obj) || obj.ToString().Contains("NULL")) return defaultValue;
                else return Convert.ToInt64(obj);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static XMLAdapter.AdapterType Value(object obj, XMLAdapter.AdapterType defaultValue)
        {
            try
            {
                if (Convert.IsDBNull(obj) || obj.ToString().Contains("NULL")) return defaultValue;
                else
                {
                    XMLAdapter.AdapterType type;
                    if (!Enum.TryParse(Convert.ToString(obj), true, out type))
                    {
                        if (Convert.ToString(obj).Equals("proprietary", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return XMLAdapter.AdapterType.TDS;
                        }
                        return defaultValue;
                    }
                    else return type;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

		public static TestBlueprint.SelectionAlgorithmType Value(object obj, TestBlueprint.SelectionAlgorithmType defaultValue)
		{
			try
			{
				if (Convert.IsDBNull(obj) || obj.ToString().Contains("NULL")) return defaultValue;
				else
				{
					if (Convert.ToString(obj).StartsWith("adaptive", StringComparison.InvariantCultureIgnoreCase))
					{
						return TestBlueprint.SelectionAlgorithmType.Adaptive;
					}
					else if (Convert.ToString(obj).ToLower() == "fixedform")
					{
						return TestBlueprint.SelectionAlgorithmType.FixedForm;
					}
					return TestBlueprint.SelectionAlgorithmType.Adaptive;
				}
			}
			catch
			{
				return defaultValue;
			}
		}

        public static int Value(object obj, int defaultValue)
        {
            try
            {
                if (Convert.IsDBNull(obj) || obj.ToString().Contains("NULL")) return defaultValue;
                else return Convert.ToInt32(obj);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static short Value(object obj, short defaultValue)
        {
            try
            {
                if (Convert.IsDBNull(obj) || obj.ToString().Contains("NULL")) return defaultValue;
                else return Convert.ToInt16(obj);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool Value(object obj, bool defaultValue)
        {
            if (Convert.IsDBNull(obj) || obj.ToString().Contains("NULL")) return defaultValue;
            else return Convert.ToBoolean(obj);
        }

        public static string Value(object obj, string defaultValue)
        {
            if (Convert.IsDBNull(obj)) return defaultValue;
            else return Convert.ToString(obj);
        }

        public static double Value(object obj, double defaultValue)
        {
            try
            {
                if (Convert.IsDBNull(obj) || obj.ToString().Contains("NULL")) return defaultValue;
                else return Convert.ToDouble(obj);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime Value(object obj, DateTime defaultValue)
        {
            try
            {
                if (Convert.IsDBNull(obj) || obj.ToString().Contains("NULL")) return defaultValue;
                else return Convert.ToDateTime(obj);
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion value from object
    }//end class
}//end namespace
