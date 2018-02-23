using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class SetOfItemStrandDAOIntegrationTest : TestPackageDaoIntegrationTestBase<SetOfItemStrandDTO>
    {
        private readonly ITestPackageDao<SetOfItemStrandDTO> testPackageDao = new SetOfItemStrandDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _fk_item AS ItemKey, \n" +
            "   _fk_strand AS StrandKey, \n" +
            "   _fk_adminsubject AS SegmentKey, \n" +
            "   loadconfig AS TestVersion \n" +
            "FROM \n" +
            "   tblSetOfItemStrands \n" +
            "WHERE \n" +
            "   _fk_item = '999-999' \n" +
            "   AND _fk_strand = 'unit-test-strand-key' \n" +
            "   AND _fk_adminsubject = 'unit-test-segment-key'";

        private readonly ItemDTO seedDataItem = new ItemDTO
        {
            ItemBankKey = 999L,
            ItemKey = 999L,
            ItemType = "unit-test-item-type",
            FilePath = "unit-test-path",
            FileName = "unit-test-name",
            DateLastUpdated = DateTime.Now,
            Key = "999-999",
            ItemId = "unit-test-id",
            TestVersion = 42L
        };

        /// <summary>
        /// Create seed data to satisfy foreign key constraints
        /// </summary>
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            new ItemDAO().Insert(new List<ItemDTO> { seedDataItem });
        }

        [TestMethod]
        public void ShouldSaveASetOfItemStrandRecord()
        {
            var strandList = new List<SetOfItemStrandDTO>
            {
                new SetOfItemStrandDTO
                {
                    ItemKey = seedDataItem.Key,
                    StrandKey = "unit-test-strand-key",
                    SegmentKey = "unit-test-segment-key",
                    TestVersion = 99L
                }
            };

            testPackageDao.Insert(strandList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);
            CompareResults(strandList[0], insertedRecords[0]);
        }
    }
}
