using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDSQASystemAPI.BL.testpackage.osstis
{
    public interface ICombinationTestMapService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="testPackage"></param>
        void CreateCombinationTestMap(TestPackage.TestPackage testPackage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testPackage"></param>
        void CreateCombinationTestFormMap(TestPackage.TestPackage testPackage);
    }
}
