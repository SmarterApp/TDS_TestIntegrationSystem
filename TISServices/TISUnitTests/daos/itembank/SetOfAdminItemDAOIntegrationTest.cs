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
    public class SetOfAdminItemDAOIntegrationTest : TestPackageDaoIntegrationTestBase<SetOfAdminItemDTO>
    {
        private const long SBAC_PT_CLIENT_ID = 2L;
        private const string TEST_ADMIN_KEY = "unit-test-admin-key";
        private const string SEGMENT_KEY = "unit-test-segment-key";
        private const string ITEM_KEY = "999-999";

        private readonly ITestPackageDao<SetOfAdminItemDTO> testPackageDao = new SetOfAdminItemDAO();

        // The BVector isn't inserted as a new record, but is updated by another process, but needs to be returned here so the populator will work.  
        // If it is missing, the populator will complain that this field wasn't returned in the query.
        private readonly string sql =
            "SELECT \n" +
            "   _fk_item AS ItemKey, \n" +
            "   _fk_adminsubject AS SegmentKey, \n" +
            "   loadconfig AS TestVersion, \n" +
            "   _fk_strand AS StrandKey, \n" +
            "   _fk_testadmin AS TestAdminKey, \n" +
            "   GroupId, \n" +
            "   ItemPosition, \n" +
            "   IsFieldTest, \n" +
            "   IsActive, \n" +
            "   BlockId, \n" +
            "   IsRequired, \n" +
            "   GroupKey, \n" +
            "   StrandName, \n" +
            "   irt_a AS IrtA, \n" +
            "   irt_b AS IrtB, \n" +
            "   irt_c AS IrtC, \n" +
            "   irt_model AS IrtModel, \n" +
            "   ClString, \n" +
            "   updateconfig AS UpdatedTestVersion, \n" +
            "   'unit-test-b-vector' AS BVector " + 
            "FROM \n" +
            "   tblSetOfAdminItems \n" +
            "WHERE \n" +
            "   _fk_item = '{0}' \n" +
            "   AND _fk_adminsubject = '{1}' \n" +
            "   AND _fk_testadmin = '{2}'";

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
        private readonly ItemDTO seedDataItem = new ItemDTO
        {
            ItemBankKey = 999L,
            ItemKey = 999L,
            ItemType = "unit-test-item-type",
            FilePath = "unit-test-path",
            FileName = "unit-test-name",
            DateLastUpdated = DateTime.Now,
            Key = ITEM_KEY,
            ItemId = "unit-test-id",
            TestVersion = 42L
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
            new ItemDAO().Insert(new List<ItemDTO> { seedDataItem });
        }

        [TestMethod]
        public void ShouldSaveASetOfAdminItemRecord()
        {
            var itemList = new List<SetOfAdminItemDTO>
            {
                new SetOfAdminItemDTO
                {
                    ItemKey = ITEM_KEY,
                    SegmentKey = SEGMENT_KEY,
                    TestVersion = 99L,
                    StrandKey = "unit-test-strand-key",
                    TestAdminKey = TEST_ADMIN_KEY,
                    GroupId = "unit-test-group-id",
                    ItemPosition = 1,
                    IsFieldTest = true,
                    IsActive = true,
                    BlockId = "block-id",
                    IsRequired = true,
                    GroupKey = "unit-test-group-key",
                    StrandName = "unit-test-strand-name",
                    IrtA = 2D,
                    IrtB = "unit-test-irt-b",
                    IrtC = 4D,
                    IrtModel = "unit-test-irt-model",
                    ClString = "unit-test-cl-string",
                    UpdatedTestVersion = 100L,
                    BVector = "unit-test-b-vector"
                }
            };

            testPackageDao.Insert(itemList);

            var insertedRecords = GetInsertedRecords(string.Format(sql, ITEM_KEY, SEGMENT_KEY, TEST_ADMIN_KEY), DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(itemList[0], insertedRecords[0]);
        }
    }
}
