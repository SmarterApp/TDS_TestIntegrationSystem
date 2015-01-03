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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TISServices.Utilities;

namespace TISServices
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Settings table
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.Cells.Add(new TableHeaderCell { Text = "Name" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Value" });
            Settings.Rows.AddAt(0, headerRow);

            TableRow row = new TableRow();
            row.Cells.Add(new TableCell() { Text = "Start Time" });
            row.Cells.Add(new TableCell() { Text = Statistics.StartTime.ToString() });
            Settings.Rows.Add(row);
            row = new TableRow();
            row.Cells.Add(new TableCell() { Text = "Requests Received" });
            row.Cells.Add(new TableCell() { Text = Statistics.NumRequestsReceived.ToString() });
            Settings.Rows.Add(row);
            row = new TableRow();
            row.Cells.Add(new TableCell() { Text = "Requests Inserted" });
            row.Cells.Add(new TableCell() { Text = Statistics.NumRequestsInserted.ToString() });
            Settings.Rows.Add(row);
        }
    }
}