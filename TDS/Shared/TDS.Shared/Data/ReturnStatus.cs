/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Runtime.Serialization;
using AIR.Common.Sql;

namespace TDS.Shared.Data
{
    /// <summary>
    /// A generic delegate for people to use when creating custom gettext functions. 
    /// </summary>
    public delegate string GetMessageText(string key, string context, bool database);

    /// <summary>
    /// This class is used to parse return status from TDS session stored procedures.
    /// </summary>
    [DataContract]
    public class ReturnStatus
    {
        private string _status = String.Empty;
        private string _reason = String.Empty;
        
        private string _context = "Default";
        private string _appKey;

        /// <summary>
        /// A custom function for getting the i18n translation for the reason.
        /// </summary>
        /// <remarks>
        /// If you want to make use of this you should assign a function here when your application initializes.
        /// This is not thread safe to assign a function. But it should be thread safe to call this function.
        /// </remarks>
        public static GetMessageText GetCustomMessageText;

        [DataMember(Name="status")]
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [DataMember(Name = "reason")]
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        public string Context
        {
            get { return _context; }
        }

        public string AppKey
        {
            get { return _appKey; }
        }

        public ReturnStatus(string status)
        {
            this._status = status;
        }

        public ReturnStatus(string status, string reason)
        {
            this._status = status;
            this._reason = reason;
        }
		
        public ReturnStatus(string status, string reason, string context)
        {
            this._status = status;
            this._reason = reason;
            this._context = context;
        }

        /// <summary>
        /// Does this reader have a return status
        /// </summary>
        public static bool Exists(IColumnReader reader)
        {
            return (reader.HasColumn("status") && reader.HasColumn("reason"));
        }

        /// <summary>
        /// Parse any current return status data.
        /// </summary>
        public static ReturnStatus Parse(IColumnReader reader)
        {
            return ParseInternal(reader);
        }

        /// <summary>
        /// Advance the reader if there is return status data and parse.
        /// </summary>
        public static ReturnStatus ReadAndParse(IColumnReader reader)
        {
            // check if the reader has return status info
            if (!Exists(reader)) return null;

            // read first record
            reader.Read();

            return ParseInternal(reader);
        }

        /// <summary>
        /// Call this function on a data reader to get that rows return status info.
        /// </summary>
        private static ReturnStatus ParseInternal(IColumnReader reader)
        {
            // check if the reader has return status info
            if (!Exists(reader)) return null;

            // first get the status
            string status = reader.GetString("status");
            ReturnStatus returnStatus = new ReturnStatus(status);

            // get the reason if it exists
            if (reader.HasValue("reason")) returnStatus._reason = reader.GetString("reason");
            
            // get i18n info
            if (reader.HasValue("context")) returnStatus._context = reader.GetString("context");
            if (reader.HasValue("appkey")) returnStatus._appKey = reader.GetString("appkey");

            return returnStatus;
        }

    }

}




