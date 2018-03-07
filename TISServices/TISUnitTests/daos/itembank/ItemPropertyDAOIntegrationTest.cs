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
    public class ItemPropertyDAOIntegrationTest : TestPackageDaoIntegrationTestBase<ItemPropertyDTO>
    {
        private const string ITEM_KEY = "999-999";

        private readonly ITestPackageDao<ItemPropertyDTO> testPackageDao = new ItemPropertyDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _fk_item AS ItemKey, \n" +
            "   propname AS PropertyName, \n" +
            "   propvalue AS PropertyValue, \n" +
            "   _fk_adminsubject AS SegmentKey, \n" +
            "   IsActive \n" +
            "FROM \n" +
            "   tblItemProps \n" +
            "WHERE \n" +
            "   _fk_item = '{0}' \n" +
            "   AND propname = 'unit-test-prop-name' \n" +
            "   AND propvalue = 'unit-test-prop-value' \n" +
            "   AND _fk_adminsubject = 'unit-test-segment-key'";

        private readonly ItemDTO seedDataItem = new ItemDTO
        {
            ItemBankKey = 999L,
            ItemKey = 999L,
            ItemType = "unit-test-item-type",
            FilePath = "unit-test-path",
            FileName = "unit-test-name",
            DateLastUpdated = DateTime.Now,
            Key = ITEM_KEY,
            ScorePoints = 13,
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
        public void ShouldSaveAnItemPropertyRecord()
        {
            var propList = new List<ItemPropertyDTO>
            {
                new ItemPropertyDTO
                {
                    ItemKey = seedDataItem.Key,
                    PropertyName = "unit-test-prop-name",
                    PropertyValue = "unit-test-prop-value",
                    SegmentKey = "unit-test-segment-key"
                }
            };

            testPackageDao.Insert(propList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, ITEM_KEY), DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(propList[0], insertedRecords[0]);
        }
    }
}
