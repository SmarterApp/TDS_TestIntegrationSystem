using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class StrandDAOIntegrationTest : TestPackageDaoIntegrationTestBase<StrandDTO>
    {
        private const long SBAC_PT_CLIENT_ID = 2L;

        private readonly ITestPackageDao<StrandDTO> testPackageDao = new StrandDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _fk_subject AS SubjectKey, \n" +
            "   [Name], \n" +
            "   _fk_parent AS ParentId, \n" +
            "   _key AS BlueprintElementId, \n" +
            "   _fk_client AS ClientKey, \n" +
            "   TreeLevel, \n" +
            "   loadconfig AS TestVersion \n" +
            "FROM \n" +
            "   tblStrand \n" +
            "WHERE \n" +
            "   _key = 'unit-test-strand-key'";

        private readonly SubjectDTO seedDataSubject = new SubjectDTO
        {
            Name = "unit-test-name",
            Grade = "unit-test-grade",
            SubjectKey = "unit-test-subject-key",
            ClientKey = SBAC_PT_CLIENT_ID,
            TestVersion = 42L
        };

        /// <summary>
        /// Create seed data to satisfy foreign key constraints
        /// </summary>
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            new SubjectDAO().Insert(new List<SubjectDTO> { seedDataSubject });
        }

        [TestMethod]
        public void ShouldSaveAStrandRecord()
        {
            var strandList = new List<StrandDTO>
            {
                new StrandDTO
                {
                    SubjectKey = seedDataSubject.SubjectKey,
                    Name = "unit-test-name",
                    ParentId = "unit-test-parent-id",
                    BlueprintElementId = "unit-test-strand-key",
                    ClientKey = SBAC_PT_CLIENT_ID,
                    TreeLevel = 42,
                    TestVersion = 99L
                }
            };

            testPackageDao.Insert(strandList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(strandList[0], insertedRecords[0]);
        }
    }
}
