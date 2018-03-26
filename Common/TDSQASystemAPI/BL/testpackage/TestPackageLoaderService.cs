using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TDSQASystemAPI.BL.testpackage.administration;
using TDSQASystemAPI.BL.testpackage.osstis;
using TDSQASystemAPI.BL.testpackage.scoring;
using TDSQASystemAPI.TestPackage.utils;
using TDSQASystemAPI.Utilities;

namespace TDSQASystemAPI.BL.testpackage
{
    public class TestPackageLoaderService : ITestPackageLoaderService
    {
        private readonly IItembankAdministrationDataService itembankAdministrationDataService;
        private readonly IItembankConfigurationDataService itembankConfigurationDataService;
        private readonly ICombinationTestMapService combinationTestMapService;
        private readonly IQcProjectMetadataService qcProjectMetadataService;
        private readonly ScoringConfigurationDataService scoringConfigurationDataService;

        public TestPackageLoaderService()
        {
            itembankAdministrationDataService = new ItembankAdministrationDataService();
            itembankConfigurationDataService = new ItembankConfigurationDataService();
            combinationTestMapService = new CombinationTestMapService();
            qcProjectMetadataService = new QcProjectMetadataService();
            scoringConfigurationDataService = new ScoringConfigurationDataService();
        }

        public TestPackageLoaderService(IItembankAdministrationDataService itembankAdministrationDataService, 
                                        IItembankConfigurationDataService itembankConfigurationDataService, 
                                        ICombinationTestMapService combinationTestMapService, 
                                        IQcProjectMetadataService qcProjectMetadataService, 
                                        ScoringConfigurationDataService scoringConfigurationDataService)
        {
            this.itembankAdministrationDataService = itembankAdministrationDataService;
            this.itembankConfigurationDataService = itembankConfigurationDataService;
            this.combinationTestMapService = combinationTestMapService;
            this.qcProjectMetadataService = qcProjectMetadataService;
            this.scoringConfigurationDataService = scoringConfigurationDataService;
        }

        public IList<ValidationError> LoadTestPackage(Stream testPackageXmlStream)
        {
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(testPackageXmlStream));
            var validationErrors = new List<ValidationError>();

            //-----------------------------------------------------------------
            // LOAD TEST PACKAGE CONFIGURATION DATA
            //-----------------------------------------------------------------
            itembankConfigurationDataService.CreateClient(testPackage);

            itembankConfigurationDataService.CreateSubject(testPackage);

            var itemStrands = itembankConfigurationDataService.CreateStrands(testPackage);

            itembankConfigurationDataService.CreateStimuli(testPackage);

            var existingItems = itembankConfigurationDataService.CreateItems(testPackage);

            itembankConfigurationDataService.LinkItemToStrands(testPackage, itemStrands);

            itembankConfigurationDataService.LinkItemsToStimuli(testPackage);

            itembankConfigurationDataService.CreateItemProperties(testPackage);

            //-----------------------------------------------------------------
            // LOAD TEST PACKAGE ADMINISTRATION DATA
            //-----------------------------------------------------------------
            itembankAdministrationDataService.SaveTestAdministration(testPackage);

            itembankAdministrationDataService.CreateSetOfAdminSubjects(testPackage);

            itembankAdministrationDataService.CreateAdminStrands(testPackage, itemStrands);

            itembankAdministrationDataService.CreateSetOfAdminItems(testPackage, itemStrands);

            itembankAdministrationDataService.CreateItemMeasurementParameters(testPackage);

            itembankAdministrationDataService.CreateAdminStimuli(testPackage);

            var testForms = itembankAdministrationDataService.CreateTestForms(testPackage);

            itembankAdministrationDataService.CreateTestFormItems(testPackage, testForms);

            itembankAdministrationDataService.CreateAffinityGroups(testPackage);

            //-----------------------------------------------------------------
            // LOAD TEST PACKAGE SCORING DATA
            //-----------------------------------------------------------------
            scoringConfigurationDataService.CreateTest(testPackage);

            scoringConfigurationDataService.CreateGrade(testPackage);

            scoringConfigurationDataService.CreateScoreFeature(testPackage);

            scoringConfigurationDataService.CreateComputationRuleParameters(testPackage);

            scoringConfigurationDataService.CreateComputationRuleParameterValue(testPackage);

            scoringConfigurationDataService.CreatePerformanceLevels(testPackage);

            //-----------------------------------------------------------------
            // LOAD TEST PACKAGE METADATA
            //-----------------------------------------------------------------
            combinationTestMapService.CreateCombinationTestMap(testPackage);

            combinationTestMapService.CreateCombinationTestFormMap(testPackage, testForms);

            qcProjectMetadataService.CreateQcProjectMetadata(testPackage);

            if (existingItems.Any())
            {
                validationErrors.Add(new ValidationError(ValidationErrorCodes.EXISTING_ITEMS_NOT_LOADED, 
                    ValidationErrorSeverityLevels.WARN, 
                    string.Format("the following items in this test package were not loaded because they already exist in TIS: \n{0}", string.Join("\n", existingItems.Select(ei => ei.ItemKey).ToArray()))));
            }
            return validationErrors;
        }
    }
}
