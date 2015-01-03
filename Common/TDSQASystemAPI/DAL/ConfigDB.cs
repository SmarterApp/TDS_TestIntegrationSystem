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
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

using TDSQASystemAPI.Config;
using TDSQASystemAPI.Utilities;
using TDSQASystemAPI.TestResults;
using System.Configuration;



namespace TDSQASystemAPI.DAL
{
    internal class ConfigDB
    {
        private Database _db = null;
        private Database _dbItems = null;

        internal ConfigDB(string handle)
        {
            try
            {
                _db = DatabaseFactory.CreateDatabase(handle);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create DB " + handle + ", sql says: " + e.Message);
            }
            try
            {
                _dbItems = DatabaseFactory.CreateDatabase("ITEMBANK");
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create DB ITEMBANK, sql says: " + e.Message);
            }
        }

        #region loading

		/// <summary>
        /// Loads 'MinHoursBetweenTest' and 'DaysToExpire' from TDS sesion
        /// Indexed by testID
		/// </summary>
		/// <param name="tc"></param>
        internal Dictionary<string, TestEnvironment> LoadTestEnvironment()
        {
            DbCommand cmd =  _dbItems.GetSqlStringCommand(ConfigDBConstants.LOAD_TEST_ENV);
            DataSet ds = _dbItems.ExecuteDataSet(cmd);

            if (ds.Tables[0].Rows.Count == 0)
                throw new Exception("The Client_TimeLimits table in the TDS Configs DB contains no rows");

            DataRow[] rows = ds.Tables[0].Select("ClientName = '" + ConfigurationManager.AppSettings["ClientName"] + "'");
            if (rows.Length == 0)
                throw new Exception("The Client_TimeLimits table in the TDS Configs DB does not contain a row for client " + ConfigurationManager.AppSettings["ClientName"]);

            Dictionary<string, TestEnvironment> testEnvironments = new Dictionary<string,TestEnvironment>();
            foreach (DataRow row in rows)
            {
                string testID = row["_efk_TestID"].ToString();
                if (testEnvironments.ContainsKey(testID))
                    throw new Exception("The Client_TimeLimits table in the TDS Configs DB contains more than one row for client " + ConfigurationManager.AppSettings["ClientName"] + " and test " + testID);
                testEnvironments[testID] =
                    new TestEnvironment(Convert.ToInt32(row["OppExpire"]), Convert.ToInt32(row["OppDelay"]));
            }
            return testEnvironments;
        }

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
        internal DataTable LoadMetaData()
        {
            DbCommand cmd = _db.GetSqlStringCommand(ConfigDBConstants.LOAD_METADATA);
            DataSet ds = _db.ExecuteDataSet(cmd);
            return ds.Tables[0];
        }

        internal DataTable LoadRTSAttributes()
        {
            DbCommand cmd = _db.GetSqlStringCommand(ConfigDBConstants.LOAD_RTS_ATTRIBUTES);
            DataSet ds = _db.ExecuteDataSet(cmd);
            return ds.Tables[0];
        }

        /*
         * TODO: we can query this from the item bank as follows...
         * 
         * SELECT SessionDB, @@servername as ServerName
         * FROM TDSCONFIGS_Client_Externs
         * where clientName = 'Delaware' 
         * and environment = 'FunctionalTest' -- or Production or UAT, depending on the Environment config setting
         * 
         * For now, using app.config settings
         * 
        internal string LoadServerName(out string databaseName)
        {
            using (DbCommand cmd = _dbSession.GetSqlStringCommand("select serverproperty('servername') AS ServerName, DB_NAME() AS DBNAME"))
            {
                DataSet ds = _dbSession.ExecuteDataSet(cmd);
                databaseName = ds.Tables[0].Rows[0]["DBNAME"].ToString();
                return ds.Tables[0].Rows[0]["ServerName"].ToString();
            }
            //using (DbCommand cmd = _db.GetSqlStringCommand("SELECT TDS_Session_Name FROM Externs"))
            //{
            //    DataSet ds = _db.ExecuteDataSet(cmd);
            //    databaseName = ds.Tables[0].Rows[0]["TDS_Session_Name"].ToString();
            //}
            //using (DbCommand cmd = _db.GetSqlStringCommand("select serverproperty('servername') AS ServerName"))
            //{
            //    DataSet ds = _db.ExecuteDataSet(cmd);
            //    return ds.Tables[0].Rows[0]["ServerName"].ToString();
            //}
        }
         * */

        #endregion loading

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        internal List<AdministrationInfo> LoadAdministration()
        {
            List<AdministrationInfo> admins = new List<AdministrationInfo>();
            DbCommand cmd = _db.GetSqlStringCommand(ConfigDBConstants.LOAD_ADMINISTRATION);
            DataSet ds = _db.ExecuteDataSet(cmd);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                admins.Add(new AdministrationInfo(Convert.ToInt64(dr["_key"]), Convert.ToDateTime(dr["startDate"]), 
                    Convert.ToDateTime(dr["endDate"])));
            }
            return admins;
        }
    }
}
