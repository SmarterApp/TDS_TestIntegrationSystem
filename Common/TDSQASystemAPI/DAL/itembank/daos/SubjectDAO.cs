﻿using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for saving <code>SubjectDTO</code>s to the <code>OSS_Itembank..tblSubject</code> table
    /// </summary>
    public class SubjectDAO : TestPackageDaoBase<SubjectDTO>
    {
        public SubjectDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            TvpType = "SubjectTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.tblSubject ([Name], Grade, _Key, _fk_Client, LoadConfig) \n" +
                "SELECT \n" +
                "   [Name], \n" +
                "   Grade, \n" +
                "   SubjectKey, \n" +
                "   ClientKey, \n" +
                "   TestVersion \n";
            SelectSql =
                "SELECT \n" +
                "   [Name], \n" +
                "   Grade, \n" +
                "   _key AS SubjectKey, \n" +
                "   _fk_client AS ClientKey, \n" +
                "   loadconfig AS TestVersion \n" +
                "FROM \n" +
                "   tblSubject \n" +
                "WHERE \n" +
                "   _key = @criteria";
        }
    }
}
