using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TDSQASystemAPI.BL.testpackage.scoring;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.scoring.daos;
using TDSQASystemAPI.TestPackage.utils;

namespace TISUnitTests.services
{
    class ScoringConfigDriver
    {
        private const string TEST_PACKAGE_XML_FILE = @"..\..\resources\test-packages\new-xsd\(SBAC_PT)SBAC-IRP-MATH-11-COMBINED-Summer-2015-2016-NEW.xml";
        
        public static void Main(string[] args)
        {
            var loadedTestPackage = TestPackageAssembler.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE));

            var service = new ScoringConfigurationDataService(new TestDAO(), new TestGradeDAO(), new TestScoreFeatureDAO(), new FeatureComputationLocationDAO(),
                new ComputationRuleParameterDAO(), new ComputationRuleParameterValueDAO(), new PerformanceLevelDAO());

            service.CreateTest(loadedTestPackage);
            service.CreateGrade(loadedTestPackage);
            service.CreateScoreFeature(loadedTestPackage);
            service.CreateComputationRuleParameters(loadedTestPackage);
            service.CreateComputationRuleParameterValue(loadedTestPackage);
            service.CreatePerformanceLevels(loadedTestPackage);            
        }
    }
}
