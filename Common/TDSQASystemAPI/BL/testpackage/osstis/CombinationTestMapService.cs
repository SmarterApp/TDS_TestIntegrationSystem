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
        private readonly ITestPackageDao<CombinationTestFormMapDTO> combinationTestFormMapDao;
        private readonly ITestPackageDao<CombinationTestMapDTO> combinationTestMapDao;

        public CombinationTestMapService()
        {
            combinationTestMapDao = new CombinationTestMapDAO();
        }

        public CombinationTestMapService(ITestPackageDao<CombinationTestMapDTO> combinationTestMapDao,
                                         ITestPackageDao<CombinationTestFormMapDTO> combinationTestFormMapDao)
        {
            this.combinationTestMapDao = combinationTestMapDao;
            this.combinationTestFormMapDao = combinationTestFormMapDao;
        }

        public void CreateCombinationTestFormMap(TestPackage.TestPackage testPackage, IList<TestFormDTO> testForms)
        {
            // CombinationTestMap records only have to be created for "combined" test packages.
            if (!testPackage.IsCombined())
            {
                return;
            }

            var combinationTestFormMapDtos = from assessment in testPackage.Assessment
                                             from segment in assessment.Segments
                                             join form in testForms
                                                on segment.Key equals form.SegmentKey
                                             select new CombinationTestFormMapDTO
                                             {
                                                 ComponentSegmentName = segment.Key,
                                                 CombinationFormKey = form.TestFormKey,
                                                 ComponentFormKey = form.TestFormKey
                                             };

            combinationTestFormMapDao.Insert(combinationTestFormMapDtos.ToList());
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
