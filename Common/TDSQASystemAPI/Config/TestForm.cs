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
    internal class TestForm
    {
		#region Properties

        private string formID;
        internal string FormID
        {
            get
            {
                return formID;
            }
        }

		private long formKey;
		internal long FormKey
		{
			get
			{
				return formKey;
			}
		}

        /// <summary>
        /// Dictionary of TestItems indexed by position on form
        /// </summary>
		private Dictionary<int, TestItem> items = new Dictionary<int,TestItem>();
		internal Dictionary<int, TestItem> Items
		{
			get
			{
				return items;
			}
		}

		#endregion

        internal TestForm(string formID, long formKey)
        {
            this.formID = formID;
            this.formKey = formKey;
        }

        
        internal void AddItem(long formKey, int position, TestItem item)
        {
            if (formKey != this.formKey)
                throw new Exception("formKey does not match form");
            if (items.ContainsKey(position))
                throw new Exception("This form already has an item in position " + position.ToString());

            items[position] = item;
        }
    }//end class
}//end namespace
