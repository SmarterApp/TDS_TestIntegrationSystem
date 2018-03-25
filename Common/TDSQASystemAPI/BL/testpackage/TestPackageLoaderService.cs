using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TDSQASystemAPI.BL.testpackage.administration;
using TDSQASystemAPI.BL.testpackage.osstis;
using TDSQASystemAPI.BL.testpackage.scoring;
using TDSQASystemAPI.TestPackage.utils;

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

        public LoadTestPackageResponse LoadTestPackage(Stream testPackageXmlStream)
        {
            var testPackage = TestPackageMapper.FromXml(new XmlTextReader(testPackageXmlStream));

            //-----------------------------------------------------------------
            // LOAD TEST PACKAGE CONFIGURATION DATA
            //-----------------------------------------------------------------
            itembankConfigurationDataService.CreateClient(testPackage);

            itembankConfigurationDataService.CreateSubject(testPackage);

            var itemStrands = itembankConfigurationDataService.CreateStrands(testPackage);

            itembankConfigurationDataService.CreateStimuli(testPackage);

            // TODO:  get the items that weren't created because they already exist
            itembankConfigurationDataService.CreateItems(testPackage);

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

            // TODO:  correct typo in method name
            itembankAdministrationDataService.CeateItemMeasurementParameters(testPackage);

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

            // TODO:  find equivalent of spLoad_ScoringComputationLocation

            scoringConfigurationDataService.CreateComputationRuleParameters(testPackage);

            scoringConfigurationDataService.CreateComputationRuleParameterValue(testPackage);

            // TODO:  find equivalent of spLoad_ScoringConversionTableDesc

            // TODO:  find equivalent of spLoad_ScoringConversionTables

            scoringConfigurationDataService.CreatePerformanceLevels(testPackage);

            //-----------------------------------------------------------------
            // LOAD TEST PACKAGE METADATA
            //-----------------------------------------------------------------
            combinationTestMapService.CreateCombinationTestMap(testPackage);

            combinationTestMapService.CreateCombinationTestFormMap(testPackage, testForms);

            qcProjectMetadataService.CreateQcProjectMetadata(testPackage);

            return new LoadTestPackageResponse();
        }
    }
}
