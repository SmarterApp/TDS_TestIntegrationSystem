using System;
using System.Collections.Generic;
using TDSQASystemAPI.DAL.osstis.dtos;

namespace TDSQASystemAPI.DAL.osstis.daos
{
    public class ProjectDAO : TestPackageDaoBase<ProjectDTO>
    {
        public ProjectDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.OSS_TIS;
            TvpType = "ProjectTable";
            SelectSql =
                "SELECT \n" +
                "   _Key AS ProjectKey, \n" +
                "   Description \n" +
                "FROM \n" +
                "   Projects \n" +
                "WHERE \n" +
                "   Description = @criteria";
        }

        /// <summary>
        /// This DAO is a "read-only" DAO - there is no need to insert a new <code>ProjectDTO</code>.
        /// </summary>
        public override void Insert(IList<ProjectDTO> recordsToSave)
        {
            throw new NotImplementedException();
        }
    }
}
