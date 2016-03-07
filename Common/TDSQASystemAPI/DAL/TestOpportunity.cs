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
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;

namespace TDSQASystemAPI.DAL
{
    public class TestOpportunity
    {
        //private Database _db;

        private static string TDSQCConnection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["TDSQC"].ConnectionString;
            }
        }

        static TestOpportunity() { }

        public TestOpportunity()
        {
            //_db = DatabaseFactory.CreateDatabase("TDSQC");
        }

        /// <summary>
        /// used to write a test result to disk
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="path"></param>
        public void WriteXmlDocumentToDisk(XmlDocument xml, string path)
        {
            using (XmlTextWriter writer = new XmlTextWriter(path, null))
            {
                writer.Formatting = Formatting.Indented;
                xml.Save(writer);
                writer.Close();
            }
        }

        /// <summary>
        /// gets all rows of TestOpportunityStatus for this oppID
        /// </summary>
        /// <param name="oppid"></param>
        /// <returns></returns>
        public DataTable GetTestOpportunityStatus(string oppid)
        {
            return TDSQC.GetTestOpportunityStatus(null, null, null, null, oppid, null, null, null, TDSQCConnection);
        }

        /// <summary>
        /// Gets the most recent TestOpportunityStatus record for the oppId passed in.
        /// </summary>
        /// <param name="oppId">null if no status found.</param>
        /// <returns>null if there is no record.</returns>
        public DataRow GetCurrentTestOpportunityStatus(string oppId)
        {
            //QASystemDataSet.TestOpportunityStatusRow currentStatus = null;
            //QASystemDataSet ds = new QASystemDataSet();

            DataTable table = new DataTable("TestOpportunityStatus");
            DataRow currentStatusRow = null;
            using (SqlConnection conn = new SqlConnection(TDSQCConnection))
            {
                using (SqlCommand command = new SqlCommand("GetCurrentTestOpportunityStatusByOppId", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@oppid", oppId);
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            if (table.Rows.Count > 0)
                currentStatusRow = table.Rows[0];
            return currentStatusRow;
            /*DbCommand selectCmd = _db.GetStoredProcCommand("GetCurrentTestOpportunityStatusByOppId");
            _db.AddInParameter(selectCmd, "@oppId", DbType.String, oppId);
            _db.LoadDataSet(selectCmd, ds, "TestOpportunityStatus");

            if (ds.TestOpportunityStatus.Rows.Count > 0)
            {
                currentStatus = ds.TestOpportunityStatus[0];
            }
            return currentStatus;*/
        }

        /// <summary>
        /// Returns a table containing the latest record for each opportunity for the
        /// provided testee and testID.  If mode is null/String.Empty, the results will not
        /// discriminate by mode.  Otherwise, only results of the specified mode will be returned.
        /// This is used to count a student's opportunities for a given test either across paper and online or
        /// for a single mode.
        /// </summary>
        /// <param name="testeeId">Student's internal RTS key</param>
        /// <param name="testID">The TestID (not test name/key)</param>
        /// <param name="mode">Optional: "paper" or "online"; default = all</param>
        /// <param name="excludePaperTestDiscreps">true excludes paper test discrepancies from the results</param>
        /// <returns></returns>
        public DataTable GetCurrentTestOpportunityStatus(long testeeId, string testID, string mode, bool excludePaperTestDiscreps)
        {
            /*QASystemDataSet ds = new QASystemDataSet();

            DbCommand selectCmd = _db.GetStoredProcCommand("GetCurrentTestOpportunityStatusByTesteeTest");
            _db.AddInParameter(selectCmd, "@TesteeEntityKey", DbType.Int64, testeeId);
            _db.AddInParameter(selectCmd, "@TestID", DbType.String, testID);
            if (!String.IsNullOrEmpty(mode))
            {
                _db.AddInParameter(selectCmd, "@Mode", DbType.String, mode);
            }
            _db.AddInParameter(selectCmd, "@ExcludePaperTestDiscreps", DbType.Boolean, excludePaperTestDiscreps);
            _db.LoadDataSet(selectCmd, ds, "TestOpportunityStatus");

            return ds.TestOpportunityStatus;*/

            DataTable table = new DataTable("TestOpportunityStatus");
            using (SqlConnection conn = new SqlConnection(TDSQCConnection))
            {
                using (SqlCommand command = new SqlCommand("GetCurrentTestOpportunityStatusByTesteeTest", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TesteeEntityKey", testeeId);
                    command.Parameters.AddWithValue("@TestID", testID);
                    if (!String.IsNullOrEmpty(mode))
                        command.Parameters.AddWithValue("@Mode", mode);
                    command.Parameters.AddWithValue("@ExcludePaperTestDiscreps", excludePaperTestDiscreps);
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }
    }
}
