/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using TDSQASystemAPI.Config;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.Utilities;
using ScoringEngine;
using ScoringEngine.ConfiguredTests;
using System.Diagnostics;

namespace TDSQASystemAPI.Config
{
    public class ConfigurationHolder
    {
        private static Dictionary<string, TestCollection> _configHolder = new Dictionary<string, TestCollection>();
        private static Dictionary<int, Dictionary<string, Dictionary<string,MetaDataEntry>>> _metaData = 
               new Dictionary<int, Dictionary<string, Dictionary<string,MetaDataEntry>>>();
        
        private static List<AdministrationInfo> _admins = new List<AdministrationInfo>();
        protected static object _syncLoc = new object();
        private static HashSet<string> _sessionDatabases = null;
        private static string _tdsSessionDatabasesValue = null;
        private static Dictionary<string, TestEnvironment> _testEnv = null;
        //first key = project ID, second key = GroupName
        private static Dictionary<int, Dictionary<string, List<RTSAttribute>>> _rtsAttributes = new Dictionary<int, Dictionary<string, List<RTSAttribute>>>();

        private static string _clientName;

        public ConfigurationHolder()
        {
        }

        public static bool IsLoaded { get; protected set; }

		/// <summary>
		/// Load all needed DB based configuration data.
		/// </summary>
		/// <param name="dbHandle"></param>
        public virtual void Load(string dbHandle)
        {
            if (!IsLoaded)
            {
                lock (_syncLoc)
                {
                    if (!IsLoaded)
                    {
                        ConfigDB db = new ConfigDB(dbHandle);
                        Logger.Log(true, "Created ConfigDB...", EventLogEntryType.Information, false, true);
                        Load(dbHandle, db);
                        IsLoaded = true;
                    }
                }
            }
        }

        protected void Load(string dbHandle, ConfigDB db)
        {
            _clientName = ConfigurationManager.AppSettings["ClientName"];
            LoadMetaData(db);
            Logger.Log(true, "Loaded Meta Data...", EventLogEntryType.Information, false, true);
            _testEnv = db.LoadTestEnvironment();
            Logger.Log(true, "Loaded Test Environment...", EventLogEntryType.Information, false, true);
            LoadRTSAttributes(db);
            Logger.Log(true, "Loaded RTS Attributes Data...", EventLogEntryType.Information, false, true);

            int? longDbCommandTimeout = null;
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["LongDbCommandTimeout"]))
                longDbCommandTimeout = int.Parse(ConfigurationManager.AppSettings["LongDbCommandTimeout"]);
            _configHolder[dbHandle] = new ScoringEngine.TestCollection(
                ConfigurationManager.ConnectionStrings["ITEMBANK"].ConnectionString,
                ConfigurationManager.AppSettings["ScoringEnvironment"],
                ClientName,
                longDbCommandTimeout);

            Logger.Log(true, "Initialized Scoring Engine...", EventLogEntryType.Information, false, true);

            _admins = db.LoadAdministration();

            //do some error checking on the TDSSessionDatabases key in the app.config
            _sessionDatabases = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            //remove all whitespace and newlines from the value in the app settings
            Regex ptrn = new Regex(@"\s*", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            _tdsSessionDatabasesValue = ptrn.Replace(ConfigurationManager.AppSettings["TDSSessionDatabases"] ?? "", "");

            if (string.IsNullOrEmpty(_tdsSessionDatabasesValue))
                Logger.Log(true, "The key TDSSessionDatabases does not appear in the app.config file", EventLogEntryType.Information, false, true);
            else
            {
                string[] dbsArr = _tdsSessionDatabasesValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String value in dbsArr)
                    if (!_sessionDatabases.Add(value))
                        Logger.Log(true, "The TDSSessionDatabases value in the app.config file contains a duplicate entry: " + value, EventLogEntryType.Information, false, true);
            }
            Logger.Log(true, "Loaded Administration and Server Name...Finished initializing configuration", EventLogEntryType.Information, false, true);
        }

        public Dictionary<string, TestCollection> ConfigHolder
        {
            get
            {
                if (!IsLoaded)
                    throw new Exception("ConfigurationHolder has not been loaded yet");
                return _configHolder;
            }
        }

        //ProjectID, GroupID, VarName
        public Dictionary<int, Dictionary<string, Dictionary<string,MetaDataEntry>>> MetaData
        {
            get 
            {
                if (!IsLoaded)
                    throw new Exception("ConfigurationHolder has not been loaded yet");
                return _metaData;
            }
        }

        public string ClientName
        {
            get
            {
                return _clientName;
            }
        }

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="dbHandle"></param>
		/// <returns></returns>
        public TestCollection TestCollection(string dbHandle)
        {
            if (!IsLoaded) Load(dbHandle);

            if (ConfigHolder.ContainsKey(dbHandle))
                return ConfigHolder[dbHandle];
            return null;
        }

        public Dictionary<string, TestEnvironment> TestEnvironment
        {
            get
            {
                if (!IsLoaded)
                    throw new Exception("ConfigurationHolder has not been loaded yet");

                return _testEnv;
            }
        }

        public MetaDataEntry GetFromMetaData(int projectID, MetaDataEntry.GroupName group, string variable)
        {
            return GetFromMetaData(projectID, group.ToString(), variable);
        }

        public MetaDataEntry GetFromMetaData(int projectID, string group, string variable)
        {
            Dictionary<string, MetaDataEntry> dctVar = GetGroupFromMetaData(projectID, group);
            if (dctVar == null || !dctVar.ContainsKey(variable)) return null;
            return dctVar[variable];
        }

        public Dictionary<string, MetaDataEntry> GetGroupFromMetaData(int projectID, string group)
        {
            if (!IsLoaded)
                throw new Exception("ConfigurationHolder has not been loaded yet");

            if (!_metaData.ContainsKey(projectID)) return null;
            Dictionary<string, Dictionary<string, MetaDataEntry>> dct = _metaData[projectID];
            if (!dct.ContainsKey(group)) return null;
            return dct[group];
        }

        public List<MetaDataEntry> GetFromMetaData(int projectID, string variable)
        {
            if (!IsLoaded)
                throw new Exception("ConfigurationHolder has not been loaded yet");
            if (!_metaData.ContainsKey(projectID)) return null;
            Dictionary<string, Dictionary<string, MetaDataEntry>> dct = _metaData[projectID];
            List<MetaDataEntry> entries = new List<MetaDataEntry>();
            //grab any entry in Metadata that is for this variable, regardless of group
            foreach (Dictionary<string, MetaDataEntry> varDict in dct.Values)
            {
                if (varDict.Keys.Contains(variable))
                    entries.Add(varDict[variable]);
            }
            return entries;
        }

        public int GetProjectIDFromMetaData(IProjectMetaDataLoader projectMetaDataLoader)
        {
            if (!IsLoaded)
                throw new Exception("ConfigurationHolder has not been loaded yet");

            if (projectMetaDataLoader == null)
                return -1;

            // a couple checks moved down from ctor
            if (!_metaData.ContainsKey(-1))
            {
                throw new Exception("MetaData needs an entry for the 'non-project', _fk_ProjectID = -1");
            }
            if (!_metaData[-1].ContainsKey("ProjectMap"))
            {
                throw new Exception("MetaData needs a 'ProjectMap' value for GroupName");
            }
            MetaDataEntry e = projectMetaDataLoader.GetProjectMetaDataEntry(_metaData[-1]["ProjectMap"]);
            return e == null ? -1 : e.IntVal;
        }

        /// <summary>
        /// Get all configured RTS attrs for the project, all groups
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns>null if no attrs configured</returns>
        public Dictionary<string, List<RTSAttribute>> GetRTSAttributes(int projectID)
        {
            if (!IsLoaded)
                throw new Exception("ConfigurationHolder has not been loaded yet");
            if (!_rtsAttributes.ContainsKey(projectID)) return null;
            return _rtsAttributes[projectID];
        }

        /// <summary>
        /// Get all configured RTS attrs for the project, group
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="groupName"></param>
        /// <returns>null of no attrs configured</returns>
        public List<RTSAttribute> GetRTSAttributes(int projectID, string groupName)
        {
            if (!IsLoaded)
                throw new Exception("ConfigurationHolder has not been loaded yet");
            List<RTSAttribute> attrs = null;
            Dictionary<string, List<RTSAttribute>> projectAttrs = GetRTSAttributes(projectID);
            if (projectAttrs != null && projectAttrs.ContainsKey(groupName))
                attrs = projectAttrs[groupName];
            return attrs;
        }

        /// <summary>
        /// Get all configured RTS attrs
        /// </summary>
        /// <returns>null if no attrs are configured</returns>
        public List<RTSAttribute> GetRTSAttributes()
        {
            if (!IsLoaded)
                throw new Exception("ConfigurationHolder has not been loaded yet");
            List<RTSAttribute> attrs = new List<RTSAttribute>();
            if (_rtsAttributes == null)
                return attrs;
            foreach (Dictionary<string, List<RTSAttribute>> projectAttrs in _rtsAttributes.Values)
            {
                foreach (List<RTSAttribute> projectGroupAttrs in projectAttrs.Values)
                {
                    attrs.AddRange(projectGroupAttrs);
                }
            }
            return attrs.Count > 0 ? attrs : null;
        }

        public Dictionary<string, List<TestAccomodation>> GetTestAccommodations(string dbHandle, string testName)
        {
            if (!IsLoaded) Load(dbHandle);

            return _configHolder[dbHandle].GetTestAccommodations(testName);
        }

        public List<AdministrationInfo> GetAdminstrationInfo()
        {
            if (!IsLoaded)
                throw new Exception("ConfigurationHolder has not been loaded yet");

            return _admins;
        }

        public bool IsSessionDatabaseConfigured(string server, string database)
        {
            if (!IsLoaded)
                throw new Exception("ConfigurationHolder has not been loaded yet");

            return _sessionDatabases != null && _sessionDatabases.Contains(String.Format("{0},{1}", server, database));
        }

        public string TDSSessionDatabasesValue()
        {
            if (!IsLoaded)
                throw new Exception("ConfigurationHolder has not been loaded yet");

            return _tdsSessionDatabasesValue;
        }

        public AdministrationInfo GetAdminstrationInfo(DateTime activeDate)
        {
            if (!IsLoaded)
                throw new Exception("ConfigurationHolder has not been loaded yet");

            foreach (AdministrationInfo admin in _admins)
            {
                if (activeDate >= admin.StartDate && activeDate < admin.EndDate)
                {
                    return admin;
                }
            }
            throw new Exception("Could not find the administration information.");
        }

        private void LoadMetaData(ConfigDB db)
        {
            DataTable dt = db.LoadMetaData();
            foreach (DataRow dr in dt.Rows)
            {
                int projectID = Utility.Value(dr["_fk_ProjectID"], -1);
                string groupName = Utility.Value(dr["GroupName"], "Unknown");
                string varName = Utility.Value(dr["VarName"], "Unknown");
                int intVal = Utility.Value(dr["IntValue"], -1);
                double floatVal = Utility.Value(dr["FloatValue"], 0.0);
                string textVal = Utility.Value(dr["TextValue"], "");
                string comment = Utility.Value(dr["Comment"], "No Comment");

                if (!_metaData.ContainsKey(projectID))
                    _metaData.Add(projectID, new Dictionary<string, Dictionary<string, MetaDataEntry>>(StringComparer.InvariantCultureIgnoreCase));

                Dictionary<string, Dictionary<string, MetaDataEntry>> projectSet = _metaData[projectID];

                if (!projectSet.ContainsKey(groupName))
                    projectSet.Add(groupName, new Dictionary<string, MetaDataEntry>(StringComparer.InvariantCultureIgnoreCase));

                Dictionary<string, MetaDataEntry> group = projectSet[groupName];
                if (group.ContainsKey(varName)) group.Remove(varName);

                group[varName] = new MetaDataEntry(projectID, groupName, varName, floatVal, intVal, textVal);
            }
        }

        private void LoadRTSAttributes(ConfigDB db)
        {
            _rtsAttributes = new Dictionary<int, Dictionary<string, List<RTSAttribute>>>();
            DataTable dt = db.LoadRTSAttributes();
            foreach (DataRow dr in dt.Rows)
            {
                //initialize to empty string by default for error message below
                int projectID = Utility.Value(dr["_fk_ProjectID"], -1);
                string groupName = Utility.Value(dr["GroupName"], "");
                string context = Utility.Value(dr["Context"], "");
                string contextDateStr = Utility.Value(dr["ContextDate"], null);
                DateTime? contextDate = null;
                if (!string.IsNullOrEmpty(contextDateStr)) 
                    contextDate = Convert.ToDateTime(contextDateStr);
                bool decrypt = Utility.Value(dr["Decrypt"], false);
                string xmlName = Utility.Value(dr["XMLName"], "");
                string entityType = Utility.Value(dr["EntityType"], "");
                string relationship = Utility.Value(dr["Relationship"], "");
                string fieldName = Utility.Value(dr["FieldName"], "");
                bool fetch = Utility.Value(dr["FetchIfNotInXml"], true);

                if (!_rtsAttributes.ContainsKey(projectID))
                    _rtsAttributes.Add(projectID, new Dictionary<string, List<RTSAttribute>>(StringComparer.InvariantCultureIgnoreCase));

                Dictionary<string, List<RTSAttribute>> projectSet = _rtsAttributes[projectID];

                if (!projectSet.ContainsKey(groupName))
                    projectSet.Add(groupName, new List<RTSAttribute>());

                List<RTSAttribute> group = projectSet[groupName];
                RTSAttribute newAtt = new RTSAttribute(projectID, groupName, context, contextDate, decrypt, xmlName, entityType, relationship, fieldName, fetch);
                group.Add(newAtt);
            }
        }

        public bool IncludeAccommodations(int projectID, MetaDataEntry.GroupName group)
        {
            return IncludeAccommodations(projectID, group.ToString());
        }
        public bool IncludeAccommodations(int projectID, string group)
        {
            return GetMetaDataFlag(projectID, group, "Accommodations", false);
        }

        public bool GetMetaDataFlag(int projectID, MetaDataEntry.GroupName group, string varName, bool defaultValue)
        {
            return GetMetaDataFlag(projectID, group.ToString(), varName, defaultValue);
        }
        public bool GetMetaDataFlag(int projectID, string group, string varName, bool defaultValue)
        {
            if (!IsLoaded)
                throw new Exception("ConfigurationHolder has not been loaded yet");

            MetaDataEntry entry = GetFromMetaData(projectID, group, varName);
            if (entry == null)
                return defaultValue;
            return Convert.ToBoolean(entry.IntVal);
        }
    }//end class
}//end namespace
