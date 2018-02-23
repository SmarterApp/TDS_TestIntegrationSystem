using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class ItemSelectionParmDAOIntegrationTest : TestPackageDaoIntegrationTestBase<ItemSelectionParmDTO>
    {
        private const long SBAC_PT_CLIENT_ID = 2L;
        private const string TEST_ADMIN_KEY = "unit-test-admin-key";
        private const string SEGMENT_KEY = "unit-test-segment-key";

        private readonly ITestPackageDao<ItemSelectionParmDTO> testPackageDao = new ItemSelectionParmDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _fk_AdminSubject AS SegmentKey, \n" +
            "   bpElementID AS BlueprintElementID, \n" +
            "   [name] AS PropertyName, \n" +
            "   [value] AS PropertyValue, \n" +
            "   label AS PropertyLabel \n" +
            "FROM \n" +
            "   tblItemSelectionParm \n" +
            "WHERE \n" +
            "   _fk_adminsubject = '{0}' \n" +
            "   AND bpElementID = 'unit-test-bp-id'" +
            "   AND [name] = 'unit-test-prop-name'";

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
        public void ShouldSaveAnItemSelectionParmRecord()
        {
            var parmList = new List<ItemSelectionParmDTO>
            {
                new ItemSelectionParmDTO
                {
                    SegmentKey = SEGMENT_KEY,
                    BlueprintElementId = "unit-test-bp-id",
                    PropertyName = "unit-test-prop-name",
                    PropertyValue = "unit-test-prop-value",
                    PropertyLabel = "unit-test-prop-label"
                }
            };

            testPackageDao.Insert(parmList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, SEGMENT_KEY), DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(parmList[0], insertedRecords[0]);
        }
    }
}
