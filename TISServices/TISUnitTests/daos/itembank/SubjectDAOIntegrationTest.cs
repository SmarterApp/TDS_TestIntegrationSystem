using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class SubjectDAOIntegrationTest : TestPackageDaoIntegrationTestBase<SubjectDTO>
    {
        private const long SBAC_PT_CLIENT_ID = 2L;
        private readonly ITestPackageDao<SubjectDTO> testPackageDao = new SubjectDAO();
        private readonly string sql =
            "SELECT \n" +
            "   [Name], \n" +
            "   Grade, \n" +
            "   _key AS SubjectKey, \n" +
            "   _fk_client AS ClientKey, \n" +
            "   loadconfig AS TestVersion \n" +
            "FROM \n" +
            "   tblSubject \n" +
            "WHERE \n" +
            "   _key = 'unit-test-subject-key'";

        [TestMethod]
        public void ShouldSaveASubjectRecord()
        {
            var subjectList = new List<SubjectDTO>
            {
                new SubjectDTO
                {
                    Name = "unit-test-name",
                    Grade = "unit-test-grade",
                    SubjectKey = "unit-test-subject-key",
                    ClientKey = SBAC_PT_CLIENT_ID,
                    TestVersion = 42L
                }
            };

            testPackageDao.Insert(subjectList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(subjectList[0], insertedRecords[0]);
        }
    }
}
