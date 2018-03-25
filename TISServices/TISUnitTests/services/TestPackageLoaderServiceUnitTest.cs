using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDSQASystemAPI.BL.testpackage.administration;
using TDSQASystemAPI.BL.testpackage.osstis;
using TDSQASystemAPI.BL.testpackage.scoring;

namespace TISUnitTests.services
{
    [TestClass]
    public class TestPackageLoaderServiceUnitTest
    {
        private readonly Mock<IItembankConfigurationDataService> mockItembankConfigurationDataService = new Mock<IItembankConfigurationDataService>();
        private readonly Mock<IItembankConfigurationDataQueryService> mockItembankConfigurationQueryDataService = new Mock<IItembankConfigurationDataQueryService>();
        private readonly Mock<IItembankAdministrationDataService> mockItembankAdministrationDataService = new Mock<IItembankAdministrationDataService>();
        private readonly Mock<ICombinationTestMapService> mockCombinationTestMapService = new Mock<ICombinationTestMapService>();
        private readonly Mock<IQcProjectMetadataService> mockQcProjectMetadataService = new Mock<IQcProjectMetadataService>();
        private readonly Mock<ScoringConfigurationDataService> mockScoringConfigurationDataService = new Mock<ScoringConfigurationDataService>();

        [TestMethod]
        public void TestPackage_ShouldLoadATestPackageWithANewClient()
        {

        }

        [TestMethod]
        public void TestPackage_ShouldLoadATestPackageWithAnExistingClient()
        {

        }

        [TestMethod]
        public void TestPackage_SHouldLoadATestPackageWithSomeItemsThatAlreadyExist()
        {

        }
    }
}
