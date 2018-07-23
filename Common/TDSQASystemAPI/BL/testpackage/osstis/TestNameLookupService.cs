using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.osstis.daos;
using TDSQASystemAPI.DAL.osstis.dtos;
using TDSQASystemAPI.TestPackage;

namespace TDSQASystemAPI.BL.testpackage.osstis
{
    public class TestNameLookupService : ITestNameLookupService
    {
        private const string INSTANCE_NAME = "OSS_TISService";
        private readonly ITestPackageDao<TestNameLookupDTO> testPackageLookupDao;

        public TestNameLookupService()
        {
            testPackageLookupDao = new TestNameLookupDAO();
        }

        public TestNameLookupService(ITestPackageDao<TestNameLookupDTO> testPackageLookupDao)
        {
            this.testPackageLookupDao = testPackageLookupDao;
        }

        private void CreateTestNameLookup(string testPackageKey)
        {
            // If a test name lookup record for this test package already exists, exit
            var existingTestNameLookup = testPackageLookupDao.Find(testPackageKey);
            if (existingTestNameLookup != null && existingTestNameLookup.Any())
            {
                return;
            }

            testPackageLookupDao.Insert(new TestNameLookupDTO
            {
                InstanceName = INSTANCE_NAME,
                TestName = testPackageKey
            });
        }

        public void CreateTestNameLookup(TestPackage.TestPackage testPackage)
        {
            CreateTestNameLookup(testPackage.GetTestPackageKey());

            foreach (Test t in testPackage.Test)
            {
                CreateTestNameLookup(t.Key);
            }                       
        }
    }
}
