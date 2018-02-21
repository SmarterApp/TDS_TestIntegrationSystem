using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

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
            var result = insertedRecords[0];
            Assert.AreEqual(segmentPropertiesList[0].ClientName, result.ClientName);
            Assert.AreEqual(segmentPropertiesList[0].SegmentId, result.SegmentId);
            Assert.AreEqual(segmentPropertiesList[0].SegmentPosition, result.SegmentPosition);
            Assert.AreEqual(segmentPropertiesList[0].ParentTest, result.ParentTest);
            Assert.AreEqual(segmentPropertiesList[0].IsPermeable, result.IsPermeable);
            Assert.AreEqual(segmentPropertiesList[0].EntryApproval, result.EntryApproval);
            Assert.AreEqual(segmentPropertiesList[0].ExitApproval, result.ExitApproval);
            Assert.AreEqual(segmentPropertiesList[0].ItemReview, result.ItemReview);
            Assert.AreEqual(segmentPropertiesList[0].Label, result.Label);
            Assert.AreEqual(segmentPropertiesList[0].ModeKey, result.ModeKey);
        }
    }
}
