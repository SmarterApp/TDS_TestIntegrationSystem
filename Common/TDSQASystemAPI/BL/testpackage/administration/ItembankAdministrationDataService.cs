using System;
using System.Collections.Generic;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.daos;
using TDSQASystemAPI.DAL.itembank.dtos;

namespace TDSQASystemAPI.BL.testpackage.administration
{
    public class ItembankAdministrationDataService : IItembankAdministrationDataService
    {
        private readonly IItembankConfigurationDataQueryService itembankConfigurationDataQueryService;
        private readonly ITestPackageDao<TestAdminDTO> testAdminDao;

        public ItembankAdministrationDataService()
        {
            testAdminDao = new TestAdminDAO();
            itembankConfigurationDataQueryService = 
                new ItembankConfigurationDataQueryService(new SubjectDAO(), new ClientDAO(), testAdminDao);
            
        }

        public ItembankAdministrationDataService(IItembankConfigurationDataQueryService itembankConfigurationDataQueryService,
                                                 ITestPackageDao<TestAdminDTO> testAdminDao)
        {
            this.itembankConfigurationDataQueryService = itembankConfigurationDataQueryService;
            this.testAdminDao = testAdminDao;
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
                TestVersion = (long)testPackage.version // whether inserting or updating, we want the version to reflect the testpackage being loaded
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
    }
}
