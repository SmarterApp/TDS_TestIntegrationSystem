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
    public class SetOfItemStimuliDAOIntegrationTest : TestPackageDaoIntegrationTestBase<SetOfItemStimuliDTO>
    {
        private readonly ITestPackageDao<SetOfItemStimuliDTO> testPackageDao = new SetOfItemStimuliDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _fk_item AS ItemKey, \n" +
            "   _fk_stimulus AS StimulusKey, \n" +
            "   _fk_adminsubject AS SegmentKey, \n" +
            "   loadconfig AS TestVersion \n" +
            "FROM \n" +
            "   tblSetOfItemStimuli \n" +
            "WHERE \n" +
            "   _fk_item = '999-999' \n" +
            "   AND _fk_stimulus = 'unit-test-stim-key' \n" +
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
        private readonly StimulusDTO seedDataStimulus = new StimulusDTO
        {
            ItemBankKey = 42L,
            ItsKey = 99L,
            FilePath = "unit-test-file-path",
            FileName = "unit-test-file-name",
            DateLastUpdated = DateTime.Now,
            StimulusKey = "unit-test-stim-key",
            TestVersion = 999L
        };

        /// <summary>
        /// Create seed data to satisfy foreign key constraints
        /// </summary>
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            new ItemDAO().Insert(new List<ItemDTO> { seedDataItem });
            new StimulusDAO().Insert(new List<StimulusDTO> { seedDataStimulus });
        }

        [TestMethod]
        public void ShouldSaveASetOfItemStimuliRecord()
        {
            var stimList = new List<SetOfItemStimuliDTO>
            {
                new SetOfItemStimuliDTO
                {
                    ItemKey = seedDataItem.Key,
                    StimulusKey = seedDataStimulus.StimulusKey,
                    SegmentKey = "unit-test-segment-key",
                    TestVersion = 99L
                }
            };

            testPackageDao.Insert(stimList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(stimList[0], insertedRecords[0]);
        }
    }
}
