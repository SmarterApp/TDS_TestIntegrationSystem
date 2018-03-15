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
        private readonly ITestPackageDao<AdminStrandDTO> adminStrandDao;
        private readonly ITestPackageDao<SetOfAdminItemDTO> setOfAdminItemDao;

        public ItembankAdministrationDataService()
        {
            setOfAdminSubjectDao = new SetOfAdminSubjectDAO();
            testAdminDao = new TestAdminDAO();
            adminStrandDao = new AdminStrandDAO();
            setOfAdminItemDao = new SetOfAdminItemDAO();
            itembankConfigurationDataQueryService = 
                new ItembankConfigurationDataQueryService(new SubjectDAO(), new ClientDAO(), testAdminDao);
            
        }

        public ItembankAdministrationDataService(IItembankConfigurationDataQueryService itembankConfigurationDataQueryService,
                                                 ITestPackageDao<TestAdminDTO> testAdminDao,
                                                 ITestPackageDao<SetOfAdminSubjectDTO> setOfAdminSubjectDao,
                                                 ITestPackageDao<AdminStrandDTO> adminStrandDao,
                                                 ITestPackageDao<SetOfAdminItemDTO> setOfAdminItemDao)
        {
            this.itembankConfigurationDataQueryService = itembankConfigurationDataQueryService;
            this.testAdminDao = testAdminDao;
            this.setOfAdminSubjectDao = setOfAdminSubjectDao;
            this.adminStrandDao = adminStrandDao;
            this.setOfAdminItemDao = setOfAdminItemDao;
        }

        public void CreateAdminStrands(TestPackage.TestPackage testPackge, IDictionary<string, StrandDTO> strandMap)
        {
            var segmentAdminStrandDtos =
                from assessment in testPackge.Assessment
                from segment in assessment.Segments
                from segBp in segment.SegmentBlueprint
                where BlueprintElementTypes.CLAIM_AND_TARGET_TYPES.Contains(strandMap[segBp.idRef].Type)
                let itemSelectionProperties = segBp.ItemSelection.ToDictionary(isp => isp.name.ToLower().Trim(), isp => isp.value.Trim())
                select new AdminStrandDTO
                {
                    AdminStrandKey = string.Format("{0}-{1}", segment.Key, strandMap[segBp.idRef].Key),
                    SegmentKey = segment.Key,
                    StrandKey = strandMap[segBp.idRef].Key,
                    MinItems = segBp.minExamItems,
                    MaxItems = segBp.maxExamItems,
                    AdaptiveCut = itemSelectionProperties.ContainsKey("adaptivecut") 
                        ? itemSelectionProperties["adaptivecut"]?.ToNullableDouble() 
                        : null,
                    StartAbility = itemSelectionProperties.ContainsKey("startability")
                        ? itemSelectionProperties["startability"]?.ToNullableDouble()
                        : null,
                    StartInfo = itemSelectionProperties.ContainsKey("startinfo")
                        ? itemSelectionProperties["startinfo"]?.ToNullableDouble()
                        : null,
                    Scalar = itemSelectionProperties.ContainsKey("scalar")
                        ? itemSelectionProperties["scalar"]?.ToNullableDouble()
                        : null,
                    LoadMin = assessment.IsSegmented() ? segBp.minExamItems as int? : null,
                    LoadMax = assessment.IsSegmented() ? segBp.maxExamItems as int? : null,
                    IsStrictMax = bool.Parse(itemSelectionProperties.GetOrDefault("isstrictmax", "false")),
                    BlueprintWeight = float.Parse(itemSelectionProperties.GetOrDefault("bpweight", "0")),
                    TestVersion = (long)testPackge.version,
                    PrecisionTarget = itemSelectionProperties.ContainsKey("precisiontarget")
                        ? itemSelectionProperties["precisiontarget"]?.ToNullableDouble()
                        : null,
                    PrecisionTargetMetWeight = itemSelectionProperties.ContainsKey("precisiontargetmetweight")
                        ? itemSelectionProperties["precisiontargetmetweight"]?.ToNullableDouble()
                        : null,
                    PrecisionTargetNotMetWeight = itemSelectionProperties.ContainsKey("precisiontargetnotmetweight")
                        ? itemSelectionProperties["precisiontargetnotmetweight"]?.ToNullableDouble()
                        : null,
                    AbilityWeight = itemSelectionProperties.ContainsKey("abilityweight")
                        ? itemSelectionProperties["abilityweight"]?.ToNullableDouble()
                        : null
                };

            adminStrandDao.Insert(segmentAdminStrandDtos.ToList());
        }

        public void CreateSetOfAdminItems(TestPackage.TestPackage testPackage, IDictionary<string, StrandDTO> strandMap)
        {
            var setOfAdminItemDtos = 
                from item in testPackage.GetAllItems()
                    let irtA = (from p in item.ItemScoreDimension.ItemScoreParameter
                                where p.measurementParameter.ToLower().Trim().Equals("a")
                                orderby p.value descending
                                select p.value)
                    let irtB = (from p in item.ItemScoreDimension.ItemScoreParameter
                                where p.measurementParameter.ToLower().Trim().StartsWith("b")
                                select p.value).Average()
                    let irtC = (from p in item.ItemScoreDimension.ItemScoreParameter
                                where p.measurementParameter.ToLower().Trim().Equals("c")
                                orderby p.value descending
                                select p.value)
                    let claimName = (from bpRef in item.BlueprintReferences
                                     join strand in strandMap
                                         on bpRef.idRef equals strand.Key
                                     where BlueprintElementTypes.CLAIM_AND_TARGET_TYPES.Contains(strand.Value.Type)
                                     select BlueprintElementTypes.CLAIM_TYPES.Contains(strand.Value.Type)
                                         ? strand.Value.Key
                                         : strand.Value.Key.Substring(0, strand.Value.Key.IndexOf('|'))).First()
                    let leafTargetKey = (from bpRef in item.BlueprintReferences
                                         join strand in strandMap
                                             on bpRef.idRef equals strand.Key
                                         where BlueprintElementTypes.TARGET_TYPES.Contains(strand.Value.Type) 
                                             && strand.Value.IsLeafTarget
                                         select strand.Value.Key)
                    let bVector = irtB == SetOfAdminItemDefaults.IRT_B
                        ? string.Format("{0:0.000000000000000}", irtB)
                        : string.Join(";", (from p in item.ItemScoreDimension.ItemScoreParameter
                            where p.measurementParameter.ToLower().Trim().StartsWith("b")
                            select string.Format("{0:0.000000000000000}", p.value)).ToArray())
                    select new SetOfAdminItemDTO
                    {
                        ItemKey = item.Key,
                        SegmentKey = item.AssessmentSegment.Key,
                        GroupId = item.ItemGroup.Key,
                        GroupKey = string.Format("{0}_{1}", item.ItemGroup.Key, SetOfAdminItemDefaults.BLOCK_ID),
                        ItemPosition = item.Position,
                        IsFieldTest = item.fieldTest,
                        IsActive = true,
                        IrtB = irtB.ToString("0.000000000000000"),
                        IsRequired = item.responseRequired,
                        BlockId = SetOfAdminItemDefaults.BLOCK_ID,
                        TestAdminKey = testPackage.publisher,
                        StrandKey = leafTargetKey.Any() 
                            ? leafTargetKey.First() 
                            : claimName,
                        TestVersion = (long)testPackage.version,
                        UpdatedTestVersion = (long)testPackage.version,
                        StrandName = claimName,
                        IrtA = irtA.Any() ? irtA.First() : SetOfAdminItemDefaults.IRT_A,
                        IrtC = irtC.Any() ? irtC.First() : SetOfAdminItemDefaults.IRT_C,
                        IrtModel = item.ItemScoreDimension.measurementModel,
                        ClString = item.AssessmentSegment.algorithmType.ToLower().Contains("adaptive")
                            ? CreateClString(item.BlueprintReferences, strandMap)
                            : null,
                        BVector = bVector
                    };

            setOfAdminItemDao.Insert(setOfAdminItemDtos.ToList());
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

        private string CreateClString(ItemGroupItemBlueprintReference[] itemBpRefs, IDictionary<string, StrandDTO> strandMap)
        {
            var strandKeys = from bpRef in itemBpRefs
                             join strand in strandMap
                                 on bpRef.idRef equals strand.Key
                             where BlueprintElementTypes.CLAIM_AND_TARGET_TYPES.Contains(strand.Value.Type)
                                 || strand.Value.Type.Equals(BlueprintElementTypes.AFFINITY_GROUP)
                             select strand.Value.Key;

            return string.Join(";", strandKeys.ToArray());
        }
    }
}
