using System.Collections.Generic;
using System.Linq;
using TDSQASystemAPI.BL.testpackage.administration;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.DAL.scoring.daos;
using TDSQASystemAPI.DAL.scoring.dtos;
using TDSQASystemAPI.TestPackage;

namespace TDSQASystemAPI.BL.testpackage.scoring
{
    public class ScoringConfigurationDataService
    {
        private readonly ITestPackageDao<TestDTO> testDAO;
        private readonly ITestPackageDao<TestGradeDTO> testGradeDAO;
        private readonly ITestPackageDao<TestScoreFeatureDTO> testScoreFeatureDAO;
        private readonly ITestPackageDao<FeatureComputationLocationDTO> featureComputationLocationDAO;
        private readonly ITestPackageDao<ComputationRuleParameterDTO> computationRuleParameterDAO;
        private readonly ITestPackageDao<ComputationRuleParameterValueDTO> computationRuleParameterValueDAO;
        private readonly ITestPackageDao<PerformanceLevelDTO> performanceLevelDAO;
        private const string BLUEPRINT_TYPE_TEST = "test";
        private const string BLUEPRINT_TYPE_PACKAGE = "package";

        public ScoringConfigurationDataService()
        {
            testDAO = new TestDAO();
            testGradeDAO = new TestGradeDAO();
            testScoreFeatureDAO = new TestScoreFeatureDAO();
            featureComputationLocationDAO = new FeatureComputationLocationDAO();
            computationRuleParameterDAO = new ComputationRuleParameterDAO();
            computationRuleParameterValueDAO = new ComputationRuleParameterValueDAO();
            performanceLevelDAO = new PerformanceLevelDAO();
        }

        public ScoringConfigurationDataService(
            ITestPackageDao<TestDTO> testDAO,
            ITestPackageDao<TestGradeDTO> testGradeDAO,
            ITestPackageDao<TestScoreFeatureDTO> testScoreFeatureDAO,
            ITestPackageDao<FeatureComputationLocationDTO> featureComputationLocationDAO,
            ITestPackageDao<ComputationRuleParameterDTO> computationRuleParameterDAO,
            ITestPackageDao<ComputationRuleParameterValueDTO> computationRuleParameterValueDAO,
            ITestPackageDao<PerformanceLevelDTO> performanceLevelDAO)
        {
            this.testDAO = testDAO;
            this.testGradeDAO = testGradeDAO;
            this.testScoreFeatureDAO = testScoreFeatureDAO;
            this.featureComputationLocationDAO = featureComputationLocationDAO;
            this.computationRuleParameterDAO = computationRuleParameterDAO;
            this.computationRuleParameterValueDAO = computationRuleParameterValueDAO;
            this.performanceLevelDAO = performanceLevelDAO;
        }

        public static string TestKey(TestPackage.TestPackage testPackage)
        {            
            var packageBlueprint = testPackage.Blueprint.FirstOrDefault(bp => bp.type.ToLower().Equals(BLUEPRINT_TYPE_PACKAGE));
            if (packageBlueprint == null)
            {
                packageBlueprint = testPackage.Blueprint.FirstOrDefault(bp => bp.type.ToLower().Equals(BLUEPRINT_TYPE_TEST));
            }
            if (packageBlueprint != null)
            {
                return packageBlueprint.id;
            } else
            {
                throw new System.InvalidOperationException("Blueprint element with type 'package' or 'test' does not exists");
            }
        }

        public void CreateTest(TestPackage.TestPackage testPackage)
        {
            var testKey = TestKey(testPackage);
            var testDTO = new TestDTO()
            {
                ClientName = testPackage.publisher,
                TestId = testKey,
                Subject = testPackage.subject
            };
            if (!testDAO.Exists(testDTO))
            {

                testDAO.Insert(testDTO);
            }
        }

        public void CreateGrade(TestPackage.TestPackage testPackage)
        {
            var testKey = TestKey(testPackage);
            var testGradeDTO = new List<TestGradeDTO>();
            
            testPackage.Test.ForEach(assessment =>
                assessment.Grades.ForEach(grade => {
                    var newTestGrade = new TestGradeDTO()
                    {
                        ClientName = testPackage.publisher,
                        TestId = testKey,
                        ReportingGrade = grade.value
                    };

                    if (!testGradeDAO.Exists(newTestGrade))
                    {
                        if (!testGradeDTO.Contains(newTestGrade))
                        {
                            testGradeDTO.Add(newTestGrade);
                        }
                    }
                }));
            testGradeDAO.Insert(testGradeDTO);
        }

        private TestScoreFeatureDTO CreateScoreFeature(string testKey, string publisher, string measureOf, string measureLabel, BlueprintElementScoringRule rule)
        {
            return new TestScoreFeatureDTO()
            {
                TestScoreFeatureKey = rule.Id,
                ClientName = publisher,
                TestId = testKey,
                MeasureOf = measureOf,
                MeasureLabel = measureLabel,
                IsScaled = measureLabel.ToLower().Contains("scale"),
                ComputationRule = rule.name,
                ComputationOrder = rule.computationOrder
            };
        }

        private List<TestScoreFeatureDTO> CreateScoreFeature(string testKey, TestPackage.TestPackage testPackage)
        {
            var createScoreFeatureList = new List<TestScoreFeatureDTO>();
            testPackage.Blueprint.ForEach(blueprint => {
                blueprint.Scoring?.Rules.ForEach(rule =>
                {
                    var measureOf = blueprint.id;
                    if (blueprint.type.Equals(BLUEPRINT_TYPE_TEST) || blueprint.type.Equals(BLUEPRINT_TYPE_PACKAGE)) 
                    {
                        measureOf = "Overall";
                    }
                    var measureLabel = (rule.measure != null) ? rule.measure : rule.name;  // may need to add rule label to test package
                    createScoreFeatureList.Add(CreateScoreFeature(testKey, testPackage.publisher, measureOf, measureLabel, rule));
                });
            });
            return createScoreFeatureList;
        }

        public void CreateScoreFeature(TestPackage.TestPackage testPackage)
        {
            var testScoreFeatureDTO = new List<TestScoreFeatureDTO>();
            var featureComputationLocationDTO = new List<FeatureComputationLocationDTO>();

            var createScoreFeatureList = new List<TestScoreFeatureDTO>();

            if (testPackage.IsCombined())
            {
                var packageBlueprint = testPackage.Blueprint.FirstOrDefault(bp => bp.type.ToLower().Equals(BLUEPRINT_TYPE_PACKAGE));
                createScoreFeatureList.AddRange(CreateScoreFeature(packageBlueprint.id, testPackage));
            } else
            {
                var testBlueprints = testPackage.Blueprint.Where(bp => bp.type.ToLower().Equals(BLUEPRINT_TYPE_TEST));
                testBlueprints.ForEach(testBlueprint =>
                    createScoreFeatureList.AddRange(CreateScoreFeature(testBlueprint.id, testPackage)));                
            }

            createScoreFeatureList.ForEach(newTestScoreFeatureDTO =>
            {
                if (!testScoreFeatureDAO.Exists(newTestScoreFeatureDTO))
                {
                    testScoreFeatureDTO.Add(newTestScoreFeatureDTO);
                    featureComputationLocationDTO.Add(new FeatureComputationLocationDTO() { TestScoreFeatureKey = newTestScoreFeatureDTO.TestScoreFeatureKey, Location = "TIS" });
                }
            });

            testScoreFeatureDAO.Insert(testScoreFeatureDTO);
            featureComputationLocationDAO.Insert(featureComputationLocationDTO);
        }

        public void CreateComputationRuleParameters(TestPackage.TestPackage testPackage)
        {
            var computationRuleParameterDTO = new List<ComputationRuleParameterDTO>();
            testPackage.Blueprint.ForEach(blueprint =>
                {
                    blueprint.Scoring?.Rules?.ForEach(rule =>
                    {
                        rule.Parameter?.ForEach(parameter =>
                        {
                            var indexType = "";

                            parameter.Property?.ForEach(property =>
                                {
                                    if (property.name.Equals("indextype"))
                                    {
                                        indexType = property.value;
                                    }
                                });

                            var newComputationRuleParameterDTO = new ComputationRuleParameterDTO()
                            {
                                ComputationRuleParameterKey = parameter.Id,
                                ComputationRule = rule.name,
                                ParameterName = parameter.name,
                                ParameterPosition = parameter.position,
                                IndexType = indexType,
                                Type = parameter.type
                            };
                            if (!computationRuleParameterDAO.Exists(newComputationRuleParameterDTO))
                            {
                                if (!computationRuleParameterDTO.Contains(newComputationRuleParameterDTO))
                                {
                                    computationRuleParameterDTO.Add(newComputationRuleParameterDTO);
                                }

                            }
                        });

                    });
                });
            computationRuleParameterDAO.Insert(computationRuleParameterDTO);
        }

        public static string ScrubComputationRuleParameterValueIndex(string index)
        {
            if (index != null)
            {
                if (index.Contains(ItembankAdministrationDataService.COMBINED_SUFFIX))
                {
                    return ItembankAdministrationDataService.CombinedId(index.Replace(ItembankAdministrationDataService.COMBINED_SUFFIX, ""));
                } else
                {
                    return index;
                }
            }
            return "";
        }

        private List<ComputationRuleParameterValueDTO> CreateComputationRuleParameterValue(
            string testKey, string publisher, string measureOf, string ruleLabel, 
            BlueprintElementScoringRule rule, BlueprintElementScoringRuleParameter parameter)
        {
            var computationRuleParameterValueDTO = new List<ComputationRuleParameterValueDTO>();
            var computaionRuleParameterDTO = new ComputationRuleParameterDTO()
            {
                ComputationRule = rule.name,
                ParameterName = parameter.name,
                ParameterPosition = parameter.position
            };
            var dbParameter = computationRuleParameterDAO.FindByExample(computaionRuleParameterDTO).FirstOrDefault();
            var testScoreFeature = new TestScoreFeatureDTO()
            {
                ClientName = publisher,
                TestId = testKey,
                ComputationRule = rule.name,
                MeasureOf = measureOf,
                MeasureLabel = ruleLabel
            };
            var dbRule = testScoreFeatureDAO.FindByExample(testScoreFeature).FirstOrDefault();
            if ((dbParameter != null) && (dbRule != null))
            {
                var computationRuleParameterKey = dbParameter.ComputationRuleParameterKey;
                var testScoreFeatureKey = dbRule.TestScoreFeatureKey;

                parameter.Value.ForEach(value =>
                {
                    var newComputationRuleParameterValueDTO = new ComputationRuleParameterValueDTO()
                    {
                        TestScoreFeatureKey = testScoreFeatureKey,
                        ComputationRuleParameterKey = computationRuleParameterKey,
                        Index = ScrubComputationRuleParameterValueIndex(value.index),
                        // Index = value.index ?? "",
                        Value = value.value
                    };
                    if (!computationRuleParameterValueDAO.Exists(newComputationRuleParameterValueDTO))
                    {
                        computationRuleParameterValueDTO.Add(newComputationRuleParameterValueDTO);
                    }
                });
            }
            return computationRuleParameterValueDTO;
        }

        public List<ComputationRuleParameterValueDTO> CreateComputationRuleParameterValue(string testKey, TestPackage.TestPackage testPackage)
        {
            var computationRuleParameterValueDTO = new List<ComputationRuleParameterValueDTO>();
            testPackage.Blueprint.ForEach(blueprint =>
            {
                blueprint.Scoring?.Rules.ForEach(rule =>
                {
                    var measureOf = blueprint.id;
                    if (blueprint.type.Equals(BLUEPRINT_TYPE_TEST) || blueprint.type.Equals(BLUEPRINT_TYPE_PACKAGE))
                    {
                        measureOf = "Overall";
                    }
                    var ruleLabel = rule.name;  // may need to add rule label to test package

                    rule.Parameter?.ForEach(parameter =>
                    {
                        computationRuleParameterValueDTO.AddRange(CreateComputationRuleParameterValue(testKey, testPackage.publisher, measureOf, ruleLabel, rule, parameter));
                    });
                });
            });
            return computationRuleParameterValueDTO;
        }

        public void CreateComputationRuleParameterValue(TestPackage.TestPackage testPackage)
        {
            var computationRuleParameterValueDTO = new List<ComputationRuleParameterValueDTO>();
            var createComputationRuleParameterValueList = new List<TestScoreFeatureDTO>();

            if (testPackage.IsCombined())
            {
                var packageBlueprint = testPackage.Blueprint.FirstOrDefault(bp => bp.type.ToLower().Equals(BLUEPRINT_TYPE_PACKAGE));
                var testKey = packageBlueprint.id;
                computationRuleParameterValueDTO.AddRange(CreateComputationRuleParameterValue(packageBlueprint.id, testPackage));
            }
            else
            {
                var testBlueprints = testPackage.Blueprint.Where(bp => bp.type.ToLower().Equals(BLUEPRINT_TYPE_TEST));
                testBlueprints.ForEach(testBlueprint => {
                    computationRuleParameterValueDTO.AddRange(CreateComputationRuleParameterValue(testBlueprint.id, testPackage));
                });                
            }
            
            computationRuleParameterValueDAO.Insert(computationRuleParameterValueDTO);
        }

        public void CreatePerformanceLevels(TestPackage.TestPackage testPackage)
        {
            var performanceLevelDTO = new List<PerformanceLevelDTO>();
            var testKey = TestKey(testPackage);
            if (testKey != null) {
                var testPackageKey = string.Format("({0}){1}-{2}", testPackage.publisher, testKey, testPackage.academicYear);
                testPackage.Blueprint.ForEach(blueprint =>
                {
                    if ((blueprint.Scoring != null) && (blueprint.Scoring.PerformanceLevels != null))
                    {
                        blueprint.Scoring.PerformanceLevels.ForEach(performanceLevel =>
                        {
                            var newPerformanceLevelDTO = new PerformanceLevelDTO()
                            {
                                ContentKey = testPackageKey,
                                PLevel = performanceLevel.pLevel,
                                ThetaLo = 0,
                                ThetaHi = 0,
                                ScaledLo = performanceLevel.scaledLo,
                                ScaledHi = performanceLevel.scaledHi,
                            };
                            if (!performanceLevelDAO.Exists(newPerformanceLevelDTO))
                            {
                                performanceLevelDTO.Add(newPerformanceLevelDTO);
                            }
                        });
                    }
                });

                performanceLevelDAO.Insert(performanceLevelDTO);
            }
        }
    }
}
