using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.DAL.itembank.daos
{
    /// <summary>
    /// A class for getting a <code>ClientDTO</code>s from the <code>OSS_Itembank..tblClient</code> table
    /// </summary>
    public class ClientDAO : TestPackageDaoBase<ClientDTO>
    {
        public ClientDAO()
        {
            DbConnectionStringName = DatabaseConnectionStringNames.ITEMBANK;
            SelectSql =
                "SELECT \n" +
                "   _Key AS ClientKey, \n" +
                "   Name, \n" +
                "   Description, \n" +
                "   HomePath \n" +
                "FROM \n" +
                "   dbo.tblClient \n" +
                "WHERE \n" +
                "   Name = @criteria";
        }
    }
}
