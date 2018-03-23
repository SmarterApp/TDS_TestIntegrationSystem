using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>SubjectDTO</code>s to the <code>OSS_Configs..Client_Subject</code> table
    /// </summary>
    public class SubjectDAO : TestPackageDaoBase<SubjectDTO>
    {
        public SubjectDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.CONFIGS;
            TvpType = "SubjectTable";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_Subject(SubjectCode, [Subject], ClientName) \n" +
                "SELECT \n" +
                "   SubjectCode, \n" +
                "   [Subject], \n" +
                "   ClientName \n";
        }
    }
}
