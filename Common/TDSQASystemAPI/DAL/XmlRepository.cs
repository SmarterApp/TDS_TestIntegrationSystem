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
using System.Data;
using System.IO;
using System.Xml;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.Data;
using TDSQASystemAPI.ExceptionHandling;
using TDSQASystemAPI.Utilities;
using System.Diagnostics;

namespace TDSQASystemAPI.DAL
{
    internal class XmlRepository
    {
        private Database _db;
        private int? dbCommandTimeout;

        public XmlRepository()
        {
            _db = DatabaseFactory.CreateDatabase("TDSQC");
            dbCommandTimeout = null;
        }

        public XmlRepository(int? dbCommandTimeout) 
            : this()
        {
            this.dbCommandTimeout = dbCommandTimeout;
        }

        public XmlRepository(string tdsqcConnectionString)
        {
            _db = new SqlDatabase(tdsqcConnectionString);
            dbCommandTimeout = null;
        }

        public XmlRepository(string tdsqcConnectionString, int? dbCommandTimeout) 
            : this(tdsqcConnectionString)
        {
            this.dbCommandTimeout = dbCommandTimeout;
        }

        public long InsertXml(string location, string testName, string oppId, long testeeKey, DateTime statusDate, bool isDemo, XmlDocument contents)
        {
            return InsertXml(location, testName, oppId, testeeKey, statusDate, isDemo, contents, null);
        }
        public long InsertXml(string location, string testName, string oppId, long testeeKey, DateTime statusDate, bool isDemo, XmlDocument contents, string callbackURL)
        {
            long fileId;
            using (DbCommand cmd = _db.GetStoredProcCommand("InsertXmlRepository"))
            {
                if (dbCommandTimeout != null)
                    cmd.CommandTimeout = dbCommandTimeout.Value;
                _db.AddInParameter(cmd, "@Location", DbType.String, location);
                _db.AddInParameter(cmd, "@TestName", DbType.String, testName);
                _db.AddInParameter(cmd, "@OppId", DbType.String, oppId);
                _db.AddInParameter(cmd, "@TesteeKey", DbType.Int64, testeeKey);                
                _db.AddInParameter(cmd, "@StatusDate", DbType.DateTime, statusDate);
                _db.AddInParameter(cmd, "@IsDemo", DbType.Boolean, isDemo);
                MemoryStream ms = new MemoryStream();
                contents.Save(ms);
                SqlXml sqlXml = new SqlXml(ms);
                _db.AddInParameter(cmd, "@Contents", DbType.Xml, sqlXml);
                fileId = Convert.ToInt64(_db.ExecuteScalar(cmd));
                //Zach 12-5-2014: added for open source TDS->TIS
                if (!string.IsNullOrEmpty(callbackURL))
                    _db.AddInParameter(cmd, "@CallbackURL", DbType.String, callbackURL);
            }
            return fileId;
        }

        public long InsertAndArchiveXML(long fileId, string oldFileLocation, string newFileLocation, XmlDocument contentsXml)
        {
            long returnFileId;

            using (DbCommand cmd = _db.GetStoredProcCommand("InsertAndArchiveXML"))
            {
                if (dbCommandTimeout != null)
                    cmd.CommandTimeout = dbCommandTimeout.Value;
                _db.AddInParameter(cmd, "@fileID", DbType.Int64, fileId);
                _db.AddInParameter(cmd, "@OldFileLocation", DbType.String, oldFileLocation);
                _db.AddInParameter(cmd, "@NewFileLocation", DbType.String, newFileLocation);
                MemoryStream ms = new MemoryStream();
                contentsXml.Save(ms);
                SqlXml sqlXml = new SqlXml(ms);
                _db.AddInParameter(cmd, "@Contents", DbType.Xml, sqlXml);                
                returnFileId = Convert.ToInt64(_db.ExecuteScalar(cmd));
            }

            return returnFileId;
        }

        public XmlDocument GetXmlContent(long fileId)
        {
            XmlDocument doc = null;
            using (DbCommand cmd = _db.GetStoredProcCommand("GetXmlContentByFileID"))
            {
                if (dbCommandTimeout != null)
                    cmd.CommandTimeout = dbCommandTimeout.Value;
                _db.AddInParameter(cmd, "@FileID", DbType.Int64, fileId);
                using (SqlDataReader rdr = (SqlDataReader)_db.ExecuteReader(cmd))
                {                    
                    if (rdr.Read())
                    {
                        doc = new XmlDocument();
                        doc.Load(rdr.GetSqlXml(0).CreateReader());
                    } 
                }                                                    
            }
            return doc;
        }

        public XmlDocument ProcessXmlFile(long fileId)
        {
            XmlDocument doc = null;
            using (DbCommand cmd = _db.GetStoredProcCommand("ProcessXmlFile"))
            {
                if (dbCommandTimeout != null)
                    cmd.CommandTimeout = dbCommandTimeout.Value;
                _db.AddInParameter(cmd, "@FileID", DbType.Int64, fileId);
                using (SqlDataReader rdr = (SqlDataReader)_db.ExecuteReader(cmd))
                {
                    if (rdr.Read() && rdr[0] != null && rdr[0] != DBNull.Value)
                    {
                        doc = new XmlDocument();
                        doc.Load(rdr.GetSqlXml(0).CreateReader());
                    }
                }
            }
            return doc;
        }
       
        public Queue<XmlRepositoryItem> GetXMLRepository(string instanceName)
        //From XMLRepository, get the fileID
        {            
            XmlRepositoryItem repDataItem;
            Queue<XmlRepositoryItem> XMLRepositoryQ = new Queue<XmlRepositoryItem>();
            using (DbCommand cmd = _db.GetStoredProcCommand("GetXMLRepository"))
            {
                if (dbCommandTimeout != null)
                    cmd.CommandTimeout = dbCommandTimeout.Value;
                _db.AddInParameter(cmd, "@InstanceName", DbType.String, instanceName);
                using (SqlDataReader rdr = (SqlDataReader)_db.ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        repDataItem.FileID = long.Parse(rdr["FileID"].ToString());
                        repDataItem.OppID = rdr["OppID"].ToString();
                        repDataItem.TesteeKey = Convert.ToInt64(rdr["_efk_Testee"]);
                        object guid = rdr["SenderBrokerGuid"]; // this is a nullable column
                        if (DBNull.Value.Equals(guid) || guid == null)
                            repDataItem.SenderGUID = null;
                        else repDataItem.SenderGUID = Guid.Parse(guid.ToString());
                        //get callbackURL, this is for OSS and will be null for Non-OSS tests
                        repDataItem.CallbackURL = rdr["CallbackURL"] == null? string.Empty: rdr["CallbackURL"].ToString();
                        
                        XMLRepositoryQ.Enqueue(repDataItem);
                    }
                }
            }
            return XMLRepositoryQ;
        }

        public bool UpdateFileLocation(long fileID, string fileLocation)
        {
            bool updated = false;
            using (DbCommand cmd = _db.GetStoredProcCommand("UpdateXmlRepositoryLocation"))
            {
                if (dbCommandTimeout != null)
                    cmd.CommandTimeout = dbCommandTimeout.Value;
                _db.AddInParameter(cmd, "@FileID", DbType.Int64, fileID);
                _db.AddInParameter(cmd, "@Location", DbType.String, fileLocation);
                updated = Convert.ToInt32(_db.ExecuteScalar(cmd)) > 0;
            }
            return updated;
        }

        public bool UpdateFileContent(long fileID, XmlDocument contents)
        {
            bool updated = false;
            using (DbCommand cmd = _db.GetStoredProcCommand("UpdateXmlRepositoryContent"))
            {
                if (dbCommandTimeout != null)
                    cmd.CommandTimeout = dbCommandTimeout.Value;
                _db.AddInParameter(cmd, "@FileID", DbType.Int64, fileID);
                MemoryStream ms = new MemoryStream();
                contents.Save(ms);
                SqlXml sqlXml = new SqlXml(ms);
                _db.AddInParameter(cmd, "@Contents", DbType.Xml, sqlXml);         
                updated = Convert.ToInt32(_db.ExecuteScalar(cmd)) > 0;
            }
            return updated;
        }

        public bool DeleteFile(long fileId)
        {
            bool deleted = false;
            using (DbCommand cmd = _db.GetStoredProcCommand("DeleteXmlRepository"))
            {
                if (dbCommandTimeout != null)
                    cmd.CommandTimeout = dbCommandTimeout.Value;
                _db.AddInParameter(cmd, "@FileID", DbType.Int64, fileId);
                deleted = Convert.ToInt32(_db.ExecuteScalar(cmd)) > 0;
            }
            return deleted;
        }
    }
}
