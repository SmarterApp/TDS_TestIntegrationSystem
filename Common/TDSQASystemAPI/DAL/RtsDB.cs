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
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;

using TDSQASystemAPI.TestResults;
using TDSQASystemAPI.Config;
using DataAccess;

namespace TDSQASystemAPI.DAL
{
    internal class RtsDB
    {
        private string _rtsDecryptionKey;
        private Database _db = null;

        internal RtsDB()
        {
            _db = DatabaseFactory.CreateDatabase("RTS_DB");
            _rtsDecryptionKey = RTSDecryption.GetDecryptionKey(ConfigurationHolder.ClientName);
        }

        //AM: all methods have been removed because they weren't being used anymore.
        //  Left class though in case we need to add more RTS DAL methods.

        internal DataTable GetAccommodations(long testeeKey, string clientName, DateTime activeDate)
        {
            DataTable table = new DataTable();
            //has columns TypeCd, Type, and Code
            using (DbCommand cmd = _db.GetStoredProcCommand("GetStudentAccommodations"))
            {
                _db.AddInParameter(cmd, "@EntityKey", DbType.Int64, testeeKey);
                _db.AddInParameter(cmd, "@activeDate", DbType.DateTime, activeDate);
                _db.AddInParameter(cmd, "@clientName", DbType.String, clientName);
                using (IDataReader rdr = _db.ExecuteReader(cmd))
                {
                    table.Load(rdr);
                }
            }
            return table;
        }

        internal DataTable GetAttributes(long testeeKey, List<RTSAttribute> attributes)
        {
            //initialize return table
            DataTable table = new DataTable();
            table.Columns.Add("Context");
            table.Columns.Add("ContextDate");
            table.Columns.Add("Name");
            table.Columns.Add("Value");
            table.Columns.Add("RelationshipType");
            table.Columns.Add("entityKey");

            //finally add each attribute to the table
            foreach (RTSAttribute attribute in attributes)
            {
                IEntityAttribute att = RTS.RTSAttributeHelper.GetEntityAttribute(attribute.EntityType, attribute.FieldName);
                if (att == null)
                    throw new NullReferenceException(string.Format("Attribute not found in RTS. Fieldname {0}, EntityType {1}", attribute.FieldName, attribute.EntityType));

                if (string.IsNullOrEmpty(attribute.Relationship))
                    GetSingleStudentAttribute(testeeKey, attribute, table, att);
                else
                {
                    int? relationshipKey = RTS.RTSAttributeHelper.GetRelationshipTypeKey(attribute.Relationship);
                    if (relationshipKey == null)
                        throw new NullReferenceException(string.Format("Relationship not found in RTS. Relationship {0}}", attribute.Relationship));

                    GetSingleRelationshipAttribute(testeeKey, attribute, table, att, relationshipKey.Value);
                }
            }

            return table;
        }

        internal void GetSingleStudentAttribute(long testeeKey, RTSAttribute attribute, DataTable table, IEntityAttribute entityAttribute)
        {
            using (DbCommand cmd = _db.GetStoredProcCommand("GetSingleStudentAttributeValue"))
            {
                _db.AddInParameter(cmd, "@EncryptionKey", DbType.String, _rtsDecryptionKey);
                _db.AddInParameter(cmd, "@isEncrypted", DbType.Boolean, entityAttribute.IsEncrypted);
                _db.AddInParameter(cmd, "@EntityKey", DbType.Int64, testeeKey);
                _db.AddInParameter(cmd, "@Date", DbType.DateTime, attribute.ContextDate);
                _db.AddInParameter(cmd, "@EntityAttributeKey", DbType.Int64, entityAttribute.Key);
                using (IDataReader rdr = _db.ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        DataRow row = table.NewRow();
                        row["Name"] = attribute.XMLName;
                        row["Context"] = attribute.Context;
                        row["ContextDate"] = attribute.ContextDate.HasValue ? System.Xml.XmlConvert.ToString(attribute.ContextDate.Value, System.Xml.XmlDateTimeSerializationMode.Unspecified) : null;
                        if (attribute.Decrypt)
                            row["Value"] = Utilities.Utility.Value(rdr["DecryptedValue"], null);
                        else row["Value"] = Utilities.Utility.Value(rdr["Value"], null);
                        row["RelationshipType"] = attribute.Relationship;
                        row["EntityKey"] = null;
                        table.Rows.Add(row);
                    }
                }
            }
        }
        internal void GetSingleRelationshipAttribute(long testeeKey, RTSAttribute attribute, DataTable table, IEntityAttribute entityAttribute, int relationshipKey)
        {
            using (DbCommand cmd = _db.GetStoredProcCommand("GetSingleRelationshipAttributeValue"))
            {
                _db.AddInParameter(cmd, "@EncryptionKey", DbType.String, _rtsDecryptionKey);
                _db.AddInParameter(cmd, "@isEncrypted", DbType.Boolean, entityAttribute.IsEncrypted);
                _db.AddInParameter(cmd, "@EntityKey", DbType.Int64, testeeKey);
                _db.AddInParameter(cmd, "@Date", DbType.DateTime, attribute.ContextDate);
                _db.AddInParameter(cmd, "@EntityAttributeKey", DbType.Int64, entityAttribute.Key);
                _db.AddInParameter(cmd, "@RelationshipTypeKey", DbType.Int64, relationshipKey);
                using (IDataReader rdr = _db.ExecuteReader(cmd))
                {
                    while (rdr.Read())
                    {
                        DataRow row = table.NewRow();
                        row["Name"] = attribute.XMLName;
                        row["Context"] = attribute.Context;
                        row["ContextDate"] = attribute.ContextDate.HasValue ? System.Xml.XmlConvert.ToString(attribute.ContextDate.Value, System.Xml.XmlDateTimeSerializationMode.Unspecified) : null;
                        if (attribute.Decrypt)
                            row["Value"] = Utilities.Utility.Value(rdr["DecryptedValue"], null);
                        else row["Value"] = Utilities.Utility.Value(rdr["Value"], null);
                        row["RelationshipType"] = attribute.Relationship;
                        row["EntityKey"] = Utilities.Utility.Value(rdr["EntityKey"], null);
                        table.Rows.Add(row);
                    }
                }
            }
        }
    }
}
