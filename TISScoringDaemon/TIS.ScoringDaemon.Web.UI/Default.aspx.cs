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
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TDS.ScoringDeamon.Web;
using AIR.Common.Threading;
using TDS.ScoringDaemon.Abstractions;

namespace TIS.ScoringDaemon.Web.UI
{
    public partial class _Default : System.Web.UI.Page
    {
        private static String convertToString(Object obj)
        {
            if (obj == null) return "null";
            if (obj.GetType().IsArray)
            {
                string value = "[";
                foreach (var obj1 in (object[])obj)
                {
                    value += convertToString(obj1);
                    value += ",";
                }
                return value.Substring(0, value.Length - 1) + "]";
            }
            return obj.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Settings table
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.Cells.Add(new TableHeaderCell { Text = "Name" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Value" });
            Settings.Rows.AddAt(0, headerRow);

            Type myType = typeof(ScoringDaemonSettings);
            PropertyInfo[] properties = myType.GetProperties(
                   BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo property in properties)
            {
                // We dont want to show any secure attributes on the UI (stuff with passwords)
                if (property.GetCustomAttributes(true).OfType<SecureAttribute>().Any())
                {
                    continue;
                }

                TableRow tableRow = new TableRow();
                tableRow.Cells.Add(new TableCell { Text = property.Name });
                var propObj = property.GetValue(myType, null);
                tableRow.Cells.Add(new TableCell { Text = convertToString(propObj) });
                Settings.Rows.Add(tableRow);
            }

            //hubInfo
            headerRow = new TableHeaderRow();
            headerRow.Cells.Add(new TableHeaderCell { Text = "ClientName" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Environment" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "DB IP" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "DB Name" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Last Poll" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Tasks Executed" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Tasks Waiting" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Min Task QDelay" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Ave Task QDelay" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Max Task QDelay" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Min Task Execution Time" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Ave Task Execution Time" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Max Task Execution Time" });
            HubInfo.Rows.AddAt(0, headerRow);

            foreach (ReponseRepoMonitor hub in MonitorCollection.GetAll())
            {
                TableRow tableRow = new TableRow();
                tableRow.Cells.Add(new TableCell { Text = hub.ClientName });
                tableRow.Cells.Add(new TableCell { Text = hub.Environment });
                tableRow.Cells.Add(new TableCell { Text = hub.DBIP });
                tableRow.Cells.Add(new TableCell { Text = hub.DBName });
                tableRow.Cells.Add(new TableCell { Text = hub.LastRun.ToString() });

                ThreadPoolStats tpStats = hub.Stats;
                if (tpStats != null)
                {
                    tableRow.Cells.Add(new TableCell { Text = tpStats.TasksExecuted.ToString() });
                    tableRow.Cells.Add(new TableCell { Text = tpStats.TasksInQCount.ToString() });
                    tableRow.Cells.Add(new TableCell { Text = tpStats.MinQDelay.ToString() });
                    tableRow.Cells.Add(new TableCell { Text = tpStats.AveQDelay.ToString() });
                    tableRow.Cells.Add(new TableCell { Text = tpStats.MaxQDelay.ToString() });
                    tableRow.Cells.Add(new TableCell { Text = tpStats.MinTaskExecutionTime.ToString() });
                    tableRow.Cells.Add(new TableCell { Text = tpStats.AveTaskExecutionTime.ToString() });
                    tableRow.Cells.Add(new TableCell { Text = tpStats.MaxTaskExecutionTime.ToString() });
                }

                HubInfo.Rows.Add(tableRow);
            }

            // Item scoring callback
            headerRow = new TableHeaderRow();
            headerRow.Cells.Add(new TableHeaderCell { Text = "Tasks Executed" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Tasks Waiting" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Min Task QDelay" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Ave Task QDelay" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Max Task QDelay" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Min Task Execution Time" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Ave Task Execution Time" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Max Task Execution Time" });
            CallBackInfo.Rows.AddAt(0, headerRow);

            ThreadPoolStats tpStats1 = ItemScoringCallbackHandler.Stats;
            if (tpStats1 != null)
            {
                TableRow tableRow = new TableRow();
                tableRow.Cells.Add(new TableCell { Text = tpStats1.TasksExecuted.ToString() });
                tableRow.Cells.Add(new TableCell { Text = tpStats1.TasksInQCount.ToString() });
                tableRow.Cells.Add(new TableCell { Text = tpStats1.MinQDelay.ToString() });
                tableRow.Cells.Add(new TableCell { Text = tpStats1.AveQDelay.ToString() });
                tableRow.Cells.Add(new TableCell { Text = tpStats1.MaxQDelay.ToString() });
                tableRow.Cells.Add(new TableCell { Text = tpStats1.MinTaskExecutionTime.ToString() });
                tableRow.Cells.Add(new TableCell { Text = tpStats1.AveTaskExecutionTime.ToString() });
                tableRow.Cells.Add(new TableCell { Text = tpStats1.MaxTaskExecutionTime.ToString() });
                CallBackInfo.Rows.Add(tableRow);
            }

        }
    }
}
