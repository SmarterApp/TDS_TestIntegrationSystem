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
    public class ItemScoreDimensionDAOIntegrationTest : TestPackageDaoIntegrationTestBase<ItemScoreDimensionDTO>
    {
        private const long SBAC_PT_CLIENT_ID = 2L;
        private const string TEST_ADMIN_KEY = "unit-test-admin-key";
        private const string SEGMENT_KEY = "unit-test-segment-key";

        //private readonly Guid ITEM_SCORE_DIMENSION_KEY = new Guid();
        private readonly ITestPackageDao<ItemScoreDimensionDTO> testPackageDao = new ItemScoreDimensionDAO();
        private readonly string sql =
            "SELECT \n" +
                "   _Key AS ItemScoreDimensionKey, \n" +
                "   _fk_Item AS ItemKey, \n" +
                "   _fk_AdminSubject AS SegmentKey, \n" +
                "   Dimension, \n" +
                "   ScorePoints, \n" +
                "   [Weight], \n" +
                "   _fk_MeasurementModel AS MeasurementModel, \n" +
                "   RecodeRule \n" +
            "FROM \n" +
            "   ItemScoreDimension \n" +
            "WHERE \n" +
            "   _Key = '{0}'";

        private readonly TestAdminDTO seedDataTestAdmin = new TestAdminDTO
        {
            AdminKey = TEST_ADMIN_KEY,
            SchoolYear = "test-year",
            Season = "season",
            ClientKey = SBAC_PT_CLIENT_ID,
            Description = "unit-test-desc",
            TestVersion = 42L
        };
        private readonly SetOfAdminSubjectDTO seedDataAdminSubject = new SetOfAdminSubjectDTO
        {
            SegmentKey = SEGMENT_KEY,
            TestAdminKey = TEST_ADMIN_KEY,
            SubjectKey = "unit-test-subject",
            TestId = "unit-test-test-id",
            StartAbility = 13.0D,
            StartInfo = 14.0D,
            MinItems = 1,
            MaxItems = 5,
            Slope = 99.0D,
            Intercept = 45.0D,
            FieldTestStartPosition = 5,
            FieldTestEndPosition = 10,
            FieldTestMinItems = 15,
            FieldTestMaxItems = 20,
            SelectionAlgorithm = "unit-test-alg",
            BlueprintWeight = 25.5D,
            CSet1Size = 50,
            CSet2Random = 51,
            CSet2InitialRandom = 52,
            VirtualTest = "unit-test-virtual",
            TestPosition = 53,
            IsSegmented = true,
            ItemWeight = 30.5D,
            AbilityOffset = 35.5D,
            CSet1Order = "unit-test-cset1-order",
            TestVersion = 999L,
            Contract = "unit-test-contract",
            TestType = "unit-test-test-type",
            PrecisionTarget = 44.75D,
            AdaptiveCut = 54.54D,
            TooCloseSEs = 99.99D,
            AbilityWeight = 100.25D,
            ComputeAbilityEstimates = true,
            RcAbilityWeight = 59.999D,
            PrecisionTargetMetWeight = 48.48D,
            PrecisionTargetNotMetWeight = 94.94D,
            TerminationOverallInfo = true,
            TerminationRcInfo = true,
            TerminationMinCount = true,
            TerminationTooClose = true,
            TerminationFlagsAnd = true,
            BlueprintMetricFunction = "unit-test-bp-func"
        };

        /// <summary>
        /// Create seed data to satisfy foreign key constraints
        /// </summary>
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            new TestAdminDAO().Insert(new List<TestAdminDTO> { seedDataTestAdmin });
            new SetOfAdminSubjectDAO().Insert(new List<SetOfAdminSubjectDTO> { seedDataAdminSubject });
        }

        [TestMethod]
        public void ShouldSaveAnItemScoreDimensionRecord()
        {
            var isdList = new List<ItemScoreDimensionDTO>
            {
                new ItemScoreDimensionDTO
                {
                    ItemScoreDimensionKey = Guid.NewGuid(),
                    ItemKey = "unit-test-item-key",
                    SegmentKey = SEGMENT_KEY,
                    Dimension = "unit-test-dimension",
                    ScorePoints = 5,
                    Weight = 40D,
                    MeasurementModel = 10,
                    RecodeRule = "unit-test-recode-rule"
                }
            };

            testPackageDao.Insert(isdList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, isdList[0].ItemScoreDimensionKey), DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(isdList[0], insertedRecords[0]);
        }
    }
}
