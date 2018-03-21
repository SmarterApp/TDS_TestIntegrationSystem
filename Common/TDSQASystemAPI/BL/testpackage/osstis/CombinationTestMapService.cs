using System;
using System.Collections.Generic;
using System.Linq;
using TDSQASystemAPI.DAL;
using TDSQASystemAPI.DAL.itembank.dtos;
using TDSQASystemAPI.DAL.osstis.daos;
using TDSQASystemAPI.DAL.osstis.dtos;

namespace TDSQASystemAPI.BL.testpackage.osstis
{
    public class CombinationTestMapService : ICombinationTestMapService
    {
        private readonly ITestPackageDao<CombinationTestMapDTO> combinationTestMapDao;

        public CombinationTestMapService()
        {
            combinationTestMapDao = new CombinationTestMapDAO();
        }

        public CombinationTestMapService(ITestPackageDao<CombinationTestMapDTO> combinationTestMapDao)
        {
            this.combinationTestMapDao = combinationTestMapDao;
        }

        public void CreateCombinationTestFormMap(TestPackage.TestPackage testPackage, IList<TestFormDTO> testForms)
        {
            throw new NotImplementedException();
        }

        public void CreateCombinationTestMap(TestPackage.TestPackage testPackage)
        {
            // CombinationTestMap records only have to be created for "combined" test packages.
            if (!testPackage.IsCombined())
            {
                return;
            }

            var combinationTestMapDtos = from assessment in testPackage.Assessment
                                         from segment in assessment.Segments
                                         select new CombinationTestMapDTO
                                         {
                                             ComponentSegmentName = segment.Key,
                                             ComponentTestName = assessment.Key,
                                             CombinationSegmentName = segment.Key,
                                             CombinationTestName = testPackage.GetCombinationTestPackageKey()
                                         };

            combinationTestMapDao.Insert(combinationTestMapDtos.ToList());
        }
    }
}
