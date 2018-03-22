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
            "   ScorePoint, \n" +
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
        public void ShouldSaveAnItemRecord()
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
    }
}
