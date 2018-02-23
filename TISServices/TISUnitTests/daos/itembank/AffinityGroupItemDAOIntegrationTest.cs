using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class AffinityGroupItemDAOIntegrationTest : TestPackageDaoIntegrationTestBase<AffinityGroupItemDTO>
    {
        private const long SBAC_PT_CLIENT_ID = 2L;
        private const string TEST_ADMIN_KEY = "unit-test-admin-key";
        private const string SEGMENT_KEY = "unit-test-segment-key";

        private readonly ITestPackageDao<AffinityGroupItemDTO> testPackageDao = new AffinityGroupItemDAO();
        private readonly string sql =
            "SELECT" +
            "   _fk_adminsubject AS SegmentKey, \n" +
            "   GroupId, \n" +
            "   _fk_item AS ItemKey \n" +
            "FROM \n" +
            "   AffinityGroupItem \n" +
            "WHERE \n" +
            "   _fk_adminsubject = '{0}' \n" +
            "   AND groupid = 'unit-test-group-id' \n" +
            "   AND _fk_item = 'unit-test-item'";

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
        private readonly AffinityGroupDTO seedDataAffinityGroup = new AffinityGroupDTO
        {
            SegmentKey = "unit-test-segment-key",
            GroupId = "unit-test-group-id",
            MinItems = 1,
            MaxItems = 2,
            Weight = 5D,
            IsStrictMax = true,
            TestVersion = 99L,
            UpdatedTestVersion = 100L,
            AbilityWeight = 10D,
            PrecisionTarget = 15D,
            StartAbility = 20D,
            StartInfo = 25D,
            PrecisionTargetMetWeight = 30D,
            PrecisionTargetNotMetWeight = 35D
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
            new AffinityGroupDAO().Insert(new List<AffinityGroupDTO> { seedDataAffinityGroup });
        }

        [TestMethod]
        public void ShouldSaveAnAffinityGroupItemRecord()
        {
            var itemList = new List<AffinityGroupItemDTO>
            {
                new AffinityGroupItemDTO
                {
                    SegmentKey = SEGMENT_KEY,
                    GroupId = "unit-test-group-id",
                    ItemKey = "unit-test-item"
                }
            };

            testPackageDao.Insert(itemList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, SEGMENT_KEY), DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(itemList[0], insertedRecords[0]);
        }
    }
}
