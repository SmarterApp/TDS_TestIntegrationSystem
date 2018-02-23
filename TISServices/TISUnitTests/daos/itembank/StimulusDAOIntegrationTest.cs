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
    public class StimulusDAOIntegrationTest : TestPackageDaoIntegrationTestBase<StimulusDTO>
    {
        private readonly ITestPackageDao<StimulusDTO> testPackageDao = new StimulusDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _efk_itembank AS ItemBankKey, \n" +
            "   _efk_itskey AS ItsKey, \n" +
            "   ClientId AS ClientKey, \n" +
            "   FilePath, \n" +
            "   [FileName], \n" +
            "   DateLastUpdated, \n" +
            "   _key AS PassageKey, \n" +
            "   loadconfig AS TestVersion \n" +
            "FROM \n" +
            "   tblStimulus \n" +
            "WHERE \n" +
            "   _key = 'unit-test-stim-key'";

        [TestMethod]
        public void ShouldSaveAStimulusRecord()
        {
            var stimulusList = new List<StimulusDTO>
            {
                new StimulusDTO
                {
                    ItemBankKey = 42L,
                    ItsKey = 99L,
                    ClientKey = "unit-test-client-key",
                    FilePath = "unit-test-file-path",
                    FileName ="unit-test-file-name",
                    DateLastUpdated = DateTime.Now,
                    PassageKey = "unit-test-stim-key",
                    TestVersion = 999L
                }
            };

            testPackageDao.Insert(stimulusList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(stimulusList[0], insertedRecords[0]);
        }
    }
}
