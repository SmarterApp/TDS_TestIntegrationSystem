using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.dtos;
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
        private readonly ITestPackageDao<ConversionTableDescDTO> conversionTableDescDAO;
        private readonly ITestPackageDao<ConversionTableDTO> conversionTableDAO;
        private readonly ITestPackageDao<PerformanceLevelDTO> performanceLevelDAO;

        public ScoringConfigurationDataService(
            ITestPackageDao<TestDTO> testDAO,
            ITestPackageDao<TestGradeDTO> testGradeDAO,
            ITestPackageDao<TestScoreFeatureDTO> testScoreFeatureDAO,
            ITestPackageDao<FeatureComputationLocationDTO> featureComputationLocationDAO,
            ITestPackageDao<ComputationRuleParameterDTO> computationRuleParameterDAO,
            ITestPackageDao<ComputationRuleParameterValueDTO> computationRuleParameterValueDAO,
            ITestPackageDao<ConversionTableDescDTO> conversionTableDescDAO,
            ITestPackageDao<ConversionTableDTO> conversionTableDAO,
            ITestPackageDao<PerformanceLevelDTO> performanceLevelDAO)
        {
            this.testDAO = testDAO;
            this.testGradeDAO = testGradeDAO;
            this.testScoreFeatureDAO = testScoreFeatureDAO;
            this.featureComputationLocationDAO = featureComputationLocationDAO;
            this.computationRuleParameterDAO = computationRuleParameterDAO;
            this.computationRuleParameterValueDAO = computationRuleParameterValueDAO;
            this.conversionTableDescDAO = conversionTableDescDAO;
            this.conversionTableDAO = conversionTableDAO;
            this.performanceLevelDAO = performanceLevelDAO;
        }

        public void CreateTest(TestPackage.TestPackage testPackage)
        {
            var testDTO = new List<TestDTO>();
            testPackage.Assessment.ForEach(assessment => testDTO.Add(
                new TestDTO(){
                    ClientName = testPackage.publisher,
                    TestId = assessment.id,
                    Subject = testPackage.subject
                }));
            testDAO.Insert(testDTO);
        }

        public void CreateGrade(TestPackage.TestPackage testPackage)
        {
            var testGradeDTO = new List<TestGradeDTO>();
            testPackage.Assessment.ForEach(assessment =>
                assessment.Grades.ForEach(grade =>
                {
                    testGradeDTO.Add(new TestGradeDTO()
                    {
                        ClientName = testPackage.publisher,
                        TestId = assessment.id,
                        ReportingGrade = grade.label
                    });
                }));
            testGradeDAO.Insert(testGradeDTO);
        }

        public void CreateScoreFeature(TestPackage.TestPackage testPackage)
        {
            var testScoreFeatureDTO = new List<TestScoreFeatureDTO>();
            var assessment = testPackage.Assessment.SelectMany(a => a.Segments.SelectMany(s => s.SegmentBlueprint.Select(b => new { a.id, b.idRef }))).ToDictionary(t=>t.idRef, t=>t.id);
            testPackage.Blueprint.ForEach(
                blueprint => blueprint.Scoring.Rules.ForEach(
                    rule => {
                        var measureOf = "";
                        // testPackage.
                        testScoreFeatureDTO.Add(new TestScoreFeatureDTO()
                        {
                            TestScoreFeatureKey = rule.Id,
                            ClientName = testPackage.publisher,
                            TestId = testPackage.id
                            MeasureOf = measureOf, // CASE WHEN TestKey = cr.bpElementID THEN 'Overall'
                                                   // WHEN tbp.bpElementID IS NOT NULL THEN tbp.bpElementName
                                                   // ELSE cr.bpElementID
                                                   // END
                            MeasureLabel = rule.name, // comes from rule label
                            IsScaled = rule.name.ToLower().Contains("scale"),  // comes from rule label
                            ComputationRule = rule.name,
                            ComputationOrder = rule.computationOrder
                        });
                    }));
            testScoreFeatureDAO.Insert(testScoreFeatureDTO);
        }

        public void CreateFeatureComputationLocations(TestPackage.TestPackage testPackage)
        {
            var ruleId = Guid.NewGuid();
            var featureComputationLocationDTO = new List<FeatureComputationLocationDTO>();
            testPackage.Blueprint.ForEach(
                blueprint => blueprint.Scoring.Rules.ForEach(rule =>                    
                    featureComputationLocationDTO.Add(new FeatureComputationLocationDTO()
                    {
                        TestScoreFeatureKey = ruleId,  // TODO: missing field
                        Location = "TIS"
                    })
                 )
            );

            featureComputationLocationDAO.Insert(featureComputationLocationDTO);
        }

        public void CreateComputationRuleParameters(TestPackage.TestPackage testPackage)
        {
            var parameterID = Guid.NewGuid();
            var parameterPosition = 1;
            var computationRuleParameterDTO = new List<ComputationRuleParameterDTO>();
            testPackage.Blueprint.ForEach(
                blueprint => blueprint.Scoring.Rules.ForEach(rule =>
                    computationRuleParameterDTO.Add(new ComputationRuleParameterDTO()
                    {
                        ComputationRuleParameterKey = parameterID,
                        ComputationRule = rule.name,
                        ParameterName = rule.Parameter[0].name,
                        ParameterPosition = parameterPosition,
                        // IndexType = CASE WHEN lcrp.PropName = 'indextype' THEN lcrp.PropValue ELSE '' END
                        IndexType = rule.Parameter[0].Property[0].value,
                        Type = rule.Parameter[0].type
                    })
                )
            );

            computationRuleParameterDAO.Insert(computationRuleParameterDTO);
        }

        public void CreateComputationRuleParameterValue(TestPackage.TestPackage testPackage)
        {
            var testScoreFeatureKey = Guid.NewGuid();
            var ruleId = Guid.NewGuid();
            

            var computationRuleParameterValueDTO = new List<ComputationRuleParameterValueDTO>();
            var computationRuleParameterDTO = new List<ComputationRuleParameterDTO>();
            testPackage.Blueprint.ForEach(
                blueprint => blueprint.Scoring.Rules.ForEach(
                    rule => rule.Parameter.ForEach(parameter => parameter.Value.ForEach(
                       value =>
                       {
                           computationRuleParameterValueDTO.Add(new ComputationRuleParameterValueDTO()
                           {

                               TestScoreFeatureKey = testScoreFeatureKey,
                               ComputationRuleParameterKey = ruleId,
                               Index = value.index,
                               Value = value.value
                           });
                       }
               ))));
   
            computationRuleParameterValueDAO.Insert(computationRuleParameterValueDTO);
        }

        public void CreateConversionTableDesc(TestPackage.TestPackage testPackage)
        {
            var conversionTableDescDTO = new List<ConversionTableDescDTO>();
            testPackage.Assessment.ForEach(assessment =>
                assessment.Grades.ForEach(grade =>
                {
                    conversionTableDescDTO.Add(new ConversionTableDescDTO()
                    {
                        ClientName = testPackage.publisher,
                        TestId = assessment.id,
                        ReportingGrade = grade.label
                    });

                }));

            conversionTableDescDAO.Insert(conversionTableDescDTO);
        }

        public void CreateConversionTables(TestPackage.TestPackage testPackage)
        {
            var conversionTableDTO = new List<ConversionTableDTO>();
            testPackage.Assessment.ForEach(assessment =>
                assessment.Grades.ForEach(grade =>
                {
                    conversionTableDTO.Add(new ConversionTableDTO()
                    {
                        ClientName = testPackage.publisher,
                        TestId = assessment.id,
                        ReportingGrade = grade.label
                    });

                }));

            conversionTableDAO.Insert(conversionTableDTO);
        }

        public void CreatePerformanceLevels(TestPackage.TestPackage testPackage)
        {
            var performanceLevelDTO = new List<PerformanceLevelDTO>();
            testPackage.Assessment.ForEach(assessment =>
                assessment.Grades.ForEach(grade =>
                {
                    performanceLevelDTO.Add(new PerformanceLevelDTO()
                    {
                        PLevel = 0,
                        ThetaLo = 0,
                        ThetaHi = 0,
                        ScaledLo = 0,
                        ScaledHi = 0,
                    });

                }));

            performanceLevelDAO.Insert(performanceLevelDTO);
        }
    }
}
