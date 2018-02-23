using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class AaItemClDAOIntegrationTest : TestPackageDaoIntegrationTestBase<AaItemClDTO>
    {
        private readonly ITestPackageDao<AaItemClDTO> testPackageDao = new AaItemClDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _fk_AdminSubject AS SegmentKey, \n" +
            "   _fk_Item AS ItemKey, \n" +
            "   ContentLevel \n" +
            "FROM \n" +
            "   AA_ItemCL \n" +
            "WHERE \n" +
            "   _fk_AdminSubject = 'unit-test-segment-key' \n" +
            "   AND _fk_item = 'unit-test-item' \n" +
            "   AND ContentLevel = 'unit-test-content-level'";

        [TestMethod]
        public void ShouldSaveAnAaItemClRecord()
        {
            var aaItemList = new List<AaItemClDTO>
            {
                new AaItemClDTO
                {
                    SegmentKey = "unit-test-segment-key",
                    ItemKey = "unit-test-item",
                    ContentLevel = "unit-test-content-level"
                }
            };

            testPackageDao.Insert(aaItemList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(aaItemList[0], insertedRecords[0]);
        }
    }
}
