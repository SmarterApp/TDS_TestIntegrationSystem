using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class TestFormDAOIntegrationTest : TestPackageDaoIntegrationTestBase<TestFormDTO>
    {
        private const long SBAC_PT_CLIENT_ID = 2L;
        private const string TEST_ADMIN_KEY = "unit-test-admin-key";

        private readonly ITestPackageDao<TestFormDTO> testPackageDao = new TestFormDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _fk_adminsubject AS SegmentKey, \n" +
            "   Cohort, \n" +
            "   [Language], \n" +
            "   _key AS TestFormKey, \n" +
            "   FormId, \n" +
            "   _efk_ITSBank AS ITSBankKey, \n" +
            "   _efk_ITSKey AS ITSKey, \n" +
            "   LoadConfig AS TestVersion \n" +
            "FROM \n" +
            "   TestForm \n" +
            "WHERE \n" +
            "   _key = 'unit-test-form-key'";

        private readonly TestAdminDTO seedDataTestAdmin = new TestAdminDTO
        {
            AdminKey = TEST_ADMIN_KEY,
            SchoolYear = "test-year",
            Season = "season",
            ClientKey = SBAC_PT_CLIENT_ID,
            Description = "unit-test-desc",
            TestVersion = 42L
        };
        private readonly SetOfAdminSubjectDTO seedDataSetOfAdminSubject = new SetOfAdminSubjectDTO
        {
            SegmentKey = "unit-test-segment-key",
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
            new SetOfAdminSubjectDAO().Insert(new List<SetOfAdminSubjectDTO> { seedDataSetOfAdminSubject });
        }

        [TestMethod]
        public void ShouldSaveATestFormRecord()
        {
            var formList = new List<TestFormDTO>
            {
                new TestFormDTO
                {
                    SegmentKey = seedDataSetOfAdminSubject.SegmentKey,
                    Cohort = "unit-test-cohort",
                    Language = "unit-test-lang",
                    TestFormKey = "unit-test-form-key",
                    FormId = "unit-test-form-id",
                    ITSBankKey = 13L,
                    ITSKey = 42L,
                    TestVersion = 99L
                }
            };

            testPackageDao.Insert(formList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(formList[0], insertedRecords[0]);
        }
    }
}
