using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    public class ClientDAO : TestPackageDaoBase<ClientDTO>
    {
        /// <summary>
        /// A class for saving <code>ClientDTO</code>s to the <code>OSS_Configs..Client</code> table
        /// </summary>
        public ClientDAO()
        {
            DbConnectionStringName = "configs";
            TvpVariableName = "@tvpClient";
            TvpType = "ClientType";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client ([Name], Internationalize, DefaultLanguage)\n" +
                "SELECT \n" +
                "   [Name], \n" +
                "   Internationalize, \n" +
                "   DefaultLanguage \n" +
                "FROM \n" +
                TvpVariableName;
        }

        public override void Insert(IList<ClientDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
