using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class SubjectDAOIntegrationTest : TestPackageDaoIntegrationTestBase<SubjectDTO>
    {
        private readonly ITestPackageDao<SubjectDTO> testPackageDao = new SubjectDAO();
        private readonly string sql =
            "SELECT " +
            "   ClientName, " +
            "   [Subject], " +
            "   SubjectCode " +
            "FROM " +
            "   Client_Subject " +
            "WHERE " +
            "   ClientName = 'unit-test-client' " +
            "   AND [Subject] = 'unit-test-subject'";

        [TestMethod]
        public void ShouldSaveASubjectRecord()
        {
            var subjectList = new List<SubjectDTO>
            {
                new SubjectDTO
                {
                    ClientName = "unit-test-client",
                    Subject = "unit-test-subject",
                    SubjectCode = "unit-test-subject-code"
                }
            };

            testPackageDao.Insert(subjectList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(subjectList[0], insertedRecords[0]);
        }
    }
}
