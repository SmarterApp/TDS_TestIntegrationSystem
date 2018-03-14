using System;
using System.Collections.Generic;
using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.Extensions;
using TDSQASystemAPI.TestPackage;
using TDSQASystemAPI.TestPackage.utils;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    public class ItembankAdministrationDataService : IItembankAdministrationDataService
    {
        private readonly IItembankConfigurationDataQueryService itembankConfigurationDataQueryService;
        private readonly ITestPackageDao<TestAdminDTO> testAdminDao;
        private readonly ITestPackageDao<SetOfAdminSubjectDTO> setOfAdminSubjectDao;

        public ItembankAdministrationDataService()
        {
            setOfAdminSubjectDao = new SetOfAdminSubjectDAO();
            testAdminDao = new TestAdminDAO();
            itembankConfigurationDataQueryService = 
                new ItembankConfigurationDataQueryService(new SubjectDAO(), new ClientDAO(), testAdminDao);
            
        }

        public ItembankAdministrationDataService(IItembankConfigurationDataQueryService itembankConfigurationDataQueryService,
                                                 ITestPackageDao<TestAdminDTO> testAdminDao,
                                                 ITestPackageDao<SetOfAdminSubjectDTO> setOfAdminSubjectDao)
        {
            this.itembankConfigurationDataQueryService = itembankConfigurationDataQueryService;
            this.testAdminDao = testAdminDao;
            this.setOfAdminSubjectDao = setOfAdminSubjectDao;
        }

        public void CreateSetOfAdminSubjects(TestPackage.TestPackage testPackage)
        {
            var setOfAdminSubjectsList = new List<SetOfAdminSubjectDTO>();

            foreach (var assessment in testPackage.Assessment)
            {
                var segmentAdminSubjectDtos = 
                    from segment in assessment.Segments
                    let segBp = segment.SegmentBlueprint.First(b => b.idRef.Equals(segment.id))
                    let itemSelectionProperties = segBp.ItemSelection.ToDictionary(isp => isp.name.ToLower().Trim(), isp => isp.value.Trim())
                    select new SetOfAdminSubjectDTO
                    {
                        SegmentKey = segment.Key,
                        TestAdminKey = testPackage.publisher,
                        SubjectKey = testPackage.GetSubjectKey(),
                        TestId = segment.id,
                        StartAbility = double.Parse(itemSelectionProperties.GetOrDefault("startability", ItemSelectionDefaults.START_ABILITY.ToString())),
                        StartInfo = double.Parse(itemSelectionProperties.GetOrDefault("startinfo", ItemSelectionDefaults.START_INFO.ToString())),
                        MinItems = segBp.maxExamItems,
                        MaxItems = segBp.maxExamItems,
                        Slope = double.Parse(itemSelectionProperties.GetOrDefault("slope", ItemSelectionDefaults.SLOPE.ToString())),
                        Intercept = double.Parse(itemSelectionProperties.GetOrDefault("intercept", ItemSelectionDefaults.INTERCEPT.ToString())),
                        FieldTestStartPosition = itemSelectionProperties.ContainsKey("ftstartpos") ? itemSelectionProperties["ftstartpos"]?.ToNullableInt() : null,
                        FieldTestEndPosition = itemSelectionProperties.ContainsKey("ftendpos") ? itemSelectionProperties["ftendpos"]?.ToNullableInt() : null,
                        FieldTestMinItems = segBp.minFieldTestItems,
                        FieldTestMaxItems = segBp.maxFieldTestItems,
                        SelectionAlgorithm = SelectionAlgorithmTypes.GetSelectionAlgorithm(segment.algorithmType),
                        BlueprintWeight = double.Parse(itemSelectionProperties.GetOrDefault("bpweight", ItemSelectionDefaults.BLUEPRINT_WEIGHT.ToString())),
                        AbilityWeight = double.Parse(itemSelectionProperties.GetOrDefault("abilityweight", ItemSelectionDefaults.ABILITY_WEIGHT.ToString())),
                        CSet1Size = int.Parse(itemSelectionProperties.GetOrDefault("cset1size", ItemSelectionDefaults.CSET1_SIZE.ToString())),
                        CSet2Random = int.Parse(itemSelectionProperties.GetOrDefault("cset2random", ItemSelectionDefaults.CSET2_RANDOM.ToString())),
                        CSet2InitialRandom = int.Parse(itemSelectionProperties.GetOrDefault("cset2initialrandom", ItemSelectionDefaults.CSET2_INITIAL_RANDOM.ToString())),
                        VirtualTest = assessment.Key,
                        TestPosition = segment.position,
                        IsSegmented = false,
                        ComputeAbilityEstimates = bool.Parse(itemSelectionProperties.GetOrDefault("computeabilityestimates", ItemSelectionDefaults.COMPUTE_ABILITY_ESTIMATES.ToString())),
                        TestVersion = (long)testPackage.version,
                        ItemWeight = double.Parse(itemSelectionProperties.GetOrDefault("itemweight", ItemSelectionDefaults.ITEM_WEIGHT.ToString())),
                        AbilityOffset = double.Parse(itemSelectionProperties.GetOrDefault("abilityoffset", ItemSelectionDefaults.ABILITY_OFFSET.ToString())),
                        CSet1Order = itemSelectionProperties.GetOrDefault("cset1order", ItemSelectionDefaults.CSET1_ORDER),
                        RcAbilityWeight = double.Parse(itemSelectionProperties.GetOrDefault("rcabilityweight", ItemSelectionDefaults.RC_ABILITY_WEIGHT.ToString())),
                        PrecisionTarget = itemSelectionProperties.ContainsKey("precisiontarget") ? itemSelectionProperties["precisiontarget"]?.ToNullableDouble() : null,
                        PrecisionTargetMetWeight = double.Parse(itemSelectionProperties.GetOrDefault("precisiontargetmetweight", ItemSelectionDefaults.PRECISION_TARGET_MET_WEIGHT.ToString())),
                        PrecisionTargetNotMetWeight = double.Parse(itemSelectionProperties.GetOrDefault("precisiontargetnotmetweight", ItemSelectionDefaults.PRECISION_TARGET_NOT_MET_WEIGHT.ToString())),
                        AdaptiveCut = itemSelectionProperties.ContainsKey("adaptivecut") ? itemSelectionProperties["adaptivecut"]?.ToNullableDouble() : null,
                        TooCloseSEs = itemSelectionProperties.ContainsKey("toocloseses") ? itemSelectionProperties["toocloseses"]?.ToNullableDouble() : null,
                        TerminationOverallInfo = bool.Parse(itemSelectionProperties.GetOrDefault("terminationoverallinfo", ItemSelectionDefaults.TERMINATION_FLAGS.ToString())),
                        TerminationMinCount = bool.Parse(itemSelectionProperties.GetOrDefault("terminationmincount", ItemSelectionDefaults.TERMINATION_FLAGS.ToString())),
                        TerminationTooClose = bool.Parse(itemSelectionProperties.GetOrDefault("terminationtooclose", ItemSelectionDefaults.TERMINATION_FLAGS.ToString())),
                        TerminationFlagsAnd = bool.Parse(itemSelectionProperties.GetOrDefault("terminationflagsand", ItemSelectionDefaults.TERMINATION_FLAGS.ToString())),
                        BlueprintMetricFunction = ItemSelectionDefaults.BLUEPRINT_METRIC_FUNCTION,
                        TestType = testPackage.type
                    };

                setOfAdminSubjectsList.AddRange(segmentAdminSubjectDtos);

                // If this is a multi-segment assessment, create the "virtual" parent assessment.
                if (assessment.IsSegmented())
                {
                    // Many of the properties in the "virtual" row will contain sums of its segments. For example, if seg1 has
                    // a "minitems" value of 3, and "maxitems" value of 4, and seg2 has a "minitems" value of 1 and a "maxitems" value of 1,
                    // the "virtual" row will contain minitems = 4 and maxitems = 5
                    var itemCounts = (from s in assessment.Segments
                                     from segBp in s.SegmentBlueprint
                                     where segBp.idRef.Equals(s.id)
                                     group segBp by 1 into g
                                     select new
                                     {
                                         MinItemCount = g.Sum(x => x.minExamItems),
                                         MaxItemCount = g.Sum(x => x.maxExamItems),
                                         MinFieldTestItemCount = g.Sum(x => x.minFieldTestItems),
                                         MaxFieldTestItemCount = g.Sum(x => x.maxFieldTestItems)
                                     }).First();

                    var virtualTestDto = new SetOfAdminSubjectDTO
                    {
                        SegmentKey = assessment.Key,
                        TestAdminKey = testPackage.publisher,
                        SubjectKey = testPackage.GetSubjectKey(),
                        TestId = assessment.id,
                        StartAbility = ItemSelectionDefaults.START_ABILITY,
                        StartInfo = ItemSelectionDefaults.START_INFO,
                        MinItems = itemCounts.MinItemCount,
                        MaxItems = itemCounts.MaxItemCount,
                        Slope = ItemSelectionDefaults.SLOPE,
                        Intercept = ItemSelectionDefaults.INTERCEPT,
                        FieldTestStartPosition = null,
                        FieldTestEndPosition = null,
                        FieldTestMinItems = itemCounts.MinFieldTestItemCount,
                        FieldTestMaxItems = itemCounts.MaxFieldTestItemCount,
                        SelectionAlgorithm = SelectionAlgorithmTypes.VIRTUAL,
                        BlueprintWeight = ItemSelectionDefaults.BLUEPRINT_WEIGHT,
                        AbilityWeight = ItemSelectionDefaults.ABILITY_WEIGHT,
                        CSet1Size = ItemSelectionDefaults.CSET1_SIZE,
                        CSet2Random = ItemSelectionDefaults.CSET2_RANDOM,
                        CSet2InitialRandom = ItemSelectionDefaults.CSET2_INITIAL_RANDOM,
                        IsSegmented = true,
                        ComputeAbilityEstimates = ItemSelectionDefaults.COMPUTE_ABILITY_ESTIMATES,
                        TestVersion = (long)testPackage.version,
                        ItemWeight = ItemSelectionDefaults.ITEM_WEIGHT,
                        AbilityOffset = ItemSelectionDefaults.ABILITY_OFFSET,
                        CSet1Order = ItemSelectionDefaults.CSET1_ORDER,
                        RcAbilityWeight = ItemSelectionDefaults.RC_ABILITY_WEIGHT,
                        PrecisionTarget = null,
                        PrecisionTargetMetWeight = ItemSelectionDefaults.PRECISION_TARGET_MET_WEIGHT,
                        PrecisionTargetNotMetWeight = ItemSelectionDefaults.PRECISION_TARGET_NOT_MET_WEIGHT,
                        AdaptiveCut = null,
                        TooCloseSEs = null,
                        TerminationOverallInfo = ItemSelectionDefaults.TERMINATION_FLAGS,
                        TerminationMinCount = ItemSelectionDefaults.TERMINATION_FLAGS,
                        TerminationTooClose = ItemSelectionDefaults.TERMINATION_FLAGS,
                        TerminationFlagsAnd = ItemSelectionDefaults.TERMINATION_FLAGS,
                        BlueprintMetricFunction = ItemSelectionDefaults.BLUEPRINT_METRIC_FUNCTION,
                        TestType = testPackage.type
                    };

                    setOfAdminSubjectsList.Add(virtualTestDto);
                }
            }

            setOfAdminSubjectDao.Insert(setOfAdminSubjectsList);
        }

        public void SaveTestAdministration(TestPackage.TestPackage testPackage)
        {
            var client = itembankConfigurationDataQueryService.FindClientByName(testPackage.publisher);
            if (client == null)
            {
                throw new InvalidOperationException(string.Format("Could not find client for name '{0}'", testPackage.publisher));
            }

            var existingTestAdmin = itembankConfigurationDataQueryService.FindTestAdmin(client.Name);
            var newTestAdmin = new TestAdminDTO
            {
                AdminKey = existingTestAdmin?.AdminKey ?? client.Name,
                ClientKey = existingTestAdmin?.ClientKey ?? client.ClientKey,
                SchoolYear = existingTestAdmin?.SchoolYear ?? testPackage.academicYear,
                Description = existingTestAdmin?.Description ?? string.Format("{0} {1} {2} Administration", client.Name, existingTestAdmin?.Season ?? string.Empty, testPackage.academicYear),
                TestVersion = (long)testPackage.version // whether inserting or updating, we want the version to reflect the testpackage being loaded
            };

            if (existingTestAdmin == null)
            {
                testAdminDao.Insert(new List<TestAdminDTO> { newTestAdmin });
            }
            else
            {
                testAdminDao.Update(newTestAdmin);
            }
        }
    }
}
