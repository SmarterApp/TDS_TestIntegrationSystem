/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using AIR.Common.Diagnostics;

namespace TDS.Shared.Logging
{
    /*
        CREATE TABLE [dbo].[Logging](
	        [LogID] [bigint] IDENTITY(1,1) NOT NULL,
	        [ActivityID] [uniqueidentifier] NOT NULL,
	        [MachineName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	        [ProcessID] [int] NOT NULL,
	        [ThreadID] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	        [EventType] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	        [Source] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	        [UserName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	        [Message] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	        [Timestamp] [datetime] NOT NULL,
         CONSTRAINT [PK_Logging] PRIMARY KEY CLUSTERED 
        (
	        [LogID] ASC
        )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
        ) ON [PRIMARY]
     */

    /// <summary>
    /// A TraceListener that supports formatted output and writes using OutputDebugString
    /// </summary>
    public class SqlTraceListener : CustomTraceListener
    {
        private const string CONNECTION_NAME_ATTR = "connectionName";
        private const string TABLE_NAME_ATTR = "tableName";
        private string connectionString;
        private string tableName;

        private static readonly string sql = @"
            INSERT INTO {0} (ActivityID, MachineName, ProcessID, ThreadID, EventType, Source, UserName, Message, Timestamp)
            VALUES (@ActivityID, @MachineName, @ProcessID, @ThreadID, @EventType, @Source, @UserName, @Message, @Timestamp)		
        ";

        /// <summary>
        /// Creates a new <see cref="SqlTraceListener"/> />
        /// </summary>
        public SqlTraceListener() : base("SqlTraceListener")
        {
        }

        protected override string[] GetSupportedAttributes()
        {
            return new string[] { CONNECTION_NAME_ATTR, TABLE_NAME_ATTR };
        }

        protected override void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            SqlCommand cmd = GetTraceSqlCommand(eventCache, source, eventType, message);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }


        protected override void TraceDataCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            SqlCommand cmd = GetTraceSqlCommand(eventCache, source, eventType, StringFormatter.FormatData(data));
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        private SqlCommand GetSqlCommand()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConfigurationManager.ConnectionStrings[Attributes[CONNECTION_NAME_ATTR]].ToString();
            }

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = Attributes[TABLE_NAME_ATTR];
            }

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(String.Format(sql, tableName), connection);
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        private SqlCommand GetTraceSqlCommand(TraceEventCache eventCache, string source, TraceEventType eventType, string message)
        {
            SqlCommand cmd = GetSqlCommand();
            cmd.Parameters.AddWithValue("ActivityID", Trace.CorrelationManager.ActivityId);
            cmd.Parameters.AddWithValue("MachineName", Environment.MachineName);
            cmd.Parameters.AddWithValue("ProcessID", eventCache.ProcessId);
            cmd.Parameters.AddWithValue("ThreadID", eventCache.ThreadId);
            cmd.Parameters.AddWithValue("EventType", eventType.ToString());
            cmd.Parameters.AddWithValue("Source", source);
            cmd.Parameters.Add(new SqlParameter("Message", SqlDbType.VarChar, -1)).Value = message;
            cmd.Parameters.AddWithValue("Timestamp", eventCache.DateTime.ToLocalTime());
            cmd.Parameters.AddWithValue("UserName", Thread.CurrentPrincipal.Identity.Name);

            return cmd;
        }

    }
}