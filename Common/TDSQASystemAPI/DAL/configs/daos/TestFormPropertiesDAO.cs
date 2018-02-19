using System.Collections.Generic;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TDSQASystemAPI.DAL.configs.daos
{
    /// <summary>
    /// A class for saving <code>TestFormPropertiesDTO</code>s to the <code>OSS_Configs..Client_TestFormProperties</code> table
    /// </summary>
    public class TestFormPropertiesDAO : TestPackageDaoBase<TestFormPropertiesDTO>
    {
        public TestFormPropertiesDAO()
        {
            DbConnectionStringName = "configs";
            TvpType = "TestFormPropertiesType";
            InsertSql =
                "INSERT \n" +
                "   dbo.Client_TestFormProperties(clientname, _efk_TestForm, FormID, TestID, [Language], StartDate, EndDate, TestKey) \n" +
                "SELECT \n" +
                "   ClientName, \n" +
                "   TestFormKey, \n" +
                "   FormId, \n" +
                "   TestId, \n" +
                "   [Language], \n" +
                "   StartDate, \n" +
                "   EndDate, \n" +
                "   TestKey \n";
        }

        public override void Insert(IList<TestFormPropertiesDTO> recordsToSave)
        {
            base.Insert(recordsToSave);
        }
    }
}
