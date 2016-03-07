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
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Configuration;

namespace TDSQASystemAPI.DAL
{
    internal class Sandlot
    {
        //private Database _db;

        public Sandlot()
        {
            //_db = DatabaseFactory.CreateDatabase("TDSQC");
        }

        /// <summary>
        /// Gets the current set of QASystemSandlotSettings for this Environment and
        /// the provided shortName.
        /// Refactored from SaveExtractToDisk below so that it could be used elsewhere
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public DataRow GetSandlotSettings(string environment, string shortName)
        {
            /*QASystemDataSet.QASystemSandlotSettingsRow result = null;
            QASystemSandlotSettingsTableAdapter sandlotAdapter = new QASystemSandlotSettingsTableAdapter();
            sandlotAdapter.Connection = (SqlConnection)_db.CreateConnection();
            foreach (QASystemDataSet.QASystemSandlotSettingsRow row in sandlotAdapter.GetData(environment).Rows)
            {
                if (row.ShortName.Equals(shortName, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = row;
                    break;
                }
            }
            return result;*/
            DataTable table = new DataTable();
            DataRow result = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["TDSQC"].ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("GetQASystemSandlotSettings", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Environment", environment);
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            foreach (DataRow row in table.Rows)
            {
                if (row["ShortName"].ToString().Equals(shortName, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = row;
                    break;
                }
            }
            return result;
        }
    }
}
