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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AIR.Common.Sql;
using TDS.ScoringDaemon.Abstractions;
using TDS.Shared.Data;
using TDS.Shared.Sql;

namespace TIS.ScoringDaemon.Sql
{
    public class AdminRepository: TDSRepository, IAdminRepository
    {
        public AdminRepository(string connectionString)
            : base(connectionString, null)
        {
        }

        public List<DataStoreInfo> GetMonitoredDataStores(string serverName)
        {
            SqlCommand cmd = CreateCommand(CommandType.StoredProcedure, "SD_GetMonitoredSites");
            cmd.AddVarChar("scoringDaemonName", serverName, 100);


            List<DataStoreInfo> hubs = new List<DataStoreInfo>();
            ExecuteReader(cmd, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                while (reader.Read())
                {
                    DataStoreInfo hub = new DataStoreInfo
                        {
                            ServiceType = reader.GetString("serviceType"),
                            ServiceRole = reader.GetString("serviceRole"),
                            ClientName = reader.GetString("clientname"),
                            Environment = reader.GetString("environment"),
                            PublicIP = reader.GetString("IP"),
                            PrivateIP = reader.GetString("privateIP"),
                            ServerName = reader.GetString("serverName"),
                            DBName = reader.GetString("dbname"),
                        };
                    hubs.Add(hub);
                }
            });

            return hubs;
        }
    }
}
