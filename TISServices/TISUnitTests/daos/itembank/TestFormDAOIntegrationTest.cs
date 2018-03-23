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

        [TestMethod]
        public void ShouldSaveATestFormRecord()
        {
            new TestAdminDAO().Insert(new List<TestAdminDTO> { seedDataTestAdmin });
            new SetOfAdminSubjectDAO().Insert(new List<SetOfAdminSubjectDTO> { seedDataSetOfAdminSubject });

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

        [TestMethod]
        public void ShouldGetTestFormDtosForAMultiSegmentedAssessment()
        {
            // Assessment with two segments.  Each segment has two forms.
            var assessmentKey = "(SBAC_PT)SBAC-Perf-ELA-11-Fall-2017-2018";

            var result = testPackageDao.Find(assessmentKey);

            Assert.AreEqual(4, result.Count);

            var firstForm = result[0];
            Assert.AreEqual("(SBAC_PT)SBAC-Perf-S1-ELA-11-Fall-2017-2018", firstForm.SegmentKey);
            Assert.AreEqual(1033L, firstForm.ITSBankKey);
            Assert.AreEqual(1033L, firstForm.ITSKey);
            Assert.AreEqual("187-1033", firstForm.TestFormKey);
            Assert.AreEqual("PracTest::ELAG11::Perf::S1::FA17::ENU", firstForm.FormId);
            Assert.AreEqual("ENU", firstForm.Language);
            Assert.AreEqual(12516L, firstForm.TestVersion);
            Assert.AreEqual("Default", firstForm.Cohort);

            var secondForm = result[1];
            Assert.AreEqual("(SBAC_PT)SBAC-Perf-S1-ELA-11-Fall-2017-2018", secondForm.SegmentKey);
            Assert.AreEqual(1034L, secondForm.ITSBankKey);
            Assert.AreEqual(1034L, secondForm.ITSKey);
            Assert.AreEqual("187-1034", secondForm.TestFormKey);
            Assert.AreEqual("PracTest::ELAG11::Perf::S1::FA17::BRL", secondForm.FormId);
            Assert.AreEqual("ENU-Braille", secondForm.Language);
            Assert.AreEqual(12516L, secondForm.TestVersion);
            Assert.AreEqual("Default", secondForm.Cohort);

            var thirdForm = result[2];
            Assert.AreEqual("(SBAC_PT)SBAC-Perf-S2-ELA-11-Fall-2017-2018", thirdForm.SegmentKey);
            Assert.AreEqual(1035L, thirdForm.ITSBankKey);
            Assert.AreEqual(1035L, thirdForm.ITSKey);
            Assert.AreEqual("187-1035", thirdForm.TestFormKey);
            Assert.AreEqual("PracTest::ELAG11::Perf::S2::FA17::ENU", thirdForm.FormId);
            Assert.AreEqual("ENU", thirdForm.Language);
            Assert.AreEqual(12516L, thirdForm.TestVersion);
            Assert.AreEqual("Default", thirdForm.Cohort);

            var fourthForm = result[3];
            Assert.AreEqual("(SBAC_PT)SBAC-Perf-S2-ELA-11-Fall-2017-2018", fourthForm.SegmentKey);
            Assert.AreEqual(1036L, fourthForm.ITSBankKey);
            Assert.AreEqual(1036L, fourthForm.ITSKey);
            Assert.AreEqual("187-1036", fourthForm.TestFormKey);
            Assert.AreEqual("PracTest::ELAG11::Perf::S2::FA17::BRL", fourthForm.FormId);
            Assert.AreEqual("ENU-Braille", fourthForm.Language);
            Assert.AreEqual(12516L, fourthForm.TestVersion);
            Assert.AreEqual("Default", fourthForm.Cohort);
        }
    }
}
