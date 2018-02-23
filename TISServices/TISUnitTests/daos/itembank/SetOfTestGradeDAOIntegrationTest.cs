using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TISUnitTests.utils;

namespace TISUnitTests.daos.itembank
{
    [TestClass]
    public class SetOfTestGradeDAOIntegrationTest : TestPackageDaoIntegrationTestBase<SetOfTestGradeDTO>
    {
        private const long SBAC_PT_CLIENT_ID = 2L;
        private const string TEST_ADMIN_KEY = "unit-test-admin-key";
        private const string SEGMENT_KEY = "unit-test-segment-key";

        private readonly ITestPackageDao<SetOfTestGradeDTO> testPackageDao = new SetOfTestGradeDAO();
        private readonly string sql =
            "SELECT \n" +
            "   TestId, \n" +
            "   Grade, \n" +
            "   RequireEnrollment, \n" +
            "   _fk_adminsubject AS SegmentKey, \n" +
            "   EnrolledSubject \n" +
            "FROM \n" +
            "   SetOfTestGrades \n" +
            "WHERE \n" +
            "   _fk_adminsubject = 'unit-test-segment-key' \n" +
            "   AND testid = 'unit-test-test-id' \n" +
            "   AND grade = 'unit-test-grade'";

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
        public void ShouldSaveASetOfTestGradeRecord()
        {
            var gradeList = new List<SetOfTestGradeDTO>
            {
                new SetOfTestGradeDTO
                {
                    TestId = "unit-test-test-id",
                    Grade = "unit-test-grade",
                    RequireEnrollment = true,
                    SegmentKey = "unit-test-segment-key",
                    EnrolledSubject = "unit-test-enrolled-subject"
                }
            };

            testPackageDao.Insert(gradeList);

            var insertedRecords = GetInsertedRecords(sql, DatabaseConnectionStringNames.ITEMBANK);

            Assert.AreEqual(1, insertedRecords.Count);
            CompareResults(gradeList[0], insertedRecords[0]);
        }
    }
}
