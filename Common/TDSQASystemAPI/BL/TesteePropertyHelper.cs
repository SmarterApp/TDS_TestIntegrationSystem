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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using TDSQASystemAPI.TestResults;

namespace TDSQASystemAPI.BL
{
    internal class TesteePropertyHelper
    {
        private DataTable _attributes;

        internal TesteePropertyHelper(DataTable attributesTable)
        {
            this._attributes = attributesTable;
        }

        internal bool AddAllAttributes(Testee testee)
        {
            if (_attributes == null || testee == null) return false;
            bool added = false;
            foreach (DataRow row in _attributes.Rows)
            {
                try
                {
                    string context = row["Context"].ToString();
                    DateTime contextDate = Convert.ToDateTime(row["ContextDate"] ?? DateTime.MinValue);
                    string name = row["Name"].ToString();
                    string value = row["Value"].ToString();
                    string relationship = (row["RelationshipType"] ?? "").ToString();

                    //if the relationshipType is null assume it's a TesteeAttribute, otherwise it is a TesteeRelationship
                    if (string.IsNullOrEmpty(relationship))
                    {
                        bool ret = testee.AddAttribute(new TesteeAttribute(context, contextDate, name, value));
                        added = added ? added : ret;
                    }
                    else
                    {
                        string entityKey = Utilities.Utility.Value(row["entityKey"], null);
                        long? EntityKey = null;
                        if (!string.IsNullOrEmpty(entityKey))
                            EntityKey = Convert.ToInt64(entityKey);
                        bool ret = testee.AddRelationship(new TesteeRelationship(context, contextDate, name, value, EntityKey));
                        added = added ? added : ret;
                    }
                }
                catch (Exception e)
                {
                    string msg = "Error adding configured Testee Attributes/Relationships. Could not parse a value in attributes datatable. Row values:";
                    foreach (DataColumn col in _attributes.Columns)
                        msg += " " + col.ColumnName + "=" + (row[col.ColumnName] ?? "null").ToString();
                    throw new FormatException(msg);
                }
            }
            return added;
        }
    }
}
