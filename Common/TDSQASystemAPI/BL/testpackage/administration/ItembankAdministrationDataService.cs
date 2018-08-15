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
        private readonly ITestPackageDao<SetOfTestGradeDTO> setOfTestGradeDao;
        private readonly ITestPackageDao<TestCohortDTO> testCohortDao;
        private readonly ITestPackageDao<AdminStrandDTO> adminStrandDao;
        private readonly ITestPackageDao<SetOfAdminItemDTO> setOfAdminItemDao;
        private readonly ITestPackageDao<ItemScoreDimensionDTO> itemScoreDimensionDao;
        private readonly ITestPackageDao<ItemMeasurementParameterDTO> itemMeasurementParameterDao;
        private readonly ITestPackageDao<AdminStimulusDTO> setOfAdminStimuliDao;
        private readonly ITestPackageDao<TestFormDTO> testFormDao;
        private readonly ITestPackageDao<TestFormItemDTO> testFormItemDao;
        private readonly ITestPackageDao<AffinityGroupDTO> affinityGroupDao;
        private readonly ITestPackageDao<AffinityGroupItemDTO> affinityGroupItemDao;

        /// <summary>
        /// A map of the measurement models that are available.  This map comes from the 
        /// <code>OSS_Itembank..MeasurementModel</code> table.  The <code>OSS_Itembank..MeasurementModel</code> is
        /// populated via a seed data script that is run when TIS is first deployed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// These values match up with what is described in the <code>Model</code> enum from
        /// <code>./TestScoringEngine/ScoringEngine/MeasurementModels/IRTModelFactory.cs</code>:
        /// <example>
        /// public enum Model { Unknown = 0, IRT3PL = 1, IRTPCL = 2, raw = 3, IRT3PLn = 4, IRTGPC = 5, IRTGRL = 6 };
        /// </example>
        /// </para>
        /// These values differ slightly from what is described in the TDS <code>itembank.measurementmodel</code>:
        /// * In TDS: the IRT3PLn id is 1 and IRT3pl is 4.  
        ///   * As long as the identifiers are consistent within the system it should be fine.
        /// In both TDS and TIS, the IRTGRL record is missing; there is no record in the <code>measurementmodel</code>
        /// table with an identifier of 6.
        /// </remarks>
        private readonly Dictionary<string, int> measurementModelMap = new Dictionary<string, int>
        {
            { "IRT3pl", 1 },
            { "IRTPCL", 2 },
            { "raw", 3 },
            { "IRT3PLn", 4 },
            { "IRTGPC", 5 }
        };

        /// <summary>
        /// An in-memory representation of what the <code>OSS_Itembank..MeasurementParameter</code> table looks like.
        /// While the <code>Description</code> field is not in use here, it can be useful to have for reference.
        /// </summary>
        private readonly IList<MeasurementParameter> measurementParameterLookup = new List<MeasurementParameter>
        {
            // IRT3pl
            new MeasurementParameter(1, 0, "a", "Slope (a)"),
            new MeasurementParameter(1, 1, "b", "Difficulty (b)"),
            new MeasurementParameter(1, 2, "c", "Guessing (c)"),
            // IRTPCL
            new MeasurementParameter(2, 0, "b0", "Difficulty cut 0 (b0)"),
            new MeasurementParameter(2, 1, "b1", "Difficulty cut 1 (b1)"),
            new MeasurementParameter(2, 2, "b2", "Difficulty cut 2 (b2)"),
            new MeasurementParameter(2, 3, "b3", "Difficulty cut 3 (b3)"),
            new MeasurementParameter(2, 4, "b4", "Difficulty cut 4 (b4)"),
            new MeasurementParameter(2, 5, "b5", "Difficulty cut 5 (b5)"),
            // IRT3PLn
            new MeasurementParameter(4, 0, "a", "Slope (a)"),
            new MeasurementParameter(4, 1, "b", "Difficulty (b)"),
            new MeasurementParameter(4, 2, "c", "Guessing (c)"),
            // IRTGPC
            new MeasurementParameter(5, 0, "a", "Slope (a)"),
            new MeasurementParameter(5, 1, "b0", "Difficulty cut 0 (b0)"),
            new MeasurementParameter(5, 2, "b1", "Difficulty cut 1 (b1)"),
            new MeasurementParameter(5, 3, "b2", "Difficulty cut 2 (b2)"),
            new MeasurementParameter(5, 4, "b3", "Difficulty cut 3 (b3)"),
            new MeasurementParameter(5, 5, "b4", "Difficulty cut 4 (b4)"),
            new MeasurementParameter(5, 6, "b5", "Difficulty cut 5 (b5)")
        };

        public ItembankAdministrationDataService()
        {
            testAdminDao = new TestAdminDAO();
            adminStrandDao = new AdminStrandDAO();
            setOfAdminSubjectDao = new SetOfAdminSubjectDAO();
            setOfTestGradeDao = new SetOfTestGradeDAO();
            testCohortDao = new TestCohortDAO();
            setOfAdminItemDao = new SetOfAdminItemDAO();
            itemScoreDimensionDao = new ItemScoreDimensionDAO();
            itemMeasurementParameterDao = new ItemMeasurementParameterDAO();
            setOfAdminStimuliDao = new AdminStimulusDAO();
            testFormDao = new TestFormDAO();
            testFormItemDao = new TestFormItemDAO();
            affinityGroupDao = new AffinityGroupDAO();
            affinityGroupItemDao = new AffinityGroupItemDAO();
            itembankConfigurationDataQueryService =
                new ItembankConfigurationDataQueryService(new SubjectDAO(), new ClientDAO(), testAdminDao);

        }

        public ItembankAdministrationDataService(IItembankConfigurationDataQueryService itembankConfigurationDataQueryService,
                                                 ITestPackageDao<TestAdminDTO> testAdminDao,
                                                 ITestPackageDao<SetOfAdminSubjectDTO> setOfAdminSubjectDao,
                                                 ITestPackageDao<SetOfTestGradeDTO> setOfTestGradeDao,
                                                 ITestPackageDao<TestCohortDTO> testCohortDao,
                                                 ITestPackageDao<AdminStrandDTO> adminStrandDao,
                                                 ITestPackageDao<SetOfAdminItemDTO> setOfAdminItemDao,
                                                 ITestPackageDao<ItemScoreDimensionDTO> itemScoreDimensionDao,
                                                 ITestPackageDao<ItemMeasurementParameterDTO> itemMeasurementParameterDao,
                                                 ITestPackageDao<AdminStimulusDTO> setOfAdminStimuliDao,
                                                 ITestPackageDao<TestFormDTO> testFormDao,
                                                 ITestPackageDao<TestFormItemDTO> testFormItemDao,
                                                 ITestPackageDao<AffinityGroupDTO> affinityGroupDao,
                                                 ITestPackageDao<AffinityGroupItemDTO> affinityGroupItemDao)
        {
            this.itembankConfigurationDataQueryService = itembankConfigurationDataQueryService;
            this.testAdminDao = testAdminDao;
            this.setOfTestGradeDao = setOfTestGradeDao;
            this.testCohortDao = testCohortDao;
            this.setOfAdminSubjectDao = setOfAdminSubjectDao;
            this.adminStrandDao = adminStrandDao;
            this.setOfAdminItemDao = setOfAdminItemDao;
            this.itemScoreDimensionDao = itemScoreDimensionDao;
            this.itemMeasurementParameterDao = itemMeasurementParameterDao;
            this.setOfAdminStimuliDao = setOfAdminStimuliDao;
            this.testFormDao = testFormDao;
            this.testFormItemDao = testFormItemDao;
            this.affinityGroupDao = affinityGroupDao;
            this.affinityGroupItemDao = affinityGroupItemDao;
        }
        public static string COMBINED_SUFFIX = "-COMBINED";

        public static string Key(TestPackage.TestPackage testPackage, string id)
        {
            return string.Format("({0}){1}-{2}", testPackage.publisher, id, testPackage.academicYear);
        }

        public static string CombinedId(string id)
        {
            return id + COMBINED_SUFFIX;
        }

        public static string CombinedKey(TestPackage.TestPackage testPackage, string id)
        {
            return Key(testPackage, CombinedId(id));
        }

        private ItemScoreDimensionDTO CreateItemScoreDimensionDTO(string segmentKey, string itemKey, ItemGroupItemItemScoreDimension dimension)
        {
            return new ItemScoreDimensionDTO
            {
                Dimension = dimension.dimension ?? string.Empty,
                RecodeRule = string.Empty,
                ScorePoints = dimension.scorePoints,
                Weight = dimension.weight,
                ItemScoreDimensionKey = Guid.NewGuid(),
                SegmentKey = segmentKey,
                ItemKey = itemKey,

                MeasurementModel = measurementModelMap[dimension.measurementModel]
            };
        }

        public void CreateItemMeasurementParameters(TestPackage.TestPackage testPackage)
        {
            var allItems = testPackage.GetAllItems();
            var scoreDimensions = new List<ItemScoreDimensionDTO>();
            foreach (var item in allItems)
            {
                item.ItemScoreDimensions.ForEach(dimension =>
                {
                    if (item.TestPackage.IsCombined())
                    {
                        
                        scoreDimensions.Add(CreateItemScoreDimensionDTO(CombinedKey(item.TestPackage, item.TestSegment.id), item.Key, dimension));
                    }
                    scoreDimensions.Add(CreateItemScoreDimensionDTO(item.TestSegment.Key, item.Key, dimension));
                });
            }

            itemScoreDimensionDao.Insert(scoreDimensions);

            var scoreDimensionMap = from dimensionDto in scoreDimensions
                                    join item in allItems
                                        on dimensionDto.ItemKey equals item.Key
                                    select new
                                    {
                                        ItemKey = item.Key,
                                        DimensionKey = dimensionDto.ItemScoreDimensionKey,
                                        MeasurementModelKey = dimensionDto.MeasurementModel,
                                        Dimension = dimensionDto.Dimension
                                    };


            var itemMeasurementParameters = from item in allItems
                                            from dimension in item.ItemScoreDimensions
                                            from param in dimension.ItemScoreParameter
                                            from dimensionDto in scoreDimensionMap
                                            where item.Key == dimensionDto.ItemKey && 
                                            (                                                
                                                (dimension.dimension?.ToString() ?? "").Equals(dimensionDto.Dimension)
                                            )
                                            select new ItemMeasurementParameterDTO
                                            {
                                                ItemScoreDimensionKey = dimensionDto.DimensionKey,
                                                MeasurementParameterKey = (from p in measurementParameterLookup
                                                                           where p.MeasurementModelKey == dimensionDto.MeasurementModelKey
                                                                                && p.Name.Equals(param.measurementParameter)
                                                                           select p).First().Number,
                                                ParmValue = param.value
                                            };

            itemMeasurementParameterDao.Insert(itemMeasurementParameters.Distinct().ToList());
        }

        private AdminStimulusDTO CreateAdminStimuli(TestPackage.TestPackage testPackage, string segmentKey, ItemGroupStimulus stimulus)
        {
            return new AdminStimulusDTO
            {
                StimulusKey = stimulus.Key,
                SegmentKey = segmentKey,
                NumItemsRequired = stimulus.ItemGroup.maxResponses.Equals("ALL", StringComparison.InvariantCultureIgnoreCase)
                                          ? -1
                                          : int.Parse(stimulus.ItemGroup.maxResponses),
                MaxItems = stimulus.ItemGroup.maxItems.Equals("ALL", StringComparison.InvariantCultureIgnoreCase)
                                          ? -1
                                          : int.Parse(stimulus.ItemGroup.maxItems),
                TestVersion = (long)testPackage.version,
                UpdatedTestVersion = (long)testPackage.version,
                GroupId = stimulus.ItemGroup.Key
            };
        }

        public void CreateAdminStimuli(TestPackage.TestPackage testPackage)
        {
            var admimStimuliDtosList = new List<AdminStimulusDTO>();
            var adminStimuliDtos = from stimulus in testPackage.GetAllStimuli()
                select CreateAdminStimuli(testPackage, stimulus.TestSegment.Key, stimulus);
            admimStimuliDtosList.AddRange(adminStimuliDtos);
            if (testPackage.IsCombined())
            {
                var combinedAdminStimuliDtos = from stimulus in testPackage.GetAllStimuli()
                    select CreateAdminStimuli(testPackage, CombinedKey(testPackage, stimulus.TestSegment.id), stimulus);
                admimStimuliDtosList.AddRange(combinedAdminStimuliDtos);
            }

            setOfAdminStimuliDao.Insert(admimStimuliDtosList.Distinct().ToList());
        }

        public void CreateAdminStrands(TestPackage.TestPackage testPackge, IDictionary<string, StrandDTO> strandMap)
        {
            var segmentAdminStrandDtos =
                from test in testPackge.Test
                from segment in test.Segments
                from segBp in segment.SegmentBlueprint
                where BlueprintElementTypes.CLAIM_AND_TARGET_TYPES.Contains(strandMap[segBp.idRef].Type)
                let itemSelectionProperties = (segBp.ItemSelection != null) ?
                    segBp.ItemSelection.ToDictionary(isp => isp.name.ToLower().Trim(), isp => isp.value.Trim()) :
                    new Dictionary<string, string>()
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
                    LoadMin = test.IsSegmented() ? segBp.minExamItems as int? : null,
                    LoadMax = test.IsSegmented() ? segBp.maxExamItems as int? : null,
                    IsStrictMax = BlueprintElementTypes.CLAIM.Equals(strandMap[segBp.idRef].Type) ? false : bool.Parse(itemSelectionProperties.GetOrDefault("isstrictmax", "false")),
                    BlueprintWeight = float.Parse(itemSelectionProperties.GetOrDefault("bpweight", "1")),
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

        public void CreateAffinityGroups(TestPackage.TestPackage testPackage)
        {
            var affinityGroupBlueprintElements = from blueprint in testPackage.GetAllTestPackageBlueprintElements()
                                                 where blueprint.Value.type.Equals(BlueprintElementTypes.AFFINITY_GROUP)
                                                 select blueprint;

            var affinityGroupDtos =
                from assessment in testPackage.Test
                from segment in assessment.Segments
                from segmentBp in segment.SegmentBlueprint
                join affinityGroupBp in affinityGroupBlueprintElements
                    on segmentBp.idRef equals affinityGroupBp.Key
                let itemSelectionProperties = segmentBp.ItemSelection.ToDictionary(isp => isp.name.ToLower().Trim(), isp => isp.value.Trim())
                select new AffinityGroupDTO
                {
                    SegmentKey = segment.Key,
                    GroupId = segmentBp.idRef,
                    MinItems = segmentBp.minExamItems,
                    MaxItems = segmentBp.maxExamItems,
                    IsStrictMax = bool.Parse(itemSelectionProperties.GetOrDefault("isstrictmax", "false")),
                    BlueprintWeight = float.Parse(itemSelectionProperties.GetOrDefault("bpweight", "0")),
                    StartAbility = itemSelectionProperties.ContainsKey("startability")
                        ? itemSelectionProperties["startability"]?.ToNullableDouble()
                        : null,
                    StartInfo = itemSelectionProperties.ContainsKey("startinfo")
                        ? itemSelectionProperties["startinfo"]?.ToNullableDouble()
                        : null,
                    AbilityWeight = itemSelectionProperties.ContainsKey("abilityweight")
                        ? itemSelectionProperties["abilityweight"]?.ToNullableDouble()
                        : null,
                    PrecisionTarget = itemSelectionProperties.ContainsKey("precisiontarget")
                        ? itemSelectionProperties["precisiontarget"]?.ToNullableDouble()
                        : null,
                    PrecisionTargetMetWeight = itemSelectionProperties.ContainsKey("precisiontargetmetweight")
                        ? itemSelectionProperties["precisiontargetmetweight"]?.ToNullableDouble()
                        : null,
                    PrecisionTargetNotMetWeight = itemSelectionProperties.ContainsKey("precisiontargetnotmetweight")
                        ? itemSelectionProperties["precisiontargetnotmetweight"]?.ToNullableDouble()
                        : null,
                    TestVersion = (long)testPackage.version,
                    UpdatedTestVersion = (long)testPackage.version
                };

            affinityGroupDao.Insert(affinityGroupDtos.ToList());

            var affinityGroupItemDtos = from item in testPackage.GetAllItems()
                                        from itemBp in item.BlueprintReferences
                                        join affinityGroupBp in affinityGroupBlueprintElements
                                            on itemBp.idRef equals affinityGroupBp.Key
                                        select new AffinityGroupItemDTO
                                        {
                                            SegmentKey = item.TestSegment.Key,
                                            ItemKey = item.Key,
                                            GroupId = itemBp.idRef
                                        };

            affinityGroupItemDao.Insert(affinityGroupItemDtos.ToList());
        }

        private SetOfAdminItemDTO CreateSetOfAdminItems(TestPackage.TestPackage testPackage, ItemGroupItem item, string segmentKey, IDictionary<string, StrandDTO> strandMap) {
            var irtA = (from dimension in item.ItemScoreDimensions
                        from p in dimension.ItemScoreParameter
                        where p.measurementParameter.ToLower().Trim().Equals("a")
                        orderby p.value descending
                        select p.value);
            var irtB = (from dimension in item.ItemScoreDimensions
                        from p in dimension.ItemScoreParameter
                        where p.measurementParameter.ToLower().Trim().StartsWith("b")
                        select p.value).Average();
            var irtC = (from dimension in item.ItemScoreDimensions
                        from p in dimension.ItemScoreParameter
                        where p.measurementParameter.ToLower().Trim().Equals("c")
                        orderby p.value descending
                        select p.value);
            var claimName = (from bpRef in item.BlueprintReferences
                             join strand in strandMap
                                 on bpRef.idRef equals strand.Key
                             where BlueprintElementTypes.CLAIM_AND_TARGET_TYPES.Contains(strand.Value.Type)
                             select BlueprintElementTypes.CLAIM_TYPES.Contains(strand.Value.Type)
                                 ? strand.Value.Key
                                 : strand.Value.Key.Substring(0, strand.Value.Key.IndexOf('|'))).First();
            var leafTargetKey = (from bpRef in item.BlueprintReferences
                                 join strand in strandMap
                                     on bpRef.idRef equals strand.Key
                                 where BlueprintElementTypes.TARGET_TYPES.Contains(strand.Value.Type)
                                     && strand.Value.IsLeafTarget
                                 select strand.Value.Key);
            var bVector = irtB == SetOfAdminItemDefaults.IRT_B
                ? string.Format("{0:0.000000000000000}", irtB)
                : string.Join(";", (from p in item.ItemScoreDimensions.First().ItemScoreParameter
                                    where p.measurementParameter.ToLower().Trim().StartsWith("b")
                                    select string.Format("{0:0.000000000000000}", p.value)).ToArray());

            return new SetOfAdminItemDTO
            {
                ItemKey = item.Key,
                SegmentKey = segmentKey,
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
                IrtModel = item.ItemScoreDimensions.First().measurementModel,
                ClString = item.TestSegment.algorithmType.ToLower().Contains("adaptive")
                           ? CreateClString(item.BlueprintReferences, strandMap)
                           : null,
                BVector = bVector
            };
        }

        public void CreateSetOfAdminItems(TestPackage.TestPackage testPackage, IDictionary<string, StrandDTO> strandMap)
        {
            var setOfAdminItemDtosList = new List<SetOfAdminItemDTO>();

            var setOfAdminItemDtos = from item in testPackage.GetAllItems()
                                     select CreateSetOfAdminItems(testPackage, item, item.TestSegment.Key, strandMap);
            setOfAdminItemDtosList.AddRange(setOfAdminItemDtos);

            if (testPackage.IsCombined())
            {
                var combinedSetOfAdminItemDtos = 
                    from item in testPackage.GetAllItems()
                    let segmentId = CombinedId(item.TestSegment.id)
                    let segmentKey = Key(testPackage, segmentId)
                    select CreateSetOfAdminItems(testPackage, item, segmentKey, strandMap);
                setOfAdminItemDtosList.AddRange(combinedSetOfAdminItemDtos);

            }

            setOfAdminItemDao.Insert(setOfAdminItemDtosList.Distinct().ToList());
        }

        private SetOfAdminSubjectDTO CreateSetOfAdminSubjects(string segmentKey, string testId, int segmentPosition, string algorithmType, TestPackage.TestPackage testPackage, TestPackage.TestSegmentSegmentBlueprintElement segBp, Dictionary<string, string> itemSelectionProperties, string virtualTest)
        {
           return new SetOfAdminSubjectDTO
            {
                SegmentKey = segmentKey,
                TestAdminKey = testPackage.publisher,
                SubjectKey = testPackage.GetSubjectKey(),
                TestId = testId,
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
                SelectionAlgorithm = SelectionAlgorithmTypes.GetSelectionAlgorithm(algorithmType),
                BlueprintWeight = double.Parse(itemSelectionProperties.GetOrDefault("bpweight", ItemSelectionDefaults.BLUEPRINT_WEIGHT.ToString())),
                AbilityWeight = double.Parse(itemSelectionProperties.GetOrDefault("abilityweight", ItemSelectionDefaults.ABILITY_WEIGHT.ToString())),
                CSet1Size = int.Parse(itemSelectionProperties.GetOrDefault("cset1size", ItemSelectionDefaults.CSET1_SIZE.ToString())),
                CSet2Random = int.Parse(itemSelectionProperties.GetOrDefault("cset2random", ItemSelectionDefaults.CSET2_RANDOM.ToString())),
                CSet2InitialRandom = int.Parse(itemSelectionProperties.GetOrDefault("cset2initialrandom", ItemSelectionDefaults.CSET2_INITIAL_RANDOM.ToString())),
                TestPosition = segmentPosition,
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
                TestType = testPackage.type,
                Contract = testPackage.publisher,
                VirtualTest = virtualTest
           };
        }

        private List<SetOfAdminSubjectDTO> CreateSetOfAdminSubjects(TestPackage.Test test)
        {
            var testPackage = test.TestPackage;

            var setOfAdminSubjectsList = new List<SetOfAdminSubjectDTO>();
            var segmentAdminSubjectDtos =
                from segment in test.Segments
                let segBp = segment.SegmentBlueprint.First(b => b.idRef.Equals(segment.id))
                let itemSelectionProperties = segBp.ItemSelection.ToDictionary(isp => isp.name.ToLower().Trim(), isp => isp.value.Trim())
                select CreateSetOfAdminSubjects(segment.Key, segment.id, segment.position, segment.algorithmType, testPackage, segBp, itemSelectionProperties, null);
            
            setOfAdminSubjectsList.AddRange(segmentAdminSubjectDtos);

            // If this is a multi-segment assessment, create the "virtual" parent assessment.
            if (test.IsSegmented())
            {
                // Many of the properties in the "virtual" row will contain sums of its segments. For example, if seg1 has
                // a "minitems" value of 3, and "maxitems" value of 4, and seg2 has a "minitems" value of 1 and a "maxitems" value of 1,
                // the "virtual" row will contain minitems = 4 and maxitems = 5
                var itemCounts = (from s in test.Segments
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

                var segment = test.Segments.First();
                var segmentBlueprint = segment.SegmentBlueprint.First(b => b.idRef.Equals(segment.id));
                var itemSelectionProperties = segmentBlueprint.ItemSelection.ToDictionary(isp => isp.name.ToLower().Trim(), isp => isp.value.Trim());
                var virtualTestDto = new SetOfAdminSubjectDTO
                {
                    SegmentKey = test.Key,
                    TestAdminKey = testPackage.publisher,
                    SubjectKey = testPackage.GetSubjectKey(),
                    TestId = test.id,
                    StartAbility = ItemSelectionDefaults.START_ABILITY,
                    StartInfo = ItemSelectionDefaults.START_INFO,
                    MinItems = itemCounts.MinItemCount,
                    MaxItems = itemCounts.MaxItemCount,
                    Slope = double.Parse(itemSelectionProperties.GetOrDefault("slope", ItemSelectionDefaults.SLOPE.ToString())),
                    Intercept = double.Parse(itemSelectionProperties.GetOrDefault("intercept", ItemSelectionDefaults.INTERCEPT.ToString())),
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
                    TestType = testPackage.type,
                    Contract = testPackage.publisher,
                    VirtualTest = test.Key
                };

                setOfAdminSubjectsList.Add(virtualTestDto);
            }
            return setOfAdminSubjectsList;
        }


        public void CreateSetOfAdminSubjects(TestPackage.TestPackage testPackage)
        {
            var setOfAdminSubjectsList = new List<SetOfAdminSubjectDTO>();

            foreach (var test in testPackage.Test)
            {
                var newSetOfAdminSubjectsList = CreateSetOfAdminSubjects(test);
                setOfAdminSubjectsList.AddRange(newSetOfAdminSubjectsList);                
            }

            if (testPackage.IsCombined())
            {
                var combinedId = testPackage.Blueprint.First(bp => bp.type.Equals("package")).id;
                var combinedKey = Key(testPackage, combinedId); // string.Format("({0}){1}-{2}", testPackage.publisher, combinedId, testPackage.academicYear);
                var testPosition = 1;
                foreach (var test in testPackage.Test)
                {
                    var segmentAdminSubjectDtos =
                    from segment in test.Segments
                    let segBp = segment.SegmentBlueprint.First(b => b.idRef.Equals(segment.id))
                    let itemSelectionProperties = segBp.ItemSelection.ToDictionary(isp => isp.name.ToLower().Trim(), isp => isp.value.Trim())
                    let segmentId = CombinedId(segment.id)
                    let segmentKey = Key(testPackage, segmentId)
                    select CreateSetOfAdminSubjects(segmentKey, segmentId, testPosition, segment.algorithmType, testPackage, segBp, itemSelectionProperties, combinedKey);

                    testPosition += 1;
                    
                    setOfAdminSubjectsList.AddRange(segmentAdminSubjectDtos);
                }
                // Many of the properties in the "virtual" row will contain sums of its segments. For example, if seg1 has
                // a "minitems" value of 3, and "maxitems" value of 4, and seg2 has a "minitems" value of 1 and a "maxitems" value of 1,
                // the "virtual" row will contain minitems = 4 and maxitems = 5                
                var itemCounts = (
                        from test in testPackage.Test
                        from s in test.Segments
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

                var firstSegment = testPackage.Test.First().Segments.First();
                var firstSegmentBlueprint = firstSegment.SegmentBlueprint.First(b => b.idRef.Equals(firstSegment.id));
                var combinedItemSelectionProperties = firstSegmentBlueprint.ItemSelection.ToDictionary(isp => isp.name.ToLower().Trim(), isp => isp.value.Trim());

                var virtualTestDto = new SetOfAdminSubjectDTO
                {
                    SegmentKey = combinedKey,
                    TestAdminKey = testPackage.publisher,
                    SubjectKey = testPackage.GetSubjectKey(),
                    TestId = combinedId,
                    StartAbility = ItemSelectionDefaults.START_ABILITY,
                    StartInfo = ItemSelectionDefaults.START_INFO,
                    MinItems = itemCounts.MinItemCount,
                    MaxItems = itemCounts.MaxItemCount,
                    Slope = double.Parse(combinedItemSelectionProperties.GetOrDefault("slope", ItemSelectionDefaults.SLOPE.ToString())),
                    Intercept = double.Parse(combinedItemSelectionProperties.GetOrDefault("intercept", ItemSelectionDefaults.INTERCEPT.ToString())),
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
                    TestType = testPackage.type,
                    Contract = testPackage.publisher,                    
                };
                setOfAdminSubjectsList.Add(virtualTestDto);
            }
            
            setOfAdminSubjectDao.Insert(setOfAdminSubjectsList);

            setOfAdminSubjectsList.Where(adminSubject => adminSubject.SelectionAlgorithm.ToLower().StartsWith("adaptive")).
                ForEach(adminSubject =>
                {
                    var newTestCohort = new TestCohortDTO()
                    {
                        SegmentKey = adminSubject.SegmentKey,
                        Cohort = 1,
                        ItemRatio = 1.0
                    };
                    if (!testCohortDao.Exists(newTestCohort))
                    {
                        testCohortDao.Insert(newTestCohort);
                    }
                });
            
        }

        private SetOfTestGradeDTO CreateSetOfTestGrades(string id, string grade, string key)
        {
            return new SetOfTestGradeDTO()
            {
                TestId = id,
                Grade = grade,
                SegmentKey = key
            };
        }

        public void CreateSetOfTestGrades(TestPackage.TestPackage testPackage)
        {
            var gradesList = new List<SetOfTestGradeDTO>();
            var grades = from test in testPackage.Test
                         from grade in test.Grades
                         select CreateSetOfTestGrades(test.id, grade.value, test.Key);

            gradesList.AddRange(grades);

            if (testPackage.IsCombined())
            {
                var combinedId = testPackage.Blueprint.First(bp => bp.type.Equals("package")).id;
                var combinedKey = Key(testPackage, combinedId);
                var combinedGrades = from test in testPackage.Test
                                     from grade in test.Grades
                                     select CreateSetOfTestGrades(combinedId, grade.value, combinedKey);
                gradesList.AddRange(combinedGrades);

            }


            this.setOfTestGradeDao.Insert(gradesList.ToList());
        }

        public void CreateTestFormItems(TestPackage.TestPackage testPackage, IList<TestFormDTO> testForms)
        {
            var fixedFormSegments = from test in testPackage.Test
                                    from segment in test.Segments
                                    where segment.algorithmType.ToLower().Trim().Equals(SelectionAlgorithmTypes.FIXED_FORM)
                                    select segment;

            var fixedFormItems = new List<ItemGroupItem>();
            foreach (var segment in fixedFormSegments)
            {
                foreach (var form in (segment.Item as TestSegmentSegmentForms).SegmentForm)
                {
                    var items = from ig in form.ItemGroup
                                from item in ig.Item
                                select item;
                    if (items.Any())
                    {
                        fixedFormItems.AddRange(items);
                    }
                }
            }

            var formItemDtos = from item in fixedFormItems
                               from presentation in item.SegmentForm.Presentations
                               let id = GetFormIdForLanguage(item.SegmentForm.id, presentation.code)
                               let testForm = testForms.First(f => f.FormId.Equals(id))
                               select new TestFormItemDTO
                               {
                                   FormPosition = item.Position,
                                   SegmentKey = item.SegmentForm.TestSegment.Key,
                                   IsActive = true,
                                   ItemKey = item.Key,
                                   TestFormKey = testForm.TestFormKey,
                                   ITSFormKey = testForm.ITSKey
                               };

            testFormItemDao.Insert(formItemDtos.Distinct().ToList());
        }

        private static int FormHashCode(string s)
        {            
            int h = 0;
            if (s.Length > 0)
            {
                char[] val = s.ToArray();

                for (int i = 0; i < s.Length; i++)
                {
                    h = 31 * h + val[i];
                }
            }
            return h;
        }

        private static int GetFormKey(string formId)
        {
            return Math.Abs(FormHashCode(formId));
        }

        private static string GetFormIdForLanguage(string formId, string languageCode)
        {
            switch (languageCode)
            {
                case "ESN":
                    return string.Format("{0}::{1}", formId, "SPA");
                case "ENU-Braille":
                    return string.Format("{0}::{1}", formId, "BRL");
                default:
                    return string.Format("{0}::{1}", formId, languageCode);
            }
        }

        private static string GetBankFormKey(int bankKey, int formKey)
        {
            return string.Format("{0}-{1}", bankKey, formKey);
        }

        public List<TestFormDTO> CreateTestForms(TestPackage.TestPackage testPackage)
        {
            var testForms = new List<TestFormDTO>();
            var fixedFormSegments = from test in testPackage.Test
                                    from segment in test.Segments
                                    where segment.algorithmType.ToLower().Trim().Equals(SelectionAlgorithmTypes.FIXED_FORM)
                                    select segment;

            fixedFormSegments.ToList().ForEach(s => {
                (s.Item as TestSegmentSegmentForms).SegmentForm.ForEach(f => {
                    f.Presentations.ForEach(p => {
                        var formId = GetFormIdForLanguage(f.id, p.code);
                        var formKey = GetFormKey(formId);
                        testForms.Add(new TestFormDTO()
                        {
                            SegmentKey = s.Key,
                            Cohort = f.cohort,
                            Language = p.code,
                            TestFormKey = GetBankFormKey(testPackage.bankKey, formKey),
                            FormId = formId,
                            ITSBankKey = testPackage.bankKey,
                            ITSKey = formKey,
                            TestVersion = (long)testPackage.version
                        });
                    });
                });
            });            

            testFormDao.Insert(testForms);

            return testForms;
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
                TestVersion = (long)testPackage.version, // whether inserting or updating, we want the version to reflect the testpackage being loaded
                Season = existingTestAdmin?.Season ?? ""
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

        /// <summary>
        /// Build up a "Target String" for an admin item.
        /// </summary>
        /// <param name="itemBpRefs">The collection of <code>ItemGroupItemBlueprintReference</code> associated to the
        /// <code>ItemGroupItem</code> being affected.</param>
        /// <param name="strandMap">The collection of <code>StrandDTO</code>s built for this <code>TestPackage</code>.</param>
        /// <returns>A semi-colon delimited <code>string</code> containing all the claim and target keys for the
        /// admin item record.</returns>
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
