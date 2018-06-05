using System.Collections.Generic;
using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.DAL.scoring.daos;
using TDSQASystemAPI.DAL.scoring.dtos;

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

        public void CreateScoreFeature(TestPackage.TestPackage testPackage)
        {
            var testScoreFeatureDTO = new List<TestScoreFeatureDTO>();
            var featureComputationLocationDTO = new List<FeatureComputationLocationDTO>();
            var packageBlueprint = testPackage.Blueprint.FirstOrDefault(bp => bp.type.ToLower().Equals(BLUEPRINT_TYPE_TEST));
            if (packageBlueprint != null)
            {
                var testKey = packageBlueprint.id;            
                testPackage.Blueprint.ForEach(blueprint => {
                    blueprint.Scoring?.Rules.ForEach(rule =>
                    {
                        var measureOf = blueprint.id;
                        if (blueprint.type.Equals(BLUEPRINT_TYPE_TEST))
                        {
                            measureOf = "Overall";
                        }
                        var ruleLabel = rule.name;  // may need to add rule label to test package
                            var newTestScoreFeatureDTO = new TestScoreFeatureDTO()
                        {
                            TestScoreFeatureKey = rule.Id,
                            ClientName = testPackage.publisher,
                            TestId = testKey,
                            MeasureOf = measureOf,
                            MeasureLabel = ruleLabel,
                            IsScaled = ruleLabel.ToLower().Contains("scale"),
                            ComputationRule = rule.name,
                            ComputationOrder = rule.computationOrder
                        };
                        if (!testScoreFeatureDAO.Exists(newTestScoreFeatureDTO))
                        {
                            testScoreFeatureDTO.Add(newTestScoreFeatureDTO);
                            featureComputationLocationDTO.Add(new FeatureComputationLocationDTO() { TestScoreFeatureKey = rule.Id, Location = "TIS" });
                        }
                    });                    
                });
                testScoreFeatureDAO.Insert(testScoreFeatureDTO);
                featureComputationLocationDAO.Insert(featureComputationLocationDTO);
            }
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

        public void CreateComputationRuleParameterValue(TestPackage.TestPackage testPackage)
        {
            var computationRuleParameterValueDTO = new List<ComputationRuleParameterValueDTO>();
            var packageBlueprint = testPackage.Blueprint.FirstOrDefault(bp => bp.type.ToLower().Equals(BLUEPRINT_TYPE_TEST));
            if (packageBlueprint != null)
            {
                var testKey = packageBlueprint.id;
                testPackage.Blueprint.ForEach(blueprint =>
                {
                    blueprint.Scoring?.Rules.ForEach(rule =>
                    {
                        var measureOf = blueprint.id;
                        if (blueprint.type.Equals(BLUEPRINT_TYPE_TEST))
                        {
                            measureOf = "Overall";
                        }
                        var ruleLabel = rule.name;  // may need to add rule label to test package

                        rule.Parameter?.ForEach(parameter =>
                        {
                            var computaionRuleParameterDTO = new ComputationRuleParameterDTO()
                            {
                                ComputationRule = rule.name,
                                ParameterName = parameter.name,
                                ParameterPosition = parameter.position
                            };
                            var dbParameter = computationRuleParameterDAO.FindByExample(computaionRuleParameterDTO).FirstOrDefault();
                            var testScoreFeature = new TestScoreFeatureDTO()
                            {
                                ClientName = testPackage.publisher,
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
                                        Index = value.index ?? "",
                                        Value = value.value
                                    };
                                    if (!computationRuleParameterValueDAO.Exists(newComputationRuleParameterValueDTO))
                                    {
                                        computationRuleParameterValueDTO.Add(newComputationRuleParameterValueDTO);
                                    }
                                });
                            }
                        });
                    });
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
