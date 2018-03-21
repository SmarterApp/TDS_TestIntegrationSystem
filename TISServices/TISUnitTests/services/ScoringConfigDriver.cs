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
        private const string TEST_PACKAGE_XML_FILE1 = @"..\..\resources\test-packages\new-xsd\(SBAC_PT)SBAC-IRP-MATH-11-COMBINED-Summer-2015-2016-NEW.xml";
        private const string TEST_PACKAGE_XML_FILE2 = @"..\..\resources\test-packages\new-xsd\V2-(SBAC_PT)IRP-GRADE-11-MATH-EXAMPLE.xml";
        

        public static void Main(string[] args)
        {
            var loadedTestPackage = TestPackageAssembler.FromXml(new XmlTextReader(TEST_PACKAGE_XML_FILE1));

            var service = new ScoringConfigurationDataService(new TestDAO(), new TestGradeDAO(), new TestScoreFeatureDAO(), new FeatureComputationLocationDAO(),
                new ComputationRuleParameterDAO(), new ComputationRuleParameterValueDAO(), new PerformanceLevelDAO());

            service.CreateTest(loadedTestPackage);
            service.CreateGrade(loadedTestPackage);
            service.CreateScoreFeature(loadedTestPackage);
            service.CreateComputationRuleParameters(loadedTestPackage);
            service.CreatePerformanceLevels(loadedTestPackage);            
        }
    }
}
