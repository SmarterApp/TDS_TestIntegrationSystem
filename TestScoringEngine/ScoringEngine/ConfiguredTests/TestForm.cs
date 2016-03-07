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
    public class TestForm
    {
		#region Properties

        private long itemBank;
        internal long ItemBank
        {
            get
            {
                return itemBank;
            }
        }

        private string formID;
        internal string FormID
        {
            get
            {
                return formID;
            }
        }

		private long formKey;
		public long FormKey
		{
			get
			{
				return formKey;
			}
		}

        private string itsFormKey;
        public string ITSFormKey
        {
            get
            {
                return itsFormKey;
            }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }
        }

        /// <summary>
        /// Dictionary of TestItems indexed by position on form
        /// </summary>
		private Dictionary<int, TestItem> items = new Dictionary<int,TestItem>();
        private Dictionary<string, int> itemPositions = new Dictionary<string, int>();
		public Dictionary<int, TestItem> Items
		{
			get
			{
				return items;
			}
		}

		#endregion

        internal TestForm(long itemBank, string formID, long formKey, DateTime startDate, DateTime endDate)
        {
            this.itemBank = itemBank;
            this.formID = formID;
            this.formKey = formKey;
            this.itsFormKey = string.Format("{0}-{1}", itemBank, formKey); 
            this.startDate = startDate;
            this.endDate = endDate;
        }

        
        internal void AddItem(long formKey, int position, TestItem item)
        {
            // TODO: Pass in itembank also and validate...
            if (formKey != this.formKey)
                throw new Exception("formKey does not match form");
            if (items.ContainsKey(position))
                throw new Exception("This form already has an item in position " + position.ToString());
            items[position] = item;
            if (itemPositions.ContainsKey(item.ItemName))
                throw new Exception("This form already has item " + item.ItemName + " at position " + itemPositions[item.ItemName] + ", can't now add it in position " + position.ToString() + " also");
            itemPositions[item.ItemName] = position;
        }

        public int GetItemPosition(string itemName)
        {
            if (itemPositions.ContainsKey(itemName))
                return itemPositions[itemName];
            else
                return -1;
        }
    }//end class
}//end namespace
