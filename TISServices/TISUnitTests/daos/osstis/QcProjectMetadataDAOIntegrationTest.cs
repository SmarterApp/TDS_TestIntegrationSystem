using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.osstis.daos;
using TDSQASystemAPI.DAL.osstis.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.osstis
{
    [TestClass]
    public class QcProjectMetadataDAOIntegrationTest : TestPackageDaoIntegrationTestBase<QcProjectMetadataDTO>
    {
        private readonly ITestPackageDao<QcProjectMetadataDTO> testPackageDao = new QcProjectMetadataDAO();
        private string sql =
            "SELECT \n" +
            "   _fk_projectid AS ProjectID, \n" +
            "   GroupName, \n" +
            "   VarName, \n" +
            "   IntValue, \n" +
            "   FloatValue, \n" +
            "   TextValue, \n" +
            "   Comment \n" +
            "FROM \n" +
            "   QC_ProjectMetadata \n" +
            "WHERE \n" +
            "   _fk_projectid = 42 \n" +
            "   AND GroupName = 'unit-test-group-name' \n" +
            "   AND VarName = 'unit-test-var-name'";

        [TestMethod]
        public void QcProjectMetadata_ShouldInsertACollectionOfQcProjectMetadataRecords()
        {
            var newQcProjectMetadataDto = new QcProjectMetadataDTO
            {
                ProjectId = 42,
                GroupName = "unit-test-group-name",
                VarName = "unit-test-var-name",
                IntValue = 13,
                FloatValue = 99M,
                TextValue = "unit-test-text-value",
                Comment = "unit-test-comment"
            };

            testPackageDao.Insert(new List<QcProjectMetadataDTO> { newQcProjectMetadataDto });

            var result = GetInsertedRecords(sql, DatabaseConnectionStringNames.OSS_TIS);
            CompareResults(newQcProjectMetadataDto, result.First());
        }
    }
}
