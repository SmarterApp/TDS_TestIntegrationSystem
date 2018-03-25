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
    public class ItemDAOIntegrationTest : TestPackageDaoIntegrationTestBase<ItemDTO>
    {
        private readonly ITestPackageDao<ItemDTO> testPackageDao = new ItemDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _efk_ItemBank AS ItemBankKey, \n" +
            "   _efk_Item AS ItemKey, \n" +
            "   ItemType, \n" +
            "   ScorePoint AS ScorePoints, \n" +
            "   FilePath, \n" +
            "   [FileName], \n" +
            "   DateLastUpdated, \n" +
            "   _Key AS [Key], \n" +
            "   loadconfig AS TestVersion \n" +
            "FROM \n" +
            "   tblItem \n" +
            "WHERE \n" +
            "   _Key = '999-999'";

        [TestMethod]
        public void Item_ShouldSaveAnItemRecord()
        {
            var itemList = new List<ItemDTO>
            {
                new ItemDTO
                {
                    ItemBankKey = 999L,
                    ItemKey = 999L,
                    ItemType = "unit-test-item-type",
                    ScorePoints = 42,
                    FilePath = "unit-test-path",
                    FileName = "unit-test-name",
                    DateLastUpdated = DateTime.Now,
                    Key = "999-999",
                    TestVersion = 42L
                }
            };

            testPackageDao.Insert(itemList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(itemList[0], insertedRecords[0]);
        }

        [TestMethod]
        public void Item_ShouldFindExistingItemsWhenOnlyLookingForOneItem()
        {
            var itemList = new List<ItemDTO>
            {
                new ItemDTO
                {
                    ItemBankKey = 999L,
                    ItemKey = 999L,
                    ItemType = "unit-test-item-type",
                    ScorePoints = 42,
                    FilePath = "unit-test-path",
                    FileName = "unit-test-name",
                    DateLastUpdated = DateTime.Now,
                    Key = "999-999",
                    TestVersion = 42L
                }
            };

            testPackageDao.Insert(itemList);

            var result = testPackageDao.Find(itemList);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void Item_ShouldFindExistingItemsWhenLookingForManyItems()
        {
            var itemList = new List<ItemDTO>
            {
                new ItemDTO
                {
                    ItemBankKey = 999L,
                    ItemKey = 999L,
                    ItemType = "unit-test-item-type",
                    ScorePoints = 42,
                    FilePath = "unit-test-path",
                    FileName = "unit-test-name",
                    DateLastUpdated = DateTime.Now,
                    Key = "999-999",
                    TestVersion = 42L
                },
                new ItemDTO
                {
                    ItemBankKey = 888L,
                    ItemKey = 888L,
                    ItemType = "2nd-unit-test-item-type",
                    ScorePoints = 35,
                    FilePath = "2nd-unit-test-path",
                    FileName = "2nd-unit-test-name",
                    DateLastUpdated = DateTime.Now,
                    Key = "888-888",
                    TestVersion = 35L
                }
            };

            testPackageDao.Insert(itemList);

            var result = testPackageDao.Find(itemList);

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void Item_ShouldThrowArgumentExceptionWhenCriteriaIsNotAnIListOfItemDTOs()
        {
            var invalidArg = "I am not a list of itemDTOs";

            var exception = Assert.ThrowsException<ArgumentException>(() => testPackageDao.Find(invalidArg));
            Assert.AreEqual("This method expects a criteria type of IList<ItemDTO>; criteria passed in is System.String", exception.Message);
        }
    }
}
