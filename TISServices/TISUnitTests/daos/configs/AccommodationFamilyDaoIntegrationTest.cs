using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class AccommodationFamilyDAOIntegrationTest : TestPackageDaoIntegrationTestBase<AccommodationFamilyDTO>
    {
        private readonly ITestPackageDao<AccommodationFamilyDTO> testPackageDao = new AccommodationFamilyDAO();
        private readonly string sql =
            "SELECT " +
            "   clientname, " +
            "   family, " +
            "   label " +
            "FROM " +
            "   Client_AccommodationFamily " +
            "WHERE " +
            "   clientname = 'unit-test'" +
            "   AND family = 'unit-test-family'";

        [TestMethod]
        public void ShouldSaveAccommodationFamilyrecord()
        {
            var accommodationFamilyList = new List<AccommodationFamilyDTO>
            {
               new AccommodationFamilyDTO
               {
                   ClientName = "unit-test",
                   Family = "unit-test-family",
                   Label = "unit-test-label"
               }
            };

            testPackageDao.Insert(accommodationFamilyList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            var result = insertedRecords[0];
            Assert.AreEqual(accommodationFamilyList[0].ClientName, result.ClientName);
            Assert.AreEqual(accommodationFamilyList[0].Family, result.Family);
            Assert.AreEqual(accommodationFamilyList[0].Label, result.Label);
        }
    }
}
