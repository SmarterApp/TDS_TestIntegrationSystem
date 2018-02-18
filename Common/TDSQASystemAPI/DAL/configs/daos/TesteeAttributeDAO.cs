﻿using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TesteeAttributeDTO</code>s to the <code>OSS_Configs..Client_TesteeAttribute</code> table
    /// </summary>
    public class TesteeAttributeDAO : TestPackageDaoBase<TesteeAttributeDTO>
    {
        public TesteeAttributeDAO()
        {
            DbConnectionStringName = "configs";
            TvpVariableName = "@tvpTesteeAttributes";
            TvpType = "TesteeAttributeType";
            InsertSql =
            "INSERT \n" +
            "   dbo.Client_TesteeAttribute (clientname, TDS_ID, RTSName, type, Label, reportName, atLogin, SortOrder) \n" +
            "SELECT \n" +
            "   ClientName, \n" +
            "   TdsId, \n" +
            "   RtsName, \n" +
            "   [Type], \n" +
            "   Label, \n" +
            "   ReportName, \n" +
            "   AtLogin, \n" +
            "   SortOrder \n" +
            "FROM \n" +
            TvpVariableName;
        }

        public override void Insert(IList<TesteeAttributeDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
