using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;

namespace TISUnitTests.daos.configs
{
    [TestClass]
    public class SegmentPropertiesDAOIntegrationTest : utils.TestPackageDaoIntegrationTestBase<SegmentPropertiesDTO>
    {
        private readonly ITestPackageDao<SegmentPropertiesDTO> testPackageDao = new SegmentPropertiesDAO();
        private readonly string sql =
            "SELECT \n" +
            "   ClientName, " +
            "   SegmentId, " +
            "   SegmentPosition, " +
            "   ParentTest, " +
            "   IsPermeable, " +
            "   EntryApproval, " +
            "   ExitApproval, " +
            "   ItemReview, " +
            "   Label, " +
            "   ModeKey " +
            "FROM " +
            "   Client_SegmentProperties " +
            "WHERE " +
            "   ClientName = 'unit-test-client' " +
            "   AND SegmentId = 'unit-test-segment-id' " +
            "   AND ParentTest = 'unit-test-parent-test' ";

        [TestMethod]
        public void ShouldSaveASegmentPropertiesRecord()
        {
            var segmentPropertiesList = new List<SegmentPropertiesDTO>
            {
                new SegmentPropertiesDTO {
                    ClientName = "unit-test-client",
                    SegmentId = "unit-test-segment-id",
                    SegmentPosition = 1,
                    ParentTest = "unit-test-parent-test",
                    IsPermeable = 1,
                    EntryApproval = 13,
                    ExitApproval = 42,
                    ItemReview = true,
                    Label = "unit-test-label",
                    ModeKey = "unit-test-modekey"
                }
            };

            testPackageDao.Insert(segmentPropertiesList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.CONFIGS);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(segmentPropertiesList[0], insertedRecords[0]);
        }
    }
}
