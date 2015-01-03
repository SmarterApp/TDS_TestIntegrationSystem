/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Data.SqlClient;

namespace TDS.Shared.Sql
{
    public class TDSRepository : SqlRepository
    {
        private readonly string _clientName;
        private readonly int _sessionType;

        public string ClientName
        {
            get { return _clientName; }
        }

        protected TDSRepository(string connectionString, string clientName, int sessionType) : base(connectionString)
        {
            _clientName = clientName;
            _sessionType = sessionType;
        }

        protected TDSRepository(string connectionString, string clientName) : this(connectionString, clientName, 0)
        {
        }

        /// <summary>
        /// Create a repository based on the information in another repository.
        /// </summary>
        /// <param name="repo"></param>
        protected TDSRepository(TDSRepository repo) : this(repo.ConnectionString, repo._clientName, repo._sessionType)
        {
        }

        /// <summary>
        /// Add the client name parameter value to the sql command.
        /// </summary>
        protected void AddClientParameter(SqlCommand cmd)
        {
            cmd.AddVarChar("clientname", _clientName, 100);
        }

        /// <summary>
        /// Add the SessionType parameter to the sql command.
        /// </summary>
        protected void AddSessionTypeParameter(SqlCommand cmd)
        {
            cmd.AddInt("sessiontype", _sessionType);
        }

    }
}
