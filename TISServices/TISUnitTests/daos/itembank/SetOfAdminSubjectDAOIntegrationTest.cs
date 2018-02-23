using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class SetOfAdminSubjectDAOIntegrationTest : TestPackageDaoIntegrationTestBase<SetOfAdminSubjectDTO>
    {
        private const long SBAC_PT_CLIENT_ID = 2L;

        private readonly ITestPackageDao<SetOfAdminSubjectDTO> testPackageDao = new SetOfAdminSubjectDAO();
        private readonly string sql =
            "SELECT \n" +
            "   _Key AS SegmentKey, \n" +
            "   _fk_testadmin AS TestAdminKey, \n" +
            "   _fk_subject AS SubjectKey, \n" +
            "   TestId, \n" +
            "   StartAbility, \n" +
            "   StartInfo, \n" +
            "   MinItems, \n" +
            "   MaxItems, \n" +
            "   Slope, \n" +
            "   Intercept, \n" +
            "   ftstartpos AS FieldTestStartPosition, \n" +
            "   ftendpos AS FieldTestEndPosition, \n" +
            "   ftminitems AS FieldTestMinItems, \n" +
            "   ftmaxitems AS FieldTestMaxItems, \n" +
            "   SelectionAlgorithm, \n" +
            "   BlueprintWeight, \n" +
            "   CSet1Size, \n" +
            "   CSet2Random, \n" +
            "   CSet2InitialRandom, \n" +
            "   VirtualTest, \n" +
            "   TestPosition, \n" +
            "   IsSegmented, \n" +
            "   ItemWeight, \n" +
            "   AbilityOffset, \n" +
            "   CSet1Order, \n" +
            "   loadconfig AS TestVersion, \n" +
            "   [Contract], \n" +
            "   TestType, \n" +
            "   PrecisionTarget, \n" +
            "   AdaptiveCut, \n" +
            "   TooCloseSEs, \n" +
            "   AbilityWeight, \n" +
            "   ComputeAbilityEstimates, \n" +
            "   RcAbilityWeight, \n" +
            "   PrecisionTargetMetWeight, \n" +
            "   PrecisionTargetNotMetWeight, \n" +
            "   TerminationOverallInfo, \n" +
            "   TerminationRcInfo, \n" +
            "   TerminationMinCount, \n" +
            "   TerminationTooClose, \n" +
            "   TerminationFlagsAnd, \n" +
            "   bpmetricfunction AS BlueprintMetricFunction \n" +
            "FROM \n" +
            "   tblSetOfAdminSubjects \n" +
            "WHERE \n" +
            "   _key = 'unit-test-segment-key'";

        private readonly TestAdminDTO seedDataTestAdmin = new TestAdminDTO
        {
            AdminKey = "unit-test-admin-key",
            SchoolYear = "test-year",
            Season = "season",
            ClientKey = SBAC_PT_CLIENT_ID,
            Description = "unit-test-desc",
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
        }

        [TestMethod]
        public void ShouldSaveASetOfAdminSubjectsRecord()
        {
            var soasList = new List<SetOfAdminSubjectDTO>
            {
                new SetOfAdminSubjectDTO
                {
                    SegmentKey = "unit-test-segment-key",
                    TestAdminKey = seedDataTestAdmin.AdminKey,
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
                }
            };

            testPackageDao.Insert(soasList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);
            CompareResults(soasList[0], insertedRecords[0]);
        }
    }
}
