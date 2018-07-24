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
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.Data;

namespace TDSQASystemAPI.BL
{
    public class XmlRepository
    {
        public enum Location { source, processing, destination, reject, archive, tistemp };

        private DAL.XmlRepository xmlRepositoryDAL;

        public XmlRepository()
        {
            xmlRepositoryDAL = new DAL.XmlRepository();
        }

        public XmlRepository(int? dbCommandTimeout)
        {
            xmlRepositoryDAL = new DAL.XmlRepository(dbCommandTimeout);
        }

        public XmlRepository(string tdsqcConnectionString)
        {
            xmlRepositoryDAL = new DAL.XmlRepository(tdsqcConnectionString);
        }

        public XmlRepository(string tdsqcConnectionString, int? dbCommandTimeout)
        {
            xmlRepositoryDAL = new DAL.XmlRepository(tdsqcConnectionString, dbCommandTimeout);
        }

        #region insert

        public long InsertXml(Location location, TestResult tr, ITestResultSerializerFactory serializerFactory)
        {
            return InsertXml(location, tr.Name, tr.Opportunity.OpportunityID, tr.Testee.EntityKey, tr.Opportunity.StatusDate, tr.Testee.IsDemo, tr.ToXml(serializerFactory)); 
        }

        public long InsertXml(Location location, XmlDocument contents, ITestResultSerializerFactory serializerFactory)
        {
            return InsertXml(location, contents, null, null, serializerFactory);
        }

        public long InsertXml(Location location, XmlDocument contents, string callbackURL, string scoremode, ITestResultSerializerFactory serializerFactory)
        {
            string testName = null;
            long testeeKey, oppId;
            bool isDemo = false;
            DateTime statusDate;
            if (serializerFactory == null)
                throw new ApplicationException("Cannot insert XML.  ITestResultSerializerFactory cannot be null.");
            XMLAdapter adapter = serializerFactory.CreateDeserializer(contents);
            adapter.GetKeyValues(out testName, out oppId, out testeeKey, out statusDate, out isDemo);

            return InsertXml(location, testName, oppId.ToString(), testeeKey, statusDate, isDemo, contents, callbackURL, scoremode);
        }

        public long InsertXml(Location location, string testName, string oppId, long testeeKey, DateTime statusDate, bool isDemo, string contents)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(contents);
            return InsertXml(location, testName, oppId, testeeKey, statusDate, isDemo, doc);
        }

        public long InsertXml(Location location, string testName, string oppId, long testeeKey, DateTime statusDate, bool isDemo, XmlDocument contents)
        {
            return InsertXml(location, testName, oppId, testeeKey, statusDate, isDemo, contents, null, null);
        }

        public long InsertXml(Location location, string testName, string oppId, long testeeKey, DateTime statusDate, bool isDemo, XmlDocument contents, string callbackURL, string scoremode)
        {
            return xmlRepositoryDAL.InsertXml(location.ToString(), testName, oppId, testeeKey, statusDate, isDemo, contents, callbackURL, scoremode);
        }

        public long InsertAndArchiveXML(long fileId, Location newFileLocation, XmlDocument contentsXml)
        {
            return xmlRepositoryDAL.InsertAndArchiveXML(fileId, Location.archive.ToString(), newFileLocation.ToString(), contentsXml);
        }

        #endregion

        #region Fetch

        public XmlDocument GetXmlContent(long fileId)
        {
            return xmlRepositoryDAL.GetXmlContent(fileId);
        }

        public XmlDocument ProcessXmlFile(long fileId)
        {
            return xmlRepositoryDAL.ProcessXmlFile(fileId);
        }

        public Queue<XmlRepositoryItem> GetXMLRepository(string instanceName)
        {
            return xmlRepositoryDAL.GetXMLRepository(instanceName);
        }        

        #endregion

        #region update

        public bool UpdateFileLocation(long fileID, Location location)
        {
            return xmlRepositoryDAL.UpdateFileLocation(fileID, location.ToString());
        }

        public bool UpdateFileContent(long fileID, XmlDocument contents)
        {
            return xmlRepositoryDAL.UpdateFileContent(fileID, contents);
        }

        #endregion

        public bool DeleteFile(long fileId)
        {
            return xmlRepositoryDAL.DeleteFile(fileId);
        }
    }
}
