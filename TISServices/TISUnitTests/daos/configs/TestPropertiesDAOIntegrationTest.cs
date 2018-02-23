using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class TestPropertiesDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestPropertiesDTO>
    {
        private readonly ITestPackageDao<TestPropertiesDTO> testPackageDao = new TestPropertiesDAO();
        private readonly string sql =
            "SELECT \n" +
            "   ClientName, \n" +
            "   TestId, \n" +
            "   IsSelectable, \n" +
            "   Label, \n" +
            "   SubjectName, \n" +
            "   MaxOpportunities, \n" +
            "   ScoreByTds, \n" +
            "   AccommodationFamily, \n" +
            "   ReportingInstrument, \n" +
            "   TIDE_ID AS TideId, \n" +
            "   GradeText \n" +
            "FROM \n" +
            "   Client_TestProperties \n" +
            "WHERE \n" +
            "   ClientName = 'unit-test-client-name' \n" +
            "   AND TestId = 'unit-test-test-id'";

        [TestMethod]
        public void ShouldSaveATestPropertyRecord()
        {
            var testPropertyList = new List<TestPropertiesDTO>
            {
                new TestPropertiesDTO
                {
                    ClientName = "unit-test-client-name",
                    TestId = "unit-test-test-id",
                    IsSelectable = true,
                    Label = "unit-test-label",
                    SubjectName = "unit-test-subject-name",
                    MaxOpportunities = 13,
                    ScoreByTds = true,
                    AccommodationFamily = "unit-test-family",
                    ReportingInstrument = "unit-test-report-instrument",
                    TideId = "unit-test-tide-id",
                    GradeText = "unit-test-grade-text"
                }
            };

            testPackageDao.Insert(testPropertyList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(testPropertyList[0], insertedRecords[0]);
        }
    }
}
