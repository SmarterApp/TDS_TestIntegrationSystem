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
            DbConnectionStringName = "configs";
            TvpType = "SubjectType";
            InsertSql =
                "INSERT \n" +
                "   ClientName, \n" +
                "   [Subject], \n" +
                "   SubjectCode \n";
        }

        public override void Insert(IList<SubjectDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
