using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.configs.daos;
using TDSQASystemAPI.DAL.configs.dtos;
using TISUnitTests.utils;

namespace TISUnitTests
{
    [TestClass]
    public class TesteeAttributesDaoIntegrationTest : TestPackageDaoIntegrationTestBase
    {
        private readonly ITestPackageDao<TesteeAttributeDTO> testeeAttributeDao = new TesteeAttributeDAO();

        [TestMethod]
        public void ShouldSaveACollectionOfTesteeAttributes()
        {
            IList<TesteeAttributeDTO> testeeAttributesList = new List<TesteeAttributeDTO>
            {
                new TesteeAttributeDTO
                {
                    RtsName = "art-id-1",
                    AtLogin = "at-login-1",
                    ClientName = "unit-test",
                    Label = "label 1",
                    ReportName = "report name 1",
                    SortOrder = 1,
                    TdsId = "tds-attribute-id 1",
                    Type = "unit test"
                },
                new TesteeAttributeDTO
                {
                    RtsName = "art-id-2",
                    AtLogin = "at-login-2",
                    ClientName = "unit-test",
                    Label = "label 2",
                    ReportName = "report name 2",
                    SortOrder = 2,
                    TdsId = "tds-attribute-id 2",
                    Type = "unit test",
                    
                }
            };

            testeeAttributeDao.Insert(testeeAttributesList);
        }
    }
}
